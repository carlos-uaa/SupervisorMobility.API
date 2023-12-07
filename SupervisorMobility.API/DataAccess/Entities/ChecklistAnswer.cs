using DocumentFormat.OpenXml.Bibliography;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class ChecklistAnswer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerId { get; set; }
        public int JobObservationId { get; set; }
        public int? LupId { get; set; }
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;

        public ICollection<FileUpload>? Evidences { get; set; }
            = new List<FileUpload>();

        public string? CommentarySV { get; set; } = string.Empty;
        public string? CommentarySSV { get; set; } = string.Empty;
    }
}
