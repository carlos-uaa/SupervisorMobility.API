using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class GroupProfile: Profile
    {
        public GroupProfile()
        {
            CreateMap<Entities.Group, Models.GroupDtos.GroupDto>();
            CreateMap<Entities.Group, Models.GroupDtos.GroupForCreationDto>().ReverseMap();
            CreateMap<Entities.Group, Models.GroupDtos.GroupForUpdateDto>().ReverseMap();
        }
    }
}
