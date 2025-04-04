using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Web.Api.Interfaces;
using Web.Api.Models;

namespace Web.Api.Services;
sealed class TokenService(IConfiguration configuration, IApiDataContext dbContext) : ITokenService
{
    public async Task<string> CreateToken(User user, CancellationToken cancellationToken)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<string> roleNames = await dbContext.UserRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role!.Name)
            .ToListAsync(cancellationToken);

        List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
        ];
        claims.AddRange(roleNames.Select(x => new Claim(ClaimTypes.Role, x)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}