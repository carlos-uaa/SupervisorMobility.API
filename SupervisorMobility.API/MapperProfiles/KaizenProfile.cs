using AutoMapper;
using SupervisorMobility.API.Models.KaizenTransactionDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class KaizenProfile : Profile
    {
        public KaizenProfile()
        {
            CreateMap<DataAccess.Entities.Kaizen, Models.KaizenDtos.KaizenDto>();
            CreateMap<DataAccess.Entities.Kaizen, Models.KaizenDtos.CreateKaizenDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Kaizen, Models.KaizenDtos.KaizenWithAllDataDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Kaizen, Models.KaizenDtos.UpdateKaizenDto>().ReverseMap();
            CreateMap<DataAccess.Entities.KaizenTransaction, KaizenTransactionDto>().ReverseMap();
            CreateMap<DataAccess.Entities.KaizenTransaction, CreateKaizenTransactionDto>().ReverseMap();
            CreateMap<DataAccess.Entities.KaizenTransaction, UpdateKaizenTransactionDto>().ReverseMap();
        }
    }
}
