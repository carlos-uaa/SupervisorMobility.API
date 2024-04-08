namespace SupervisorMobility.API.Models.KaizenDtos
{
    public class UpdateKaizenTransactionDto
    {
        public int KaizenTransactionId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public bool? IsActive { get; set; }
    }
}
