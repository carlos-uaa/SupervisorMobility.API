using SupervisorMobility.API.Interfaces.SOS;

namespace SupervisorMobility.API.Interfaces.SOS
{
    public interface ISTROSyncDistributionService
    {
        Task<bool> SyncDistributionsWithSTROs(int IdDistribution);
    }
}
