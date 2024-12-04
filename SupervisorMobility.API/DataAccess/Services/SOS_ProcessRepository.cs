using AutoMapper;
using Azure.Core.GeoJson;
using CsvHelper;

using DuoVia.FuzzyStrings;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Migrations;
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
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
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
    public class SOS_ProcessRepository : ISOS_ProcessRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;


        public SOS_ProcessRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region SOS_DataPool
        public async Task<SOSHub> CreateSOScollection(SOSHub SOS_EntityToCreate)
        {
            _context.SOSHubs.Add(SOS_EntityToCreate);
            await _context.SaveChangesAsync();

            return SOS_EntityToCreate;
        }
        public async Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeHistory = false, bool includeDeleteds = false, bool includeCollections = false, bool includePeopleCollections = false, bool includePats = false)
        {
            var query = _context.SOSHubs.AsNoTracking().Where(SOS => SOS.SOSHubId == HubId && SOS.IsActive == true);

            var sosHub = await _context.SOSHubs.Where(SOS => SOS.SOSHubId == HubId && SOS.IsActive == true)
                                     .FirstOrDefaultAsync();

            // Verificar si sosHub no es nulo antes de cargar las colecciones relacionadas
            if (sosHub != null)
            {
                if (includeAnalysesBkup)
                {
                    await _context.Entry(sosHub).Collection(s => s.AnalysesBkup).LoadAsync();
                }

                if (includeSections)
                {
                    await _context.Entry(sosHub).Collection(s => s.Sections).LoadAsync();
                    // Cargar también los análisis dentro de Sections
                    foreach (var section in sosHub.Sections)
                    {
                        await _context.Entry(section).Collection(s => s.Analyses).LoadAsync();
                    }
                }

                if (includeImages)
                {
                    await _context.Entry(sosHub).Collection(s => s.Images).LoadAsync();
                }

                if (includeVideos)
                {
                    await _context.Entry(sosHub).Collection(s => s.Videos).LoadAsync();
                }

                if (includeCommentaries)
                {
                    await _context.Entry(sosHub).Collection(s => s.ProcessSheetCommentary).LoadAsync();
                }

                if (includeTools)
                {
                    await _context.Entry(sosHub).Collection(s => s.ToolsUsed).LoadAsync();
                    // Cargar los Tool dentro de ToolsUsed
                    foreach (var toolUsed in sosHub.ToolsUsed)
                    {
                        await _context.Entry(toolUsed).Reference(t => t.Tool).LoadAsync();
                    }
                }

                if (includeEquipments)
                {
                    await _context.Entry(sosHub).Collection(s => s.SafetyEquipment).LoadAsync();
                }

                if (includeMaterials)
                {
                    await _context.Entry(sosHub).Collection(s => s.MaterialsUsed).LoadAsync();
                    // Cargar Material en cada MaterialsUsed
                    foreach (var materialUsed in sosHub.MaterialsUsed)
                    {
                        await _context.Entry(materialUsed).Reference(m => m.Material).LoadAsync();
                    }
                }

                if (includeInformation)
                {
                    await _context.Entry(sosHub).Reference(i => i.Plant).LoadAsync();
                    await _context.Entry(sosHub).Reference(i => i.Area).LoadAsync();
                    await _context.Entry(sosHub).Reference(i => i.Distribution).LoadAsync();
                    await _context.Entry(sosHub).Reference(i => i.Department).LoadAsync();
                }

                if (includePeople)
                {
                    await _context.Entry(sosHub).Collection(o => o.ApproverOwners).LoadAsync();
                    foreach (var approverOwner in sosHub.ApproverOwners)
                    {
                        await _context.Entry(approverOwner).Collection(o => o.Areas).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(e => e.ReviewerEditors).LoadAsync();
                    foreach (var reviewerEditor in sosHub.ReviewerEditors)
                    {
                        await _context.Entry(reviewerEditor).Reference(o => o.Area).LoadAsync();
                    }
                }

                if (includeDocuments)
                {
                    await _context.Entry(sosHub).Collection(o => o.CommonDirection).LoadAsync();
                }

                if (includeModel)
                {
                    await _context.Entry(sosHub).Collection(s => s.AppliedModels).LoadAsync();
                }

                if (includeHistory)
                {
                    await _context.Entry(sosHub).Collection(m => m.History).LoadAsync();
                }

                if (includePats)
                {
                    await _context.Entry(sosHub).Collection(o => o.PATs).LoadAsync();
                    foreach (var pat in sosHub.PATs)
                    {
                        await _context.Entry(pat).Reference(aa => aa.SSVresponsible).LoadAsync();
                        await _context.Entry(pat).Reference(aa => aa.Supervisor).LoadAsync();
                    }
                }


                if (includeCollections)
                {
                    await _context.Entry(sosHub).Collection(a => a.SOSAnalysis).Query().Where(d => d.IsActive == true).LoadAsync();
                    foreach (var analysis in sosHub.SOSAnalysis)
                    {
                        await _context.Entry(analysis).Collection(aa => aa.AnalysisLogbooks).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(c => c.SOSCombination).Query().Where(d => d.IsActive == true).LoadAsync();
                    foreach (var combination in sosHub.SOSCombination)
                    {
                        await _context.Entry(combination).Collection(aa => aa.CombinationLogbooks).LoadAsync();
                        await _context.Entry(combination).Collection(aa => aa.Turns).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(d => d.SOSDistribution).Query().Where(d => d.IsActive == true).LoadAsync(); foreach (var distribution in sosHub.SOSDistribution)
                    {
                        await _context.Entry(distribution).Collection(aa => aa.DistributionLogbooks).LoadAsync();
                        await _context.Entry(distribution).Collection(aa => aa.Turns).LoadAsync();

                        await _context.Entry(distribution).Collection(aa => aa.Analyses).LoadAsync();
                        await _context.Entry(distribution).Collection(aa => aa.Sequences).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(f => f.SOSFlow).Query().Where(d => d.IsActive == true).LoadAsync();
                    foreach (var flow in sosHub.SOSFlow)
                    {
                        await _context.Entry(flow).Collection(aa => aa.FlowLogbooks).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(s => s.SOSSequence).Query().Where(d => d.IsActive == true).LoadAsync();
                    foreach (var sequence in sosHub.SOSSequence)
                    {
                        await _context.Entry(sequence).Collection(aa => aa.SequenceLogbooks).LoadAsync();
                    }
                }
                if (includePeopleCollections)
                {
                    await _context.Entry(sosHub).Collection(a => a.SOSAnalysis).LoadAsync();
                    foreach (var analysis in sosHub.SOSAnalysis)
                    {
                        // Cargar AnalysisLogbooks y sus relaciones
                        await _context.Entry(analysis).Collection(aa => aa.AnalysisLogbooks).LoadAsync();
                        foreach (var logbook in analysis.AnalysisLogbooks)
                        {
                            await _context.Entry(logbook).Reference(al => al.Approver).LoadAsync();
                            await _context.Entry(logbook).Reference(al => al.Reviewer).LoadAsync();
                        }
                    }

                    await _context.Entry(sosHub).Collection(c => c.SOSCombination).LoadAsync();
                    foreach (var combination in sosHub.SOSCombination)
                    {
                        // Cargar CombinationLogbooks y sus relaciones
                        await _context.Entry(combination).Collection(al => al.CombinationLogbooks).LoadAsync();
                        foreach (var logbook in combination.CombinationLogbooks)
                        {
                            await _context.Entry(logbook).Reference(al => al.Approver).LoadAsync();
                            await _context.Entry(logbook).Reference(al => al.Reviewer).LoadAsync();
                        }

                        await _context.Entry(combination).Reference(al => al.ReviewerHS).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(d => d.SOSDistribution).LoadAsync();
                    foreach (var distribution in sosHub.SOSDistribution)
                    {
                        // Cargar DistributionLogbooks y sus relaciones
                        await _context.Entry(distribution).Collection(aa => aa.DistributionLogbooks).LoadAsync();
                        foreach (var logbook in distribution.DistributionLogbooks)
                        {
                            await _context.Entry(logbook).Reference(al => al.Approver).LoadAsync();
                            await _context.Entry(logbook).Reference(al => al.Reviewer).LoadAsync();
                        }
                    }

                    await _context.Entry(sosHub).Collection(f => f.SOSFlow).LoadAsync();
                    foreach (var flow in sosHub.SOSFlow)
                    {
                        // Cargar FlowLogbooks y sus relaciones
                        await _context.Entry(flow).Collection(aa => aa.FlowLogbooks).LoadAsync();
                        foreach (var logbook in flow.FlowLogbooks)
                        {
                            await _context.Entry(logbook).Reference(al => al.Approver).LoadAsync();
                            await _context.Entry(logbook).Reference(al => al.Reviewer).LoadAsync();
                        }

                        await _context.Entry(flow).Reference(al => al.ReviewerHS).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(s => s.SOSSequence).LoadAsync();
                    foreach (var sequence in sosHub.SOSSequence)
                    {
                        // Cargar SequenceLogbooks y sus relaciones
                        await _context.Entry(sequence).Collection(aa => aa.SequenceLogbooks).LoadAsync();
                        foreach (var logbook in sequence.SequenceLogbooks)
                        {
                            await _context.Entry(logbook).Reference(al => al.Approver).LoadAsync();
                            await _context.Entry(logbook).Reference(al => al.Reviewer).LoadAsync();
                        }
                    }
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
                query = query.Include(t => t.ToolsUsed).ThenInclude(p => p.Tool);
            }

            if (includeEquipments)
            {
                query = query.Include(e => e.SafetyEquipment);
            }

            if (includeMaterials)
            {
                query = query.Include(m => m.MaterialsUsed).ThenInclude(p => p.Material);
            }

            if (includeInformation)
            {
                query = query.Include(i => i.Plant).Include(t => t.Area).Include(d => d.Distribution).Include(d => d.Department);
            }

            if (includePeople)
            {
                query = query.Include(o => o.ApproverOwners).Include(e => e.ReviewerEditors);
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

            _mapper.Map(HubUpdate, SosEntity);

            // Marca la entidad como modificada
            _context.Entry(SosEntity).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateSOSHub(SOSHub SosEntity)
        {

            _context.SOSHubs.Update(SosEntity);

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
                query = query.Include(o => o.ApproverOwners).Include(e => e.ReviewerEditors);
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
        public async Task<AsyncVoidMethodBuilder> AddProcessSheetCommentaryToSOSCollection(SOSHub master, Commentary slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Commentaries.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.ProcessSheetCommentary == null)
                {
                    master.ProcessSheetCommentary = new List<Commentary>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.ProcessSheetCommentary.Any(c => c.CommentaryId == slave.CommentaryId))
                {
                    master.ProcessSheetCommentary.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
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

        public async Task<AsyncVoidMethodBuilder> AddToolToSOSCollection(SOSHub master, ToolUsed slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.ToolsUsed.Local.FirstOrDefault(entry => entry.ToolId == slave.ToolId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.ToolsUsed.Attach(slave);
                    }
                }

                // Añadir la Material a la colección de ToolsUsed del master
                if (master.ToolsUsed == null)
                {
                    master.ToolsUsed = new List<ToolUsed>();
                }

                // Verificar si la Material ya está en la colección
                if (!master.ToolsUsed.Any(t => t.ToolId == slave.ToolId))
                {
                    master.ToolsUsed.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddEquipmentToSOSCollection(SOSHub master, Equipment slave)

        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Equipments.Local.FirstOrDefault(entry => entry.EquipmentId == slave.EquipmentId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Equipments.Attach(slave);
                    }
                }

                // Añadir la herramienta a la colección de Equipment del master
                if (master.SafetyEquipment == null)
                {
                    master.SafetyEquipment = new List<Equipment>();
                }

                // Verificar si Equipment ya está en la colección
                if (!master.SafetyEquipment.Any(t => t.EquipmentId == slave.EquipmentId))
                {
                    master.SafetyEquipment.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
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

        public async Task<AsyncVoidMethodBuilder> AddReviewerEditorToSOSCollection(SOSHub master, User slave)
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
                var localSlaveEntry = _context.Users.Local.FirstOrDefault(entry => entry.UserId == slave.UserId);
                if (localSlaveEntry == null)
                {
                    // Si no está rastreado, adjunta el slave al contexto
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Users.Attach(slave);
                    }

                    // Agrega el slave a la colección del master
                    if (master.ReviewerEditors == null)
                    {
                        master.ReviewerEditors = new List<User>();
                    }

                    if (!master.ReviewerEditors.Any(cd => cd.UserId == slave.UserId))
                    {
                        master.ReviewerEditors.Add(slave);
                    }
                }
                else
                {
                    // Agrega el localSlaveEntry a la colección del master
                    if (master.ReviewerEditors == null)
                    {
                        master.ReviewerEditors = new List<User>();
                    }

                    if (!master.ReviewerEditors.Any(cd => cd.UserId == localSlaveEntry.UserId))
                    {
                        master.ReviewerEditors.Add(localSlaveEntry);
                    }
                }

                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
                Console.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddApproverOwnersToSOSCollection(SOSHub master, User slave)
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
                var localSlaveEntry = _context.Users.Local.FirstOrDefault(entry => entry.UserId == slave.UserId);
                if (localSlaveEntry == null)
                {
                    // Si no está rastreado, adjunta el slave al contexto
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Users.Attach(slave);
                    }

                    // Agrega el slave a la colección del master
                    if (master.ApproverOwners == null)
                    {
                        master.ApproverOwners = new List<User>();
                    }

                    if (!master.ApproverOwners.Any(cd => cd.UserId == slave.UserId))
                    {
                        master.ApproverOwners.Add(slave);
                    }
                }
                else
                {
                    // Agrega el localSlaveEntry a la colección del master
                    if (master.ApproverOwners == null)
                    {
                        master.ApproverOwners = new List<User>();
                    }

                    if (!master.ApproverOwners.Any(cd => cd.UserId == localSlaveEntry.UserId))
                    {
                        master.ApproverOwners.Add(localSlaveEntry);
                    }
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
                Console.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddMaterialToSOSCollection(SOSHub master, MaterialUsed slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }

                // Verificar si el Material slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.MaterialsUsed.Local.FirstOrDefault(entry => entry.MaterialId == slave.MaterialId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.MaterialsUsed.Attach(slave);
                    }
                }

                // Añadir la herramienta a la colección de ToolsUsed del master
                if (master.MaterialsUsed == null)
                {
                    master.MaterialsUsed = new List<MaterialUsed>();
                }

                // Verificar si la Materials ya está en la colección
                if (!master.MaterialsUsed.Any(t => t.MaterialId == slave.MaterialId))
                {
                    master.MaterialsUsed.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddProductToSOSCollection(SOSHub master, Product slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(master);
                    }
                }

                // Verificar si el AppliedModel slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Products.Local.FirstOrDefault(entry => entry.ProductId == slave.ProductId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Products.Attach(slave);
                    }
                }

                // Añadir el producto a la colección de AppliedModels del master
                if (master.AppliedModels == null)
                {
                    master.AppliedModels = new List<Product>();
                }

                // Verificar si la Prodcut ya está en la colección
                if (!master.AppliedModels.Any(t => t.ProductId == slave.ProductId))
                {
                    master.AppliedModels.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
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
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllReviewerEditors(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.ReviewerEditors?.Count > 0)
            {
                Master.ReviewerEditors.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [ReviewerEditors]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();

        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllApproverOwners(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.ApproverOwners?.Count > 0)
            {
                Master.ApproverOwners.Clear();

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [ApproverOwners]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción que pueda ocurrir
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return new AsyncVoidMethodBuilder();

        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllProducts(SOSHub Master)
        {
            if (_context.Entry(Master).State == EntityState.Detached)
            {
                _context.SOSHubs.Attach(Master);
            }

            if (Master.AppliedModels?.Count > 0)
            {
                Master.AppliedModels.Clear();

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Manejar las excepciones relacionadas con la actualización de la base de datos
                    Console.WriteLine($"DbUpdateException [AppliedModels]: {ex.Message}");
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

        #region Users
        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.Where(p => p.UserId == id).FirstOrDefaultAsync();
        }

        #endregion

        #region Products
        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.Where(p => p.ProductId == id).FirstOrDefaultAsync();
        }

        #endregion
        #region Tool
        public async Task<int> AddRangeTool(List<Tool> ToolsToAdd)
        {
            _context.Tools.AddRange(ToolsToAdd);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<ToolUsed>> AddRangeToolsUsed(List<ToolUsed> ToolsUsedToAdd)
        {
            _context.ToolsUsed.AddRange(ToolsUsedToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var tooluse in ToolsUsedToAdd)
            {
                _context.Entry(tooluse).State = EntityState.Detached;
            }

            return ToolsUsedToAdd;
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
        public async Task<ToolUsed> GetToolUsedById(int id)
        {
            var toolUse = await _context.ToolsUsed.AsNoTracking().Where(t => t.ToolUsedId == id).FirstOrDefaultAsync();
            return toolUse;
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

        public async Task<int> UpdateToolUsed(ToolUsedForUpdateDto ToolForUpdate)
        {
            try
            {
                var query = _context.ToolsUsed.Where(t => t.ToolUsedId == ToolForUpdate.ToolUsedId);

                ToolUsed Toolused = await query.FirstOrDefaultAsync();

                if (Toolused == null)
                {
                    throw new InvalidOperationException("Toolused not found or is not active.");
                }

                var localEntry = _context.ToolsUsed.Local.FirstOrDefault(entry => entry.ToolUsedId == ToolForUpdate.ToolUsedId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(ToolForUpdate);
                }
                else
                {
                    if (_context.Entry(Toolused).State == EntityState.Detached)
                    {
                        _context.ToolsUsed.Attach(Toolused);
                    }

                    _mapper.Map(ToolForUpdate, Toolused);
                    _context.ToolsUsed.Update(Toolused);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Toolused: " + ex.Message);
                return 0;
            }
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
        public async Task<List<MaterialUsed>> AddRangeMaterialUsed(List<MaterialUsed> MaterialsUsedToAdd)
        {
            _context.MaterialsUsed.AddRange(MaterialsUsedToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var materialuse in MaterialsUsedToAdd)
            {
                _context.Entry(materialuse).State = EntityState.Detached;
            }

            return MaterialsUsedToAdd;
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
        public async Task<MaterialUsed> GetMaterialUsedById(int id)
        {
            var Material = await _context.MaterialsUsed.AsNoTracking().Where(t => t.MaterialUsedId == id).FirstOrDefaultAsync();
            return Material;
        }

        public async Task<IEnumerable<Material>> GetAllMaterials()
        {
            var Materials = _context.Materials.AsNoTracking().Where(t => t.IsActive == true);
            return await Materials.OrderBy(t => t.MaterialId).ToListAsync();
        }
        public async Task<IEnumerable<Material>> GetMatchMaterials(string MaterialToFind)
        {
            return _context.Materials.AsNoTracking().Where(t => t.PartName.DiceCoefficient(MaterialToFind) > 0.5).ToList();
        }
        public async Task<int> UpdateMaterial(MaterialForUpdateDto MaterialForUpdate, Material MaterialEntity)
        {
            _mapper.Map(MaterialForUpdate, MaterialEntity);
            _context.Update(MaterialEntity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateMaterialUsed(MaterialsUsedForUpdateDto materialForUpdate)
        {
            try
            {
                var query = _context.MaterialsUsed
                                    .Where(t => t.MaterialUsedId == materialForUpdate.MaterialUsedId);

                MaterialUsed materialused = await query.FirstOrDefaultAsync();

                if (materialused == null)
                {
                    throw new InvalidOperationException("materialused not found or is not active.");
                }

                var localEntry = _context.MaterialsUsed.Local.FirstOrDefault(entry => entry.MaterialUsedId == materialForUpdate.MaterialUsedId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(materialForUpdate);
                }
                else
                {
                    if (_context.Entry(materialused).State == EntityState.Detached)
                    {
                        _context.MaterialsUsed.Attach(materialused);
                    }

                    _mapper.Map(materialForUpdate, materialused);
                    _context.MaterialsUsed.Update(materialused);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the materialused: " + ex.Message);
                return 0;
            }
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
            var query = _context.Sections.AsNoTracking().Where(t => t.SectionId == id);

            query = query.Include(s => s.Analyses);
            return await query.FirstOrDefaultAsync();
        }

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
            return await _context.Commentaries.AsNoTracking().Where(t => t.CommentaryId == Id).FirstOrDefaultAsync();
        }
        public async Task<List<Commentary>> AddRangeCommentary(List<Commentary> commentariesToAdd)
        {
            _context.Commentaries.AddRange(commentariesToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas secciones del contexto
            foreach (var section in commentariesToAdd)
            {
                _context.Entry(section).State = EntityState.Detached;
            }

            return commentariesToAdd;
        }
        public async Task<int> UpdateCommentary(UpdateCommentaryDto CommentaryForUpdate)
        {
            try
            {
                var query = _context.Commentaries.Where(t => t.CommentaryId == CommentaryForUpdate.CommentaryId && t.IsActive == true);

                Commentary commentary = await query.FirstOrDefaultAsync();

                if (commentary == null)
                {
                    throw new InvalidOperationException("Commentary not found or is not active.");
                }

                // Verifica si la entidad ya está siendo rastreada
                var localEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == CommentaryForUpdate.CommentaryId);
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
                        _context.Commentaries.Attach(commentary);
                    }

                    _mapper.Map(CommentaryForUpdate, commentary);
                    _context.Commentaries.Update(commentary);
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
        #region SosTime
        public async Task<SOSTime> GetSOSTimeById(int id)
        {
            return await _context.SOSTimes.AsNoTracking().Where(t => t.SOSTimeId == id && t.IsActive == true).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateTime(SOSTimeForUpdateDto timeForUpdate)
        {
            try
            {
                var query = _context.SOSTimes
                                    .Where(t => t.SOSTimeId == timeForUpdate.SOSTimeId);

                SOSTime time = await query.FirstOrDefaultAsync();

                if (time == null)
                {
                    throw new InvalidOperationException("Time not found or is not active.");
                }

                var localEntry = _context.SOSTimes.Local.FirstOrDefault(entry => entry.SOSTimeId == timeForUpdate.SOSTimeId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(timeForUpdate);
                }
                else
                {
                    if (_context.Entry(time).State == EntityState.Detached)
                    {
                        _context.SOSTimes.Attach(time);
                    }

                    _mapper.Map(timeForUpdate, time);
                    _context.SOSTimes.Update(time);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Time: " + ex.Message);
                return 0;
            }
        }
        public async Task<List<SOSTime>> AddRangeSOSTimes(List<SOSTime> SOSTimesToAdd)
        {
            _context.SOSTimes.AddRange(SOSTimesToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas AnalysisLogbook del contexto
            foreach (var time in SOSTimesToAdd)
            {
                _context.Entry(time).State = EntityState.Detached;
            }

            return SOSTimesToAdd;
        }

        public async Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSAnalysis(SOSAnalysis master, SOSTime slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == master.SOSAnalysisId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSTimes.Local.FirstOrDefault(entry => entry.SOSTimeId == slave.SOSTimeId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSTimes.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Times == null)
                {
                    master.Times = new List<SOSTime>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Times.Any(c => c.SOSTimeId == slave.SOSTimeId))
                {
                    master.Times.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSSequence(SOSSequence master, SOSTime slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSSequenceId == master.SOSSequenceId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSTimes.Local.FirstOrDefault(entry => entry.SOSTimeId == slave.SOSTimeId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSTimes.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Times == null)
                {
                    master.Times = new List<SOSTime>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Times.Any(c => c.SOSTimeId == slave.SOSTimeId))
                {
                    master.Times.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSOSTimeToSOSDistribution(SOSDistribution master, SOSTime slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == master.SOSDistributionId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSTimes.Local.FirstOrDefault(entry => entry.SOSTimeId == slave.SOSTimeId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSTimes.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Times == null)
                {
                    master.Times = new List<SOSTime>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Times.Any(c => c.SOSTimeId == slave.SOSTimeId))
                {
                    master.Times.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSAnalysis(SOSAnalysis Master)
        {
            Master.Times?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSDistribution(SOSDistribution Master)
        {
            Master.Times?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSSequence(SOSSequence Master)
        {
            Master.Times?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Turn
        public async Task<Turn> GetTurnById(int id)
        {
            return await _context.Turns.AsNoTracking().Where(t => t.TurnId == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateTurn(TurnForUpdateDto TurnForUpdate)
        {
            try
            {
                var query = _context.Turns
                                    .Where(t => t.TurnId == TurnForUpdate.TurnId);

                Turn Turn = await query.FirstOrDefaultAsync();

                if (Turn == null)
                {
                    throw new InvalidOperationException("Turn not found or is not active.");
                }

                var localEntry = _context.Turns.Local.FirstOrDefault(entry => entry.TurnId == TurnForUpdate.TurnId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(TurnForUpdate);
                }
                else
                {
                    if (_context.Entry(Turn).State == EntityState.Detached)
                    {
                        _context.Turns.Attach(Turn);
                    }

                    _mapper.Map(TurnForUpdate, Turn);
                    _context.Turns.Update(Turn);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Turn: " + ex.Message);
                return 0;
            }
        }
        public async Task<List<Turn>> AddRangeTurns(List<Turn> TurnsToAdd)
        {
            _context.Turns.AddRange(TurnsToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas AnalysisLogbook del contexto
            foreach (var Turn in TurnsToAdd)
            {
                _context.Entry(Turn).State = EntityState.Detached;
            }

            return TurnsToAdd;
        }

        public async Task<AsyncVoidMethodBuilder> AddTurnToSOSCombination(SOSCombination master, Turn slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSCombinations.Local.FirstOrDefault(entry => entry.SOSCombinationId == master.SOSCombinationId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSCombinations.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Turns.Local.FirstOrDefault(entry => entry.TurnId == slave.TurnId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Turns.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Turns == null)
                {
                    master.Turns = new List<Turn>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Turns.Any(c => c.TurnId == slave.TurnId))
                {
                    master.Turns.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddTurnToSOSDistribution(SOSDistribution master, Turn slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == master.SOSDistributionId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Turns.Local.FirstOrDefault(entry => entry.TurnId == slave.TurnId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Turns.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Turns == null)
                {
                    master.Turns = new List<Turn>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Turns.Any(c => c.TurnId == slave.TurnId))
                {
                    master.Turns.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> RemoveAllTurnsFromSOSDistribution(SOSDistribution Master)
        {
            Master.Turns?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> RemoveAllTurnsFromSOSCombination(SOSCombination Master)
        {
            Master.Turns?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
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
                query = query.Include(t => t.AnalysisLogbooks).ThenInclude(l => l.Approver);
                query = query.Include(t => t.AnalysisLogbooks).ThenInclude(l => l.Reviewer);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModels);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed).ThenInclude(t => t.Tool);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.MaterialsUsed).ThenInclude(m => m.Material);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);

                query = query.Include(m => m.Times);
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



            return sosAnalyses;
        }
        public async Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysisByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var sosHubIds = await _context.SOSHubs.Where(hub => hub.DistributionId == Distribution_Id)
                                                   .Select(hub => hub.SOSHubId).ToListAsync();

            var query = _context.SOSAnalyses.AsNoTracking().Where(SOS => sosHubIds.Contains(SOS.SOSHubId) && SOS.IsActive == true);

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


            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(ms => ms.Sections).ThenInclude(msa => msa.Analyses);
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



            return sosAnalyses;
        }

        public async Task<int> UpdateSOSAnalysis(SOSAnalysisForUpdateDto AnalysisUpdate, SOSAnalysis AnalysisEntity)
        {
            try
            {
                // Adjunta la entidad al contexto si no está ya adjunta
                if (_context.Entry(AnalysisEntity).State == EntityState.Detached)
                {
                    _context.SOSAnalyses.Attach(AnalysisEntity);
                }

                var localEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == AnalysisEntity.SOSAnalysisId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(AnalysisUpdate);
                }
                else
                {
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
            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSAnalyses.Attach(SosHubEntity);
            }
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

        public async Task<List<SOSAnalysisLogbook>> AddRangeSOSAnalysisLogbook(List<SOSAnalysisLogbook> SOSAnalysisLogbooksToAdd)
        {
            _context.SOSAnalysisLogbooks.AddRange(SOSAnalysisLogbooksToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas AnalysisLogbook del contexto
            foreach (var analysislogbook in SOSAnalysisLogbooksToAdd)
            {
                _context.Entry(analysislogbook).State = EntityState.Detached;
            }

            return SOSAnalysisLogbooksToAdd;
        }
        #endregion
        #region Add To Sos Analysis
        public async Task<AsyncVoidMethodBuilder> AddNoteToSOSAnalysis(SOSAnalysis master, Commentary slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == master.SOSAnalysisId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Commentaries.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Notes == null)
                {
                    master.Notes = new List<Commentary>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Notes.Any(c => c.CommentaryId == slave.CommentaryId))
                {
                    master.Notes.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddSOSAnalysisLogbookToSOSAnalysis(SOSAnalysis master, SOSAnalysisLogbook slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSAnalysisLogbooks.Local.FirstOrDefault(entry => entry.SOSAnalysisLogbookId == slave.SOSAnalysisLogbookId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSAnalysisLogbooks.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.AnalysisLogbooks == null)
                {
                    master.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.AnalysisLogbooks.Any(c => c.SOSAnalysisLogbookId == slave.SOSAnalysisLogbookId))
                {
                    master.AnalysisLogbooks.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Remove from SOSAnalysis

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
        #region SOSAnalysisLogbook
        public async Task<SOSAnalysisLogbook> GetSOSAnalysisLogbookById(int id)
        {
            return await _context.SOSAnalysisLogbooks.AsNoTracking().Where(t => t.SOSAnalysisLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<int> CreateSOSAnalysisLogbook(SOSAnalysisLogbook LogBook_ToCreate)
        {
            _context.SOSAnalysisLogbooks.Add(LogBook_ToCreate);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAnalysisLogbook(SOSAnalysisLogbookForUpdateDto analysisForUpdate)
        {
            try
            {
                var query = _context.SOSAnalysisLogbooks
                                    .Where(t => t.SOSAnalysisLogbookId == analysisForUpdate.SOSAnalysisLogbookId);

                SOSAnalysisLogbook analysisLogbook = await query.FirstOrDefaultAsync();

                if (analysisLogbook == null)
                {
                    throw new InvalidOperationException("Analysis Logbook not found or is not active.");
                }

                var localEntry = _context.SOSAnalysisLogbooks.Local.FirstOrDefault(entry => entry.SOSAnalysisLogbookId == analysisForUpdate.SOSAnalysisLogbookId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(analysisForUpdate);
                }
                else
                {
                    if (_context.Entry(analysisLogbook).State == EntityState.Detached)
                    {
                        _context.SOSAnalysisLogbooks.Attach(analysisLogbook);
                    }

                    _mapper.Map(analysisForUpdate, analysisLogbook);
                    _context.SOSAnalysisLogbooks.Update(analysisLogbook);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the analysis Logbook: " + ex.Message);
                return 0;
            }
        }
        #endregion
        //Sequence
        #region SOSSequence
        public async Task<int> CreateSOSSequence(SOSSequence SOS_SequenceToCreate)
        {
            _context.SOSSequences.Add(SOS_SequenceToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSSequence> GetSOSSequence(int SOSSequenceId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var query = _context.SOSSequences.AsNoTracking().Where(SOS => SOS.SOSSequenceId == SOSSequenceId && SOS.IsActive == true);

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
                query = query.Include(t => t.SequenceLogbooks).ThenInclude(l => l.Approver);
                query = query.Include(t => t.SequenceLogbooks).ThenInclude(l => l.Reviewer);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModels);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed).ThenInclude(t => t.Tool);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.MaterialsUsed).ThenInclude(m => m.Material);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);
                query = query.Include(m => m.Times);
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
                sosHub.SequenceLogbooks = sosHub.SequenceLogbooks.Where(t => t.IsActive == true).ToList();
            }



            return sosHub;
        }

        public async Task<IEnumerable<SOSSequence>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var query = _context.SOSSequences.AsNoTracking().Where(SOS => SOS.IsActive == true);

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
                query = query.Include(t => t.SequenceLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
            }

            var sosSequences = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.Illustrations = SOSSequence.Illustrations.Where(i => i.IsActive == true).ToList();
                }
            }

            if (includeNotes)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.Notes = SOSSequence.Notes.Where(v => v.IsActive == true).ToList();
                }
            }

            if (includeLogbooks)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.SequenceLogbooks = SOSSequence.SequenceLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }



            return sosSequences;
        }
        public async Task<IEnumerable<SOSSequence>> GetAllSOSSequenceByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var sosHubIds = await _context.SOSHubs.Where(hub => hub.DistributionId == Distribution_Id)
                                                    .Select(hub => hub.SOSHubId).ToListAsync();

            var query = _context.SOSSequences.AsNoTracking().Where(sequence => sosHubIds.Contains(sequence.SOSHubId) && sequence.IsActive == true);

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
                query = query.Include(t => t.SequenceLogbooks);
            }

            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(ms => ms.Sections).ThenInclude(msa => msa.Analyses);

            }

            var sosSequences = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.Illustrations = SOSSequence.Illustrations.Where(i => i.IsActive == true).ToList();
                }
            }

            if (includeNotes)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.Notes = SOSSequence.Notes.Where(v => v.IsActive == true).ToList();
                }
            }

            if (includeLogbooks)
            {
                foreach (var SOSSequence in sosSequences)
                {
                    SOSSequence.SequenceLogbooks = SOSSequence.SequenceLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }



            return sosSequences;
        }

        public async Task<int> UpdateSOSSequence(SOSSequenceForUpdateDto SequenceUpdate, SOSSequence SequenceEntity)
        {
            try
            {
                // Adjunta la entidad al contexto si no está ya adjunta
                if (_context.Entry(SequenceEntity).State == EntityState.Detached)
                {
                    _context.SOSSequences.Attach(SequenceEntity);
                }

                var localEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSSequenceId == SequenceEntity.SOSSequenceId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(SequenceUpdate);
                }
                else
                {
                    _mapper.Map(SequenceUpdate, SequenceEntity);
                    _context.SOSSequences.Update(SequenceEntity);
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

        public async Task<int> RemoveSOSSequence(int SOS_Sequence_id)
        {
            var SOS_SequenceEntity = await GetSOSSequence(SOS_Sequence_id);
            SOS_SequenceEntity.IsActive = false;
            _context.SOSSequences.Update(SOS_SequenceEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddIlustrationToSOSSequence(int SOS_Sequence_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSSequence(SOS_Sequence_id, includeImages: true);
            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSSequences.Attach(SosHubEntity);
            }
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

        public async Task<int> RemoveIlustrationFromSOSSequence(int SOS_Sequence_id, int ImageFile_id)
        {
            var SOSSequenceEntity = await GetSOSSequence(SOS_Sequence_id, includeImages: true);

            var Sketch = SOSSequenceEntity.Illustrations.ToList().Find(i => i.FileUploadId == ImageFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSSequences.Update(SOSSequenceEntity);

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Add Range SOS Sequence

        public async Task<List<SOSSequenceLogbook>> AddRangeSOSSequenceLogbook(List<SOSSequenceLogbook> SOSSequenceLogbooksToAdd)
        {
            _context.SOSSequenceLogbooks.AddRange(SOSSequenceLogbooksToAdd);
            await _context.SaveChangesAsync();

            // Desvincular las nuevas AnalysisLogbook del contexto
            foreach (var sequencelogbook in SOSSequenceLogbooksToAdd)
            {
                _context.Entry(sequencelogbook).State = EntityState.Detached;
            }

            return SOSSequenceLogbooksToAdd;
        }
        #endregion
        #region Add To Sos Sequence
        public async Task<AsyncVoidMethodBuilder> AddNoteToSOSSequence(SOSSequence master, Commentary slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSSequenceId == master.SOSSequenceId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Commentaries.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Notes == null)
                {
                    master.Notes = new List<Commentary>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Notes.Any(c => c.CommentaryId == slave.CommentaryId))
                {
                    master.Notes.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddSOSSequenceLogbookToSOSSequence(SOSSequence master, SOSSequenceLogbook slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSSequenceLogbooks.Local.FirstOrDefault(entry => entry.SOSSequenceLogbookId == slave.SOSSequenceLogbookId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSSequenceLogbooks.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.SequenceLogbooks == null)
                {
                    master.SequenceLogbooks = new List<SOSSequenceLogbook>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.SequenceLogbooks.Any(c => c.SOSSequenceLogbookId == slave.SOSSequenceLogbookId))
                {
                    master.SequenceLogbooks.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Remove from SOSSequence

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSequenceLogbookFromSOSSequence(SOSSequence Master)
        {
            Master.SequenceLogbooks?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSSequence(SOSSequence Master)
        {
            Master.Notes?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        #endregion
        #region SOSSequenceLogbook
        public async Task<SOSSequenceLogbook> GetSOSSequenceLogbookById(int id)
        {
            return await _context.SOSSequenceLogbooks.AsNoTracking().Where(t => t.SOSSequenceLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateSequenceLogbook(SOSSequenceLogbookForUpdateDto SequenceForUpdate)
        {
            try
            {
                var query = _context.SOSSequenceLogbooks
                                    .Where(t => t.SOSSequenceLogbookId == SequenceForUpdate.SOSSequenceLogbookId);

                SOSSequenceLogbook SequenceLogbook = await query.FirstOrDefaultAsync();

                if (SequenceLogbook == null)
                {
                    throw new InvalidOperationException("Sequence Logbook not found or is not active.");
                }

                var localEntry = _context.SOSSequenceLogbooks.Local.FirstOrDefault(entry => entry.SOSSequenceLogbookId == SequenceForUpdate.SOSSequenceLogbookId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(SequenceForUpdate);
                }
                else
                {
                    if (_context.Entry(SequenceLogbook).State == EntityState.Detached)
                    {
                        _context.SOSSequenceLogbooks.Attach(SequenceLogbook);
                    }

                    _mapper.Map(SequenceForUpdate, SequenceLogbook);
                    _context.SOSSequenceLogbooks.Update(SequenceLogbook);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Sequence Logbook: " + ex.Message);
                return 0;
            }
        }
        public async Task<int> CreateSOSSequenceLogbook(SOSSequenceLogbook LogBook_ToCreate)
        {
            _context.SOSSequenceLogbooks.Add(LogBook_ToCreate);
            return await _context.SaveChangesAsync();
        }
        #endregion
        //Distribution
        #region SOSDistribution
        public async Task<int> CreateSOSDistribution(SOSDistribution SOS_DistributionToCreate)
        {
            var analysesCopy = SOS_DistributionToCreate.Analyses.ToList();

            for (int j = 0; j < analysesCopy.Count; j++)
            {
                var Analysis = analysesCopy[j];
                var localMasterEntry = _context.SOSAnalyses.Local
                    .FirstOrDefault(entry => entry.SOSAnalysisId == Analysis.SOSAnalysisId);

                if (localMasterEntry != null)
                {
                    SOS_DistributionToCreate.Analyses.Remove(Analysis);
                    SOS_DistributionToCreate.Analyses.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(Analysis).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(Analysis);
                    }
                }
            }

            var sequencesCopy = SOS_DistributionToCreate.Sequences.ToList();

            for (int j = 0; j < sequencesCopy.Count; j++)
            {
                var sequence = sequencesCopy[j];
                var localMasterEntry = _context.SOSSequences.Local
                    .FirstOrDefault(entry => entry.SOSSequenceId == sequence.SOSSequenceId);

                if (localMasterEntry != null)
                {
                    SOS_DistributionToCreate.Sequences.Remove(sequence);
                    SOS_DistributionToCreate.Sequences.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(sequence).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(sequence);
                    }
                }
            }

            _context.SOSDistributions.Add(SOS_DistributionToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {
            var query = _context.SOSDistributions.AsNoTracking().Where(SOS => SOS.SOSDistributionId == SOSDistributionId && SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Illustrations);
            }

            if (includeTimes)
            {
                query = query.Include(t => t.SOSDistributionAdditionalTime);
            }

            if (includeNotes)
            {
                query = query.Include(query => query.Notes);
            }

            if (includeLogbooks)
            {
                query = query.Include(t => t.DistributionLogbooks).ThenInclude(l => l.Approver);
                query = query.Include(t => t.DistributionLogbooks).ThenInclude(l => l.Reviewer);
                query = query.Include(t => t.SOSDistributionAdditionalTime);
            }

            if (includeTurns)
            {
                query = query.Include(t => t.Turns).ThenInclude(t => t.Supervisor);
                query = query.Include(t => t.Turns).ThenInclude(t => t.Operator);
            }

            if (includeCollections)
            {
                query = query.AsNoTracking().Include(s => s.Sequences).ThenInclude(sh => sh.SOSHub).ThenInclude(shs => shs.Sections).ThenInclude(shsa => shsa.Analyses);
                query = query.AsNoTracking().Include(s => s.Analyses).ThenInclude(sh => sh.SOSHub).ThenInclude(shs => shs.Sections).ThenInclude(shsa => shsa.Analyses);
            }

            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModels);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed).ThenInclude(t => t.Tool);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.MaterialsUsed).ThenInclude(m => m.Material);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Plant);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Department);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ApproverOwners);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ReviewerEditors);
                query = query.Include(m => m.Times);
                query = query.Include(m => m.Turns).ThenInclude(t => t.Operator);
                query = query.Include(m => m.Turns).ThenInclude(t => t.Supervisor);
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
                sosHub.DistributionLogbooks = sosHub.DistributionLogbooks.Where(t => t.IsActive == true).ToList();
            }

            if (includeCollections)
            {
                sosHub.Sequences = sosHub.Sequences.Where(t => t.IsActive == true).ToList();
                sosHub.Analyses = sosHub.Analyses.Where(t => t.IsActive == true).ToList();
            }



            return sosHub;
        }

        public async Task<IEnumerable<SOSDistribution>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false)
        {
            var query = _context.SOSDistributions.AsNoTracking().Where(SOS => SOS.IsActive == true);

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
                query = query.Include(t => t.DistributionLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
            }

            var sosDistributions = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var SOSDistribution in sosDistributions)
                {
                    SOSDistribution.Illustrations = SOSDistribution.Illustrations.Where(i => i.IsActive == true).ToList();
                }
            }

            if (includeNotes)
            {
                foreach (var SOSDistribution in sosDistributions)
                {
                    SOSDistribution.Notes = SOSDistribution.Notes.Where(v => v.IsActive == true).ToList();
                }
            }

            if (includeLogbooks)
            {
                foreach (var SOSDistribution in sosDistributions)
                {
                    SOSDistribution.DistributionLogbooks = SOSDistribution.DistributionLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }



            return sosDistributions;
        }

        public async Task<int> UpdateSOSDistribution(SOSDistributionForUpdateDto DistributionUpdate, SOSDistribution DistributionEntity)
        {
            try
            {
                // Adjunta la entidad al contexto si no está ya adjunta
                if (_context.Entry(DistributionEntity).State == EntityState.Detached)
                {
                    _context.SOSDistributions.Attach(DistributionEntity);
                }

                var localEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == DistributionEntity.SOSDistributionId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(DistributionUpdate);
                }
                else
                {
                    _mapper.Map(DistributionUpdate, DistributionEntity);
                    _context.SOSDistributions.Update(DistributionEntity);
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


        public async Task<int> RemoveSOSDistribution(int SOS_Distribution_id)
        {
            var SOS_DistributionEntity = await GetSOSDistribution(SOS_Distribution_id);
            SOS_DistributionEntity.IsActive = false;
            _context.SOSDistributions.Update(SOS_DistributionEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddIlustrationToSOSDistribution(int SOS_Distribution_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSDistribution(SOS_Distribution_id, includeImages: true);
            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSDistributions.Attach(SosHubEntity);
            }
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

        public async Task<int> RemoveIlustrationFromSOSDistribution(int SOS_Distribution_id, int ImageFile_id)
        {
            var SOSDistributionEntity = await GetSOSDistribution(SOS_Distribution_id, includeImages: true);

            var Sketch = SOSDistributionEntity.Illustrations.ToList().Find(i => i.FileUploadId == ImageFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSDistributions.Update(SOSDistributionEntity);

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Add Range SOS Distribution

        public async Task<List<SOSDistributionLogbook>> AddRangeSOSDistributionLogbook(List<SOSDistributionLogbook> SOSDistributionLogbooksToAdd)
        {
            _context.SOSDistributionLogbooks.AddRange(SOSDistributionLogbooksToAdd);

            await _context.SaveChangesAsync();

            // Desvincular las nuevas distributionlogbook del contexto
            foreach (var distributionlogbook in SOSDistributionLogbooksToAdd)
            {
                _context.Entry(distributionlogbook).State = EntityState.Detached;
            }

            return SOSDistributionLogbooksToAdd;
        }
        #endregion
        #region Add To Sos Distribution
        public async Task<AsyncVoidMethodBuilder> AddNoteToSOSDistribution(SOSDistribution master, Commentary slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == master.SOSDistributionId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.Commentaries.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.Notes == null)
                {
                    master.Notes = new List<Commentary>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.Notes.Any(c => c.CommentaryId == slave.CommentaryId))
                {
                    master.Notes.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddAnalysisToSOSDistribution(SOSDistribution master, SOSAnalysis slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == master.SOSDistributionId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == slave.SOSAnalysisId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(slave);
                    }
                }

                // Añadir el Analysis a la colección de Analyses del master
                if (master.Analyses == null)
                {
                    master.Analyses = new List<SOSAnalysis>();
                }

                // Verificar si el analysis ya está en la colección
                if (!master.Analyses.Any(c => c.SOSAnalysisId == slave.SOSAnalysisId))
                {
                    master.Analyses.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSequenceToSOSDistribution(SOSDistribution master, SOSSequence slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSDistributionId == master.SOSDistributionId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSSequenceId == slave.SOSSequenceId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(slave);
                    }
                }

                // Añadir el Analysis a la colección de Analyses del master
                if (master.Sequences == null)
                {
                    master.Sequences = new List<SOSSequence>();
                }

                // Verificar si el analysis ya está en la colección
                if (!master.Sequences.Any(c => c.SOSSequenceId == slave.SOSSequenceId))
                {
                    master.Sequences.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }





        public async Task<AsyncVoidMethodBuilder> AddSOSDistributionLogbookToSOSDistribution(SOSDistribution master, SOSDistributionLogbook slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSDistributionLogbooks.Local.FirstOrDefault(entry => entry.SOSDistributionLogbookId == slave.SOSDistributionLogbookId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSDistributionLogbooks.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.DistributionLogbooks == null)
                {
                    master.DistributionLogbooks = new List<SOSDistributionLogbook>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.DistributionLogbooks.Any(c => c.SOSDistributionLogbookId == slave.SOSDistributionLogbookId))
                {
                    master.DistributionLogbooks.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddSOSDistributionAdditionalTimeToSOSDistribution(SOSDistribution master, SOSDistributionAdditionalTime slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSDistributions.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSDistributions.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSDistributionAdditionalTimes.Local.FirstOrDefault(entry => entry.SOSDistributionAdditionalTimeId == slave.SOSDistributionAdditionalTimeId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSDistributionAdditionalTimes.Attach(slave);
                    }
                }

                // Verificar si el comentario ya está en la colección
                master.SOSDistributionAdditionalTime = slave;

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        #endregion
        #region Remove from SOSDistribution

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(SOSDistribution Master)
        {
            Master.DistributionLogbooks?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSDistribution(SOSDistribution Master)
        {
            Master.Notes?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(SOSDistribution Master)
        {
            Master.SOSDistributionAdditionalTime = null;
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSequencesFromSOSDistribution(SOSDistribution Master)
        {
            foreach(var sec in Master.Sequences)
            {
                SOSSequence item = _context.SOSSequences.Where(ss => ss.SOSSequenceId == sec.SOSSequenceId).Include(d => d.Distributions).FirstOrDefault();
                SOSDistribution dis = item.Distributions.First(dis => dis.SOSDistributionId == Master.SOSDistributionId);

                item.Distributions.Remove(dis);
            }

            Master.Sequences?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisFromSOSDistribution(SOSDistribution Master)
        {
            foreach (var sec in Master.Analyses)
            {
                SOSAnalysis item = _context.SOSAnalyses.Where(sa => sa.SOSAnalysisId == sec.SOSAnalysisId).Include(d => d.Distributions).FirstOrDefault();

                SOSDistribution dis = item.Distributions.First(dis => dis.SOSDistributionId == Master.SOSDistributionId);

                item.Distributions.Remove(dis);
            }
            Master.Analyses?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region SOSDistributionLogbook
        public async Task<SOSDistributionLogbook> GetSOSDistributionLogbookById(int id)
        {
            return await _context.SOSDistributionLogbooks.AsNoTracking().Where(t => t.SOSDistributionLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }


        public async Task<int> UpdateDistributionLogbook(SOSDistributionLogbookForUpdateDto DistributionForUpdate)
        {
            try
            {
                var query = _context.SOSDistributionLogbooks
                                    .Where(t => t.SOSDistributionLogbookId == DistributionForUpdate.SOSDistributionLogbookId);

                SOSDistributionLogbook DistributionLogbook = await query.FirstOrDefaultAsync();

                if (DistributionLogbook == null)
                {
                    throw new InvalidOperationException("Distribution Logbook not found or is not active.");
                }

                var localEntry = _context.SOSDistributionLogbooks.Local.FirstOrDefault(entry => entry.SOSDistributionLogbookId == DistributionForUpdate.SOSDistributionLogbookId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(DistributionForUpdate);
                }
                else
                {
                    if (_context.Entry(DistributionLogbook).State == EntityState.Detached)
                    {
                        _context.SOSDistributionLogbooks.Attach(DistributionLogbook);
                    }

                    _mapper.Map(DistributionForUpdate, DistributionLogbook);
                    _context.SOSDistributionLogbooks.Update(DistributionLogbook);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Distribution Logbook: " + ex.Message);
                return 0;
            }
        }
        public async Task<int> CreateSOSDistributionLogbook(SOSDistributionLogbook LogBook_ToCreate)
        {
            _context.SOSDistributionLogbooks.Add(LogBook_ToCreate);
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region SOS Distribution Additional Time
        public async Task<SOSDistributionAdditionalTime> GetSOSDistributionAdditionalTimeId(int Id)
        {
            return await _context.SOSDistributionAdditionalTimes.AsNoTracking().Where(t => t.SOSDistributionAdditionalTimeId == Id).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateSOSDistributionAdditionalTime(SOSDistributionAdditionalTimeForUpdateDto SOSDistributionAdditionalTimeForUpdate)
        {
            try
            {
                var query = _context.SOSDistributionAdditionalTimes.Where(t => t.SOSDistributionAdditionalTimeId == SOSDistributionAdditionalTimeForUpdate.SOSDistributionAdditionalTimeId && t.IsActive == true);

                SOSDistributionAdditionalTime sosDistributionAdditionalTime = await query.FirstOrDefaultAsync();

                if (sosDistributionAdditionalTime == null)
                {
                    throw new InvalidOperationException("SOS Distribution Additional Time not found or is not active.");
                }

                // Verifica si la entidad ya está siendo rastreada
                var localEntry = _context.SOSDistributionAdditionalTimes.Local.FirstOrDefault(entry => entry.SOSDistributionAdditionalTimeId == SOSDistributionAdditionalTimeForUpdate.SOSDistributionAdditionalTimeId);
                if (localEntry != null)
                {
                    // Si la entidad localmente rastreada es diferente, usa esa instancia
                    _context.Entry(localEntry).CurrentValues.SetValues(SOSDistributionAdditionalTimeForUpdate);
                }
                else
                {
                    // Si no, adjunta la entidad obtenida de la base de datos
                    if (_context.Entry(sosDistributionAdditionalTime).State == EntityState.Detached)
                    {
                        _context.SOSDistributionAdditionalTimes.Attach(sosDistributionAdditionalTime);
                    }

                    _mapper.Map(SOSDistributionAdditionalTimeForUpdate, sosDistributionAdditionalTime);
                    _context.SOSDistributionAdditionalTimes.Update(sosDistributionAdditionalTime);
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
        //Combination
        #region SOSCombination
        public async Task<int> CreateSOSCombination(SOSCombination SOS_CombinationToCreate)
        {
            _context.SOSCombinations.Add(SOS_CombinationToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var query = _context.SOSCombinations.AsNoTracking().Where(SOS => SOS.SOSCombinationId == SOSCombinationId && SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Illustrations);
            }

            //if (includeNotes)
            //{
            //    query = query.Include(query => query.Notes);
            //}

            if (includeLogbooks)
            {
                query = query.Include(t => t.CombinationLogbooks).ThenInclude(l => l.Approver);
                query = query.Include(t => t.CombinationLogbooks).ThenInclude(l => l.Reviewer);
                query = query.Include(c => c.ReviewerHS);
            }

            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModels);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed).ThenInclude(t => t.Tool);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.MaterialsUsed).ThenInclude(m => m.Material);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Plant);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Department);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ApproverOwners);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ReviewerEditors);
                query = query.Include(m => m.Turns).ThenInclude(t => t.Operator);
                query = query.Include(m => m.Turns).ThenInclude(t => t.Supervisor);
            }

            if (includeImagesSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Images);
            }

            var sosHub = await query.FirstOrDefaultAsync();

            if (sosHub == null)
                return null;

            //Filtrar los subobjetos manualmente después de la carga inicial
            if (includeImages)
            {
                sosHub.Illustrations = sosHub.Illustrations.Where(i => i.IsActive == true).ToList();
            }

            //if (includeNotes)
            //{
            //    sosHub.Notes = sosHub.Notes.Where(v => v.IsActive == true).ToList();
            //}

            if (includeLogbooks)
            {
                sosHub.CombinationLogbooks = sosHub.CombinationLogbooks.Where(t => t.IsActive == true).ToList();
            }



            return sosHub;
        }

        public async Task<IEnumerable<SOSCombination>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false)
        {
            var query = _context.SOSCombinations.AsNoTracking().Where(SOS => SOS.IsActive == true);

            if (includeImages)
            {
                query = query.Include(i => i.Illustrations);
            }

            //if (includeNotes)
            //{
            //    query = query.Include(query => query.Notes);
            //}

            if (includeLogbooks)
            {
                query = query.Include(t => t.CombinationLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
            }

            var sosCombinations = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            if (includeImages)
            {
                foreach (var SOSCombination in sosCombinations)
                {
                    SOSCombination.Illustrations = SOSCombination.Illustrations.Where(i => i.IsActive == true).ToList();
                }
            }

            //if (includeNotes)
            //{
            //    foreach (var SOSCombination in sosCombinations)
            //    {
            //        SOSCombination.Notes = SOSCombination.Notes.Where(v => v.IsActive == true).ToList();
            //    }
            //}

            if (includeLogbooks)
            {
                foreach (var SOSCombination in sosCombinations)
                {
                    SOSCombination.CombinationLogbooks = SOSCombination.CombinationLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }



            return sosCombinations;
        }

        public async Task<int> UpdateSOSCombination(SOSCombinationForUpdateDto CombinationUpdate, SOSCombination CombinationEntity)
        {
            try
            {
                // Adjunta la entidad al contexto si no está ya adjunta
                if (_context.Entry(CombinationEntity).State == EntityState.Detached)
                {
                    _context.SOSCombinations.Attach(CombinationEntity);
                }

                var localEntry = _context.SOSCombinations.Local.FirstOrDefault(entry => entry.SOSCombinationId == CombinationEntity.SOSCombinationId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(CombinationUpdate);
                }
                else
                {
                    _mapper.Map(CombinationUpdate, CombinationEntity);
                    _context.SOSCombinations.Update(CombinationEntity);
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

        public async Task<int> RemoveSOSCombination(int SOS_Combination_id)
        {
            var SOS_CombinationEntity = await GetSOSCombination(SOS_Combination_id);
            SOS_CombinationEntity.IsActive = false;
            _context.SOSCombinations.Update(SOS_CombinationEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddIlustrationToSOSCombination(int SOS_Combination_id, FileUpload evidence)
        {
            var SosHubEntity = await GetSOSCombination(SOS_Combination_id, includeImages: true);
            if (_context.Entry(SosHubEntity).State == EntityState.Detached)
            {
                _context.SOSCombinations.Attach(SosHubEntity);
            }
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

        public async Task<int> RemoveIlustrationFromSOSCombination(int SOS_Combination_id, int ImageFile_id)
        {
            var SOSCombinationEntity = await GetSOSCombination(SOS_Combination_id, includeImages: true);

            var Sketch = SOSCombinationEntity.Illustrations.ToList().Find(i => i.FileUploadId == ImageFile_id);
            if (Sketch != null)
            {
                Sketch.IsActive = false;
            }

            _context.SOSCombinations.Update(SOSCombinationEntity);

            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Add Range SOS Combination

        public async Task<List<SOSCombinationLogbook>> AddRangeSOSCombinationLogbook(List<SOSCombinationLogbook> SOSCombinationLogbooksToAdd)
        {
            _context.SOSCombinationLogbooks.AddRange(SOSCombinationLogbooksToAdd);
            await _context.SaveChangesAsync();

            // Desvincular las nuevas combinationlogbook del contexto
            foreach (var combinationlogbook in SOSCombinationLogbooksToAdd)
            {
                _context.Entry(combinationlogbook).State = EntityState.Detached;
            }

            return SOSCombinationLogbooksToAdd;
        }
        #endregion
        #region Add To Sos Combination
        //public async Task<AsyncVoidMethodBuilder> AddNoteToSOSCombination(SOSCombination master, Commentary slave)
        //{
        //    try
        //    {
        //        // Verificar si el master ya está siendo rastreado en el contexto
        //        var localMasterEntry = _context.SOSCombinations.Local.FirstOrDefault(entry => entry.SOSCombinationId == master.SOSCombinationId);
        //        if (localMasterEntry != null)
        //        {
        //            master = localMasterEntry;
        //        }
        //        else
        //        {
        //            if (_context.Entry(master).State == EntityState.Detached)
        //            {
        //                _context.SOSCombinations.Attach(master);
        //            }
        //        }

        //        // Verificar si el slave ya está siendo rastreado en el contexto
        //        var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
        //        if (localSlaveEntry != null)
        //        {
        //            slave = localSlaveEntry;
        //        }
        //        else
        //        {
        //            if (_context.Entry(slave).State == EntityState.Detached)
        //            {
        //                _context.Commentaries.Attach(slave);
        //            }
        //        }

        //        // Añadir el comentario a la colección de ProcessSheetCommentary del master
        //        if (master.Notes == null)
        //        {
        //            master.Notes = new List<Commentary>();
        //        }

        //        // Verificar si el comentario ya está en la colección
        //        if (!master.Notes.Any(c => c.CommentaryId == slave.CommentaryId))
        //        {
        //            master.Notes.Add(slave);
        //        }

        //        // Guardar los cambios
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
        //        Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
        //    }
        //    return new AsyncVoidMethodBuilder();
        //}

        public async Task<AsyncVoidMethodBuilder> AddSOSCombinationLogbookToSOSCombination(SOSCombination master, SOSCombinationLogbook slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSCombinations.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSCombinations.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSCombinationLogbooks.Local.FirstOrDefault(entry => entry.SOSCombinationLogbookId == slave.SOSCombinationLogbookId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSCombinationLogbooks.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.CombinationLogbooks == null)
                {
                    master.CombinationLogbooks = new List<SOSCombinationLogbook>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.CombinationLogbooks.Any(c => c.SOSCombinationLogbookId == slave.SOSCombinationLogbookId))
                {
                    master.CombinationLogbooks.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Remove from SOSCombination

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSCombinationLogbookFromSOSCombination(SOSCombination Master)
        {
            Master.CombinationLogbooks?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        //public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSCombination(SOSCombination Master)
        //{
        //    Master.Notes?.Clear();
        //    _context.SaveChanges();
        //    return new AsyncVoidMethodBuilder();
        //}

        #endregion
        #region SOSCombinationLogbook
        public async Task<SOSCombinationLogbook> GetSOSCombinationLogbookById(int id)
        {
            return await _context.SOSCombinationLogbooks.AsNoTracking().Where(t => t.SOSCombinationLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateCombinationLogbook(SOSCombinationLogbookForUpdateDto CombinationForUpdate)
        {
            try
            {
                var query = _context.SOSCombinationLogbooks
                                    .Where(t => t.SOSCombinationLogbookId == CombinationForUpdate.SOSCombinationLogbookId);

                SOSCombinationLogbook CombinationLogbook = await query.FirstOrDefaultAsync();

                if (CombinationLogbook == null)
                {
                    throw new InvalidOperationException("Combination Logbook not found or is not active.");
                }

                var localEntry = _context.SOSCombinationLogbooks.Local.FirstOrDefault(entry => entry.SOSCombinationLogbookId == CombinationForUpdate.SOSCombinationLogbookId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(CombinationForUpdate);
                }
                else
                {
                    if (_context.Entry(CombinationLogbook).State == EntityState.Detached)
                    {
                        _context.SOSCombinationLogbooks.Attach(CombinationLogbook);
                    }

                    _mapper.Map(CombinationForUpdate, CombinationLogbook);
                    _context.SOSCombinationLogbooks.Update(CombinationLogbook);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Combination Logbook: " + ex.Message);
                return 0;
            }
        }
        public async Task<int> CreateSOSCombinationLogbook(SOSCombinationLogbook LogBook_ToCreate)
        {
            _context.SOSCombinationLogbooks.Add(LogBook_ToCreate);
            return await _context.SaveChangesAsync();
        }
        #endregion

        //Flow
        #region SOSFlow
        public async Task<int> CreateSOSFlow(SOSFlow SOS_FlowToCreate)
        {
            _context.SOSFlows.Add(SOS_FlowToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var query = _context.SOSFlows.AsNoTracking().Where(SOS => SOS.SOSFlowId == SOSFlowId && SOS.IsActive == true);

            //if (includeImages)
            //{
            //    query = query.Include(i => i.Illustrations);
            //}

            //if (includeNotes)
            //{
            //    query = query.Include(query => query.Notes);
            //}

            if (includeLogbooks)
            {
                query = query.Include(t => t.FlowLogbooks).ThenInclude(l => l.Approver);
                query = query.Include(t => t.FlowLogbooks).ThenInclude(l => l.Reviewer);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub).ThenInclude(o => o.ApproverOwners);
                query = query.Include(m => m.SOSHub).ThenInclude(r => r.ReviewerEditors);

                query = query.Include(m => m.SOSHub).ThenInclude(s => s.Sections).ThenInclude(a => a.Analyses);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.AppliedModels);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.ToolsUsed).ThenInclude(t => t.Tool);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.MaterialsUsed).ThenInclude(m => m.Material);
                query = query.Include(m => m.SOSHub).ThenInclude(s => s.SafetyEquipment);
                query = query.Include(m => m.SOSHub).ThenInclude(p => p.Plant);
                query = query.Include(m => m.SOSHub).ThenInclude(d => d.Department);
                query = query.Include(m => m.SOSHub).ThenInclude(d => d.Distribution);
            }




            var sosHub = await query.FirstOrDefaultAsync();

            if (sosHub == null)
                return null;


            if (includeLogbooks)
            {
                sosHub.FlowLogbooks = sosHub.FlowLogbooks.Where(t => t.IsActive == true).ToList();
            }



            return sosHub;
        }

        public async Task<IEnumerable<SOSFlow>> GetAllSOSFlow(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false)
        {
            var query = _context.SOSFlows.AsNoTracking().Where(SOS => SOS.IsActive == true);

            //if (includeImages)
            //{
            //    query = query.Include(i => i.Illustrations);
            //}

            //if (includeNotes)
            //{
            //    query = query.Include(query => query.Notes);
            //}

            if (includeLogbooks)
            {
                query = query.Include(t => t.FlowLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHub);
            }

            var sosFlows = await query.OrderBy(s => s.SOSHubId).ToListAsync();

            //if (includeImages)
            //{
            //    foreach (var SOSFlow in sosFlows)
            //    {
            //        SOSFlow.Illustrations = SOSFlow.Illustrations.Where(i => i.IsActive == true).ToList();
            //    }
            //}

            //if (includeNotes)
            //{
            //    foreach (var SOSFlow in sosFlows)
            //    {
            //        SOSFlow.Notes = SOSFlow.Notes.Where(v => v.IsActive == true).ToList();
            //    }
            //}

            if (includeLogbooks)
            {
                foreach (var SOSFlow in sosFlows)
                {
                    SOSFlow.FlowLogbooks = SOSFlow.FlowLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }



            return sosFlows;
        }

        public async Task<int> UpdateSOSFlow(SOSFlowForUpdateDto FlowUpdate, SOSFlow FlowEntity)
        {
            try
            {
                // Adjunta la entidad al contexto si no está ya adjunta
                if (_context.Entry(FlowEntity).State == EntityState.Detached)
                {
                    _context.SOSFlows.Attach(FlowEntity);
                }

                var localEntry = _context.SOSFlows.Local.FirstOrDefault(entry => entry.SOSFlowId == FlowEntity.SOSFlowId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(FlowUpdate);
                }
                else
                {
                    _mapper.Map(FlowUpdate, FlowEntity);
                    _context.SOSFlows.Update(FlowEntity);
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

        public async Task<int> RemoveSOSFlow(int SOS_Flow_id)
        {
            var SOS_FlowEntity = await GetSOSFlow(SOS_Flow_id);
            SOS_FlowEntity.IsActive = false;
            _context.SOSFlows.Update(SOS_FlowEntity);
            return await _context.SaveChangesAsync();
        }

        #endregion
        #region Add Range SOS Flow

        public async Task<List<SOSFlowLogbook>> AddRangeSOSFlowLogbook(List<SOSFlowLogbook> SOSFlowLogbooksToAdd)
        {
            _context.SOSFlowLogbooks.AddRange(SOSFlowLogbooksToAdd);
            await _context.SaveChangesAsync();

            // Desvincular las nuevas Flowlogbook del contexto
            foreach (var Flowlogbook in SOSFlowLogbooksToAdd)
            {
                _context.Entry(Flowlogbook).State = EntityState.Detached;
            }

            return SOSFlowLogbooksToAdd;
        }
        #endregion
        #region Add To Sos Flow
        //public async Task<AsyncVoidMethodBuilder> AddNoteToSOSFlow(SOSFlow master, Commentary slave)
        //{
        //    try
        //    {
        //        // Verificar si el master ya está siendo rastreado en el contexto
        //        var localMasterEntry = _context.SOSFlows.Local.FirstOrDefault(entry => entry.SOSFlowId == master.SOSFlowId);
        //        if (localMasterEntry != null)
        //        {
        //            master = localMasterEntry;
        //        }
        //        else
        //        {
        //            if (_context.Entry(master).State == EntityState.Detached)
        //            {
        //                _context.SOSFlows.Attach(master);
        //            }
        //        }

        //        // Verificar si el slave ya está siendo rastreado en el contexto
        //        var localSlaveEntry = _context.Commentaries.Local.FirstOrDefault(entry => entry.CommentaryId == slave.CommentaryId);
        //        if (localSlaveEntry != null)
        //        {
        //            slave = localSlaveEntry;
        //        }
        //        else
        //        {
        //            if (_context.Entry(slave).State == EntityState.Detached)
        //            {
        //                _context.Commentaries.Attach(slave);
        //            }
        //        }

        //        // Añadir el comentario a la colección de ProcessSheetCommentary del master
        //        if (master.Notes == null)
        //        {
        //            master.Notes = new List<Commentary>();
        //        }

        //        // Verificar si el comentario ya está en la colección
        //        if (!master.Notes.Any(c => c.CommentaryId == slave.CommentaryId))
        //        {
        //            master.Notes.Add(slave);
        //        }

        //        // Guardar los cambios
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
        //        Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
        //    }
        //    return new AsyncVoidMethodBuilder();
        //}

        //public async Task AddIlustrationToSOSFlow(int SOS_Flow_id, FileUpload evidence)
        //{
        //    var SosHubEntity = await GetSOSFlow(SOS_Flow_id, includeImages: true);
        //    if (_context.Entry(SosHubEntity).State == EntityState.Detached)
        //    {
        //        _context.SOSFlows.Attach(SosHubEntity);
        //    }
        //    if (SosHubEntity != null)
        //    {

        //        if (SosHubEntity.Illustrations != null)
        //        {
        //            SosHubEntity.Illustrations.Add(evidence);
        //        }
        //        else
        //        {
        //            SosHubEntity.Illustrations = new List<FileUpload>
        //            {
        //                evidence
        //            };
        //        }
        //    }
        //}

        public async Task<AsyncVoidMethodBuilder> AddSOSFlowLogbookToSOSFlow(SOSFlow master, SOSFlowLogbook slave)
        {
            try
            {
                // Verificar si el master ya está siendo rastreado en el contexto
                var localMasterEntry = _context.SOSFlows.Local.FirstOrDefault(entry => entry.SOSHubId == master.SOSHubId);
                if (localMasterEntry != null)
                {
                    master = localMasterEntry;
                }
                else
                {
                    if (_context.Entry(master).State == EntityState.Detached)
                    {
                        _context.SOSFlows.Attach(master);
                    }
                }

                // Verificar si el slave ya está siendo rastreado en el contexto
                var localSlaveEntry = _context.SOSFlowLogbooks.Local.FirstOrDefault(entry => entry.SOSFlowLogbookId == slave.SOSFlowLogbookId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSFlowLogbooks.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.FlowLogbooks == null)
                {
                    master.FlowLogbooks = new List<SOSFlowLogbook>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.FlowLogbooks.Any(c => c.SOSFlowLogbookId == slave.SOSFlowLogbookId))
                {
                    master.FlowLogbooks.Add(slave);
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente, puedes loguearlo o lanzar una excepción personalizada
                Debug.WriteLine("An error occurred while updating the SOSHub: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        #endregion
        #region Remove from SOSFlow

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSFlowLogbookFromSOSFlow(SOSFlow Master)
        {
            Master.FlowLogbooks?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        //public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSFlow(SOSFlow Master)
        //{
        //    Master.Notes?.Clear();
        //    _context.SaveChanges();
        //    return new AsyncVoidMethodBuilder();
        //}

        //public async Task<int> RemoveIlustrationFromSOSFlow(int SOS_Flow_id, int ImageFile_id)
        //{
        //    var SOSFlowEntity = await GetSOSFlow(SOS_Flow_id, includeImages: true);

        //    var Sketch = SOSFlowEntity.Illustrations.ToList().Find(i => i.FileUploadId == ImageFile_id);
        //    if (Sketch != null)
        //    {
        //        Sketch.IsActive = false;
        //    }

        //    _context.SOSFlows.Update(SOSFlowEntity);

        //    return await _context.SaveChangesAsync();
        //}

        #endregion
        #region SOSFlowLogbook
        public async Task<SOSFlowLogbook> GetSOSFlowLogbookById(int id)
        {
            return await _context.SOSFlowLogbooks.AsNoTracking().Where(t => t.SOSFlowLogbookId == id && t.IsActive == true).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateFlowLogbook(SOSFlowLogbookForUpdateDto FlowForUpdate)
        {
            try
            {
                var query = _context.SOSFlowLogbooks
                                    .Where(t => t.SOSFlowLogbookId == FlowForUpdate.SOSFlowLogbookId);

                SOSFlowLogbook FlowLogbook = await query.FirstOrDefaultAsync();

                if (FlowLogbook == null)
                {
                    throw new InvalidOperationException("Flow Logbook not found or is not active.");
                }

                var localEntry = _context.SOSFlowLogbooks.Local.FirstOrDefault(entry => entry.SOSFlowLogbookId == FlowForUpdate.SOSFlowLogbookId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(FlowForUpdate);
                }
                else
                {
                    if (_context.Entry(FlowLogbook).State == EntityState.Detached)
                    {
                        _context.SOSFlowLogbooks.Attach(FlowLogbook);
                    }

                    _mapper.Map(FlowForUpdate, FlowLogbook);
                    _context.SOSFlowLogbooks.Update(FlowLogbook);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the Flow Logbook: " + ex.Message);
                return 0;
            }
        }
        public async Task<int> CreateSOSFlowLogbook(SOSFlowLogbook LogBook_ToCreate)
        {
            _context.SOSFlowLogbooks.Add(LogBook_ToCreate);
            return await _context.SaveChangesAsync();
        }
        #endregion

        //CommonOper
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
        public async Task<bool> SaveChanges()
        {
            try
            {
                return (_context.SaveChanges() >= 0);
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
