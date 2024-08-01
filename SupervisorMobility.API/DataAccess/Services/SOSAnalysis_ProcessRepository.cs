using AutoMapper;
using CsvHelper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2021.PowerPoint.Tasks;
using DuoVia.FuzzyStrings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
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
            return await _context.SaveChangesAsync();
        }
        public async Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeHistory = false, bool includeDeleteds = false)
        {
            var query = _context.SOSHubs.AsNoTracking().Where(SOS => SOS.SOSHubId == HubId && SOS.IsActive == true);

            if (includeAnalysesBkup)
            {
                query = query.Include(i => i.AnalysesBkup);
            }

            if (includeSections)
            {
                query = query.Include(i => i.Sections).ThenInclude(s => s.Analyses);
            }

            if (includeImages)
            {
                query = query.Include(i => i.Images);
            }

            if (includeVideos)
            {
                query = query.Include(query => query.Videos);
            }
            if (includeCommentaries)
            {
                query = query.Include(query => query.ProcessSheetCommentary);
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
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Distribution).Include(d => d.Department);
            }

            if (includePeople)
            {
                query = query.Include(o => o.Owner).Include(e => e.Editor);
            }

            if (includeDocuments)
            {
                query = query.Include(o => o.CommonDirection);
            }

            if (includeModel)
            {
                query = query.Include(m => m.AppliedModel);
            }

            if (includeHistory)
            {
                query = query.Include(m => m.History);
            }

            var sosHub = await query.FirstOrDefaultAsync();

            if (sosHub == null)
                return null;

            // Filtrar los subobjetos manualmente después de la carga inicial
            if (!includeDeleteds)
            {
                if (includeAnalysesBkup)
                {
                    sosHub.AnalysesBkup = sosHub.AnalysesBkup.Where(i => i.IsActive == true).ToList();
                }

                if (includeSections)
                {
                    sosHub.Sections = sosHub.Sections.Where(i => i.IsActive == true).ToList();
                }

                if (includeImages)
                {
                    sosHub.Images = sosHub.Images.Where(i => i.IsActive == true).ToList();
                }

                if (includeVideos)
                {
                    sosHub.Videos = sosHub.Videos.Where(v => v.IsActive == true).ToList();
                }

                if (includeCommentaries)
                {
                    sosHub.ProcessSheetCommentary = sosHub.ProcessSheetCommentary.Where(t => t.IsActive == true).ToList();
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
            }

            return sosHub;
        }
        public async Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var query = _context.SOSHubs.AsNoTracking().Where(h => h.IsActive == true);

            if (includeAnalysesBkup)
            {
                query = query.Include(i => i.AnalysesBkup);
            }

            if (includeSections)
            {
                query = query.Include(i => i.Sections).ThenInclude(s => s.Analyses);
            }

            if (includeImages)
            {
                query = query.Include(i => i.Images);
            }

            if (includeVideos)
            {
                query = query.Include(query => query.Videos);
            }

            if (includeCommentaries)
            {
                query = query.Include(query => query.ProcessSheetCommentary);
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
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Distribution).Include(d => d.Department);
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

            if (includeCommentaries)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.ProcessSheetCommentary = sosHub.ProcessSheetCommentary.Where(t => t.IsActive == true).ToList();
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
            // Adjunta la entidad al contexto si no está ya adjunta
            if (_context.Entry(SosEntity).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(SosEntity);
            }

            // Mapea los cambios del DTO a la entidad
            _mapper.Map(HubUpdate, SosEntity);

            // Marca la entidad como modificada
            _context.Entry(SosEntity).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveSOSHub(int SOS_DataPool_id)
        {
            var SosEntity = await GetSOSHub(SOS_DataPool_id);
            SosEntity.IsActive = false;
            _context.SOSHubs.Update(SosEntity);
            return await _context.SaveChangesAsync();
        }



        #endregion
        #region SOS History Collection
        public async Task<int> CreateHistorySOScollection(SOSHubHistory SOS_EntityToCreate)
        {
            _context.SOSHubsHistory.Add(SOS_EntityToCreate);
            return await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<SOSHubHistory>> GetAllHistorySOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var query = _context.SOSHubsHistory.AsNoTracking().Where(h => h.IsActive == true && h.SOSHubId == HubId);

            if (includeAnalysesBkup)
            {
                query = query.Include(i => i.AnalysesBkup);
            }

            if (includeSections)
            {
                query = query.Include(i => i.Sections).ThenInclude(s => s.Analyses);
            }

            if (includeImages)
            {
                query = query.Include(i => i.Images);
            }

            if (includeVideos)
            {
                query = query.Include(query => query.Videos);
            }

            if (includeCommentaries)
            {
                query = query.Include(query => query.ProcessSheetCommentary);
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
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Distribution).Include(d => d.Department);
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

            if (includeCommentaries)
            {
                foreach (var sosHub in sosHubs)
                {
                    sosHub.ProcessSheetCommentary = sosHub.ProcessSheetCommentary.Where(t => t.IsActive == true).ToList();
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


        public async Task<AsyncVoidMethodBuilder> AddHistoryToSOSCollection(SOSHub Master, SOSHubHistory Slave)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.History != null)
            {
                Master.History.Add(Slave);
            }
            else
            {
                Master.History = new List<SOSHubHistory>();
                Master.History.Add(Slave);
            }
            await _context.SaveChangesAsync();
            return new AsyncVoidMethodBuilder();
        }

        #endregion

        #region AddTo Sos Hub
        public async Task<AsyncVoidMethodBuilder> AddProcessSheetCommentaryToSOSCollection(SOSHub Master, Commentary Slave)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }
            if (Master.ProcessSheetCommentary != null)
            {
                Master.ProcessSheetCommentary.Add(Slave);
            }
            else
            {
                Master.ProcessSheetCommentary = new List<Commentary>();
                Master.ProcessSheetCommentary.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddAnaysisBkupToSOSCollection(SOSHub master, AnalysisBkup slave)
        {
            try
            {
                // Verifica si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry == null)
                {
                    // Si no está rastreado, adjunta el master al contexto
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }
                else
                {
                    master = localMasterEntry;
                }

                // Verifica si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.AnalysisBkups.Local.FirstOrDefault(entry => entry.AnalysisBkupId == slave.AnalysisBkupId);
                if (localSlaveEntry == null)
                {
                    // Si no está rastreado, adjunta el slave al contexto
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.AnalysisBkups.Attach(slave);
                    }
                }
                else
                {
                    slave = localSlaveEntry;
                }

                // Agrega el slave a la colección del master
                if (master.AnalysesBkup == null)
                {
                    master.AnalysesBkup = new List<AnalysisBkup>();
                }

                if (!master.AnalysesBkup.Contains(slave))
                {
                    master.AnalysesBkup.Add(slave);
                }

                // Guarda los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddSectionSOSCollection(SOSHub master, Section slave)
        {

            try
            {
                // Verifica si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry == null)
                {
                    // Si no está rastreado, adjunta el master al contexto
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }
                else
                {
                    master = localMasterEntry;
                }

                // Verifica si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Sections.Local.FirstOrDefault(entry => entry.SectionId == slave.SectionId);
                if (localSlaveEntry == null)
                {
                    // Si no está rastreado, adjunta el slave al contexto
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Sections.Attach(slave);
                    }
                }
                else
                {
                    slave = localSlaveEntry;
                }

                // Agrega el slave a la colección del master
                if (master.Sections == null)
                {
                    master.Sections = new List<Section>();
                }

                if (!master.Sections.Contains(slave))
                {
                    master.Sections.Add(slave);
                }

                // Guarda los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddToolToSOSCollection(SOSHub Master, Tool Slave)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.ToolsUsed != null)
            {

                Master.ToolsUsed.Add(Slave);
            }
            else
            {
                Master.ToolsUsed = new List<Tool>();
                Master.ToolsUsed.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddEquipmentToSOSCollection(SOSHub Master, Equipment Slave)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }
            if (Master.SafetyEquipment != null)
            {
                Master.SafetyEquipment.Add(Slave);
            }
            else
            {
                Master.SafetyEquipment = new List<Equipment>();
                Master.SafetyEquipment.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddCommonDirectionsToSOSCollection(SOSHub master, List<CommonDirection> slaves)
        {
            try
            {
                // Verifica si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry == null)
                {
                    // Si no está rastreado, adjunta el master al contexto
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }
                else
                {
                    master = localMasterEntry;
                }

                foreach (var slave in slaves)
                {
                    // Verifica si el slave ya está siendo rastreado en el contexto
                    var localSlaveEntry = _context.CommonDirections.Local.FirstOrDefault(entry => entry.CommonDirectionId == slave.CommonDirectionId);
                    if (localSlaveEntry == null)
                    {
                        // Si no está rastreado, adjunta el slave al contexto
                        if (_context.Entry(slave).State == EntityState.Detached)
                        {
                            _context.CommonDirections.Attach(slave);
                        }

                        // Agrega el slave a la colección del master
                        if (master.CommonDirection == null)
                        {
                            master.CommonDirection = new List<CommonDirection>();
                        }

                        if (!master.CommonDirection.Any(cd => cd.CommonDirectionId == slave.CommonDirectionId))
                        {
                            master.CommonDirection.Add(slave);
                        }
                    }
                    else
                    {
                        // Agrega el localSlaveEntry a la colección del master
                        if (master.CommonDirection == null)
                        {
                            master.CommonDirection = new List<CommonDirection>();
                        }

                        if (!master.CommonDirection.Any(cd => cd.CommonDirectionId == localSlaveEntry.CommonDirectionId))
                        {
                            master.CommonDirection.Add(localSlaveEntry);
                        }
                    }
                }

                // Guarda los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }

            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddMaterialToSOSCollection(SOSHub Master, Material Slave)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.MaterialsUsed != null)
            {
                Master.MaterialsUsed.Add(Slave);
            }
            else
            {
                Master.MaterialsUsed = new List<Material>();
                Master.MaterialsUsed.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task AddImageToSOSData(int SOS_DataPool_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSHub(SOS_DataPool_id, includeImages: true);
            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(SosHubEntity);
            }

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

            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(SosHubEntity);
            }

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

        #endregion

        #region Remove from Sos Hub
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisBkups(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.AnalysesBkup?.Count > 0)
            {
                Master.AnalysesBkup.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllAnalysisBkups]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSections(SOSHub Master)
        {

            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.Sections?.Count > 0)
            {
                Master.Sections.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllSections]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllProcessSheetCommentary(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.ProcessSheetCommentary?.Count > 0)
            {
                Master.ProcessSheetCommentary.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllProcessSheetCommentary]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();

        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllToolsEquipmentMaterial(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.ToolsUsed?.Count > 0)
            {
                Master.ToolsUsed.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllToolsEquipmentMaterial]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            if (Master.SafetyEquipment?.Count > 0)
            {
                Master.SafetyEquipment.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllToolsEquipmentMaterial]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            if (Master.MaterialsUsed?.Count > 0)
            {
                Master.MaterialsUsed.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllToolsEquipmentMaterial]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }


            return new AsyncVoidMethodBuilder();

        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllCommonDirections(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.CommonDirection?.Count > 0)
            {
                Master.CommonDirection.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [SOSDataRemoveAllCommonDirections]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();

        }


        public async Task<int> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            var SOSHubEntity = await GetSOSHub(SOS_DataPool_id, includeImages: true);

            var Sketch = SOSHubEntity.Images.ToList().Find(i => i.FileUploadId == ImageFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSHubs.Update(SOSHubEntity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id)
        {
            var SOSHubEntity = await GetSOSHub(SOS_DataPool_id, includeVideos: true);

            var Sketch = SOSHubEntity.Videos.ToList().Find(i => i.FileUploadId == VideoFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSHubs.Update(SOSHubEntity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveCDFromSOSData(int SOS_DataPool_id, int File_id)
        {
            var SOSHubEntity = await GetSOSHub(SOS_DataPool_id, includeDocuments: true);

            var Sketch = SOSHubEntity.CommonDirection.ToList().Find(i => i.CommonDirectionId == File_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSHubs.Update(SOSHubEntity);

            return await _context.SaveChangesAsync();
        }
        #endregion

        #region AddTo Ranges
        public async Task<List<Section>> AddRangeSections(List<Section> SectionsToAdd)
        {
            _context.Sections.AddRange(SectionsToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var section in SectionsToAdd)
            {
                _context.Entry(section).State = EntityState.Detached;
                foreach (var analysis in section.Analyses)
                {
                    _context.Entry(analysis).State = EntityState.Detached;
                }
            }

            return SectionsToAdd;
        }
        public async Task<List<Commentary>> AddRangeCommentary(List<Commentary> commentariesToAdd)
        {
            _context.Comments.AddRange(commentariesToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var section in commentariesToAdd)
            {
                _context.Entry(section).State = EntityState.Detached;
            }

            return commentariesToAdd;
        }

        public async Task<List<AnalysisBkup>> AddRangeAnalysisBkup(List<AnalysisBkup> analysisBkupsToAdd)
        {
            _context.AnalysisBkups.AddRange(analysisBkupsToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var section in analysisBkupsToAdd)
            {
                _context.Entry(section).State = EntityState.Detached;
            }

            return analysisBkupsToAdd;
        }

        #endregion

        #region Tool
        public async Task<int> AddRangeTool(List<Tool> ToolsToAdd)
        {
            _context.Tools.AddRange(ToolsToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Tool> CreateNewTool(Tool TooltoCreate)
        {
            _context.Add(TooltoCreate);
            await _context.SaveChangesAsync();

            return TooltoCreate;
        }
        public async Task<Tool> GetToolById(int id)
        {
            var tool = await _context.Tools.AsNoTracking().Where(t => t.ToolId == id && t.IsActive == true).FirstOrDefaultAsync();
            return tool;
        }
        public async Task<IEnumerable<Tool>> GetAllTools()
        {
            var tools = _context.Tools.AsNoTracking().Where(t => t.IsActive == true);
            return await tools.OrderBy(t => t.ToolId).ToListAsync();
        }
        public async Task<IEnumerable<Tool>> GetMatchTools(string ToolToFind)
        {
            return _context.Tools.AsNoTracking().Where(t => t.ToolName.DiceCoefficient(ToolToFind) > 0.5).ToList();
        }
        public async Task<int> UpdateTool(ToolForUpdateDto ToolForUpdate, Tool ToolEntity)
        {

            _mapper.Map(ToolForUpdate, ToolEntity);
            _context.Update(ToolEntity);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteTool(int id)
        {
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
            var Material = await _context.Materials.AsNoTracking().Where(t => t.MaterialId == id && t.IsActive == true).FirstOrDefaultAsync();
            return Material;
        }
        public async Task<IEnumerable<Material>> GetAllMaterials()
        {
            var Materials = _context.Materials.AsNoTracking().Where(t => t.IsActive == true);
            return await Materials.OrderBy(t => t.MaterialId).ToListAsync();
        }
        public async Task<IEnumerable<Material>> GetMatchMaterials(string MaterialToFind)
        {
            return _context.Materials.AsNoTracking().Where(t => t.MaterialName.DiceCoefficient(MaterialToFind) > 0.5).ToList();
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
            var Equipment = await _context.Equipments.AsNoTracking().Where(t => t.EquipmentId == id && t.IsActive == true).FirstOrDefaultAsync();
            return Equipment;
        }
        public async Task<IEnumerable<Equipment>> GetAllEquipments()
        {
            var Equipments = _context.Equipments.AsNoTracking().Where(t => t.IsActive == true);
            return await Equipments.OrderBy(t => t.EquipmentId).ToListAsync();
        }
        public async Task<IEnumerable<Equipment>> GetMatchEquipments(string EquipmentToFind)
        {
            return _context.Equipments.AsNoTracking().Where(t => t.EquipmentName.DiceCoefficient(EquipmentToFind) > 0.5).ToList();
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

        #region Analysis Bkup
        public async Task<AnalysisBkup> GetAnalysisBkupId(int id)
        {
            var bkup = await _context.AnalysisBkups.AsNoTracking().Where(t => t.AnalysisBkupId == id).FirstOrDefaultAsync();
            return bkup;
        }

        public async Task<int> UpdateAnalysisBkup(AnalysisBkupForUpdateDto analysisBkupForUpdate)
        {
            try
            {
                var query = _context.AnalysisBkups.Where(t => t.AnalysisBkupId == analysisBkupForUpdate.AnalysisBkupId);

                AnalysisBkup analysisBkupEntity = await query.FirstOrDefaultAsync();

                if (analysisBkupEntity == null)
                {
                    throw new InvalidOperationException("analysisBkup not found or is not active.");
                }

                // Verifica si la entidad ya está siendo rastreada
                var localEntry = _context.AnalysisBkups.Local.FirstOrDefault(entry => entry.AnalysisBkupId == analysisBkupForUpdate.AnalysisBkupId);
                if (localEntry != null)
                {
                    // Si la entidad localmente rastreada es diferente, usa esa instancia
                    _context.Entry(localEntry).CurrentValues.SetValues(analysisBkupForUpdate);
                }
                else
                {
                    // Si no, adjunta la entidad obtenida de la base de datos
                    if (_context.Entry(analysisBkupEntity).State == EntityState.Detached)
                    {
                        _context.AnalysisBkups.Attach(analysisBkupEntity);
                    }

                    _mapper.Map(analysisBkupForUpdate, analysisBkupEntity);
                    _context.AnalysisBkups.Update(analysisBkupEntity);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the analysisBk.", ex.Message);
                return 0;

            }
        }
        #endregion

        #region Section
        public async Task<Section> GetSectionById(int id)
        {
            var query = _context.Sections.AsNoTracking().Where(t => t.SectionId == id );

            query = query.Include(s => s.Analyses);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> UpdateSection(SectionForUpdateDto sectionForUpdate)
        {
            try
            {
                var query = _context.Sections
                                    .Include(c => c.Analyses)
                                    .Where(t => t.SectionId == sectionForUpdate.SectionId);

                Section section = await query.FirstOrDefaultAsync();

                if (section == null)
                {
                    throw new InvalidOperationException("Section not found or is not active.");
                }

                var localEntry = _context.Sections.Local.FirstOrDefault(entry => entry.SectionId == sectionForUpdate.SectionId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(sectionForUpdate);
                    UpdateAnalyses(localEntry.Analyses, sectionForUpdate.Analyses);
                }
                else
                {
                    if (_context.Entry(section).State == EntityState.Detached)
                    {
                        _context.Sections.Attach(section);
                    }

                    _mapper.Map(sectionForUpdate, section);
                    _context.Sections.Update(section);
                    UpdateAnalyses(section.Analyses, sectionForUpdate.Analyses);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the section: " + ex.Message);
                return 0;
            }
        }

        private void UpdateAnalyses(ICollection<Analysis> existingAnalyses, ICollection<AnalysisForUpdateDto> updatedAnalyses)
        {
            foreach (var updatedAnalysis in updatedAnalyses)
            {
                var existingAnalysis = existingAnalyses.FirstOrDefault(a => a.AnalysisId == updatedAnalysis.AnalysisId);

                if (existingAnalysis != null)
                {
                    _context.Entry(existingAnalysis).CurrentValues.SetValues(updatedAnalysis);
                    existingAnalysis.CriticalPoints = updatedAnalysis.CriticalPoints;
                    existingAnalysis.Reasons = updatedAnalysis.Reasons;
                }
                else
                {
                    var newAnalysis = _mapper.Map<Analysis>(updatedAnalysis);
                    existingAnalyses.Add(newAnalysis);
                }
            }

            var analysesToRemove = existingAnalyses.Where(a => !updatedAnalyses.Any(ua => ua.AnalysisId == a.AnalysisId)).ToList();
            foreach (var analysisToRemove in analysesToRemove)
            {
                existingAnalyses.Remove(analysisToRemove);
            }
        }

      
        #endregion

        #region Commentary
        public async Task<Commentary> GetCommentaryById(int Id)
        {
            return await _context.Comments.AsNoTracking().Where(t => t.ComentaryId == Id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCommentary(UpdateCommentaryDto CommentaryForUpdate)
        {
            try
            {
                var query = _context.Comments.Where(t => t.ComentaryId == CommentaryForUpdate.ComentaryId && t.IsActive == true);

                Commentary commentary = await query.FirstOrDefaultAsync();

                if (commentary == null)
                {
                    throw new InvalidOperationException("Commentary not found or is not active.");
                }

                // Verifica si la entidad ya está siendo rastreada
                var localEntry = _context.Comments.Local.FirstOrDefault(entry => entry.ComentaryId == CommentaryForUpdate.ComentaryId);
                if (localEntry != null)
                {
                    // Si la entidad localmente rastreada es diferente, usa esa instancia
                    _context.Entry(localEntry).CurrentValues.SetValues(CommentaryForUpdate);
                }
                else
                {
                    // Si no, adjunta la entidad obtenida de la base de datos
                    if (_context.Entry(commentary).State == EntityState.Detached)
                    {
                        _context.Comments.Attach(commentary);
                    }

                    _mapper.Map(CommentaryForUpdate, commentary);
                    _context.Comments.Update(commentary);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the Commentary.", ex.Message);
                return 0;

            }
        }
        #endregion

        #region CommonDirection


        public async Task<CommonDirection> CreateNewCommonDir(CommonDirection CommonDirtoCreate)
        {
            _context.Add(CommonDirtoCreate);
            await _context.SaveChangesAsync();

            return CommonDirtoCreate;
        }

        public async Task<List<CommonDirection>> AddRangeCommonDirection(List<CommonDirection> CommonDirtoCreate)
        {
            _context.CommonDirections.AddRange(CommonDirtoCreate);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var section in CommonDirtoCreate)
            {
                _context.Entry(section).State = EntityState.Detached;
            }
            return CommonDirtoCreate;
        }

        public async Task<CommonDirection> GetCommonDirectionById(int id)
        {
            var query = _context.CommonDirections.AsNoTracking().Where(t => t.CommonDirectionId == id);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCommonDirection(CommonDirectionDto commonDirectionForUpdate)
        {
            try
            {
                var query = _context.CommonDirections.Where(t => t.CommonDirectionId == commonDirectionForUpdate.CommonDirectionId);

                CommonDirection commonDirection = await query.FirstOrDefaultAsync();

                if (commonDirection == null)
                {
                    throw new InvalidOperationException("commonDirection not found or is not active.");
                }

                var localEntry = _context.CommonDirections.Local.FirstOrDefault(entry => entry.CommonDirectionId == commonDirectionForUpdate.CommonDirectionId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(commonDirectionForUpdate);
                }
                else
                {
                    if (_context.Entry(commonDirection).State == EntityState.Detached)
                    {
                        _context.CommonDirections.Attach(commonDirection);
                    }

                    _mapper.Map(commonDirectionForUpdate, commonDirection);
                    _context.CommonDirections.Update(commonDirection);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the commonDirection.", ex.Message);
                return 0;

            }
        }

        public async Task<List<CommonDirection>> GetAllCommonDirectionInactives()
        {
            var linkedCommonDirectionIds = await _context.SOSHubs
        .AsNoTracking()
        .SelectMany(hub => hub.CommonDirection.Select(cd => cd.CommonDirectionId))
        .ToListAsync();

            var unlinkedCommonDirections = await _context.CommonDirections
                .AsNoTracking()
                .Where(cd => !linkedCommonDirectionIds.Contains(cd.CommonDirectionId))
                .ToListAsync();

            return unlinkedCommonDirections;
        }

        #endregion

        #region SOSAnalysis
        public async Task<int> CreateSOSAnalysis(SOSAnalysis SOS_AnalysisToCreate)
        {
            _context.SOSAnalyses.Add(SOS_AnalysisToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var query = _context.SOSAnalyses.AsNoTracking().Where(SOS => SOS.SOSAnalysisId == SOSAnalysisId && SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Illustrations);
            }

            if (includeNotes)
            {
                query = query.Include(query => query.Notes);
            }

            if (includeLogbooks)
            {
                query = query.Include(t => t.AnalysisLogbooks);
            }

            if (includeSpecialCases)
            {
                query = query.Include(e => e.SpecialCasesAbnormalSituations);
            }

            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModel);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);
            }

            if (includeImagesSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Images);
            }


            var sosHub = await query.FirstOrDefaultAsync();

            if (sosHub == null)
                return null;

            // Filtrar los subobjetos manualmente después de la carga inicial
            if (includeImages)
            {
                sosHub.Illustrations = sosHub.Illustrations.Where(i => i.IsActive == true).ToList();
            }

            if (includeNotes)
            {
                sosHub.Notes = sosHub.Notes.Where(v => v.IsActive == true).ToList();
            }

            if (includeLogbooks)
            {
                sosHub.AnalysisLogbooks = sosHub.AnalysisLogbooks.Where(t => t.IsActive == true).ToList();
            }

            if (includeSpecialCases)
            {
                sosHub.SpecialCasesAbnormalSituations = sosHub.SpecialCasesAbnormalSituations.Where(e => e.IsActive == true).ToList();
            }

            return sosHub;
        }

        public async Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var query = _context.SOSAnalyses.AsNoTracking().Where(SOS => SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Illustrations);
            }

            if (includeNotes)
            {
                query = query.Include(query => query.Notes);
            }

            if (includeLogbooks)
            {
                query = query.Include(t => t.AnalysisLogbooks);
            }

            if (includeSpecialCases)
            {
                query = query.Include(e => e.SpecialCasesAbnormalSituations);
            }

            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
            }

            var sosAnalyses = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var SOSAnalysis in sosAnalyses)
                {
                    SOSAnalysis.Illustrations = SOSAnalysis.Illustrations.Where(i => i.IsActive == true).ToList();
                }
            }

            if (includeNotes)
            {
                foreach (var SOSAnalysis in sosAnalyses)
                {
                    SOSAnalysis.Notes = SOSAnalysis.Notes.Where(v => v.IsActive == true).ToList();
                }
            }

            if (includeLogbooks)
            {
                foreach (var SOSAnalysis in sosAnalyses)
                {
                    SOSAnalysis.AnalysisLogbooks = SOSAnalysis.AnalysisLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }

            if (includeSpecialCases)
            {
                foreach (var SOSAnalysis in sosAnalyses)
                {
                    SOSAnalysis.SpecialCasesAbnormalSituations = SOSAnalysis.SpecialCasesAbnormalSituations.Where(e => e.IsActive == true).ToList();
                }
            }

            return sosAnalyses;
        }

        public async Task<int> UpdateSOSAnalysis(SOSAnalysisForUpdateDto AnalysisUpdate, SOSAnalysis AnalysisEntity)
        {
            try
            {
                var localEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == AnalysisEntity.SOSAnalysisId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(AnalysisUpdate);
                }
                else
                {
                    if (_context.Entry(AnalysisEntity).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(AnalysisEntity);
                    }

                    _mapper.Map(AnalysisUpdate, AnalysisEntity);
                    _context.SOSAnalyses.Update(AnalysisEntity);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the Commentary.", ex.Message);
                return 0;

            }
        }

        public async Task<int> RemoveSOSAnalysis(int SOS_Analysis_id)
        {
            var SOS_AnalysisEntity = await GetSOSAnalysis(SOS_Analysis_id);
            SOS_AnalysisEntity.IsActive = false;
            _context.SOSAnalyses.Update(SOS_AnalysisEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddIlustrationToSOSAnalysis(int SOS_Analysis_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSAnalysis(SOS_Analysis_id, includeImages: true);

            if (SosHubEntity != null)
            {

                if (SosHubEntity.Illustrations != null)
                {
                    SosHubEntity.Illustrations.Add(evidence);
                }
                else
                {
                    SosHubEntity.Illustrations = new List<FileUpload>
                    {
                        evidence
                    };
                }
            }
        }

        public async Task<int> RemoveIlustrationFromSOSAnalysis(int SOS_Analysis_id, int ImageFile_id)
        {
            var SOSAnalysisEntity = await GetSOSAnalysis(SOS_Analysis_id, includeImages: true);

            var Sketch = SOSAnalysisEntity.Illustrations.ToList().Find(i => i.FileUploadId == ImageFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSAnalyses.Update(SOSAnalysisEntity);

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Add Range SOS Analysis
        public async Task<int> AddRangeSpecialCasesAbnormalSituations(List<SpecialCaseAbnormalSituation> SpecialCasesAbnormalSituationsToAdd)
        {
            _context.SpecialCasesAbnormalSituations.AddRange(SpecialCasesAbnormalSituationsToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> AddRangeSOSAnalysisLogbook(List<SOSAnalysisLogbook> SOSAnalysisLogbooksToAdd)
        {
            _context.SOSAnalysisLogbooks.AddRange(SOSAnalysisLogbooksToAdd);
            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Add To Sos Analysis
        public async Task<AsyncVoidMethodBuilder> AddNoteToSOSAnalysis(SOSAnalysis Master, Commentary Slave)
        {
            if (Master.Notes != null)
            {
                Master.Notes.Add(Slave);
            }
            else
            {
                Master.Notes = new List<Commentary>();
                Master.Notes.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSpecialCasesAbnormalSituationsToSOSAnalysis(SOSAnalysis Master, SpecialCaseAbnormalSituation Slave)
        {
            if (Master.SpecialCasesAbnormalSituations != null)
            {
                Master.SpecialCasesAbnormalSituations.Add(Slave);
            }
            else
            {
                Master.SpecialCasesAbnormalSituations = new List<SpecialCaseAbnormalSituation>();
                Master.SpecialCasesAbnormalSituations.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSOSAnalysisLogbookToSOSAnalysis(SOSAnalysis Master, SOSAnalysisLogbook Slave)
        {
            if (Master.AnalysisLogbooks != null)
            {
                Master.AnalysisLogbooks.Add(Slave);
            }
            else
            {
                Master.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                Master.AnalysisLogbooks.Add(Slave);
            }
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Remove from SOSAnalysis
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSpecialCasesAbnormalSituationsFromSOSAnalysis(SOSAnalysis Master)
        {
            Master.SpecialCasesAbnormalSituations?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(SOSAnalysis Master)
        {
            Master.AnalysisLogbooks?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSAnalysis(SOSAnalysis Master)
        {
            Master.Notes?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        #endregion
        #region SpecialCaseAbnormalSituation
        public async Task<SpecialCaseAbnormalSituation> GetSpecialCaseAbnormalSituationById(int id)
        {
            return await _context.SpecialCasesAbnormalSituations.AsNoTracking().Where(t => t.SpecialCaseAbnormalSituationId == id && t.IsActive == true).FirstOrDefaultAsync();
        }
        #endregion
        #region SOSAnalysisLogbook
        public async Task<SOSAnalysisLogbook> GetSOSAnalysisLogbookById(int id)
        {
            return await _context.SOSAnalysisLogbooks.AsNoTracking().Where(t => t.SOSAnalysisLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }
        #endregion
        #region CommonOperations
        public async Task<FileUpload?> FetchFileAsync(int fileid)
        {
            return await _context.Files.AsNoTracking()
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
        public async Task<bool> SaveChanges()
        {
            try
            {
                return (await _context.SaveChangesAsync() >= 0);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }


        #endregion

    }
}
