using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HCIProfile : Profile
    {
        public HCIProfile()
        {
            CreateMap<HCI, Models.HCIDtos.HCIDto>();
            CreateMap<HCI, Models.HCIDtos.CreateHCIDto>();
            CreateMap<HCI, Models.HCIDtos.UpdateHCIDto>();
            CreateMap<HCITransaction, Models.HCITransactionDtos.HCITransactionDto>();
            CreateMap<HCITransaction, Models.HCITransactionDtos.CreateHCITransactionDto>();
            CreateMap<HCITransaction, Models.HCITransactionDtos.UpdateHCITransactionDto>();
            CreateMap<Commentary, Models.CommentaryDtos.CommentaryDto>();
            CreateMap<Commentary, Models.CommentaryDtos.CreateCommentaryDto>();
            CreateMap<Commentary, Models.CommentaryDtos.UpdateCommentaryDto>();
        }
    }
}
