using AutoMapper;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;

namespace SupervisorMobility.API.Profiles
{
    public class JobCategoryStructureProfile : Profile
    {
        public JobCategoryStructureProfile()
        {
            CreateMap<Entities.JobCategoryStructure, JobCategoryStructureWithoutChecklistQuestionsDto>();
            CreateMap<Entities.JobCategoryStructure, JobCategoryStructureWithJustchecklistQuestionsDto>();
            CreateMap<JobCategoryStructureSequenceForUpdateDto, Entities.JobCategoryStructure>();
            CreateMap<Entities.JobCategoryStructure, JobCategoryStructureForCreationDto>().ReverseMap();
            CreateMap<JobCategoryStructureDto, Entities.JobCategoryStructure>().ReverseMap();
            CreateMap<JobCategoryStructureForUpdateDto, Entities.JobCategoryStructure>().ReverseMap();
        }
    }
}
