using System;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Tests.TestHelpers;

public static class AsyncTestHelper
{
    public static async Task WaitForAsync(Func<bool> condition, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(1);
        var start = DateTime.UtcNow;

        while (!condition())
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException("Condition was not met within the allotted time.");
            }

            await Task.Delay(10);
        }
    }
}
