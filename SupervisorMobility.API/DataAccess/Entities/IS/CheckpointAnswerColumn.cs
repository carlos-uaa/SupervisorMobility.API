using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class CheckpointAnswerColumn
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColumnId { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }

        public string? RAN { get; set; }

        public ICollection<CheckpointNormAnswer>? CheckpointsResults { get; set; }
        = new List<CheckpointNormAnswer>();

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }
        public Commentary? InspectorObservations { get; set; }
        public FileUpload? InspectorsSignature { get; set; } = new();

    }
}
