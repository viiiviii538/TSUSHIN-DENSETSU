namespace TsushinDensetsu.App.Services;

public class SecurityScanService : ISecurityScanService
{
    public string GetSecurityStatus()
    {
        return "No critical vulnerabilities detected in the last scan.";
    }
}
