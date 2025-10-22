using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Services;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class SpeedTestServiceTests
{
    [Fact]
    public async Task RunTestAsync_ReturnsParserResult()
    {
        const string output = "test output";
        var expectedResult = new SpeedTestResult(100, 50, 10, 1, output);

        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(0, output, string.Empty));

        var parserMock = new Mock<ISpeedTestResultParser>(MockBehavior.Strict);
        parserMock
            .Setup(parser => parser.Parse(output))
            .Returns(expectedResult);

        var service = new SpeedTestService(runnerMock.Object, parserMock.Object);

        var result = await service.RunTestAsync();

        Assert.Same(expectedResult, result);
        parserMock.Verify(parser => parser.Parse(output), Times.Once);
    }

    [Fact]
    public async Task RunTestAsync_ThrowsWhenExitCodeIsNonZero()
    {
        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(1, string.Empty, "network error"));

        var parserMock = new Mock<ISpeedTestResultParser>(MockBehavior.Strict);

        var service = new SpeedTestService(runnerMock.Object, parserMock.Object);

        var exception = await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());
        Assert.Contains("network error", exception.Message);
        parserMock.Verify(parser => parser.Parse(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RunTestAsync_RethrowsSpeedTestExceptionFromParser()
    {
        const string output = "invalid";
        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(0, output, string.Empty));

        var parserMock = new Mock<ISpeedTestResultParser>(MockBehavior.Strict);
        parserMock
            .Setup(parser => parser.Parse(output))
            .Throws(new SpeedTestException("parser error"));

        var service = new SpeedTestService(runnerMock.Object, parserMock.Object);

        var exception = await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());
        Assert.Equal("parser error", exception.Message);
    }

    [Fact]
    public async Task RunTestAsync_WrapsUnexpectedParserExceptions()
    {
        const string output = "oops";
        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(0, output, string.Empty));

        var parserMock = new Mock<ISpeedTestResultParser>(MockBehavior.Strict);
        parserMock
            .Setup(parser => parser.Parse(output))
            .Throws(new InvalidOperationException("bad"));

        var service = new SpeedTestService(runnerMock.Object, parserMock.Object);

        var exception = await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());
        Assert.Equal("速度測定結果の解析に失敗しました。出力形式をご確認ください。", exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public async Task RunTestAsync_WrapsWin32ExceptionWithFriendlyMessage()
    {
        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Win32Exception());

        var parserMock = new Mock<ISpeedTestResultParser>(MockBehavior.Strict);

        var service = new SpeedTestService(runnerMock.Object, parserMock.Object);

        var exception = await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());

        Assert.Equal("速度測定ツール 'speedtest.exe' を起動できませんでした。インストール状況を確認してください。", exception.Message);
    }
}
