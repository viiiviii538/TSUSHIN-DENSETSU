using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task MainViewModel_ReflectsInjectedServiceData()
    {
        var expectedSecurityStatus = "通信網は安全です";
        var expectedDevices = new[]
        {
            new NetworkDevice("メインルーター", "192.168.10.1", "ゲートウェイ"),
            new NetworkDevice("家庭用NAS", "192.168.10.20", "ストレージ"),
            new NetworkDevice("リビングハブ", "192.168.10.30", "スイッチ")
        };

        var securityScanService = new FakeSecurityScanService(expectedSecurityStatus);
        var networkTopologyService = new FakeNetworkTopologyService(expectedDevices);
        var speedTestService = new FakeSpeedTestService();

        var securityScanViewModel = new SecurityScanViewModel(securityScanService);
        var networkTopologyViewModel = new NetworkTopologyViewModel(networkTopologyService);
        var speedTestViewModel = new SpeedTestViewModel(speedTestService);

        var mainViewModel = new MainViewModel(speedTestViewModel, securityScanViewModel, networkTopologyViewModel);

        securityScanViewModel.RunSecurityScanCommand.Execute(null);
        await WaitForAsync(() => mainViewModel.SecurityStatus == expectedSecurityStatus);

        networkTopologyViewModel.RefreshTopologyCommand.Execute(null);
        await WaitForAsync(() => mainViewModel.NetworkDevices.SequenceEqual(expectedDevices));

        var expectedOverview = string.Join(", ", expectedDevices.Select(device => $"{device.Name} ({device.Role})"));

        Assert.Equal(expectedSecurityStatus, mainViewModel.SecurityStatus);
        Assert.Equal(expectedDevices, mainViewModel.NetworkDevices);
        Assert.Equal(expectedOverview, mainViewModel.NetworkOverview);
    }

    private static async Task WaitForAsync(Func<bool> condition, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(1);
        var start = DateTime.UtcNow;

        while (!condition())
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException("Condition was not met within the allotted time.");
            }

            await Task.Delay(10);
        }
    }

    private sealed class FakeSecurityScanService : ISecurityScanService
    {
        private readonly string _status;

        public FakeSecurityScanService(string status)
        {
            _status = status;
        }

        public Task<string> GetSecurityStatusAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_status);
        }
    }

    private sealed class FakeNetworkTopologyService : INetworkTopologyService
    {
        private readonly IReadOnlyCollection<NetworkDevice> _devices;

        public FakeNetworkTopologyService(IReadOnlyCollection<NetworkDevice> devices)
        {
            _devices = devices;
        }

        public IReadOnlyCollection<NetworkDevice> GetNetworkDevices()
        {
            return _devices;
        }
    }

    private sealed class FakeSpeedTestService : ISpeedTestService
    {
        public Task<SpeedTestResult> RunTestAsync(CancellationToken cancellationToken = default)
        {
            var result = new SpeedTestResult(0, 0, 0, 0, "テストは実行されませんでした。");
            return Task.FromResult(result);
        }
    }
}
