using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class KnowledgeProfile : Profile
    {
        public KnowledgeProfile()
        {
            CreateMap<DataAccess.Entities.SOS.STRO.Knowledge,DataAccess.Entities.SOS.STRO.Collections.Knowledge.Dtos.CreateKnowledgeDto>().ReverseMap();
        }
    }
}
