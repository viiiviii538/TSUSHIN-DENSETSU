using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public class SpeedTestService : ISpeedTestService
{
    private const string DefaultExecutableName = "speedtest.exe";
    private const string DefaultArguments = "--accept-license --accept-gdpr";

    private const RegexOptions DefaultRegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    private static readonly Regex LatencyRegex = new(@"(Latency|Idle Latency):\s*(?<value>[\d\.,]+)\s*ms(?<rest>[^\n]*)", DefaultRegexOptions);
    private static readonly Regex JitterWithLabelRegex = new(@"jitter[:\s]*(?<jitter>[\d\.,]+)\s*ms", DefaultRegexOptions);
    private static readonly Regex JitterTrailingRegex = new(@"(?<jitter>[\d\.,]+)\s*ms\s*jitter", DefaultRegexOptions);

    private readonly IProcessRunner _processRunner;
    private readonly string _executablePath;

    public SpeedTestService(IProcessRunner processRunner, string? executablePath = null)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _executablePath = string.IsNullOrWhiteSpace(executablePath) ? DefaultExecutableName : executablePath;
    }

    public async Task<SpeedTestResult> RunTestAsync(CancellationToken cancellationToken = default)
    {
        ProcessRunResult processResult;

        try
        {
            processResult = await _processRunner.RunAsync(_executablePath, DefaultArguments, cancellationToken).ConfigureAwait(false);
        }
        catch (Win32Exception ex)
        {
            throw new SpeedTestException($"速度測定ツール '{_executablePath}' を起動できませんでした。インストール状況を確認してください。", ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new SpeedTestException("速度測定がキャンセルされました。", ex);
        }
        catch (Exception ex)
        {
            throw new SpeedTestException("速度測定ツールの実行中に予期しないエラーが発生しました。", ex);
        }

        if (processResult.ExitCode != 0)
        {
            var errorMessage = string.IsNullOrWhiteSpace(processResult.StandardError)
                ? processResult.StandardOutput
                : processResult.StandardError;

            throw new SpeedTestException($"速度測定ツールがエラー終了しました: {errorMessage.Trim()}");
        }

        try
        {
            return ParseOutput(processResult.StandardOutput);
        }
        catch (SpeedTestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SpeedTestException("速度測定結果の解析に失敗しました。出力形式をご確認ください。", ex);
        }
    }

    private static SpeedTestResult ParseOutput(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            throw new SpeedTestException("速度測定結果が取得できませんでした。");
        }

        var download = ParseBandwidth(output, "Download");
        var upload = ParseBandwidth(output, "Upload");
        var (latency, jitter) = ParseLatency(output);

        return new SpeedTestResult(download, upload, latency, jitter, output);
    }

    private static double ParseBandwidth(string output, string label)
    {
        var pattern = $"{Regex.Escape(label)}:\\s*(?<value>[\\d\\.,]+)\\s*(?<unit>[GMK]?bps)";
        var regex = new Regex(pattern, DefaultRegexOptions);
        var match = regex.Match(output);

        if (!match.Success)
        {
            throw new SpeedTestException($"{label} の値を解析できませんでした。");
        }

        var value = ParseDouble(match.Groups["value"].Value);
        var unit = match.Groups["unit"].Value.ToLowerInvariant();

        return unit switch
        {
            "gbps" => value * 1000,
            "mbps" => value,
            "kbps" => value / 1000,
            _ => value
        };
    }

    private static (double latency, double jitter) ParseLatency(string output)
    {
        var match = LatencyRegex.Match(output);

        if (!match.Success)
        {
            throw new SpeedTestException("Ping の値を解析できませんでした。");
        }

        var latency = ParseDouble(match.Groups["value"].Value);
        var jitter = 0d;

        var rest = match.Groups["rest"].Value;
        if (!string.IsNullOrWhiteSpace(rest))
        {
            var jitterMatch = JitterWithLabelRegex.Match(rest);
            if (!jitterMatch.Success)
            {
                jitterMatch = JitterTrailingRegex.Match(rest);
            }

            if (jitterMatch.Success)
            {
                jitter = ParseDouble(jitterMatch.Groups["jitter"].Value);
            }
        }

        return (latency, jitter);
    }

    private static double ParseDouble(string value)
    {
        var normalized = value.Replace(',', '.');
        if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new SpeedTestException($"数値 '{value}' を解析できませんでした。");
        }

        return parsed;
    }
}
