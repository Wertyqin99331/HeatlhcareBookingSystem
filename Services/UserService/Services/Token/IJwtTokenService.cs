namespace UserService.Services.Token;

using CSharpFunctionalExtensions;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, IList<string> roles);
    Result<string, string> ReadValueFromClaims(string claimType);
}