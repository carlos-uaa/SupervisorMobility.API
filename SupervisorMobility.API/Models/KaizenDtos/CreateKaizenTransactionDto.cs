namespace SupervisorMobility.API.Models.KaizenDtos
{
    public class CreateKaizenTransactionDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public bool? IsActive { get; set; }
    }
}
