namespace SupervisorMobility.API.Models.CommentaryDtos
{
    public class CommentaryDto
    {
        public int ComentaryId { get; set; }

        public string Comment { get; set; }

        public bool? IsActive { get; set; }
    }
}
