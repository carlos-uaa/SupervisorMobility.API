using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Dtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_SynopticTableRepository
{
    public interface ISOS_SynopticTableRepository
    {
        //SynopticTableofOperatingRequirements
        #region SOSSynopticTableofOperatingRequirements
        Task<int> CreateSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements SOS_SynopticTableofOperatingRequirementsToCreate);
        Task<SOSSynopticTableofOperatingRequirements> GetSOSSynopticTableofOperatingRequirements(int SOSSynopticTableofOperatingRequirementsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);
        Task<IEnumerable<SOSSynopticTableofOperatingRequirements>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);

        Task<SOSSynopticTableofOperatingRequirements> UpdateSOSSynopticTableofOperatingRequirements(int sosSynopticTableofOperatingRequirements_Id, SOSSynopticTableofOperatingRequirementsForUpdateDto sosUpdateEntity);
        Task<int> RemoveSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id);

        Task AddIlustrationToSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id, int ImageFile_id);

        #endregion

        #region Add To Sos SynopticTableofOperatingRequirements
        Task<AsyncVoidMethodBuilder> AddSOSHubToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSHub slave);
        Task<AsyncVoidMethodBuilder> AddSOSSynopticRequirementsLogbookToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, SOSSynopticRequirementsLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, Commentary Slave);
        Task<AsyncVoidMethodBuilder> AddAnalysisToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSAnalysis slave);
        Task<AsyncVoidMethodBuilder> AddSequenceToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSSequence slave);
        Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, SOSSynopticRequirementsOperationSequence Slave);
        Task<List<SOSSynopticRequirementsLogbook>> AddRangeSOSSynopticRequirementsLogbook(List<SOSSynopticRequirementsLogbook> SOSSynopticRequirementsLogbooksToAdd);

        #endregion
        #region Remove from SosSynopticTableofOperatingRequirements
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSynopticRequirementsLogbookFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSHubsFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSequencesFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSynopticTableofOperatingRequirementsAdditionalTimeFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master);
        #endregion
        #region SOSSynopticRequirementsLogbook
        Task<SOSSynopticRequirementsLogbook> GetSOSSynopticRequirementsLogbookById(int id);
        Task<int> UpdateSynopticRequirementsLogbook(SOSSynopticRequirementsLogbookForUpdateDto SynopticTableofOperatingRequirementsForUpdate);

        Task<int> CreateSOSSynopticRequirementsLogbook(SOSSynopticRequirementsLogbook LogBook_ToCreate);
        #endregion

        #region SOSSynopticRequirementsOperationSequences
        Task<SOSSynopticRequirementsOperationSequence> GetSOSSynopticRequirementsOperationSequencesById(int id);
        Task<int> UpdateSOSSynopticRequirementsOperationSequences(SOSSynopticRequirementsOperationSequenceForUpdateDto OperationSequenceForUpdate);
        Task<List<SOSSynopticRequirementsOperationSequence>> AddRangeSOSSynopticRequirementsOperationSequences(List<SOSSynopticRequirementsOperationSequence> SOSOperationSequencesToAdd);
        Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, List<SOSSynopticRequirementsOperationSequence> operationSequences);
        #endregion

        #region SOSSynopticTableofControlPoints
        Task<int> CreateSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints SOS_SynopticTableofControlPointsToCreate);
        Task<SOSSynopticTableofControlPoints> GetSOSSynopticTableofControlPoints(int SOSSynopticTableofControlPointsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);
        Task<IEnumerable<SOSSynopticTableofControlPoints>> GetAllSOSSynopticTableofControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);
        Task<SOSSynopticTableofControlPoints> UpdateSOSSynopticTableofControlPoints(int sosSynopticTableofControlPoints_Id, SOSSynopticTableofControlPointsForUpdateDto sosUpdateEntity);
        Task<int> RemoveSOSSynopticTableofControlPoints(int SOS_SynopticTableofControlPoints_id);
        #endregion

        #region Add To Sos SynopticTableofControlPoints
        Task<AsyncVoidMethodBuilder> AddSOSSynopticPointsLogbookToSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints Master, SOSSynopticPointsLogbook Slave);

        #endregion

        #region SOSSynopticPointsLogbook
        Task<int> CreateSOSSynopticPointsLogbook(SOSSynopticPointsLogbook LogBook_ToCreate);

        #endregion
    }
}
