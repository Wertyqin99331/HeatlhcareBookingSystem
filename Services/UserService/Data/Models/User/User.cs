using Microsoft.AspNetCore.Identity;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Models.User;

public class User: IdentityUser<Guid>
{
    protected User() {}
    
    protected User(string email, Name firstname, Name surname, DateOnly birthDate): base(email)
    {
        this.Firstname = firstname;
        this.Surname = surname;
        this.BirthDate = birthDate;
    }
    
    public Name Firstname { get; set; } = null!;
    public Name Surname { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
}