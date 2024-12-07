namespace UserService.Services.Token;

public static class DependencyInjection
{
    public static IServiceCollection AddJwtTokenServices(this IServiceCollection services)
    {
        services.AddTransient<IJwtTokenService, JwtTokenService>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SECTION_NAME);

        return services;
    }
}