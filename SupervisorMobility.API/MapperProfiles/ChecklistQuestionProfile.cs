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
            CreateMap<Entities.ChecklistQuestion, JobCategoryStructureWithoutChecklistQuestionsDto>();
            CreateMap<ChecklistQuestionSequenceForUpdateDto, Entities.ChecklistQuestion>();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionWithoutNavigationPropertiesDto>()
                .ForMember(dest=>dest.Pillars, opts => opts.MapFrom(src=>src.Pillars.Select(p=>p.PillarId)))
                .ReverseMap();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForCreationDto>().ReverseMap();
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForUpdateDto>().ReverseMap();
        }
    }
}
