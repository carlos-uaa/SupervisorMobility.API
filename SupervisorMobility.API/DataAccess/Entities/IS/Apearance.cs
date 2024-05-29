using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class Apearance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApearanceId { get; set; }
        public bool? IsActive { get; set; }
        
        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public ICollection<Commentary>? Observations { get; set; }

        public int? ManufacturerId { get; set; }
        public User? Manufacturer { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverUserId { get; set; }
        public User? ApproverUser { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? CheckDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? ApprovedDate { get; set; }

        //Item de la categoria
        public ICollection<DataPanel>? DataPanelItems { get; set; }
         = new List<DataPanel>(); 
     

        //Item de los problemas
        public ICollection<ProblemDefect>? ProblemDefectItems { get; set; }
         = new List<ProblemDefect>();

        public ICollection<LogbookAparence>? LogbooksAparence { get; set; }
         = new List<LogbookAparence>();
    }
}
