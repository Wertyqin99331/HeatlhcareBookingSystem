using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace UserService.Services.Token;

public class JwtTokenService(IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions): IJwtTokenService
{
    public string GenerateToken(Guid id, IList<string> roles)
    {
        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, id.ToString()),
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var token = GenerateToken(claims);

        return token;
    }

    public Result<string, string> ReadValueFromClaims(string claimType)
    {
        if (httpContextAccessor.HttpContext is null)
            return Result.Failure<string, string>("HttpContext is null");

        var claimValue = httpContextAccessor.HttpContext.User.FindFirstValue(claimType);
        if (claimValue is null)
            return Result.Failure<string, string>("Claim not found");
        
        return Result.Success<string, string>(claimValue);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Secret));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Value.Issuer,
            Audience = jwtOptions.Value.Audience,
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}