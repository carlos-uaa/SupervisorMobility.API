using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class PatUserRoleProfile : Profile
    {
        public PatUserRoleProfile()
        {
            CreateMap<DataAccess.Entities.PatUserRole, Models.PATDtos.PatUserRoleDtos.PatUserRoleDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatUserRole, Models.PATDtos.PatUserRoleDtos.PatUserRoleForCreateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatUserRole, Models.PATDtos.PatUserRoleDtos.PatUserRoleForUpdateDto>().ReverseMap();
        }
    }


}
