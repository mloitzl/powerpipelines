using AutoMapper;
using blogdeployments.domain;

namespace blogdeployments.handler;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateDeployment, Deployment>();
        CreateMap<Deployment, CreateDeployment>();
    }
}