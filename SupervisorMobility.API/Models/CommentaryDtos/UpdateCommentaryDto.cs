namespace SupervisorMobility.API.Models.CommentaryDtos
{
    public class UpdateCommentaryDto
    {
        public int CommentaryId { get; set; }

        public string Comment { get; set; }

        public bool? IsActive { get; set; }
    }
}
