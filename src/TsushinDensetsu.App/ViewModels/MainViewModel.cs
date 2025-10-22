using System.Collections.Generic;
using System.Linq;
using TsushinDensetsu.App.Domain;
using TsushinDensetsu.App.Services;

namespace TsushinDensetsu.App.ViewModels;

public class MainViewModel
{
    public MainViewModel(
        SpeedTestViewModel speedTestViewModel,
        SecurityScanViewModel securityScanViewModel,
        NetworkTopologyViewModel networkTopologyViewModel,
        ISecurityScanService securityScanService,
        INetworkTopologyService networkTopologyService)
    {
        WelcomeMessage = "ようこそ、通信伝説ダッシュボードへ！";
        SpeedTest = speedTestViewModel;
        SecurityScan = securityScanViewModel;
        NetworkTopology = networkTopologyViewModel;
        SecurityStatus = securityScanService.GetSecurityStatus();
        NetworkDevices = networkTopologyService.GetNetworkDevices();
        NetworkOverview = string.Join(", ", NetworkDevices.Select(device => $"{device.Name} ({device.Role})"));
    }

    public string WelcomeMessage { get; }

    public SpeedTestViewModel SpeedTest { get; }

    public SecurityScanViewModel SecurityScan { get; }

    public NetworkTopologyViewModel NetworkTopology { get; }

    public string SecurityStatus { get; }

    public IReadOnlyCollection<NetworkDevice> NetworkDevices { get; }

    public string NetworkOverview { get; }
}
