using AutoMapper;
using blogdeployments.agent.Handler;
using blogdeployments.domain.Events;

namespace blogdeployments.agent.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShutdownRequested, Shutdown>();
    }
    
}