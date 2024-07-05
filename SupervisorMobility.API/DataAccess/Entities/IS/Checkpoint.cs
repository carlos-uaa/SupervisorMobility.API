using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class Checkpoint
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckpointId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string CheckpointTitle { get; set; } = string.Empty;
        public string CheckpointDescription { get; set; } = string.Empty;

        public ICollection<FileUpload>? Sketches { get; set; } = new List<FileUpload>();
        public ICollection<CheckpointNorm>? Standars { get; set; } = new List<CheckpointNorm>();
    
    }
}
