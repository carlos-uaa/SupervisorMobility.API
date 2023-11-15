using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class ChecklistAnswerProfile : Profile
    {
        public ChecklistAnswerProfile()
        {
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerDto>();
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerForCreationDto>().ReverseMap();
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerForUpdateDto>().ReverseMap();
        }
    }
}
