using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_DistributionRepository
{
    public interface ISOS_DistributionRepository
    {
        //SOS Distribution
        #region SOSDistribution
        Task<int> CreateSOSDistribution(SOSDistribution SOS_DistributionToCreate);
        Task<string> GetDistributionName(int distributionID);
        Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false);
        Task<int> GetIdDistributionBySosHub(int IdSosHub);
        Task<IEnumerable<SOSDistribution>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false);

        Task<int> UpdateSOSDistribution(SOSDistributionForUpdateDto DistributionUpdate, SOSDistribution DistributionEntity);
        Task<int> RemoveSOSDistribution(int SOS_Distribution_id);

        Task AddIlustrationToSOSDistribution(int SOS_Distribution_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSDistribution(int SOS_Distribution_id, int ImageFile_id);

        #endregion

        #region Add To Sos Distribution
        Task<AsyncVoidMethodBuilder> AddSOSHubToSOSDistribution(SOSDistribution master, SOSHub slave);
        Task<AsyncVoidMethodBuilder> AddSOSDistributionLogbookToSOSDistribution(SOSDistribution Master, SOSDistributionLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddSOSDistributionAdditionalTimeToSOSDistribution(SOSDistribution Master, SOSDistributionAdditionalTime Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSDistribution(SOSDistribution Master, Commentary Slave);
        Task<AsyncVoidMethodBuilder> AddAnalysisToSOSDistribution(SOSDistribution master, SOSAnalysis slave);
        Task<AsyncVoidMethodBuilder> AddSequenceToSOSDistribution(SOSDistribution master, SOSSequence slave);
        Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSDistribution(SOSDistribution Master, SOSDistributionOperationSequence Slave);
        Task<List<SOSDistributionLogbook>> AddRangeSOSDistributionLogbook(List<SOSDistributionLogbook> SOSDistributionLogbooksToAdd);

        #endregion
        #region Remove from SosDistribution
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSHubsFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSequencesFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(SOSDistribution Master);
        #endregion
        #region SOSDistributionLogbook
        Task<SOSDistributionLogbook> GetSOSDistributionLogbookById(int id);
        Task<int> UpdateDistributionLogbook(SOSDistributionLogbookForUpdateDto DistributionForUpdate);

        Task<int> CreateSOSDistributionLogbook(SOSDistributionLogbook LogBook_ToCreate);
        #endregion
        #region SOS Distribution Additional time
        Task<SOSDistributionAdditionalTime> GetSOSDistributionAdditionalTimeId(int id);
        Task<int> UpdateSOSDistributionAdditionalTime(SOSDistributionAdditionalTimeForUpdateDto SOSDistributionAdditionalTimeForUpdate);
        #endregion
        #region SOSDistributionOperationSequences
        Task<SOSDistributionOperationSequence> GetSOSDistributionOperationSequencesById(int id);
        Task<int> UpdateSOSDistributionOperationSequences(SOSDistributionOperationSequenceForUpdateDto OperationSequenceForUpdate);
        Task<AsyncVoidMethodBuilder> DeleteSOSDistributionOperationSequencesById(int OperationSequenceId);
        Task<List<SOSDistributionOperationSequence>> AddRangeSOSDistributionOperationSequences(List<SOSDistributionOperationSequence> SOSOperationSequencesToAdd);
        Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSDistribution(SOSDistribution Master, List<SOSDistributionOperationSequence> operationSequences);
        #endregion
    }
}
