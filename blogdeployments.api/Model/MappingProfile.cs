using AutoMapper;
using blogdeployments.api.Handler;
using blogdeployments.domain;

namespace blogdeployments.api.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDeployment, Deployment>();
        CreateMap<CreateDeployment, Deployment>();
        CreateMap<DeploymentViewModel, RegisterDeployment>();
        CreateMap<Deployment, CreateDeployment>();
    }
}
