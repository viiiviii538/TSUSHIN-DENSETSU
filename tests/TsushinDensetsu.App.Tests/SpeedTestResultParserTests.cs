using TsushinDensetsu.App.Services;
using Xunit;

namespace TsushinDensetsu.App.Tests;

public class SpeedTestResultParserTests
{
    private readonly SpeedTestResultParser _parser = new();

    [Fact]
    public void Parse_ParsesStandardOutput()
    {
        const string output = """
Speedtest by Ookla
Latency: 11.01 ms   (jitter: 0.42 ms)
Download: 180.64 Mbps (data used: 262.8 MB)
Upload: 42.51 Mbps (data used: 63.6 MB)
""";

        var result = _parser.Parse(output);

        Assert.Equal(180.64, result.DownloadMbps, 2);
        Assert.Equal(42.51, result.UploadMbps, 2);
        Assert.Equal(11.01, result.PingMilliseconds, 2);
        Assert.Equal(0.42, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public void Parse_ParsesGbpsAndIdleLatencyLabel()
    {
        const string output = """
Speedtest by Ookla
Idle Latency: 5.5 ms   (jitter: 0.10 ms)
Download: 1.25 Gbps (data used: 500.0 MB)
Upload: 940.5 Mbps (data used: 300.0 MB)
""";

        var result = _parser.Parse(output);

        Assert.Equal(1250, result.DownloadMbps, 2);
        Assert.Equal(940.5, result.UploadMbps, 2);
        Assert.Equal(5.5, result.PingMilliseconds, 2);
        Assert.Equal(0.10, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public void Parse_ParsesCommaDecimalValues()
    {
        const string output = """
Speedtest by Ookla
Latency: 11,50 ms   (jitter: 0,25 ms)
Download: 123,45 Mbps (data used: 200,0 MB)
Upload: 67,89 Mbps (data used: 100,0 MB)
""";

        var result = _parser.Parse(output);

        Assert.Equal(123.45, result.DownloadMbps, 2);
        Assert.Equal(67.89, result.UploadMbps, 2);
        Assert.Equal(11.50, result.PingMilliseconds, 2);
        Assert.Equal(0.25, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public void Parse_ParsesKbpsAndTrailingJitter()
    {
        const string output = """
Speedtest by Ookla
Latency: 30.50 ms (0.42 ms jitter)
Download: 850.5 Kbps (data used: 10.0 MB)
Upload: 1.20 Mbps (data used: 15.0 MB)
""";

        var result = _parser.Parse(output);

        Assert.Equal(0.8505, result.DownloadMbps, 4);
        Assert.Equal(1.20, result.UploadMbps, 2);
        Assert.Equal(30.50, result.PingMilliseconds, 2);
        Assert.Equal(0.42, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public void Parse_DefaultsToZeroWhenJitterMissing()
    {
        const string output = """
Speedtest by Ookla
Latency: 18.25 ms
Download: 75.00 Mbps (data used: 100.0 MB)
Upload: 25.50 Mbps (data used: 50.0 MB)
""";

        var result = _parser.Parse(output);

        Assert.Equal(75.00, result.DownloadMbps, 2);
        Assert.Equal(25.50, result.UploadMbps, 2);
        Assert.Equal(18.25, result.PingMilliseconds, 2);
        Assert.Equal(0, result.JitterMilliseconds, 2);
        Assert.Equal(output, result.RawOutput);
    }

    [Fact]
    public void Parse_ThrowsWhenOutputIsEmpty()
    {
        var exception = Assert.Throws<SpeedTestException>(() => _parser.Parse(string.Empty));

        Assert.Equal("速度測定結果が取得できませんでした。", exception.Message);
    }

    [Fact]
    public void Parse_ThrowsWhenDownloadMissing()
    {
        const string output = "Download missing";

        var exception = Assert.Throws<SpeedTestException>(() => _parser.Parse(output));

        Assert.Equal("Download の値を解析できませんでした。", exception.Message);
    }

    [Fact]
    public void Parse_ThrowsWhenLatencyMissing()
    {
        const string output = "Download: 10 Mbps\nUpload: 10 Mbps";

        var exception = Assert.Throws<SpeedTestException>(() => _parser.Parse(output));

        Assert.Equal("Ping の値を解析できませんでした。", exception.Message);
    }
}
