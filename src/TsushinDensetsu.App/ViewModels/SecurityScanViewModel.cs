using System;
using System.Threading.Tasks;
using TsushinDensetsu.App.Services;

namespace TsushinDensetsu.App.ViewModels;

public class SecurityScanViewModel : ViewModelBase
{
    private readonly ISecurityScanService _securityScanService;
    private string _scanStatus = "セキュリティ診断の準備ができています。";

    public SecurityScanViewModel(ISecurityScanService securityScanService)
    {
        _securityScanService = securityScanService ?? throw new ArgumentNullException(nameof(securityScanService));
        RunSecurityScanCommand = new AsyncRelayCommand(RunSecurityScanAsync);
    }

    public AsyncRelayCommand RunSecurityScanCommand { get; }

    public string ScanStatus
    {
        get => _scanStatus;
        private set => SetProperty(ref _scanStatus, value);
    }

    private async Task RunSecurityScanAsync()
    {
        try
        {
            ScanStatus = "診断を実行しています...";
            var status = await _securityScanService.GetSecurityStatusAsync();
            ScanStatus = status;
        }
        catch (Exception ex)
        {
            ScanStatus = $"診断中にエラーが発生しました: {ex.Message}";
        }
    }
}
