using System;
using System.Threading;
using System.Threading.Tasks;
using TsushinDensetsu.App.Services;

namespace TsushinDensetsu.App.ViewModels;

public class SpeedTestViewModel : ViewModelBase
{
    private readonly ISpeedTestService _speedTestService;

    private string _downloadDisplay = "-";
    private string _uploadDisplay = "-";
    private string _pingDisplay = "-";
    private string _jitterDisplay = "-";
    private string _statusMessage = "計測ボタンを押すと速度測定を開始します。";
    private bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;

    public SpeedTestViewModel(ISpeedTestService speedTestService)
    {
        _speedTestService = speedTestService ?? throw new ArgumentNullException(nameof(speedTestService));
        StartTestCommand = new AsyncRelayCommand(StartTestAsync, () => !IsRunning);
    }

    public AsyncRelayCommand StartTestCommand { get; }

    public string DownloadDisplay
    {
        get => _downloadDisplay;
        private set => SetProperty(ref _downloadDisplay, value);
    }

    public string UploadDisplay
    {
        get => _uploadDisplay;
        private set => SetProperty(ref _uploadDisplay, value);
    }

    public string PingDisplay
    {
        get => _pingDisplay;
        private set => SetProperty(ref _pingDisplay, value);
    }

    public string JitterDisplay
    {
        get => _jitterDisplay;
        private set => SetProperty(ref _jitterDisplay, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (SetProperty(ref _isRunning, value))
            {
                StartTestCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private async Task StartTestAsync()
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        StatusMessage = "計測中です。しばらくお待ちください...";
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var result = await _speedTestService.RunTestAsync(_cancellationTokenSource.Token);
            DownloadDisplay = $"{result.DownloadMbps:F2} Mbps";
            UploadDisplay = $"{result.UploadMbps:F2} Mbps";
            PingDisplay = $"{result.PingMilliseconds:F2} ms";
            JitterDisplay = $"{result.JitterMilliseconds:F2} ms";
            StatusMessage = "速度測定が完了しました。";
        }
        catch (SpeedTestException ex)
        {
            StatusMessage = $"速度測定に失敗しました: {ex.Message}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"予期しないエラーが発生しました: {ex.Message}";
        }
        finally
        {
            var tokenSource = _cancellationTokenSource;
            _cancellationTokenSource = null;
            tokenSource?.Dispose();
            IsRunning = false;
        }
    }
}
