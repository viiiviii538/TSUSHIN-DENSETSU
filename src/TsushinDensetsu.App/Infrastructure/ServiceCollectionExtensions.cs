using Microsoft.Extensions.DependencyInjection;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using TsushinDensetsu.App.Views;

namespace TsushinDensetsu.App.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTsushinDensetsuApp(this IServiceCollection services)
    {
        services.AddSingleton<ISpeedTestService, SpeedTestService>();
        services.AddSingleton<ISecurityScanService, SecurityScanService>();
        services.AddSingleton<INetworkTopologyService, NetworkTopologyService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        return services;
    }
}
