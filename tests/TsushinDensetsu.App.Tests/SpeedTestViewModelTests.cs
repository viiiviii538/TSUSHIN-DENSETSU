using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.ViewModels;
using Xunit;
using static TsushinDensetsu.App.Tests.TestHelpers.AsyncTestHelper;

namespace TsushinDensetsu.App.Tests;

public class SpeedTestViewModelTests
{
    [Fact]
    public async Task StartTestCommand_CompletesSuccessfully_UpdatesDisplaysAndStatus()
    {
        var result = new SpeedTestResult(123.456, 78.9, 12.345, 0.987, "raw output");
        var completionSource = new TaskCompletionSource<SpeedTestResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        var serviceMock = new Mock<ISpeedTestService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.RunTestAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken _) => completionSource.Task);

        var viewModel = new SpeedTestViewModel(serviceMock.Object);

        Assert.True(viewModel.StartTestCommand.CanExecute(null));

        viewModel.StartTestCommand.Execute(null);

        await WaitForAsync(() => viewModel.IsRunning);

        Assert.True(viewModel.IsRunning);
        Assert.False(viewModel.StartTestCommand.CanExecute(null));

        completionSource.SetResult(result);

        await WaitForAsync(() => !viewModel.IsRunning);

        Assert.False(viewModel.IsRunning);
        Assert.Equal("123.46 Mbps", viewModel.DownloadDisplay);
        Assert.Equal("78.90 Mbps", viewModel.UploadDisplay);
        Assert.Equal("12.35 ms", viewModel.PingDisplay);
        Assert.Equal("0.99 ms", viewModel.JitterDisplay);
        Assert.Equal("速度測定が完了しました。", viewModel.StatusMessage);
        Assert.True(viewModel.StartTestCommand.CanExecute(null));

        serviceMock.Verify(service => service.RunTestAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartTestCommand_SetsFriendlyErrorMessageForSpeedTestException()
    {
        var serviceMock = new Mock<ISpeedTestService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.RunTestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new SpeedTestException("network issue"));

        var viewModel = new SpeedTestViewModel(serviceMock.Object);

        viewModel.StartTestCommand.Execute(null);

        await WaitForAsync(() => viewModel.StatusMessage == "速度測定に失敗しました: network issue");

        Assert.False(viewModel.IsRunning);
        Assert.Equal("速度測定に失敗しました: network issue", viewModel.StatusMessage);
        Assert.True(viewModel.StartTestCommand.CanExecute(null));

        serviceMock.Verify(service => service.RunTestAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartTestCommand_SetsFriendlyErrorMessageForUnexpectedException()
    {
        var serviceMock = new Mock<ISpeedTestService>(MockBehavior.Strict);
        serviceMock
            .Setup(service => service.RunTestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var viewModel = new SpeedTestViewModel(serviceMock.Object);

        viewModel.StartTestCommand.Execute(null);

        await WaitForAsync(() => viewModel.StatusMessage == "予期しないエラーが発生しました: boom");

        Assert.False(viewModel.IsRunning);
        Assert.Equal("予期しないエラーが発生しました: boom", viewModel.StatusMessage);
        Assert.True(viewModel.StartTestCommand.CanExecute(null));

        serviceMock.Verify(service => service.RunTestAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
