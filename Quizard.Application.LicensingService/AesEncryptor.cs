using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quizard.Core.Interfaces;

namespace Quizard.Application.LicensingService;

/// <inheritdoc />
public class AesEncryptor : IAesEncryptor
{
    private readonly byte[] _key;
    private readonly ILogger<AesEncryptor> _logger;

    public AesEncryptor(IConfiguration config, ILogger<AesEncryptor> logger)
    {
        _logger = logger;
        var key = config["AesKey"] ?? throw new ArgumentNullException(nameof(config));
        if (key.Length != 32)
            throw new ArgumentException("Ключ AES должен быть 32 байта для AES-256");
        _key = Convert.FromBase64String(key);
        
        _logger.LogDebug("Сервис лицензирования проинициализирован");
    }
    
    /// <inheritdoc />
    public async Task<string> EncryptAsync(string plainText)
    {
#if DEBUG
        _logger.LogTrace("Выполняется шифрование строки - {plain}", plainText);
#endif
        
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        
        await using var ms = new MemoryStream();
        await ms.WriteAsync(aes.IV.AsMemory(0, aes.IV.Length));

        await using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        await using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            await sw.WriteAsync(plainText);
        }
        
        _logger.LogTrace("Данные зашифрованы");
        var cipherBytes = ms.ToArray();
        return Convert.ToBase64String(cipherBytes);
    }

    /// <inheritdoc />   
    public async Task<string> DecryptAsync(string cipherText)
    {
        _logger.LogTrace("Выполняется расшифровка строки");
        var allBytes = Convert.FromBase64String(cipherText);

        await using var ms = new MemoryStream(allBytes);
        using var aes = Aes.Create();
        aes.Key = _key;
        
        var iv = new byte[aes.BlockSize / 8];
        var unused = await ms.ReadAsync(iv);
        aes.IV = iv;
        
        await using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return await sr.ReadToEndAsync();
    }
}