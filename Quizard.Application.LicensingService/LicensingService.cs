using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;

namespace Quizard.Application.LicensingService;

/// <inheritdoc />
public class LicensingService : ILicensingService
{
    private readonly IAesEncryptor _aesEncryptor;
    private readonly IQuizardDbManager _dbManager;
    private readonly ILogger<LicensingService> _logger;

    /// <summary> Конструктор сервиса лицензирования </summary>
    /// <param name="aesEncryptor"> Сервис шифрования </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public LicensingService(IAesEncryptor aesEncryptor, IQuizardDbManager dbManager, ILogger<LicensingService> logger)
    {
        _logger = logger;
        _dbManager = dbManager;
        _aesEncryptor = aesEncryptor;

        _logger.LogDebug("Сервис лицензирования проинициализирован");
    }

    /// <inheritdoc />
    public async Task<string> GenerateSecretSalt()
    {
        _logger.LogInformation("Выполняется генерация секретной соли");

        var rawSalt = Ulid.NewUlid();
        var rawSaltString = rawSalt.ToString();
        var rawCreated = rawSalt.Time;

        var encryptedSalt = await _aesEncryptor.EncryptAsync(rawSaltString);
        var encryptedCreatedAt = await _aesEncryptor.EncryptAsync(rawCreated.ToString());

        var secret = new LicenseSecret
        {
            Salt = encryptedSalt,
            CreatedAt = encryptedCreatedAt
        };
        await _dbManager.LicenseSecretRepository.AddLicenseSecretAsync(secret);

        return rawSaltString;
    }

    /// <inheritdoc/>  
    public async Task<ClaimsPrincipal> UploadLicenseKey(string licenseKey)
    {
        var handler = new JwtSecurityTokenHandler();

        var secrets = await _dbManager.LicenseSecretRepository
            .GetLicenseSecretsAsync(secret => secret.License == null, withLicense: true).ConfigureAwait(false);

        foreach (var secret in secrets)
        {
            var rawSalt = await _aesEncryptor.DecryptAsync(secret.Salt).ConfigureAwait(false);

            var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawSalt));
            var signingKey = new SymmetricSecurityKey(keyBytes);

            IdentityModelEventSource.ShowPII = true;
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal;
            try
            {
                principal = handler.ValidateToken(licenseKey, validationParameters, out _);
            }
            catch (Exception)
            {
                continue;
            }

            var jwt = (JwtSecurityToken)handler.ReadToken(licenseKey)!;
            var exp = long.Parse(jwt.Claims.First(c => c.Type == nameof(License.ExpirationTime)).Value);
            var games = int.Parse(jwt.Claims.First(c => c.Type == nameof(License.GamesLeft)).Value);

            var encryptedExp = await _aesEncryptor.EncryptAsync(exp.ToString());
            var encryptedGamesLeft = await _aesEncryptor.EncryptAsync(games.ToString());

            var license = new License
            {
                LicenseKey = licenseKey,
                ExpirationTime = encryptedExp,
                GamesLeft = encryptedGamesLeft,
                Active = true,
                SaltId = secret.Id
            };

            await _dbManager.LicenseRepository.AddLicenseAsync(license).ConfigureAwait(false);

            _logger.LogInformation("Зарегистрирована новая лицензия {LicenseKey} для saltId={SaltId}",
                licenseKey, secret.Id);

            return principal;
        }

        throw new LicenseValidationException("Не удалось найти подходящую соль для данного лицензионного ключа");
    }

    /// <inheritdoc/> 
    public async Task<bool> CheckActiveLicense(bool changeGameCounter = false)
    {
        await foreach (var license in _dbManager.LicenseRepository.IterateLicenseAsync(withSalt: true))
        {
            if (!license.Active)
                continue;

            var rawSalt = await _aesEncryptor.DecryptAsync(license.Salt.Salt);
            var rawExp = await _aesEncryptor.DecryptAsync(license.ExpirationTime);
            var rawGames = await _aesEncryptor.DecryptAsync(license.GamesLeft);

            var expUnix = long.Parse(rawExp);
            var gamesLeft = int.Parse(rawGames);

            var keyBytes = await Task.Run(() => SHA256.HashData(Encoding.UTF8.GetBytes(rawSalt)));
            var signingKey = new SymmetricSecurityKey(keyBytes);

            ClaimsPrincipal principal;
            try
            {
                principal = await Task.Run(() =>
                {
                    var handler = new JwtSecurityTokenHandler();
                    var parameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false
                    };
                    return handler.ValidateToken(license.LicenseKey, parameters, out _);
                });
            }
            catch (SecurityTokenException ex)
            {
                license.Active = false;
                await _dbManager.LicenseRepository.AddLicenseAsync(license);
                _logger.LogCritical(ex, "JWT-подпись для лицензии {Id} не прошла проверку", license.Id);
                throw new LicenseValidationException("Невалидный лицензионный ключ");
            }

            var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (expUnix < nowUnix
                || expUnix != long.Parse(principal.FindFirst(nameof(License.ExpirationTime))!.Value))
            {
                license.Active = false;
                await _dbManager.LicenseRepository.UpdateLicenseAsync(license.Id, license);
                throw new LicenseValidationException("Лицензия просрочена или не соответствует данным БД");
            }


            var tokenGames = int.Parse(principal.FindFirst(nameof(License.GamesLeft))!.Value);
            if (gamesLeft <= 0)
            {
                license.Active = false;
                await _dbManager.LicenseRepository.UpdateLicenseAsync(license.Id, license);
                throw new LicenseValidationException("Не осталось игровых сессий");
            }

            if (gamesLeft > tokenGames)
            {
                license.Active = false;
                await _dbManager.LicenseRepository.UpdateLicenseAsync(license.Id, license);
                throw new LicenseValidationException("Несовпадение счётчика игр с токеном");
            }

            if (!changeGameCounter) return true;

            var newGames = gamesLeft - 1;
            license.GamesLeft = await _aesEncryptor.EncryptAsync(newGames.ToString());
            await _dbManager.LicenseRepository.UpdateLicenseAsync(license.Id, license);
            return true;
        }

        throw new ActiveLicenseNotFoundException();
    }

    /// <inheritdoc/> 
    public async Task<bool> CanUploadNonLicensedContentAsync()
    {
        await foreach (var license in _dbManager.LicenseRepository.IterateLicenseAsync(false))
        {
            if (!license.Active)
                continue;

            return true;
        }

        throw new ActiveLicenseNotFoundException();
    }
}