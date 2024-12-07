using System.ComponentModel.DataAnnotations;

namespace UserService.Services.Token;

public class JwtOptions
{
    public const string SECTION_NAME = "Jwt";
    
    [Required] public string Secret { get; init; } = null!;
    
    [Required] public string Issuer { get; init; } = null!;
    
    [Required] public string Audience { get; init; } = null!;
}