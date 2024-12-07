using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Models.User;

public sealed class Patient: User
{
    private Patient()
    {}
    
    private Patient(string email, Name firstname, Name surname, DateOnly birthDate) : base(firstname, surname, birthDate)
    {
        this.Email = email;
        this.NormalizedEmail = email.ToUpper();
        this.UserName = email;
    }

    public static Result<Patient, string> Create(string email, string firstname, string surname, DateOnly birthDate)
    {
        var firstnameResult = Name.Create(firstname);
        if (firstnameResult.IsFailure)
            return firstnameResult.Error;
        
        var surnameResult = Name.Create(surname);
        if (surnameResult.IsFailure)
            return surnameResult.Error;
        
        return new Patient(email, firstnameResult.Value, surnameResult.Value, birthDate);
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
    }
}