namespace TsushinDensetsu.App.Services;

public interface ISpeedTestResultParser
{
    SpeedTestResult Parse(string output);
}
