using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Domain;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class MainViewModelTests
{
    [Fact]
    public async Task MainDashboard_TracksChildViewModelUpdates()
    {
        var speedTestServiceMock = new Mock<ISpeedTestService>(MockBehavior.Strict);
        var securityScanServiceMock = new Mock<ISecurityScanService>(MockBehavior.Strict);
        var topologyServiceMock = new Mock<INetworkTopologyService>(MockBehavior.Strict);

        securityScanServiceMock
            .Setup(service => service.GetSecurityStatus())
            .Returns("全て安全です");

        var devices = new[]
        {
            new NetworkDevice("ホームルーター", "192.168.0.1", "ゲートウェイ"),
            new NetworkDevice("リビングハブ", "192.168.0.2", "スイッチ")
        };

        topologyServiceMock
            .Setup(service => service.GetNetworkDevices())
            .Returns(devices);

        var speedTestViewModel = new SpeedTestViewModel(speedTestServiceMock.Object);
        var securityScanViewModel = new SecurityScanViewModel(securityScanServiceMock.Object);
        var networkTopologyViewModel = new NetworkTopologyViewModel(topologyServiceMock.Object);

        var mainViewModel = new MainViewModel(speedTestViewModel, securityScanViewModel, networkTopologyViewModel);

        Assert.Equal(securityScanViewModel.ScanStatus, mainViewModel.SecurityStatus);
        Assert.Empty(mainViewModel.NetworkDevices);
        Assert.Equal("現在登録されているネットワーク機器はありません。", mainViewModel.NetworkOverview);

        securityScanViewModel.RunSecurityScanCommand.Execute(null);
        await WaitForAsync(() => mainViewModel.SecurityStatus == "全て安全です");

        networkTopologyViewModel.RefreshTopologyCommand.Execute(null);
        await WaitForAsync(() => mainViewModel.NetworkDevices.Count == devices.Length);

        var expectedOverview = string.Join(", ", devices.Select(device => $"{device.Name} ({device.Role})"));

        Assert.Equal("全て安全です", mainViewModel.SecurityStatus);
        Assert.Equal(devices, mainViewModel.NetworkDevices);
        Assert.Equal(expectedOverview, mainViewModel.NetworkOverview);

        securityScanServiceMock.Verify(service => service.GetSecurityStatus(), Times.Once);
        topologyServiceMock.Verify(service => service.GetNetworkDevices(), Times.Once);
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
}
