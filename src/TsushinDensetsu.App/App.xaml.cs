using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TsushinDensetsu.App.Infrastructure;
using TsushinDensetsu.App.Views;

namespace TsushinDensetsu.App;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddTsushinDensetsuApp();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
