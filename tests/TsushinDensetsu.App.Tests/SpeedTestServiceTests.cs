using System.Threading;
using System.Threading.Tasks;
using Moq;
using TsushinDensetsu.App.Services;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class SpeedTestServiceTests
{
    [Fact]
    public async Task RunTestAsync_ParsesStandardOutput()
    {
        const string output = """
Speedtest by Ookla
Latency: 11.01 ms   (jitter: 0.42 ms)
Download: 180.64 Mbps (data used: 262.8 MB)
Upload: 42.51 Mbps (data used: 63.6 MB)
""";

        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(0, output, string.Empty));

        var service = new SpeedTestService(runnerMock.Object);

        var result = await service.RunTestAsync();

        Assert.Equal(180.64, result.DownloadMbps, 2);
        Assert.Equal(42.51, result.UploadMbps, 2);
        Assert.Equal(11.01, result.PingMilliseconds, 2);
        Assert.Equal(0.42, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public async Task RunTestAsync_ThrowsWhenExitCodeIsNonZero()
    {
        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(1, string.Empty, "network error"));

        var service = new SpeedTestService(runnerMock.Object);

        var exception = await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());
        Assert.Contains("network error", exception.Message);
    }

    [Fact]
    public async Task RunTestAsync_ThrowsWhenParsingFails()
    {
        const string output = "unexpected format";

        var runnerMock = new Mock<IProcessRunner>(MockBehavior.Strict);
        runnerMock
            .Setup(runner => runner.RunAsync("speedtest.exe", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessRunResult(0, output, string.Empty));

        var service = new SpeedTestService(runnerMock.Object);

        await Assert.ThrowsAsync<SpeedTestException>(() => service.RunTestAsync());
    }
}
