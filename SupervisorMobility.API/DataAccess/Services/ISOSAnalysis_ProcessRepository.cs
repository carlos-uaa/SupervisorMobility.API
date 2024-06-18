using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface ISOSAnalysis_ProcessRepository
    {
        #region SOS_DataPool

        Task<int> CreateSOScollection(SOSHub SOS_EntityToCreate);
        Task<SOSHub> GetSOSHub(int HubId, bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false);
        Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false);

        Task<int> UpdateSOSHub(SOSHubForUpdateDto HubUpdate, SOSHub SosEntity);
        Task AddImageToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task AddVideoToSOSData(int SOS_DataPool_id, FileUpload evidence);
        Task<int> AddRangeCommentary(List<Commentary> commentariesToAdd);
        Task<int> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id);
        Task<int> RemoveVideoFromSOSData(int SOS_DataPool_id, int ImageFile_id);
        #endregion


        #region commonOperations
         Task<FileUpload?> FetchFileAsync(int fileid);
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task<bool> SaveChangesAsync();
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
    }
}
