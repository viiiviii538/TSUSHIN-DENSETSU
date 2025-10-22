using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TsushinDensetsu.App.Services;

public class SpeedTestService : ISpeedTestService
{
    private const string DefaultExecutableName = "speedtest.exe";
    private const string DefaultArguments = "--accept-license --accept-gdpr";

    private readonly IProcessRunner _processRunner;
    private readonly ISpeedTestResultParser _resultParser;
    private readonly string _executablePath;

    public SpeedTestService(IProcessRunner processRunner, ISpeedTestResultParser resultParser, string? executablePath = null)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _resultParser = resultParser ?? throw new ArgumentNullException(nameof(resultParser));
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
            return _resultParser.Parse(processResult.StandardOutput);
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
}
