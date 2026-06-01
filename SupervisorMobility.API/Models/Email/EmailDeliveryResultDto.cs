namespace SupervisorMobility.API.Models.Email
{
    public class EmailDeliveryResultDto
    {
        public int EmailDeliveryResultID { get; set; }
        public string ToEmail { get; set; }
        public string? FromEmail { get; set; }
        public string Subject { get; set; }
        public string? MessageBody { get; set; }
        public bool IsDelivered { get; set; }
        public string DeliveryStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
        public DateTime SentDateTime { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
        public string? SmtpServer { get; set; }
        public int? Port { get; set; }
        public int? RetryAttempts { get; set; }
        public DateTime? NextRetryDateTime { get; set; }
        public int? SentByUserID { get; set; }
        public string? SentByUserName { get; set; }
        public string? EmailType { get; set; }
        public string? ReferenceEntity { get; set; }
        public int? ReferenceEntityID { get; set; }
        public string? MessageID { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDateTime { get; set; }
    }

    public class CreateEmailDeliveryResultDto
    {
        public string ToEmail { get; set; }
        public string? FromEmail { get; set; }
        public string Subject { get; set; }
        public string? MessageBody { get; set; }
        public bool IsDelivered { get; set; }
        public string DeliveryStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
        public string? SmtpServer { get; set; }
        public int? Port { get; set; }
        public int? SentByUserID { get; set; }
        public string? EmailType { get; set; }
        public string? ReferenceEntity { get; set; }
        public int? ReferenceEntityID { get; set; }
        public string? MessageID { get; set; }
    }

    public class UpdateEmailDeliveryResultDto
    {
        public bool? IsDelivered { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
        public int? RetryAttempts { get; set; }
        public DateTime? NextRetryDateTime { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? ReadDateTime { get; set; }
    }
}