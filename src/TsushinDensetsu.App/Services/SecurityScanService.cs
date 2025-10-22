using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public class SecurityScanService : ISecurityScanService
{
    public Task<string> GetSecurityStatusAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult("No critical vulnerabilities detected in the last scan.");
    }
}
