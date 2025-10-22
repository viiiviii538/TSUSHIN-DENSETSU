using System;

namespace TsushinDensetsu.App.Services;

public class SpeedTestException : Exception
{
    public SpeedTestException(string message)
        : base(message)
    {
    }

    public SpeedTestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
