using AutoMapper;
using blogdeployments.domain.Events;
using blogdeployments.power.Handler;

namespace blogdeployments.power.Model;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<PowerOnRequested, PowerOn>();
        CreateMap<ShutdownInitiated, CheckHostStatus>().ForMember(
            dest => dest.Adresses,
            opt =>
                opt.MapFrom(src =>
                    src.Adresses));
    }
}