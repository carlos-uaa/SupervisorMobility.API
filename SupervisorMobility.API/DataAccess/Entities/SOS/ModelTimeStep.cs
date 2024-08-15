using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class ModelTimeStep
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModelTimeStepId { get; set; }

        public int SectionId {  get; set; }
        public string? Times { get; set; } = "§§§§";
    }
}