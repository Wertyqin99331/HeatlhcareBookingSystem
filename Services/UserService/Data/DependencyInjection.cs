using Microsoft.AspNetCore.Identity;
using UserService.Data.Models;
using UserService.Data.Models.User;

namespace UserService.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<UserDbContext>();

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddIdentityCore<Patient>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();
            
        services.AddIdentityCore<Doctor>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}