namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATForUpdateDto
    {
        public int SupervisorId { get; set; }
        public int Status { get; set; }
        public int? SSVresponsibleID { get; set; }


        public int PlantId { get; set; }
        public int AreaId { get; set; }

        public int DistributionId { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set { AplicationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }

        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
