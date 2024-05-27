using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class Template
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateId { get; set; }
        public bool? IsActive { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }


        //Item de la categoria
        public ICollection<Checkpoint>? CheckpointItems { get; set; }
          = new List<Checkpoint>();
        //specificacion de la categoria
        public ICollection<CheckpointNorm>? CheckpointNormItems { get; set; }
          = new List<CheckpointNorm>();



        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? CheckDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? FinishedDate { get; set; }

        //public ICollection<DataPanel> PanelItems { get; set; }
        // = new List<DataPanel>();
    }
}
