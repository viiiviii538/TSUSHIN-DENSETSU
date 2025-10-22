using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TsushinDensetsu.App.Services;
using TsushinDensetsu.App.Domain;

namespace TsushinDensetsu.App.ViewModels;

public class NetworkTopologyViewModel : ViewModelBase
{
    private readonly INetworkTopologyService _networkTopologyService;
    private string _topologySummary = "回線構成図を表示する準備ができています。";
    private IReadOnlyList<NetworkDevice> _devices = Array.Empty<NetworkDevice>();

    public NetworkTopologyViewModel(INetworkTopologyService networkTopologyService)
    {
        _networkTopologyService = networkTopologyService ?? throw new ArgumentNullException(nameof(networkTopologyService));
        RefreshTopologyCommand = new AsyncRelayCommand(RefreshTopologyAsync);
    }

    public AsyncRelayCommand RefreshTopologyCommand { get; }

    public IReadOnlyList<NetworkDevice> Devices
    {
        get => _devices;
        private set => SetProperty(ref _devices, value);
    }

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
            var normalizedDevices = (devices ?? Array.Empty<NetworkDevice>()).ToList();
            Devices = normalizedDevices;

            if (Devices.Count == 0)
            {
                TopologySummary = "現在登録されているネットワーク機器はありません。";
                return;
            }

            var entries = Devices
                .Select(device => $"{device.Name}（役割: {device.Role}）");

            TopologySummary = "取得した機器一覧:" + Environment.NewLine + string.Join(Environment.NewLine, entries);
        }
        catch (Exception ex)
        {
            Devices = Array.Empty<NetworkDevice>();
            TopologySummary = $"構成情報の取得に失敗しました: {ex.Message}";
        }
    }
}
