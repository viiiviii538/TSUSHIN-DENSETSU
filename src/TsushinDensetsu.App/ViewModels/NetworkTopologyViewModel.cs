using System;
using System.Linq;
using System.Threading.Tasks;
using TsushinDensetsu.App.Services;

namespace TsushinDensetsu.App.ViewModels;

public class NetworkTopologyViewModel : ViewModelBase
{
    private readonly INetworkTopologyService _networkTopologyService;
    private string _topologySummary = "回線構成図を表示する準備ができています。";

    public NetworkTopologyViewModel(INetworkTopologyService networkTopologyService)
    {
        _networkTopologyService = networkTopologyService ?? throw new ArgumentNullException(nameof(networkTopologyService));
        RefreshTopologyCommand = new AsyncRelayCommand(RefreshTopologyAsync);
    }

    public AsyncRelayCommand RefreshTopologyCommand { get; }

    public string TopologySummary
    {
        get => _topologySummary;
        private set => SetProperty(ref _topologySummary, value);
    }

    private async Task RefreshTopologyAsync()
    {
        try
        {
            TopologySummary = "ネットワーク構成を更新しています…";

            var devices = await Task.Run(() => _networkTopologyService.GetNetworkDevices());

            if (devices.Count == 0)
            {
                TopologySummary = "現在登録されているネットワーク機器はありません。";
                return;
            }

            var entries = devices
                .Select(device => $"{device.Name}（役割: {device.Role}）");

            TopologySummary = "取得した機器一覧:" + Environment.NewLine + string.Join(Environment.NewLine, entries);
        }
        catch (Exception ex)
        {
            TopologySummary = $"構成情報の取得に失敗しました: {ex.Message}";
        }
    }
}
