using System.Collections.Generic;
using TsushinDensetsu.App.Domain;

namespace TsushinDensetsu.App.Services;

public interface INetworkTopologyService
{
    IReadOnlyCollection<NetworkDevice> GetNetworkDevices();
}
