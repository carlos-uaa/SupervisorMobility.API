using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class JobObservationType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobObservationTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<JobObservationConfig> JobObservations { get; set; }
            = new List<JobObservationConfig>();

        public JobObservationType(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
