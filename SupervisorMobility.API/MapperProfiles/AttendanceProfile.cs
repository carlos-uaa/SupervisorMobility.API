using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<DataAccess.Entities.Attendance, Models.AttendanceDtos.AttendanceWithNavigationDetailsDto>();
            CreateMap<DataAccess.Entities.Attendance, Models.AttendanceDtos.AttendanceWithoutDetailsDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Attendance, Models.AttendanceDtos.AttendanceForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Attendance, Models.AttendanceDtos.AttendanceForUpdateDto>().ReverseMap();
            CreateMap<Models.AttendanceDtos.AttendanceWithoutDetailsDto, Models.AttendanceDtos.AttendanceForUpdateDto>().ReverseMap();
        }
    }
}
