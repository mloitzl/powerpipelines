using AutoMapper;
using blogdeployments.domain;

namespace blogdeployments.repository.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<blogdeployments.repository.DeploymentDocument, Deployment>();
        CreateMap<Deployment, blogdeployments.repository.DeploymentDocument>();
    }
}