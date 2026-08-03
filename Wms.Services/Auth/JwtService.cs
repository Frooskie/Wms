using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wms.Core.Entities;
using Wms.Core.Interfaces;
using Wms.Core.Options;

namespace Wms.Services.Auth;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettingsValue = jwtSettings.Value;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.FullName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettingsValue.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtSettingsValue.Issuer,
            jwtSettingsValue.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettingsValue.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}