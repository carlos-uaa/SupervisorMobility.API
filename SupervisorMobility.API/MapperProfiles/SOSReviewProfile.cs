using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class SOSReviewProfile : Profile
    {

        public SOSReviewProfile() {
            CreateMap<DataAccess.Entities.SOSReviewProgram, Models.SOSReviewDtos.SOSReviewWithAllDto>().ReverseMap();
            CreateMap<DataAccess.Entities.SOSReviewProgram, Models.SOSReviewDtos.SOSReviewWithOutDataDto>().ReverseMap();
            CreateMap<DataAccess.Entities.SOSReviewProgram, Models.SOSReviewDtos.SOSReviewForCreateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.SOSReviewProgram, Models.SOSReviewDtos.SOSReviewForUpdateDto>().ReverseMap();
        }
    }
}
