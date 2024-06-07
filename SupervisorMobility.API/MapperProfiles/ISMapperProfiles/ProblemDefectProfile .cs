using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class ProblemDefectProfile : Profile
    {
        public ProblemDefectProfile()
        {
            //data panel
            CreateMap<ProblemDefect, ProblemDefectDto>().ReverseMap();
            CreateMap<ProblemDefect, ProblemDefectForCreateDto>().ReverseMap();
            CreateMap<ProblemDefect, ProblemDefectForUpdateDto>().ReverseMap();
        }
    }
}
