
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.ChecklistAnswerDtos
{
    public class ChecklistAnswerDto
    {
        public int AnswerId { get; set; }
        public int? JobObservationId { get; set; }
        public int? QuestionID { get; set; }
        public string? Prompt { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
       
        public ICollection<FileUploadGeneralDto>? Evidences { get; set; }
           = new List<FileUploadGeneralDto>();

        public string? CommentarySV { get; set; } = string.Empty;
        public string? CommentarySSV { get; set; } = string.Empty;
    }

}
