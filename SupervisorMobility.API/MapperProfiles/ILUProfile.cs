using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;

namespace SupervisorMobility.API.MapperProfiles
{
    public class ILUProfile : Profile
    {
        public ILUProfile() {
            CreateMap<ILULevel, Models.ILU.ILULevelDto>().ReverseMap();
            CreateMap<ILULevel, Models.ILU.ILULevelForCreateDto>().ReverseMap();
            CreateMap<ILULevel, Models.ILU.ILULevelsForUpdate>().ReverseMap();
            CreateMap<ILURegister, Models.ILURegisterDtos.ILURegisterDto>().ReverseMap();
            CreateMap<ILURegister, Models.ILURegisterDtos.ILURegisterForCreationDto>().ReverseMap();
            CreateMap<ILURegister, Models.ILURegisterDtos.ILURegisterForUpdateDto>().ReverseMap();
            CreateMap<ILURegister, Models.ILURegisterDtos.ILURegisterWithoutNavigationDto>().ReverseMap();
        }
    }

   
}
