using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class PatDistributionCommentProfile : Profile
    {
        public PatDistributionCommentProfile()
        {
            CreateMap<DataAccess.Entities.PatDistributionComment, Models.PATDtos.PatDistributionCommentDtos.PatDistributionCommentDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatDistributionComment, Models.PATDtos.PatDistributionCommentDtos.PatDistributionCommentForCreateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PatDistributionComment, Models.PATDtos.PatDistributionCommentDtos.PatDistributionCommentForUpdateDto>().ReverseMap();
            CreateMap<Models.PATDtos.PatDistributionCommentDtos.PatDistributionCommentForCreateDto, Models.PATDtos.PatDistributionCommentDtos.PatDistributionCommentForUpdateDto>().ReverseMap();
        }
    }


}
