using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_AnalysisRepository
{
    public interface ISOS_AnalysisRepository
    {
        //SOS Analysis
        #region SOSAnalysis
        Task<int> CreateSOSAnalysis(SOSAnalysis SOS_AnalysisToCreate);
        Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysisByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysisByArea(int area, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);

        Task<int> UpdateSOSAnalysis(SOSAnalysisForUpdateDto AnalysisUpdate, SOSAnalysis AnalysisEntity);
        Task<int> RemoveSOSAnalysis(int SOS_Analysis_id);

        Task AddIlustrationToSOSAnalysis(int SOS_Analysis_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSAnalysis(int SOS_Analysis_id, int ImageFile_id);

        #endregion
        #region Add Range SOS Analysis
        Task<List<SOSAnalysisLogbook>> AddRangeSOSAnalysisLogbook(List<SOSAnalysisLogbook> SOSAnalysisLogbooksToAdd);
        #endregion
        #region Add To Sos Analysis
        Task<AsyncVoidMethodBuilder> AddSOSAnalysisLogbookToSOSAnalysis(SOSAnalysis Master, SOSAnalysisLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSAnalysis(SOSAnalysis Master, Commentary Slave);

        #endregion
        #region Remove from SosAnalysis
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(SOSAnalysis Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSAnalysis(SOSAnalysis Master);
        #endregion
        #region SOSAnalysisLogbook
        Task<SOSAnalysisLogbook> GetSOSAnalysisLogbookById(int id);
        Task<int> CreateSOSAnalysisLogbook(SOSAnalysisLogbook LogBook_ToCreate);
        Task<int> UpdateAnalysisLogbook(SOSAnalysisLogbookForUpdateDto analysisForUpdate);
        #endregion
    }
}
