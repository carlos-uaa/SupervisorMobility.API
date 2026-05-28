using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_SequenceRepository
{
    public interface ISOS_SequenceRepository
    {
        //SOS Sequence
        #region SOSSequence
        Task<int> CreateSOSSequence(SOSSequence SOS_SequenceToCreate);
        Task<SOSSequence> GetSOSSequence(int SOSSequenceId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSSequence>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<IEnumerable<SOSSequence>> GetAllSOSSequenceByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<IEnumerable<SOSSequence>> GetAllSOSSequenceByArea(int Area_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);

        Task<int> UpdateSOSSequence(SOSSequenceForUpdateDto SequenceUpdate, SOSSequence SequenceEntity);
        Task<int> RemoveSOSSequence(int SOS_Sequence_id);

        Task AddIlustrationToSOSSequence(int SOS_Sequence_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSSequence(int SOS_Sequence_id, int ImageFile_id);

        #endregion
        #region Add Range SOS Sequence
        Task<List<SOSSequenceLogbook>> AddRangeSOSSequenceLogbook(List<SOSSequenceLogbook> SOSSequenceLogbooksToAdd);
        #endregion
        #region Add To Sos Sequence
        Task<AsyncVoidMethodBuilder> AddSOSSequenceLogbookToSOSSequence(SOSSequence Master, SOSSequenceLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSSequence(SOSSequence Master, Commentary Slave);

        #endregion
        #region Remove from SosSequence
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSequenceLogbookFromSOSSequence(SOSSequence Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSSequence(SOSSequence Master);
        #endregion
        #region SOSSequenceLogbook
        Task<SOSSequenceLogbook> GetSOSSequenceLogbookById(int id);
        Task<int> CreateSOSSequenceLogbook(SOSSequenceLogbook LogBook_ToCreate);
        Task<int> UpdateSequenceLogbook(SOSSequenceLogbookForUpdateDto SequenceForUpdate);
        #endregion
    }
}
