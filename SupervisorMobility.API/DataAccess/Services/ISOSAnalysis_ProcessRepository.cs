using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface ISOSAnalysis_ProcessRepository
    {
        #region SOS_DataPool
        Task<int> CreateSOScollection(SOSHub SOS_EntityToCreate);
        Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false);
        Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<int> UpdateSOSHub(SOSHubForUpdateDto HubUpdate, SOSHub SosEntity);
        Task<int> RemoveSOSHub(int SOS_DataPool_id);
        #endregion
        #region History Hub Collection
        Task<int> CreateHistorySOScollection(SOSHubHistory SOS_EntityToCreate);
        Task<IEnumerable<SOSHubHistory>> GetAllHistorySOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<AsyncVoidMethodBuilder> AddHistoryToSOSCollection(SOSHub Master, SOSHubHistory Slave);

        #endregion
        #region AddTo Sos Hub
        Task<AsyncVoidMethodBuilder> AddProcessSheetCommentaryToSOSCollection(SOSHub Master, Commentary Slave);
        Task<AsyncVoidMethodBuilder> AddAnaysisBkupToSOSCollection(SOSHub Master, AnalysisBkup Slave);
        Task<AsyncVoidMethodBuilder> AddSectionSOSCollection(SOSHub Master, Section Slave);
        Task<AsyncVoidMethodBuilder> AddMaterialToSOSCollection(SOSHub Master, Material Slave);
        Task<AsyncVoidMethodBuilder> AddEquipmentToSOSCollection(SOSHub Master, Equipment Slave);
        Task<AsyncVoidMethodBuilder> AddToolToSOSCollection(SOSHub Master, Tool Slave);
        Task<AsyncVoidMethodBuilder> AddCommonDirectionsToSOSCollection(SOSHub Master, List<CommonDirection> Slave);
        Task AddImageToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task AddVideoToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task AddCDToSOSData(int SOS_DataPool_id, FileUpload evidence);
        #endregion
        #region Remove from Sos Hub
        Task<int> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id);
        Task<int> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id);
        Task<int> RemoveCDFromSOSData(int SOS_DataPool_id, int File_id);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllToolsEquipmentMaterial(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisBkups(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSections(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllProcessSheetCommentary(SOSHub Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllCommonDirections(SOSHub Master);
        #endregion 
        #region AddTo Ranges
        Task<int> AddRangeCommentary(List<Commentary> commentariesToAdd);
        Task<int> AddRangeAnalysisBkup(List<AnalysisBkup> analysisBkupsToAdd);
        Task<int> AddRangeSections(List<Section> SectionsToAdd);
        #endregion
        #region Tool
        Task<int> AddRangeTool(List<Tool> ToolsToAdd);
        Task<Tool> CreateNewTool(Tool TooltoCreate);
        Task<Tool> GetToolById(int id);
        Task<IEnumerable<Tool>> GetAllTools();
        Task<IEnumerable<Tool>> GetMatchTools(string ToolToFind);
        Task<int> UpdateTool(ToolForUpdateDto ToolForUpdate, Tool ToolEntity);
        Task<int> DeleteTool(int id);
        #endregion
        #region Material
        Task<int> AddRangeMaterial(List<Material> MaterialsToAdd);
        Task<Material> CreateNewMaterial(Material MaterialtoCreate);
        Task<Material> GetMaterialById(int id);
        Task<IEnumerable<Material>> GetAllMaterials();
        Task<IEnumerable<Material>> GetMatchMaterials(string MaterialToFind);
        Task<int> UpdateMaterial(MaterialForUpdateDto materialForUpdate, Material MaterialEntity);
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
        Task<Commentary> GetCommentaryById(int id);
        #endregion
        #region SpecialCaseAbnormalSituation
        Task<SpecialCaseAbnormalSituation> GetSpecialCaseAbnormalSituationById(int id);
        #endregion
        #region SOSAnalysisLogbook
        Task<SOSAnalysisLogbook> GetSOSAnalysisLogbookById(int id);
        #endregion
        #region CommonDirection
        Task<List<CommonDirectionDto>> ManageRangeCommonDirs(List<CommonDirectionDto> listToManage, int SOSHubId);
        Task<CommonDirection> CreateNewCommonDir(CommonDirection CommonDirtoCreate);
        Task<List<CommonDirection>> TrackCommonDirs(List<CommonDirectionDto> commonDirections);
        #endregion
        #region Analysis Bkup
        Task<AnalysisBkup> GetAnalysisBkupId(int id);

        #endregion
        #region Section
        Task<Section> GetSectionById(int id);

        #endregion
        #region SOSAnalysis
        Task<int> CreateSOSAnalysis(SOSAnalysis SOS_AnalysisToCreate);
        Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes= false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);

        Task<int> UpdateSOSAnalysis(SOSAnalysisForUpdateDto AnalysisUpdate, SOSAnalysis AnalysisEntity);
        Task<int> RemoveSOSAnalysis(int SOS_Analysis_id);

        Task AddIlustrationToSOSAnalysis(int SOS_Analysis_id, FileUpload evidence);
        Task<int> RemoveIlustrationFromSOSAnalysis(int SOS_Analysis_id, int ImageFile_id);

        #endregion
        #region Add Range SOS Analysis
        Task<int> AddRangeSpecialCasesAbnormalSituations(List<SpecialCaseAbnormalSituation> SpecialCasesAbnormalSituationsToAdd);
        Task<int> AddRangeSOSAnalysisLogbook(List<SOSAnalysisLogbook> SOSAnalysisLogbooksToAdd);
        #endregion
        #region Add To Sos Analysis
        Task<AsyncVoidMethodBuilder> AddSpecialCasesAbnormalSituationsToSOSAnalysis(SOSAnalysis Master, SpecialCaseAbnormalSituation Slave);
        Task<AsyncVoidMethodBuilder> AddSOSAnalysisLogbookToSOSAnalysis(SOSAnalysis Master, SOSAnalysisLogbook Slave);
        Task<AsyncVoidMethodBuilder> AddNoteToSOSAnalysis(SOSAnalysis Master, Commentary Slave);

        #endregion
        #region Remove from Sos Hub
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSpecialCasesAbnormalSituationsFromSOSAnalysis(SOSAnalysis Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(SOSAnalysis Master);
        Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSAnalysis(SOSAnalysis Master);

        #endregion
        #region commonOperations
        Task<FileUpload?> FetchFileAsync(int fileid);
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task<bool> SaveChangesAsync();
        #endregion

    }
}
