using System.Collections.Generic;
using TsushinDensetsu.App.Domain;

namespace TsushinDensetsu.App.Services;

public class NetworkTopologyService : INetworkTopologyService
{
    public IReadOnlyCollection<NetworkDevice> GetNetworkDevices()
    {
        return new List<NetworkDevice>
        {
            new("Core Router", "10.0.0.1", "Backbone"),
            new("Distribution Switch", "10.0.10.2", "Aggregation"),
            new("Edge Firewall", "10.0.20.5", "Security"),
            new("Monitoring Server", "10.0.30.12", "Operations")
        };
    }
}
