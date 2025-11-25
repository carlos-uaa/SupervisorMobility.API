using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Dtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
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
        Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeHistory = false, bool includeDeleteds = false, bool includeCollections = false, bool includePeopleCollections = false, bool includePats = false);
        Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false,bool includeSOSDistribution = false, int userId = 0);
        Task<int> UpdateSOSHub(SOSHubForUpdateDto HubUpdate, SOSHub SosEntity);
        Task<int> UpdateSOSHub(SOSHub SosEntity);
        Task<int> RemoveSOSHub(int SOS_DataPool_id);
        #endregion

        #region AddTo Sos Hub
        Task<AsyncVoidMethodBuilder> AddProcessSheetCommentaryToSOSCollection(SOSHub Master, Commentary Slave);
        Task<AsyncVoidMethodBuilder> AddAnaysisBkupToSOSCollection(SOSHub Master, AnalysisBkup Slave);
        Task<AsyncVoidMethodBuilder> AddHCISOSCollection(SOSHub master, HCI slave);
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
     
        Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSAnalysis(SOSAnalysis Master);
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

        #region commonOperations
        Task<FileUpload?> FetchFileAsync(int fileid);
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task<bool> SaveChangesAsync();
        Task<bool> SaveChanges();
        #endregion

    }
}
