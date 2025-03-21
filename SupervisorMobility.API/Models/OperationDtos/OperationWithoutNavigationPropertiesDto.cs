namespace SupervisorMobility.API.Models.OperationDtos
{
    public class OperationWithoutNavigationPropertiesDto
    {
        public int OperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? restrictionorcomment { get; set; } = string.Empty;
        public string? jsonTimeProduct { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public string? NameTime { get; set; }
        public string? Time { get; set; }
        public int CriticalType { get; set; }

        public string? AdditionalTime { get; set; }
        public string? StandardTime { get; set; }

        public bool? IsActive { get; set; }
    }
}
