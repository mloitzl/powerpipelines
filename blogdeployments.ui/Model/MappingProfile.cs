using AutoMapper;
using blogdeployments.domain.Events;
using blogdeployments.ui.Handler;

namespace blogdeployments.ui.Model;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PowerOnCompleted, SendHostNotification>();
            CreateMap<ShutdownCompleted, SendHostNotification>();
            CreateMap<ClusterIsUp, SendClusterNotification>();
            CreateMap<ClusterIsDown, SendClusterNotification>();
        }
    }
