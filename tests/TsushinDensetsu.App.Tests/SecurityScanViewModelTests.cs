using System;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class SecurityScanViewModelTests
{
    [Fact]
    public async Task RunSecurityScanCommand_UpdatesStatusThroughLifecycle()
    {
        var serviceMock = new Mock<ISecurityScanService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetSecurityStatus())
            .Returns("全て安全です");

        var viewModel = new SecurityScanViewModel(serviceMock.Object);

        Assert.Equal("セキュリティ診断の準備ができています。", viewModel.ScanStatus);

        viewModel.RunSecurityScanCommand.Execute(null);

        await WaitForAsync(() => viewModel.ScanStatus == "診断を実行しています...");
        await WaitForAsync(() => viewModel.ScanStatus == "全て安全です");

        serviceMock.Verify(service => service.GetSecurityStatus(), Times.Once);
    }

    [Fact]
    public async Task RunSecurityScanCommand_ServiceThrows_SetsErrorStatus()
    {
        var serviceMock = new Mock<ISecurityScanService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetSecurityStatus())
            .Throws(new InvalidOperationException("boom"));

        var viewModel = new SecurityScanViewModel(serviceMock.Object);

        viewModel.RunSecurityScanCommand.Execute(null);

        await WaitForAsync(() => viewModel.ScanStatus.StartsWith("診断を実行しています"));
        await WaitForAsync(() => viewModel.ScanStatus == "診断中にエラーが発生しました: boom");

        serviceMock.Verify(service => service.GetSecurityStatus(), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullSecurityScanService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SecurityScanViewModel(null!));
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
