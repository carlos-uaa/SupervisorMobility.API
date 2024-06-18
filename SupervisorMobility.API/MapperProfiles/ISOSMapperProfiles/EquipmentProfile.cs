using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;

namespace SupervisorMobility.API.MapperProfiles.ISOSMapperProfiles
{
    public class EquipmentProfile : Profile
    {
        public EquipmentProfile()
        {
            CreateMap<Equipment, EquipmentDto>().ReverseMap();
            CreateMap<Equipment, EquipmentForCreateDto>().ReverseMap();
            CreateMap<Equipment, EquipmentForUpdateDto>().ReverseMap();
        }
    }
}