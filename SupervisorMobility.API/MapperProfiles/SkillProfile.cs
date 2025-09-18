using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<DataAccess.Entities.SOS.STRO.Skill,DataAccess.Entities.SOS.STRO.Collections.Skill.Dtos.CreateSkillDto>().ReverseMap();
        }
    }
}
