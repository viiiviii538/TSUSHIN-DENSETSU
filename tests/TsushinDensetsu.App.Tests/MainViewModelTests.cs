using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TsushinDensetsu.App.Domain;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class MainViewModelTests
{
    [Fact]
    public void Constructor_PopulatesDashboardPropertiesFromServices()
    {
        var expectedStatus = "すべて安全です";
        var expectedDevices = new List<NetworkDevice>
        {
            new("ルーター", "192.168.0.1", "ゲートウェイ"),
            new("サーバー", "192.168.0.10", "アプリケーション")
        };

        var securityScanService = new FakeSecurityScanService(expectedStatus);
        var networkTopologyService = new FakeNetworkTopologyService(expectedDevices);

        var speedTestViewModel = new StubSpeedTestViewModel();
        var securityScanViewModel = new StubSecurityScanViewModel(securityScanService);
        var networkTopologyViewModel = new StubNetworkTopologyViewModel(networkTopologyService);

        var viewModel = new MainViewModel(
            speedTestViewModel,
            securityScanViewModel,
            networkTopologyViewModel,
            securityScanService,
            networkTopologyService);

        Assert.Same(speedTestViewModel, viewModel.SpeedTest);
        Assert.Same(securityScanViewModel, viewModel.SecurityScan);
        Assert.Same(networkTopologyViewModel, viewModel.NetworkTopology);

        Assert.Equal(expectedStatus, viewModel.SecurityStatus);
        Assert.Equal(expectedDevices, viewModel.NetworkDevices);

        var expectedOverview = "ルーター (ゲートウェイ), サーバー (アプリケーション)";
        Assert.Equal(expectedOverview, viewModel.NetworkOverview);
    }

    private sealed class FakeSecurityScanService : ISecurityScanService
    {
        private readonly string _status;

        public FakeSecurityScanService(string status)
        {
            _status = status;
        }

        public string GetSecurityStatus() => _status;
    }

    private sealed class FakeNetworkTopologyService : INetworkTopologyService
    {
        public FakeNetworkTopologyService(IReadOnlyCollection<NetworkDevice> devices)
        {
            Devices = devices;
        }

        public IReadOnlyCollection<NetworkDevice> Devices { get; }

        public IReadOnlyCollection<NetworkDevice> GetNetworkDevices() => Devices;
    }

    private sealed class StubSpeedTestViewModel : SpeedTestViewModel
    {
        public StubSpeedTestViewModel()
            : base(new StubSpeedTestService())
        {
        }
    }

    private sealed class StubSpeedTestService : ISpeedTestService
    {
        public Task<SpeedTestResult> RunTestAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new SpeedTestResult(0, 0, 0, 0, string.Empty));
        }
    }

    private sealed class StubSecurityScanViewModel : SecurityScanViewModel
    {
        public StubSecurityScanViewModel(ISecurityScanService securityScanService)
            : base(securityScanService)
        {
        }
    }

    private sealed class StubNetworkTopologyViewModel : NetworkTopologyViewModel
    {
        public StubNetworkTopologyViewModel(INetworkTopologyService networkTopologyService)
            : base(networkTopologyService)
        {
        }
    }
}
