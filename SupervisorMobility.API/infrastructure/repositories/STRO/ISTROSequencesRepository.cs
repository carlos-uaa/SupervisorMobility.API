using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;

namespace SupervisorMobility.API.infrastructure.repositories.STRO
{
    public interface ISTROSequencesRepository
    {
        Task<List<SOSSynopticRequirementsOperationSequence>> GetAllSTROSequencesByIdSosHubId(int IdDistribution);
        Task<SOSSynopticRequirementsOperationSequence> AddSTROSequences(SOSSynopticRequirementsOperationSequence AddSTROSequencesDto);
        Task<SOSSynopticRequirementsOperationSequence> UpdateSTROSequences(SOSSynopticRequirementsOperationSequence updateSTROSequencesDto);
        Task DeleteSTROSequences(int Id);
    }
}