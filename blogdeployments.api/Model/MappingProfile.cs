using AutoMapper;
using blogdeployments.api.Handler;
using blogdeployments.domain;

namespace blogdeployments.api.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDeployment, Deployment>();
        CreateMap<CompleteDeployment, Deployment>();
        CreateMap<CreateDeployment, Deployment>();
        CreateMap<DeploymentViewModel, RegisterDeployment>();
        CreateMap<DeploymentViewModel, CompleteDeployment>();
        CreateMap<Deployment, CreateDeployment>();
    }
}
