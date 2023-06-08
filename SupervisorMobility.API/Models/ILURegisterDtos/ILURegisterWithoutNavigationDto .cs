namespace SupervisorMobility.API.Models.ILURegisterDtos
{
    public class ILURegisterWithoutNavigationDto
    {
        public int ILURegisterid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? OperationId { get; set; }


        public int? OperatorId { get; set; }


        public int? ILULevelId { get; set; }
        public bool isActive { get; set; }
    }
}
