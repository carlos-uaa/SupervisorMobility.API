using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.Models.SOS.SpecialCaseAbnormalSituationDtos
{
    public class SpecialCaseAbnormalSituationDto
    {
        public int SpecialCaseAbnormalSituationId { get; set; }
        public string? key { get; set; }

        public string? PartName { get; set; }
        public string? PartNumber { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
