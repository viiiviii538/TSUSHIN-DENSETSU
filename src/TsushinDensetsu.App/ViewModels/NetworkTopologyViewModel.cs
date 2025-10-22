using System.Threading.Tasks;

namespace TsushinDensetsu.App.ViewModels;

public class NetworkTopologyViewModel : ViewModelBase
{
    private string _topologySummary = "回線構成図を表示する準備ができています。";

    public NetworkTopologyViewModel()
    {
        RefreshTopologyCommand = new AsyncRelayCommand(RefreshTopologyAsync);
    }

    public AsyncRelayCommand RefreshTopologyCommand { get; }

    public string TopologySummary
    {
        get => _topologySummary;
        private set => SetProperty(ref _topologySummary, value);
    }

    private Task RefreshTopologyAsync()
    {
        // TODO: Implement topology discovery to render diagrams generated from live network data.
        TopologySummary = "構成図の自動生成機能は開発予定です。将来はネットワーク機器の接続関係が図として表示されます。";
        return Task.CompletedTask;
    }
}
