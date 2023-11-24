using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class ChecklistAnswerProfile : Profile
    {
        public ChecklistAnswerProfile()
        {
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerDto>();
            CreateMap<Models.ChecklistAnswerDtos.ChecklistAnswerDto, Entities.ChecklistAnswer>().ReverseMap();
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerForCreationDto>().ReverseMap();
            CreateMap<Entities.ChecklistAnswer, Models.ChecklistAnswerDtos.ChecklistAnswerForUpdateDto>().ReverseMap();
        }
    }
}
