using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class UserCareerPath
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserCareerPathId { get; set; }
        public int CareerPathNo { get; set; }

        public DateTime? ChangeDate { get; set; }

        public string? OperationDescription { get; set; }


    }
}
