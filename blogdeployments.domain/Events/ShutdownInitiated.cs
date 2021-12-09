using System.Net;

namespace blogdeployments.domain.Events;

public class ShutdownInitiated : IEvent
{
    public IPAddress[] Adresses { get; }

    public ShutdownInitiated(IPAddress[] adresses)
    {
        Adresses = adresses;
    }
}