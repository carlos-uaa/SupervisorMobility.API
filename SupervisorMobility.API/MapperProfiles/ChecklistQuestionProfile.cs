using AutoMapper;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Profiles
{
    public class ChecklistQuestionProfile : Profile
    {
        public ChecklistQuestionProfile()
        {
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionDto>();
            CreateMap<Entities.ChecklistQuestion, ChecklistCategoryWithoutChecklistQuestionsDto>();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForCreationDto>().ReverseMap();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForUpdateDto>().ReverseMap();
        }
    }
}
