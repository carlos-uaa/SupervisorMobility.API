using AutoMapper;
using Azure.Core.GeoJson;
using CsvHelper;
using DocumentFormat.OpenXml.VariantTypes;
using DuoVia.FuzzyStrings;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Dtos;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Enums;
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
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
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
using System.Linq;
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
            try
            {
                _context.SOSHubs.Add(SOS_EntityToCreate);
                await _context.SaveChangesAsync();

                return SOS_EntityToCreate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while creating the SOSHub: " + ex.Message);
                throw;
            }
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
                    //await _context.Entry(sosHub).Collection(s => s.Images.Where(p=>p.IsActive == true)).LoadAsync();
                    await _context.Entry(sosHub).Collection(s => s.Images).Query().Where(p => p.IsActive == true).LoadAsync();
                }

                if (includeVideos)
                {
                    await _context.Entry(sosHub).Collection(s => s.Videos).Query().Where(p => p.IsActive == true).LoadAsync();
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
                    await _context.Entry(sosHub).Reference(o => o.Creator).LoadAsync();
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

                        await _context.Entry(pat).Collection(aa => aa.Supervisors).LoadAsync();
                    }
                }


                if (includeCollections)
                {
                    await _context.Entry(sosHub).Reference(a => a.Hci).Query().Where(d => d.IsActive == true).LoadAsync();


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

                    await _context.Entry(sosHub).Collection(d => d.SOSSynopticControlPoints).Query().Where(d => d.IsActive == true).LoadAsync(); foreach (var synoptic in sosHub.SOSSynopticControlPoints)
                    {
                        await _context.Entry(synoptic).Collection(aa => aa.SynopticPointsLogbooks).LoadAsync();

                        await _context.Entry(synoptic).Collection(aa => aa.Analyses).LoadAsync();
                        await _context.Entry(synoptic).Collection(aa => aa.Sequences).LoadAsync();
                    }

                    await _context.Entry(sosHub).Collection(d => d.SOSSynopticOperatingRequirements).Query().Where(d => d.IsActive == true).LoadAsync(); foreach (var synoptic in sosHub.SOSSynopticOperatingRequirements)
                    {
                        await _context.Entry(synoptic).Collection(aa => aa.SynopticRequirementsLogbooks).LoadAsync();

                        await _context.Entry(synoptic).Collection(aa => aa.Analyses).LoadAsync();
                        await _context.Entry(synoptic).Collection(aa => aa.Sequences).LoadAsync();
                    }


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

        public async Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeSOSDistribution = false, int userId=0)
        {
            IQueryable<SOSHub> query;

            //si tiene mas de una area asignada hacemos el filtrado por las areas asignadas
            if ( userId>0)
            {
                var areaIds = await _context.Users.AsNoTracking().Where(u => u.UserId == userId).Select(u => u.Areas.Select(a => a.AreaId).ToList()).FirstOrDefaultAsync();

                if (areaIds != null && areaIds.Count > 0)
                {

                    query = _context.SOSHubs.AsNoTracking().Where(h => h.IsActive == true && h.AreaId.HasValue && areaIds.Contains(h.AreaId.Value));
                }
                else
                {
                    //buscamos si el usuario tiene solo una area asignada 
                    var areaId = await _context.Users.AsNoTracking().Where(u => u.UserId == userId).Select(u => u.Areas.Select(a => a.AreaId).FirstOrDefault()).FirstOrDefaultAsync();

                    //si solo tiene una sola area hacemos el filtrado sencillo
                    if (areaId != 0)
                    {
                        query = _context.SOSHubs.AsNoTracking().Where(h => h.IsActive == true && h.AreaId == areaId);
                    }
                    else
                    {
                        query = _context.SOSHubs.AsNoTracking().Where(h => h.IsActive == true);
                    }
                }
            }
            else
            {
                query = _context.SOSHubs.AsNoTracking().Where(h => h.IsActive == true);
            }






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
                query = query.Include(o => o.Creator).Include(o => o.ApproverOwners).Include(e => e.ReviewerEditors);
            }

            if (includeSOSDistribution)
            {
                query = query.Include(d => d.SOSDistribution);
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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while creating the SOSHub: " + ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateSOSHub(SOSHub SosEntity)
        {

            _context.SOSHubs.Update(SosEntity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveSOSHub(int SOS_DataPool_id)
        {
            var SosEntity = await GetSOSHub(SOS_DataPool_id, includeCollections: true);

            SosEntity.IsActive = false;

            foreach (var item in SosEntity.SOSAnalysis)
            {
                item.IsActive = false;
            }

            foreach (var item in SosEntity.SOSCombination)
            {
                item.IsActive = false;
            }
            foreach (var item in SosEntity.SOSDistribution)
            {
                item.IsActive = false;
            }
            foreach (var item in SosEntity.SOSFlow)
            {
                item.IsActive = false;
            }
            foreach (var item in SosEntity.SOSSequence)
            {
                item.IsActive = false;
            }


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
                query = query.Include(o => o.Creator).Include(o => o.ApproverOwners).Include(e => e.ReviewerEditors);
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

        public async Task<AsyncVoidMethodBuilder> AddHCISOSCollection(SOSHub master, HCI slave)
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


                // Agrega el slave a la colección del master
                if (master.Hci == null)
                {
                    master.Hci = slave;
                }

                if (!(master.Hci.HCIId == slave.HCIId))
                {
                    master.Hci = slave;
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
            //foreach (var section in SectionsToAdd)
            //{
            //    if (section.Analyses != null)
            //    {
            //        var analysesList = section.Analyses.ToList();
            //        section.Analyses.Clear();

            //        foreach (var analysis in analysesList)
            //        {
            //            analysis.IsActive = true;
            //            // Si el análisis ya existe (tiene un ID válido), adjúntalo al contexto
            //            if (analysis.AnalysisId > 0)
            //            {
            //                var tracked = _context.Analyses.Local.FirstOrDefault(a => a.AnalysisId == analysis.AnalysisId);
            //                if (tracked != null)
            //                {
            //                    tracked.IsActive = true;
            //                    section.Analyses.Add(tracked);
            //                }
            //                else
            //                {
            //                    _context.Analyses.Attach(analysis);
            //                    section.Analyses.Add(analysis);
            //                }
            //            }
            //            else
            //            {
            //                // Si es nuevo, simplemente agrégalo
            //                section.Analyses.Add(analysis);
            //            }
            //        }
            //    }
            //}

            _context.Sections.AddRange(SectionsToAdd);

            _context.SaveChanges();

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

        public async Task<AsyncVoidMethodBuilder> RemoveAllTimesFromSOSAnalysis(SOSAnalysis Master)
        {
            Master.Times?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSDistribution(SOSDistribution Master, List<SOSDistributionOperationSequence> operationSequences)
        {
            var validIds = operationSequences.Select(x => x.SOSDistributionOperationSequenceId).ToList();
            // Buscar en la base de datos todas las OperationSequences que NO estén en la lista de IDs válidos
            var toRemove = await _context.SOSDistributionOperationSequence
                .Where(x => !validIds.Contains(x.SOSDistributionOperationSequenceId))
                .ToListAsync();

            if (toRemove.Any())
            {
                _context.SOSDistributionOperationSequence.RemoveRange(toRemove);
            }

            Master.SOSDistributionOperationSequence?.Clear();
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
