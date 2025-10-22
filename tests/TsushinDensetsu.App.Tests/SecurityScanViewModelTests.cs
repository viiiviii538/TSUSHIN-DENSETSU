using System;
using System.Threading;
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
        const string initialStatus = "セキュリティ診断の準備ができています。";
        const string runningStatus = "診断を実行しています...";
        const string completedStatus = "全て安全です";

        var serviceMock = new Mock<ISecurityScanService>(MockBehavior.Strict);
        var statusSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        serviceMock
            .Setup(service => service.GetSecurityStatusAsync(It.IsAny<CancellationToken>()))
            .Returns(statusSource.Task);

        var viewModel = new SecurityScanViewModel(serviceMock.Object);

        Assert.Equal(initialStatus, viewModel.ScanStatus);

        viewModel.RunSecurityScanCommand.Execute(null);

        await WaitForAsync(() => viewModel.ScanStatus == runningStatus);

        statusSource.SetResult(completedStatus);

        await WaitForAsync(() => viewModel.ScanStatus == completedStatus);

        serviceMock.Verify(service => service.GetSecurityStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunSecurityScanCommand_ServiceThrows_SetsErrorStatus()
    {
        var serviceMock = new Mock<ISecurityScanService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.GetSecurityStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var viewModel = new SecurityScanViewModel(serviceMock.Object);

        viewModel.RunSecurityScanCommand.Execute(null);

        const string runningStatus = "診断を実行しています...";
        const string expectedErrorStatus = "診断中にエラーが発生しました: boom";

        await WaitForAsync(() => viewModel.ScanStatus == runningStatus);
        await WaitForAsync(() => viewModel.ScanStatus == expectedErrorStatus);

        serviceMock.Verify(service => service.GetSecurityStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
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
