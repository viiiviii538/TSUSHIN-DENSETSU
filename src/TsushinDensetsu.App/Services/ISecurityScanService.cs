using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public interface ISecurityScanService
{
    Task<string> GetSecurityStatusAsync(CancellationToken cancellationToken = default);
}
