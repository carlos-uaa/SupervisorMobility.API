using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.MaterialDtos;

namespace SupervisorMobility.API.MapperProfiles.ISOSMapperProfiles
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialDto>().ReverseMap();
            CreateMap<Material, MaterialForCreateDto>().ReverseMap();
            CreateMap<Material, MaterialForUpdateDto>().ReverseMap();
        }
    }
}