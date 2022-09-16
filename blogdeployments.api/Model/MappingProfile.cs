using AutoMapper;
using blogdeployments.api.Handler;
using blogdeployments.domain;
using blogdeployments.handler;

namespace blogdeployments.api.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDeployment, Deployment>();
        CreateMap<CompleteDeployment, Deployment>();
        CreateMap<DeploymentViewModel, RegisterDeployment>();
        CreateMap<DeploymentViewModel, CompleteDeployment>();
        CreateMap<GetDeploymentsViewModel, GetDeployments>();
    }
}