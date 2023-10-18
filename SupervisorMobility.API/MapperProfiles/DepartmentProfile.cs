using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Entities.Department, Models.DepartmentDtos.DepartmentDto>();
            CreateMap<Entities.Department, Models.DepartmentDtos.DepartmentForCreationDto>().ReverseMap();
            CreateMap<Entities.Department, Models.DepartmentDtos.DepartmentForUpdateDto>().ReverseMap();
        }
    }
}
