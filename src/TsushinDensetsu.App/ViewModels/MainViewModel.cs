using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TsushinDensetsu.App.Domain;

namespace TsushinDensetsu.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _securityStatus = string.Empty;
    private IReadOnlyList<NetworkDevice> _networkDevices = Array.Empty<NetworkDevice>();
    private string _networkOverview = string.Empty;

    public MainViewModel(
        SpeedTestViewModel speedTestViewModel,
        SecurityScanViewModel securityScanViewModel,
        NetworkTopologyViewModel networkTopologyViewModel)
    {
        SpeedTest = speedTestViewModel ?? throw new ArgumentNullException(nameof(speedTestViewModel));
        SecurityScan = securityScanViewModel ?? throw new ArgumentNullException(nameof(securityScanViewModel));
        NetworkTopology = networkTopologyViewModel ?? throw new ArgumentNullException(nameof(networkTopologyViewModel));

        WelcomeMessage = "ようこそ、通信伝説ダッシュボードへ！";

        SecurityScan.PropertyChanged += OnSecurityScanPropertyChanged;
        NetworkTopology.PropertyChanged += OnNetworkTopologyPropertyChanged;

        UpdateSecurityStatus();
        UpdateNetworkSnapshot();
    }

    public string WelcomeMessage { get; }

    public SpeedTestViewModel SpeedTest { get; }

    public SecurityScanViewModel SecurityScan { get; }

    public NetworkTopologyViewModel NetworkTopology { get; }

    public string SecurityStatus
    {
        get => _securityStatus;
        private set => SetProperty(ref _securityStatus, value);
    }

    public IReadOnlyList<NetworkDevice> NetworkDevices
    {
        get => _networkDevices;
        private set => SetProperty(ref _networkDevices, value);
    }

    public string NetworkOverview
    {
        get => _networkOverview;
        private set => SetProperty(ref _networkOverview, value);
    }

    private void OnSecurityScanPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(SecurityScanViewModel.ScanStatus))
        {
            UpdateSecurityStatus();
        }
    }

    private void OnNetworkTopologyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(NetworkTopologyViewModel.Devices))
        {
            UpdateNetworkSnapshot();
        }
    }

    private void UpdateSecurityStatus()
    {
        SecurityStatus = SecurityScan.ScanStatus;
    }

    private void UpdateNetworkSnapshot()
    {
        var devices = NetworkTopology.Devices ?? Array.Empty<NetworkDevice>();
        NetworkDevices = devices;
        NetworkOverview = devices.Count == 0
            ? "現在登録されているネットワーク機器はありません。"
            : string.Join(", ", devices.Select(device => $"{device.Name} ({device.Role})"));
    }
}
