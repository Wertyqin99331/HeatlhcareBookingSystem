using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Models.User;

public class Patient: User
{
    private Patient()
    {}
    
    private Patient(string email, Name firstname, Name surname, DateOnly birthDate) : base(email, firstname, surname, birthDate)
    {
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
        builder.ToTable("Patients").HasKey(c => c.Id);

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