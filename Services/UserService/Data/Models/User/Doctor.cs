using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Models.User;

public sealed class Doctor : User
{
    private Doctor()
    {}
    
    private Doctor(string email, Name firstname, Name surname, DateOnly birthDate) : base(email, firstname, surname,
        birthDate)
    {
    }

    public List<DoctorSpecialization> Specializations { get; private set; } = [];

    public static Result<Doctor, string> Create(string email, string firstname, string surname, DateOnly birthDate)
    {
        var firstnameResult = Name.Create(firstname);
        if (firstnameResult.IsFailure)
        {
            return firstnameResult.Error;
        }

        var surnameResult = Name.Create(surname);
        if (surnameResult.IsFailure)
        {
            return surnameResult.Error;
        }

        return new Doctor(email, firstnameResult.Value, surnameResult.Value, birthDate);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors").HasKey(c => c.Id);

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
        
        builder.HasMany(d => d.Specializations)
            .WithMany(s => s.Doctors)
            .UsingEntity(j => j.ToTable("DoctorsSpecializations"));
    }
}