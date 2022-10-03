namespace SupervisorMobility.API.Models.OperationDtos
{
    public class OperationWithoutNavigationPropertiesDto
    {
        public int OperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
