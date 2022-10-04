using AutoMapper;
using blogdeployments.domain.Events;

namespace blogdeployments.dockeragent.Model;

public class MappingProfile: Profile
{

    public MappingProfile()
    {
        CreateMap<PowerOnRequested, StartContainers>();
        CreateMap<ShutdownCompleted, StopContainers>();
    }
}