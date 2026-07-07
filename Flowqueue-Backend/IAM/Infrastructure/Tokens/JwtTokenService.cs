using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Microsoft.IdentityModel.Tokens;

namespace Flowqueue_Backend.IAM.Infrastructure.Tokens;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var secret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            throw new InvalidOperationException("JWT secret must be configured and be at least 32 characters long.");

        var issuer = configuration["Jwt:Issuer"] ?? "Flowqueue-Backend";
        var audience = configuration["Jwt:Audience"] ?? "FlowQueue";
        var expiresInMinutes = int.TryParse(configuration["Jwt:ExpiresInMinutes"], out var parsedMinutes)
            ? parsedMinutes
            : 120;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Value),
            new Claim("fullName", user.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
