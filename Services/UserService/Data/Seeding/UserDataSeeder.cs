using Microsoft.AspNetCore.Identity;
using UserService.Data.Models.User;
using UserService.Data.ValueObjects.User;

namespace UserService.Data.Seeding;

public static class UserDataSeeder
{
    public static async Task SeedAdmin(this IApplicationBuilder app)
    {
        var adminRoleId = Guid.Parse("a4de4771-679c-4e81-91e4-9cd286983ad2");
        var adminId = Guid.Parse("c93a3e45-03e8-4f76-ab68-a81fb625c1a2");
        
        using var scope = app.ApplicationServices.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>()
            {
                Id = adminRoleId,
                Name = UserRole.Admin.ToString(),
                NormalizedName = UserRole.Admin.ToString().ToUpper(),
                ConcurrencyStamp = adminRoleId.ToString()
            });
        }

        if (await userManager.FindByEmailAsync("admin@admin.ru") is null)
        {
            var admin = new User()
            {
                Id = adminId,
                Email = "admin@admin.ru",
                EmailConfirmed = true,
                Firstname = Name.Create("Admin").Value,
                Surname = Name.Create("Admin").Value,
                BirthDate = DateOnly.FromDateTime(DateTime.Now),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(admin, "Admin123");
            
            if (await userManager.IsInRoleAsync(admin, "Admin"))
            {
                return;
            }
            
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    public static async Task SeedRoles(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        UserRole[] rolesToAdd = [UserRole.Patient, UserRole.Doctor];
        foreach (var role in rolesToAdd)
        {
            if (!await roleManager.RoleExistsAsync(role.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>()
                {
                    Id = Guid.NewGuid(),
                    Name = role.ToString(),
                    NormalizedName = role.ToString().ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }
    }
}