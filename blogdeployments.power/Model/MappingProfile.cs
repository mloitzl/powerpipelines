using AutoMapper;
using blogdeployments.domain.Events;
using blogdeployments.power.Handler;

namespace blogdeployments.power.Model;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<PowerOnRequested, PowerOn>();
        CreateMap<ShutdownInitiated, PowerOff>();
    }
}