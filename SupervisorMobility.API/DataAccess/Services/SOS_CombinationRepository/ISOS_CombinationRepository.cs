using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_Combination
{
    public interface ISOS_CombinationRepository
    {
        //SOS Combination
        #region SOSCombination
        Task<int> CreateSOSCombination(SOSCombination SOS_CombinationToCreate);
        Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeProcess = false);
        Task<IEnumerable<SOSCombination>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false);

        Task<int> UpdateSOSCombination(SOSCombinationForUpdateDto CombinationUpdate, SOSCombination CombinationEntity);
        Task<int> RemoveSOSCombination(int SOS_Combination_id);


        #endregion

        #region Add To Sos Combination
        Task<List<SOSCombinationLogbook>> AddRangeSOSCombinationLogbook(List<SOSCombinationLogbook> SOSCombinationLogbooksToAdd);
        Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSCombination(SOSCombination Master, SOSCombinationOperationSequence Slave);
        Task<AsyncVoidMethodBuilder> AddSOSCombinationLogbookToSOSCombination(SOSCombination Master, SOSCombinationLogbook Slave);
        Task AddIlustrationToSOSCombination(int SOS_Combination_id, FileUpload evidence);
        //Task<AsyncVoidMethodBuilder> AddNoteToSOSCombination(SOSCombination Master, Commentary Slave);

        #endregion
        #region Remove from SosCombination
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSCombinationLogbookFromSOSCombination(SOSCombination Master);
        //Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSCombination(SOSCombination Master);
        Task<int> RemoveIlustrationFromSOSCombination(int SOS_Combination_id, int ImageFile_id);
        #endregion
        #region SOSCombinationLogbook
        Task<SOSCombinationLogbook> GetSOSCombinationLogbookById(int id);
        Task<int> UpdateCombinationLogbook(SOSCombinationLogbookForUpdateDto CombinationForUpdate);
        Task<int> CreateSOSCombinationLogbook(SOSCombinationLogbook LogBook_ToCreate);
        #endregion
        #region SOSCombinationOperationSequences
        Task<SOSCombinationOperationSequence> GetSOSCombinationOperationSequencesById(int id);
        Task<int> UpdateSOSCombinationOperationSequences(SOSCombinationOperationSequenceForUpdateDto OperationSequenceForUpdate);
        Task<List<SOSCombinationOperationSequence>> AddRangeSOSCombinationOperationSequences(List<SOSCombinationOperationSequence> SOSOperationSequencesToAdd);
        Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSCombination(SOSCombination Master);
        #endregion
    }
}
