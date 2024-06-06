using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class ProblemDefect
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProblemDefectId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir si son definidos o abiertos
        public int ItemOrder { get; set; }
        public string DefectDescription { get; set; } = string.Empty;



    }
}
