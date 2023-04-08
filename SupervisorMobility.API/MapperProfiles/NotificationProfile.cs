using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.MapperProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() {
            CreateMap<Notification, Models.NotificationDtos.NotificationDto>();
            CreateMap<Notification, Models.NotificationDtos.NotificationToCreateDto>().ReverseMap();
        }
    }
}
