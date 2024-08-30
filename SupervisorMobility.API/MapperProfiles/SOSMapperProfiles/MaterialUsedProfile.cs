using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.MaterialDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class MaterialUsedProfile : Profile
    {
        public MaterialUsedProfile()
        {
            CreateMap<MaterialUsed, MaterialsUsedDto>().ReverseMap();
            CreateMap<MaterialUsed, MaterialsUsedForCreateDto>().ReverseMap();
            CreateMap<MaterialUsed, MaterialsUsedForUpdateDto>().ReverseMap();
        }
    }
}