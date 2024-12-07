using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Models.User;

public class User: IdentityUser<Guid>
{
    public User() {}

    protected User(Name firstname, Name surname, DateOnly birthDate)
    {
        this.Firstname = firstname;
        this.Surname = surname;
        this.BirthDate = birthDate;
    }
    
    public Name Firstname { get; set; } = null!;
    public Name Surname { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ComplexProperty(c => c.Firstname, pb =>
        {
            pb.Property(fn => fn.Value)
                .HasMaxLength(Name.MAX_LENGTH)
                .HasColumnName("Firstname");
        });
        
        builder.ComplexProperty(c => c.Surname, pb =>
        {
            pb.Property(sn => sn.Value)
                .HasMaxLength(Name.MAX_LENGTH)
                .HasColumnName("Surname");
        });
    }
}