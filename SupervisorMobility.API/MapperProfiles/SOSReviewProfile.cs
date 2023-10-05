using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;

namespace SupervisorMobility.API.MapperProfiles
{
    public class SOSReviewProfile : Profile
    {

        public SOSReviewProfile() {
            CreateMap<SOSReviewProgram, Models.SOSReviewDtos.SOSReviewWithAllDto>().ReverseMap();
            CreateMap<SOSReviewProgram, Models.SOSReviewDtos.SOSReviewWithOutDataDto>().ReverseMap();
            CreateMap<SOSReviewProgram, Models.SOSReviewDtos.SOSReviewForCreateDto>().ReverseMap();
            CreateMap<SOSReviewProgram, Models.SOSReviewDtos.SOSReviewForUpdateDto>().ReverseMap();
            CreateMap<SOSRegisterJobObservation, Models.SOSReviewDtos.SOSReviewsRegisterDto>().ReverseMap();
            CreateMap<SOSRegisterJobObservation, Models.SOSReviewDtos.SOSReviewsRegisterForUpdateDto>().ReverseMap();
            CreateMap<SOSRegUserOperation, Models.SOSReviewDtos.SOSRegUserOperationDto>().ReverseMap();
            CreateMap<SOSRegUserOperation, Models.SOSReviewDtos.SOSRegUserOperationForUpdateDto>().ReverseMap();
        }
    }
}
