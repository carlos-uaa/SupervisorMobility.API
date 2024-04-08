namespace SupervisorMobility.API.Models.KaizenDtos
{
    public class KaizenTransactionDto
    {
        public int? KaizenTransactionId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public bool? IsActive { get; set; }
    }
}
