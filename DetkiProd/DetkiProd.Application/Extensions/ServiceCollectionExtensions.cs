using DetkiProd.Application.Interfaces;
using DetkiProd.Application.Mappings;
using DetkiProd.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DetkiProd.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DetkiProdUserToDto).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IVideoService, VideoService>();

        return services;
    }
}
