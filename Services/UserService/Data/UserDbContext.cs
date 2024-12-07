using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Models;
using UserService.Data.Models.User;
using UserService.Data.ValueObjects.User;

namespace UserService.Data;

public class UserDbContext(IConfiguration configuration): IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Patient> Clients { get; set; } = null!;
    public DbSet<Doctor> Doc { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("postgres"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        builder.HasDefaultSchema("UserService");
        SeedAdmin(builder);
    }

    private void SeedAdmin(ModelBuilder builder)
    {
        
    }
}