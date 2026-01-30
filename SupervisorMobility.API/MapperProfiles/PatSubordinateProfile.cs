using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class PatSubordinateProfile : Profile
    {
        public PatSubordinateProfile()
        {
            CreateMap<DataAccess.Entities.PatSubordinate, Models.PATDtos.PatSubordinateDtos.PatSubordinateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatSubordinate, Models.PATDtos.PatSubordinateDtos.PatSubordinateForCreateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatSubordinate, Models.PATDtos.PatSubordinateDtos.PatSubordinateForUpdateDto>().ReverseMap();
            CreateMap<Models.PATDtos.PatSubordinateDtos.PatSubordinateForCreateDto, Models.PATDtos.PatSubordinateDtos.PatSubordinateForUpdateDto>().ReverseMap();
        }
    }


}
