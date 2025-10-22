using Microsoft.Extensions.DependencyInjection;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using TsushinDensetsu.App.Views;

namespace TsushinDensetsu.App.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTsushinDensetsuApp(this IServiceCollection services)
    {
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<ISpeedTestService, SpeedTestService>();
        services.AddSingleton<ISecurityScanService, SecurityScanService>();
        services.AddSingleton<INetworkTopologyService, NetworkTopologyService>();

        services.AddSingleton<SpeedTestViewModel>();
        services.AddSingleton<SecurityScanViewModel>();
        services.AddSingleton<NetworkTopologyViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        return services;
    }
}
