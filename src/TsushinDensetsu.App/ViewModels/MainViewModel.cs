using System.Collections.Generic;
using System.Linq;
using TsushinDensetsu.App.Domain;
using TsushinDensetsu.App.Services;

namespace TsushinDensetsu.App.ViewModels;

public class MainViewModel
{
    public MainViewModel(
        ISpeedTestService speedTestService,
        ISecurityScanService securityScanService,
        INetworkTopologyService networkTopologyService)
    {
        WelcomeMessage = "ようこそ、通信伝説ダッシュボードへ！";
        SpeedTestSummary = speedTestService.GetSpeedSummary();
        SecurityStatus = securityScanService.GetSecurityStatus();
        NetworkDevices = networkTopologyService.GetNetworkDevices();
        NetworkOverview = string.Join(", ", NetworkDevices.Select(device => $"{device.Name} ({device.Role})"));
    }

    public string WelcomeMessage { get; }

    public string SpeedTestSummary { get; }

    public string SecurityStatus { get; }

    public IReadOnlyCollection<NetworkDevice> NetworkDevices { get; }

    public string NetworkOverview { get; }
}
