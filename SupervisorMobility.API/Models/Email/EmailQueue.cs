using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.Email
{
    public class EmailQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailQueueID { get; set; }

        // Relación con Usuario que creó el registro
        [ForeignKey(nameof(MadeBy))]
        public int? MadeByID { get; set; }
        public User? MadeBy { get; set; }

        // Relación con PIR relacionado
        // [ForeignKey(nameof(TargetRelation))]
        // public int? TargetRelationID { get; set; }
        // public PIR? TargetRelation { get; set; }

        public string NotificationType { get; set; }
        
        // Relación con Usuario Staff
        [ForeignKey(nameof(Staff))]
        public int? StaffID { get; set; }
        public User? Staff { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsSend { get; set; }
        public DateTime? SendDate { get; set; }

        public int? Attempts { get; set; } = 0;
    }
}