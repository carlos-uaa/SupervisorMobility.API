using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.Email
{
    public class EmailDeliveryResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailDeliveryResultID { get; set; }

        [Required]
        [MaxLength(255)]
        [StringLength(255)]
        public string ToEmail { get; set; }

        [MaxLength(255)]
        [StringLength(255)]
        public string? FromEmail { get; set; }

        [Required]
        [MaxLength(500)]
        [StringLength(500)]
        public string Subject { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? MessageBody { get; set; }

        [Required]
        public bool IsDelivered { get; set; }

        [MaxLength(200)]
        [StringLength(200)]
        public string DeliveryStatus { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ErrorMessage { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ErrorDetails { get; set; }

        public DateTime SentDateTime { get; set; } = DateTime.Now;

        public DateTime? DeliveryDateTime { get; set; }

        [MaxLength(100)]
        [StringLength(100)]
        public string? SmtpServer { get; set; }

        public int? Port { get; set; }

        public int? RetryAttempts { get; set; } = 0;

        public DateTime? NextRetryDateTime { get; set; }

        public int? SentByUserID { get; set; }
        public User? SentByUser { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        public string? EmailType { get; set; }

        [MaxLength(100)]
        [StringLength(100)]
        public string? ReferenceEntity { get; set; }

        public int? ReferenceEntityID { get; set; }

        [MaxLength(200)]
        [StringLength(200)]
        public string? MessageID { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadDateTime { get; set; }
    }
}