using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


    namespace SupervisorMobility.API.DataAccess.Entities.SOS
    {
        public class CommentaryHistory
    { 
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentaryHistoryId { get; set; }
        public int ComentaryId { get; set; }
       
        public string? Comment { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
