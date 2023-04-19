using AutoMapper;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.NotificationDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() {
            CreateMap<Notification, NotificationDto>();
            CreateMap<Notification, NotificationToCreateDto>().ReverseMap();
            CreateMap<Notification, NotificationForUpdateDto>().ReverseMap();
        }
    }
}
