using AutoMapper;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;

namespace SupervisorMobility.API.Profiles
{
    public class ChecklistCategoryProfile : Profile
    {
        public ChecklistCategoryProfile()
        {
            CreateMap<Entities.ChecklistCategory, ChecklistCategoryWithoutChecklistQuestionsDto>();
            CreateMap<Entities.ChecklistCategory, ChecklistCategoryWithJustchecklistQuestions>();
            CreateMap<Entities.ChecklistCategory, ChecklistCategoryForCreationDto>().ReverseMap();
            CreateMap<ChecklistCategoryDto, Entities.ChecklistCategory>().ReverseMap();
            CreateMap<ChecklistCategoryForUpdateDto, Entities.ChecklistCategory>().ReverseMap();
        }
    }
}
