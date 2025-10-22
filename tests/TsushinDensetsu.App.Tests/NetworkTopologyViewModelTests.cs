using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Domain;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class NetworkTopologyViewModelTests
{
    [Fact]
    public async Task RefreshTopologyCommand_WithDevices_SetsSummaryList()
    {
        var devices = new[]
        {
            new NetworkDevice("ホームルーター", "192.168.0.1", "ゲートウェイ"),
            new NetworkDevice("リビングハブ", "192.168.0.2", "スイッチ")
        };

        var serviceMock = new Mock<INetworkTopologyService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetNetworkDevices())
            .Returns(devices);

        var viewModel = new NetworkTopologyViewModel(serviceMock.Object);

        viewModel.RefreshTopologyCommand.Execute(null);

        var expectedSummary = "取得した機器一覧:" + Environment.NewLine +
            string.Join(Environment.NewLine, devices.Select(device => $"{device.Name}（役割: {device.Role}）"));

        await WaitForAsync(() => viewModel.TopologySummary == expectedSummary);

        Assert.Equal(expectedSummary, viewModel.TopologySummary);
        serviceMock.Verify(service => service.GetNetworkDevices(), Times.Once);
    }

    [Fact]
    public async Task RefreshTopologyCommand_WithNoDevices_ShowsEmptyMessage()
    {
        var serviceMock = new Mock<INetworkTopologyService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetNetworkDevices())
            .Returns(Array.Empty<NetworkDevice>());

        var viewModel = new NetworkTopologyViewModel(serviceMock.Object);

        viewModel.RefreshTopologyCommand.Execute(null);

        await WaitForAsync(() => viewModel.TopologySummary == "現在登録されているネットワーク機器はありません。");

        Assert.Equal("現在登録されているネットワーク機器はありません。", viewModel.TopologySummary);
        serviceMock.Verify(service => service.GetNetworkDevices(), Times.Once);
    }

    [Fact]
    public async Task RefreshTopologyCommand_ServiceThrows_ShowsErrorMessage()
    {
        var serviceMock = new Mock<INetworkTopologyService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetNetworkDevices())
            .Throws(new InvalidOperationException("boom"));

        var viewModel = new NetworkTopologyViewModel(serviceMock.Object);

        viewModel.RefreshTopologyCommand.Execute(null);

        await WaitForAsync(() => viewModel.TopologySummary == "構成情報の取得に失敗しました: boom");

        Assert.Equal("構成情報の取得に失敗しました: boom", viewModel.TopologySummary);
        serviceMock.Verify(service => service.GetNetworkDevices(), Times.Once);
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
