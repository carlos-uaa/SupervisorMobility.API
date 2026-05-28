using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.Models.Email;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;

namespace SupervisorMobility.API.MapperProfiles
{
    public class EmailDeliveryResultProfiles : Profile
    {
        public EmailDeliveryResultProfiles()
        {
            CreateMap<CreateEmailDeliveryResultDto, EmailDeliveryResult>().ReverseMap();
            CreateMap<EmailDeliveryResult, EmailDeliveryResultDto>()
                .ForMember(dest => dest.SentByUserName, opt => opt.MapFrom(src => src.SentByUser != null ? src.SentByUser.Name : null));
        }
    }
}
