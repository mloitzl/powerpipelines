using AutoMapper;
using blogdeployments.domain;

namespace blogdeployments.repository.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeploymentDocument, Deployment>();
        CreateMap<Deployment, DeploymentDocument>();
        CreateMap<ClusterPowerStatusDocument, ClusterPowerStatus>();
        CreateMap<ClusterPowerStatus, ClusterPowerStatusDocument>();
    }
}