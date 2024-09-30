using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOS.ToolsUsedDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface ISOS_ProcessRepository
    {
        #region SOS_DataPool
        Task<SOSHub> CreateSOScollection(SOSHub SOS_EntityToCreate);
        Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeHistory = false, bool includeDeleteds = false, bool includeCollections = false, bool includePeopleCollections = false);
        Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<int> UpdateSOSHub(SOSHubForUpdateDto HubUpdate, SOSHub SosEntity);
        Task<int> UpdateSOSHub(SOSHub SosEntity);
        Task<int> RemoveSOSHub(int SOS_DataPool_id);
        #endregion

        #region AddTo Sos Hub
        Task<AsyncVoidMethodBuilder> AddProcessSheetCommentaryToSOSCollection(SOSHub Master, Commentary Slave);
        Task<AsyncVoidMethodBuilder> AddAnaysisBkupToSOSCollection(SOSHub Master, AnalysisBkup Slave);
        Task<AsyncVoidMethodBuilder> AddSectionSOSCollection(SOSHub Master, Section Slave);
        Task<AsyncVoidMethodBuilder> AddMaterialToSOSCollection(SOSHub Master, MaterialUsed Slave);
        Task<AsyncVoidMethodBuilder> AddEquipmentToSOSCollection(SOSHub Master, Equipment Slave);
        Task<AsyncVoidMethodBuilder> AddToolToSOSCollection(SOSHub Master, ToolUsed Slave);
        Task<AsyncVoidMethodBuilder> AddCommonDirectionsToSOSCollection(SOSHub Master, List<CommonDirection> Slave);
        Task AddImageToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task AddVideoToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task<AsyncVoidMethodBuilder> AddReviewerEditorToSOSCollection(SOSHub Master, User Slave);
        Task<AsyncVoidMethodBuilder> AddApproverOwnersToSOSCollection(SOSHub Master, User Slave);
        
        Task<AsyncVoidMethodBuilder> AddProductToSOSCollection(SOSHub Master, Product Slave);

        #endregion
        #region Remove from Sos Hub
        Task<int> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id);
        Task<int> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id);
        Task<int> RemoveCDFromSOSData(int SOS_DataPool_id, int File_id);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllToolsEquipmentMaterial(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllReviewerEditors(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllApproverOwners(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisBkups(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSections(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllProcessSheetCommentary(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllCommonDirections(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllProducts(SOSHub Master);
        #endregion 
      
        #region Tool
        Task<int> AddRangeTool(List<Tool> ToolsToAdd);
        Task<List<ToolUsed>> AddRangeToolsUsed(List<ToolUsed> ToolsUsedToAdd);
        Task<Tool> CreateNewTool(Tool TooltoCreate);
        Task<Tool> GetToolById(int id);
        Task<ToolUsed> GetToolUsedById(int id);
        Task<IEnumerable<Tool>> GetAllTools();
        Task<IEnumerable<Tool>> GetMatchTools(string ToolToFind);
        Task<int> UpdateTool(ToolForUpdateDto ToolForUpdate, Tool ToolEntity);
        Task<int> UpdateToolUsed(ToolUsedForUpdateDto toolForUpdate);

        Task<int> DeleteTool(int id);
        #endregion
        #region Material
        Task<int> AddRangeMaterial(List<Material> MaterialsToAdd);
        Task<List<MaterialUsed>> AddRangeMaterialUsed(List<MaterialUsed> MaterialsUsedToAdd);
        Task<Material> CreateNewMaterial(Material MaterialtoCreate);
        Task<Material> GetMaterialById(int id);
        Task<MaterialUsed> GetMaterialUsedById(int id);
        Task<IEnumerable<Material>> GetAllMaterials();
        Task<IEnumerable<Material>> GetMatchMaterials(string MaterialToFind);
        Task<int> UpdateMaterial(MaterialForUpdateDto materialForUpdate, Material MaterialEntity);
        Task<int> UpdateMaterialUsed(MaterialsUsedForUpdateDto materialForUpdate);
        Task<int> DeleteMaterial(int id);
        #endregion
        #region Equipment
        Task<int> AddRangeEquipment(List<Equipment> EquipmentsToAdd);
        Task<Equipment> CreateNewEquipment(Equipment EquipmenttoCreate);
        Task<Equipment> GetEquipmentById(int id);
        Task<IEnumerable<Equipment>> GetAllEquipments();
        Task<IEnumerable<Equipment>> GetMatchEquipments(string EquipmentToFind);
        Task<int> UpdateEquipment(EquipmentForUpdateDto EquipmentForUpdate, Equipment EquipmentEntity);
        Task<int> DeleteEquipment(int id);
        #endregion
        #region Commentary
        Task<List<Commentary>> AddRangeCommentary(List<Commentary> commentariesToAdd);
        Task<Commentary> GetCommentaryById(int id);
        Task<int> UpdateCommentary(UpdateCommentaryDto CommentaryForUpdate);

        #endregion
        #region Users
        Task<User> GetUserById(int id);

        #endregion
        #region Product
        Task<Product> GetProductById(int id);
        #endregion
        #region CommonDirection
        Task<CommonDirection> GetCommonDirectionById(int id);
        Task<int> UpdateCommonDirection(CommonDirectionDto commonDirectionForUpdate);
        Task<CommonDirection> CreateNewCommonDir(CommonDirection CommonDirtoCreate);
        Task<List<CommonDirection>> AddRangeCommonDirection(List<CommonDirection> CommonDirtoCreate);
        Task<List<CommonDirection>> GetAllCommonDirectionInactives();
        #endregion
        #region Analysis Bkup
        Task<List<AnalysisBkup>> AddRangeAnalysisBkup(List<AnalysisBkup> analysisBkupsToAdd);
        Task<AnalysisBkup> GetAnalysisBkupId(int id);
        Task<int> UpdateAnalysisBkup(AnalysisBkupForUpdateDto analysisBkupForUpdate);
        #endregion
        #region Section
        Task<List<Section>> AddRangeSections(List<Section> SectionsToAdd);
        Task<Section> GetSectionById(int id);
        Task<int> UpdateSection(SectionForUpdateDto sectionForUpdate);

        #endregion
        #region SosTime
        Task<SOSTime> GetSOSTimeById(int id);
        Task<int> UpdateTime(SOSTimeForUpdateDto timeForUpdate);

        Task<List<SOSTime>> AddRangeSOSTimes(List<SOSTime> SOSTimesToAdd);

        Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSAnalysis(SOSAnalysis Master, SOSTime Slave);
        Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSSequence(SOSSequence Master, SOSTime Slave);
        Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSDistribution(SOSDistribution Master, SOSTime Slave);

        Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSAnalysis(SOSAnalysis Master);
        Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSSequence(SOSSequence Master);
        #endregion
        #region Turn
        Task<Turn> GetTurnById(int id);
        Task<int> UpdateTurn(TurnForUpdateDto TurnForUpdate);

        Task<List<Turn>> AddRangeTurns(List<Turn> TurnsToAdd);
        Task<AsyncVoidMethodBuilder> AddTurnToSOSCombination(SOSCombination Master, Turn Slave);
        Task<AsyncVoidMethodBuilder> AddTurnToSOSDistribution(SOSDistribution Master, Turn Slave);

        Task<AsyncVoidMethodBuilder> RemoveAllTurnsFromSOSCombination(SOSCombination Master);
        Task<AsyncVoidMethodBuilder> RemoveAllTurnsFromSOSDistribution(SOSDistribution Master);
        #endregion
       

        #region HistoryHubCollection
        Task<int> CreateHistorySOScollection(SOSHubHistory SOS_EntityToCreate);
        Task<IEnumerable<SOSHubHistory>> GetAllHistorySOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<AsyncVoidMethodBuilder> AddHistoryToSOSCollection(SOSHub Master, SOSHubHistory Slave);

        #endregion

        //SOS Analysis
        #region SOSAnalysis
        Task<int> CreateSOSAnalysis(SOSAnalysis SOS_AnalysisToCreate);
        Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);

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
        //SOS Sequence
        #region SOSSequence
        Task<int> CreateSOSSequence(SOSSequence SOS_SequenceToCreate);
        Task<SOSSequence> GetSOSSequence(int SOSSequenceId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSSequence>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);

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
        //SOS Distribution
        #region SOSDistribution
        Task<int> CreateSOSDistribution(SOSDistribution SOS_DistributionToCreate);
        Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false);
        Task<IEnumerable<SOSDistribution>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false);

        Task<int> UpdateSOSDistribution(SOSDistributionForUpdateDto DistributionUpdate, SOSDistribution DistributionEntity);
        Task<int> RemoveSOSDistribution(int SOS_Distribution_id);

        Task AddIlustrationToSOSDistribution(int SOS_Distribution_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSDistribution(int SOS_Distribution_id, int ImageFile_id);

        #endregion
        #region Add Range SOS Distribution
        Task<List<SOSDistributionLogbook>> AddRangeSOSDistributionLogbook(List<SOSDistributionLogbook> SOSDistributionLogbooksToAdd);
        #endregion
        #region Add To Sos Distribution
        Task<AsyncVoidMethodBuilder> AddSOSDistributionLogbookToSOSDistribution(SOSDistribution Master, SOSDistributionLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddSOSDistributionAdditionalTimeToSOSDistribution(SOSDistribution Master, SOSDistributionAdditionalTime Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSDistribution(SOSDistribution Master, Commentary Slave);

        #endregion
        #region Remove from SosDistribution
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(SOSDistribution Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSDistribution(SOSDistribution Master);
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

        //SOS Combination
        #region SOSCombination
        Task<int> CreateSOSCombination(SOSCombination SOS_CombinationToCreate);
        Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSCombination>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false);

        Task<int> UpdateSOSCombination(SOSCombinationForUpdateDto CombinationUpdate, SOSCombination CombinationEntity);
        Task<int> RemoveSOSCombination(int SOS_Combination_id);


        #endregion
        #region Add Range SOS Combination
        Task<List<SOSCombinationLogbook>> AddRangeSOSCombinationLogbook(List<SOSCombinationLogbook> SOSCombinationLogbooksToAdd);
        #endregion
        #region Add To Sos Combination
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

        //SOS Flow
        #region SOSFlow
        Task<int> CreateSOSFlow(SOSFlow SOS_FlowToCreate);
        Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false);
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

        #region commonOperations
        Task<FileUpload?> FetchFileAsync(int fileid);
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task<bool> SaveChangesAsync();
        Task<bool> SaveChanges();
        #endregion

    }
}
