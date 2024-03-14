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
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForCreationDto>().ForMember(p=>p.Pillars, opt=> opt.Ignore()).ReverseMap().ForMember(p=>p.Pillars, opt=>opt.Ignore());
            CreateMap<Entities.ChecklistQuestion, ChecklistQuestionForUpdateDto>().ForMember(p => p.Pillars, opt => opt.Ignore()).ReverseMap().ForMember(p => p.Pillars, opt => opt.Ignore());
        }
    }
}
