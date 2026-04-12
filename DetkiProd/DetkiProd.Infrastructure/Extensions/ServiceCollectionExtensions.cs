using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Enums;
using DetkiProd.Domain.Repositories;
using DetkiProd.Domain.Security;
using DetkiProd.Infrastructure.Authentication;
using DetkiProd.Infrastructure.Cache.Repositories;
using DetkiProd.Infrastructure.Persistence;
using DetkiProd.Infrastructure.Persistence.Repositories;
using DetkiProd.Infrastructure.Security;
using DetkiProd.Infrastructure.Telegram;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Telegram.Bot;

namespace DetkiProd.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptionsSection = configuration.GetSection("JwtOptions");
        var jwtOptions = jwtOptionsSection.Get<JwtOptions>();

        if (jwtOptions == null)
            throw new InvalidOperationException("JwtOptions configuration is missing");

        var adminOptionsSection = configuration.GetSection("AdminOptions");
        var adminOptions = adminOptionsSection.Get<AdminOptions>();

        if (adminOptions == null)
            throw new InvalidOperationException("AdminOptions configuration is missing");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(nameof(UserRole.Admin)));
        });

        services.Configure<JwtOptions>(jwtOptionsSection);
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.Configure<AdminOptions>(adminOptionsSection);
        services.AddScoped<IAdminOptionsProvider, AdminOptionsProvider>();

        return services;
    }

    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DetkiProdContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IDetkiProdUserRepository, DetkiProdUserRepository>();
        services.AddScoped<IDetkiProdUserRoleRepository, DetkiProdUserRoleRepository>();
        services.AddScoped<IDetkiProdProjectRepository, DetkiProdProjectRepository>();

        return services;
    }

    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(
           _ => ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!));

        services.AddScoped<ICacheRepository, RedisCacheRepository>();

        return services;
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
    {
        var botToken = configuration["Telegram:BotToken"]
            ?? throw new InvalidOperationException(
                 "Telegram bot token not found in configuration. " +
                 "Please set 'Telegram:BotToken' in appsettings.json");

        var botApiLocalServerUrl = configuration["Telegram:BotApiLocalServerUrl"]
            ?? throw new InvalidOperationException(
                "Telegram bot api local server url not found in configuration. " +
                 "Please set 'Telegram:BotApiLocalServerUrl' in appsettings.json");

        var botClientOptions = new TelegramBotClientOptions(botToken, botApiLocalServerUrl);
        var botClient = new TelegramBotClient(botClientOptions, new HttpClient() { Timeout = TimeSpan.FromMinutes(10)});

        services.AddSingleton<ITelegramBotClient>(botClient);
        services.AddScoped<ITelegramUpdateHandler, TelegramUpdateHandler>();
        services.AddHostedService<TelegramBotHostedService>();

        return services;
    }
}
