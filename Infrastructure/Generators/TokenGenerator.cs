using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.Generators;

public class TokenGenerator
{
    public static string Generate(JwtSettings jwt, Guid userId, string role, string issuer, string audience)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new("role", role)
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwt.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken accessToken = new
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(accessToken);
    }
}