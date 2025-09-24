namespace SupervisorMobility.API.Interfaces.SOS
{
    public interface ISTOperatingRequirementsService
    {
        Task<byte[]> GenerateExcelSTOperatingRequirements(int id);
    }
}
