namespace SupervisorMobility.API.Models.OperationDtos
{
    public class OperationWithoutNavigationPropertiesDto
    {
        public int OperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CriticalType { get; set; }

        public bool? IsActive { get; set; }
    }
}
