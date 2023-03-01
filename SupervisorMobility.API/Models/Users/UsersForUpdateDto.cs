namespace SupervisorMobility.API.Models.Users
{
    public class UsersForUpdateDto
    {
        public int Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Permissions { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DisabledDate { get; set; }

    }
}
