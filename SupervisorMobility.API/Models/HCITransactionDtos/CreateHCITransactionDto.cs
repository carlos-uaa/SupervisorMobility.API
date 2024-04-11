namespace SupervisorMobility.API.Models.HCITransactionDtos
{
    public class CreateHCITransactionDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public int Type { get; set; }
        public bool? IsActive { get; set; }
    }
}
