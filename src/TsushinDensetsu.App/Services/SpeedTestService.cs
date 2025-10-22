namespace TsushinDensetsu.App.Services;

public class SpeedTestService : ISpeedTestService
{
    public string GetSpeedSummary()
    {
        return "Download: 940 Mbps, Upload: 860 Mbps";
    }
}
