using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Quizard.Application.LicensingService.Interfaces;
using Quizard.Core.Entities;

namespace Quizard.Application.LicensingApplication.Services;

/// <inheritdoc/>
public class LicenseKeyGenerator : ILicenseKeyGenerator
{
    /// <inheritdoc/>
    public string GenerateLicenseKey(string salt, IReadOnlyDictionary<string, object> claims)
    {
        var issuer = Assembly.GetExecutingAssembly().GetName().Name!;
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(salt));
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        
        var jwtClaims = new List<Claim>
        {
            new(nameof(License.ExpirationTime), claims["ExpirationTime"].ToString()!),
            new(nameof(License.GamesLeft), claims["GamesLeft"].ToString()!)
        };
        
        var expUnix = Convert.ToInt64(claims["ExpirationTime"]);
        var expires = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            claims: jwtClaims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}