namespace SupervisorMobility.API.Models.ProductOperationDtos
{
    public class ProductOperationWithoutNavigationPropertiesDto
    {
        public int ProductOperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
