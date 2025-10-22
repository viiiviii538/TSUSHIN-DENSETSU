using System.Threading.Tasks;

namespace TsushinDensetsu.App.ViewModels;

public class SecurityScanViewModel : ViewModelBase
{
    private string _scanStatus = "セキュリティ診断の準備ができています。";

    public SecurityScanViewModel()
    {
        RunSecurityScanCommand = new AsyncRelayCommand(RunSecurityScanAsync);
    }

    public AsyncRelayCommand RunSecurityScanCommand { get; }

    public string ScanStatus
    {
        get => _scanStatus;
        private set => SetProperty(ref _scanStatus, value);
    }

    private Task RunSecurityScanAsync()
    {
        // TODO: Replace this placeholder with real security scan logic that gathers vulnerability details.
        ScanStatus = "診断機能は準備中です。今後、脆弱性チェックの結果がここに表示されます。";
        return Task.CompletedTask;
    }
}
