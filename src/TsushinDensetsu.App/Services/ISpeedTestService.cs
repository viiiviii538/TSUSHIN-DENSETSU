using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public interface ISpeedTestService
{
    Task<SpeedTestResult> RunTestAsync(CancellationToken cancellationToken = default);
}
