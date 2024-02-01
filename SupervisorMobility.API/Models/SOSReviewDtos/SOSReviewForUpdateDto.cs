using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewForUpdateDto
    {
        public int Status { get; set; }
        public  ICollection<SOSReviewDistSuggestionDto>? Suggestions { get; set; }
           = new List<SOSReviewDistSuggestionDto>();
        public ICollection<UsersWithoutNavigationWithoutPeopleDetails>? Supervisors { get; set; }

        public int? PlantId { get; set; }

        public int? AreaId { get; set; }

        public DateTime? CreationDate { get; set; }
        public int? AplicationYear { get; set; }


        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public bool SuggestionApplied { get; set; }

        public bool IsActive { get; set; }
    }
}
