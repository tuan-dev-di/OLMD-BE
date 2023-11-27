using System;
using OptimizingLastMile.Configure;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Repositories.Base;
using OptimizingLastMile.Repositories.FeedBacks;
using OptimizingLastMile.Repositories.Notifications;
using OptimizingLastMile.Repositories.Orders;
using OptimizingLastMile.Services.Accounts;
using OptimizingLastMile.Services.Audits;
using OptimizingLastMile.Services.Auths;
using OptimizingLastMile.Services.Firebases;
using OptimizingLastMile.Services.Maps;
using OptimizingLastMile.Services.Orders;
using OptimizingLastMile.Services.Otps;

namespace OptimizingLastMile.Utils;

public static class RegisterDI
{
    public static void RegisterDIService(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Config
        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
        services.Configure<MapConfig>(configuration.GetSection("MapService"));
        services.Configure<OTPConfig>(configuration.GetSection("Otp"));
        services.Configure<ServiceAccountFirebaseConfig>(configuration.GetSection("Firebase"));
        services.Configure<FeatureConfig>(configuration.GetSection("FeatureConfig"));
        services.Configure<AlgorithmConfig>(configuration.GetSection("AlgorithmConfig"));

        // Service
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMapService, MapService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddSingleton<IFirebaseService, FirebaseService>();

        // Repository
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IFeedBackRepository, FeedBackRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // HttpClient
        services.AddHttpClient<IMapService, MapService>();
    }
}

