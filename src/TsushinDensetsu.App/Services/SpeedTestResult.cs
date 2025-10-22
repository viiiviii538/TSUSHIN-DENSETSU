using System.Globalization;

namespace TsushinDensetsu.App.Services;

public sealed class SpeedTestResult
{
    public SpeedTestResult(double downloadMbps, double uploadMbps, double pingMilliseconds, double jitterMilliseconds, string rawOutput)
    {
        DownloadMbps = downloadMbps;
        UploadMbps = uploadMbps;
        PingMilliseconds = pingMilliseconds;
        JitterMilliseconds = jitterMilliseconds;
        RawOutput = rawOutput;
    }

    public double DownloadMbps { get; }

    public double UploadMbps { get; }

    public double PingMilliseconds { get; }

    public double JitterMilliseconds { get; }

    public string RawOutput { get; }

    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "Down: {0:F2} Mbps, Up: {1:F2} Mbps, Ping: {2:F2} ms, Jitter: {3:F2} ms",
            DownloadMbps,
            UploadMbps,
            PingMilliseconds,
            JitterMilliseconds);
    }
}
