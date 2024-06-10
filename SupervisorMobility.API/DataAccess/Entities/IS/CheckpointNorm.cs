using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class CheckpointNorm
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckpointNormId { get; set; }

        public bool? IsActive { get; set; }

        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;
        public int CheckpointId { get; set; }
        public Checkpoint? Checkpoint { get; set; }
        public ICollection<FileUpload>? Sketches { get; set; } = new List<FileUpload>();

    }
}
