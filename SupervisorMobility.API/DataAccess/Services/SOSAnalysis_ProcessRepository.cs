using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DuoVia.FuzzyStrings;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using Tavis.UriTemplates;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class SOSAnalysis_ProcessRepository : ISOSAnalysis_ProcessRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;


        public SOSAnalysis_ProcessRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region SOS_DataPool
        public async Task<int> CreateSOScollection(SOSHub SOS_EntityToCreate)
        {
           _context.SOSHubs.Add(SOS_EntityToCreate);
            return _context.SaveChanges();
        }


        public async Task<SOSHub> GetSOSHub(int HubId, bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var query = _context.SOSHubs.Where(SOS => SOS.SOSHubId == HubId && SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Images);
            }

            if (includeVideos)
            {
                query = query.Include(query => query.Videos);
            }

            if (includeTools)
            {
                query = query.Include(t => t.ToolsUsed);
            }

            if (includeEquipments)
            {
                query = query.Include(e => e.SafetyEquipment);
            }

            if (includeMaterials)
            {
                query = query.Include(m => m.MaterialsUsed);
            }

            if (includeInformation)
            {
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Department);
            }

            if (includePeople)
            {
                query = query.Include(o => o.Owner).Include(e => e.Editor);
            }
             
            if (includeDocuments)
            {
                query = query.Include(o => o.CommonDirection);
            }


            var sosHub = await query.FirstOrDefaultAsync();

            if (sosHub == null)
                return null;

            // Filtrar los subobjetos manualmente después de la carga inicial
            if (includeImages)
            {
                sosHub.Images = sosHub.Images.Where(i => i.IsActive == true).ToList();
            }

            if (includeVideos)
            {
                sosHub.Videos = sosHub.Videos.Where(v => v.IsActive == true).ToList();
            }

            if (includeTools)
            {
                sosHub.ToolsUsed = sosHub.ToolsUsed.Where(t => t.IsActive == true).ToList();
            }

            if (includeEquipments)
            {
                sosHub.SafetyEquipment = sosHub.SafetyEquipment.Where(e => e.IsActive == true).ToList();
            }

            if (includeMaterials)
            {
                sosHub.MaterialsUsed = sosHub.MaterialsUsed.Where(m => m.IsActive == true).ToList();
            }

            if (includeDocuments)
            {
                sosHub.CommonDirection = sosHub.CommonDirection.Where(i => i.IsActive == true).ToList();
            }

            return sosHub;
        }
        public async Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var query = _context.SOSHubs.Where(h => h.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Images);
            }

            if (includeVideos)
            {
                query = query.Include(query => query.Videos);
            }

            if (includeTools)
            {
                query = query.Include(t => t.ToolsUsed);
            }

            if (includeEquipments)
            {
                query = query.Include(e => e.SafetyEquipment);
            }

            if (includeMaterials)
            {
                query = query.Include(m => m.MaterialsUsed);
            }

            if (includeInformation)
            {
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Department);
            }

            if (includePeople)
            {
                query = query.Include(o => o.Owner).Include(e => e.Editor);
            }

            var sosHubs = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.Images = sosHub.Images.Where(i => i.IsActive == true).ToList();
                }
            }

            if (includeVideos)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.Videos = sosHub.Videos.Where(v => v.IsActive == true).ToList();
                }
            }

            if (includeTools)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.ToolsUsed = sosHub.ToolsUsed.Where(t => t.IsActive == true).ToList();
                }
            }

            if (includeEquipments)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.SafetyEquipment = sosHub.SafetyEquipment.Where(e => e.IsActive == true).ToList();
                }
            }

            if (includeMaterials)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.MaterialsUsed = sosHub.MaterialsUsed.Where(m => m.IsActive == true).ToList();
                }
            }

            if (includeDocuments)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.CommonDirection = sosHub.CommonDirection.Where(m => m.IsActive == true).ToList();
                }
            }

            return sosHubs;

        }
        public async Task<int> UpdateSOSHub(SOSHubForUpdateDto HubUpdate, SOSHub SosEntity)
        {
            _mapper.Map(HubUpdate, SosEntity);
            _context.SOSHubs.Update(SosEntity);
            return await _context.SaveChangesAsync();
        }
        public async Task AddImageToSOSData(int SOS_DataPool_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSHub(SOS_DataPool_id, includeImages: true);

            if (SosHubEntity != null)
            {

                if (SosHubEntity.Images != null)
                {
                    SosHubEntity.Images.Add(evidence);
                }
                else
                {
                    SosHubEntity.Images = new List<FileUpload>
                    {
                        evidence
                    };
                }
            }

        }
        public async Task AddVideoToSOSData(int SOS_DataPool_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSHub(SOS_DataPool_id, includeImages: true);

            if (SosHubEntity != null)
            {

                if (SosHubEntity.Videos != null)
                {
                    SosHubEntity.Videos.Add(evidence);
                }
                else
                {
                    SosHubEntity.Videos = new List<FileUpload>
                    {
                        evidence
                    };
                }
            }

        }

        public async Task AddCDToSOSData(int SOS_DataPool_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSHub(SOS_DataPool_id, includeDocuments: true);

            if (SosHubEntity != null)
            {

                if (SosHubEntity.CommonDirection != null)
                {
                    SosHubEntity.CommonDirection.Add(evidence);
                }
                else
                {
                    SosHubEntity.CommonDirection = new List<FileUpload>
                    {
                        evidence
                    };
                }
            }

        }

        public async Task<int> AddRangeCommentary(List<Commentary> commentariesToAdd)
        {
            _context.Comments.AddRange(commentariesToAdd);
            return await _context.SaveChangesAsync();
        }
        public Task<int> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveVideoFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveCDFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Tool
        public async Task<int> AddRangeTool(List<Tool> ToolsToAdd) {
            _context.Tools.AddRange(ToolsToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Tool> CreateNewTool(Tool TooltoCreate) {
            _context.Add(TooltoCreate);
             await _context.SaveChangesAsync();

            return TooltoCreate;
        }
        public async  Task<Tool> GetToolById(int id) {
            var tool = await _context.Tools.Where(t => t.ToolId == id && t.IsActive== true).FirstOrDefaultAsync();
            return tool;
        }
        public async  Task<IEnumerable<Tool>> GetAllTools() {
            var tools =  _context.Tools.Where(t => t.IsActive == true);
            return await tools.OrderBy(t => t.ToolId).ToListAsync();
        }
        public async  Task<IEnumerable<Tool>> GetMatchTools(string ToolToFind) {
            return _context.Tools.Where(t => t.ToolName.DiceCoefficient(ToolToFind) > 0.5).ToList();
        }
        public async  Task<int> UpdateTool(ToolForUpdateDto ToolForUpdate, Tool ToolEntity) {

            _mapper.Map(ToolForUpdate, ToolEntity);
            _context.Update(ToolEntity);

            return await _context.SaveChangesAsync();
        }
        public async  Task<int> DeleteTool(int id) { 
            var ToolEntity = await GetToolById(id);
            ToolEntity.IsActive = false;

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Material
        public async Task<int> AddRangeMaterial(List<Material> MaterialsToAdd)
        {
            _context.Materials.AddRange(MaterialsToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Material> CreateNewMaterial(Material MaterialtoCreate)
        {
            _context.Add(MaterialtoCreate);
            await _context.SaveChangesAsync();

            return MaterialtoCreate;
        }
        public async Task<Material> GetMaterialById(int id)
        {
            var Material = await _context.Materials.Where(t => t.MaterialId == id && t.IsActive == true).FirstOrDefaultAsync();
            return Material;
        }
        public async Task<IEnumerable<Material>> GetAllMaterials()
        {
            var Materials = _context.Materials.Where(t => t.IsActive == true);
            return await Materials.OrderBy(t => t.MaterialId).ToListAsync();
        }
        public async Task<IEnumerable<Material>> GetMatchMaterials(string MaterialToFind)
        {
            return _context.Materials.Where(t => t.MaterialName.DiceCoefficient(MaterialToFind) > 0.5).ToList();
        }
        public async Task<int> UpdateMaterial(MaterialForUpdateDto MaterialForUpdate, Material MaterialEntity)
        {
            _mapper.Map(MaterialForUpdate, MaterialEntity);
            _context.Update(MaterialEntity);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteMaterial(int id)
        {
            var MaterialEntity = await GetMaterialById(id);
            MaterialEntity.IsActive = false;

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Equipment
        public async Task<int> AddRangeEquipment(List<Equipment> EquipmentsToAdd)
        {
            _context.Equipments.AddRange(EquipmentsToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Equipment> CreateNewEquipment(Equipment EquipmenttoCreate)
        {
            _context.Add(EquipmenttoCreate);
            await _context.SaveChangesAsync();

            return EquipmenttoCreate;
        }
        public async Task<Equipment> GetEquipmentById(int id)
        {
            var Equipment = await _context.Equipments.Where(t => t.EquipmentId == id && t.IsActive == true).FirstOrDefaultAsync();
            return Equipment;
        }
        public async Task<IEnumerable<Equipment>> GetAllEquipments()
        {
            var Equipments = _context.Equipments.Where(t => t.IsActive == true);
            return await Equipments.OrderBy(t => t.EquipmentId).ToListAsync();
        }
        public async Task<IEnumerable<Equipment>> GetMatchEquipments(string EquipmentToFind)
        {
            return _context.Equipments.Where(t => t.EquipmentName.DiceCoefficient(EquipmentToFind) > 0.5).ToList();
        }
        public async Task<int> UpdateEquipment(EquipmentForUpdateDto EquipmentForUpdate, Equipment EquipmentEntity)
        {

            _mapper.Map(EquipmentForUpdate, EquipmentEntity);
            _context.Update(EquipmentEntity);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteEquipment(int id)
        {
            var EquipmentEntity = await GetEquipmentById(id);
            EquipmentEntity.IsActive = false;

            return await _context.SaveChangesAsync();
        }
        #endregion






        #region CommonOperations
        public async Task<FileUpload?> FetchFileAsync(int fileid)
        {
            return await _context.Files
                .Where(p => p.FileUploadId == fileid).FirstOrDefaultAsync();
        }
        public async Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile)
        {
            var finalNewFile = _mapper.Map<FileUpload>(newFile);
            _context.Files.Add(finalNewFile);
            await _context.SaveChangesAsync();
            return finalNewFile;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
                #endregion


    }
}
