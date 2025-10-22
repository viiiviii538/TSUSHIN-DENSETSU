using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public interface IProcessRunner
{
    Task<ProcessRunResult> RunAsync(string fileName, string arguments, CancellationToken cancellationToken = default);
}
