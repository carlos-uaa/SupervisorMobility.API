using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_FlowRepository
{
    public interface ISOS_FlowRepository
    {
        //SOS Flow
        #region SOSFlow
        Task<int> CreateSOSFlow(SOSFlow SOS_FlowToCreate);
        Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includePeople = false);
        Task<IEnumerable<SOSFlow>> GetAllSOSFlow(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false);

        Task<int> UpdateSOSFlow(SOSFlowForUpdateDto FlowUpdate, SOSFlow FlowEntity);
        Task<int> RemoveSOSFlow(int SOS_Flow_id);


        #endregion
        #region Add Range SOS Flow
        Task<List<SOSFlowLogbook>> AddRangeSOSFlowLogbook(List<SOSFlowLogbook> SOSFlowLogbooksToAdd);
        #endregion
        #region Add To Sos Flow
        Task<AsyncVoidMethodBuilder> AddSOSFlowLogbookToSOSFlow(SOSFlow Master, SOSFlowLogbook Slave);
        //Task AddIlustrationToSOSFlow(int SOS_Flow_id, FileUpload evidence);

        #endregion
        #region Remove from SosFlow
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSFlowLogbookFromSOSFlow(SOSFlow Master);
        //Task<int> RemoveIlustrationFromSOSFlow(int SOS_Flow_id, int ImageFile_id);

        #endregion
        #region SOSFlowLogbook
        Task<SOSFlowLogbook> GetSOSFlowLogbookById(int id);
        Task<int> UpdateFlowLogbook(SOSFlowLogbookForUpdateDto flowForUpdate);
        Task<int> CreateSOSFlowLogbook(SOSFlowLogbook LogBook_ToCreate);
        #endregion
    }
}
