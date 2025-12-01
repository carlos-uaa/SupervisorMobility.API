using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_DistributionRepository
{
    public class SOS_DistributionRepository : ISOS_DistributionRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public SOS_DistributionRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Distribution
        #region SOSDistribution
        public async Task<string> GetDistributionName(int distributionID)
        {
            if (distributionID == 0) return string.Empty;
            try
            {
                var name = _context.SOSDistributions
                    .Where(d => d.SOSDistributionId == distributionID)
                    .Select(d => d.InternalControlNumber)
                    .FirstOrDefault();

                return name ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while retrieving the distribution name: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a new SOSDistribution record with its related entities,
        /// including Analyses, Sequences, SOSHubs, Sections, and Operation Sequences.
        /// </summary>
        /// <param name="SOS_DistributionToCreate">
        /// The SOSDistribution entity to be created and persisted in the database.
        /// </param>
        /// <returns>
        /// Returns the number of state entries written to the database.
        /// </returns>
        public async Task<int> CreateSOSDistribution(SOSDistribution SOS_DistributionToCreate)
        {
            // +============ ANALYSES =============+ \\
            // NOTE: Ensure that all Analyses are properly tracked by the EF Core context.
            var analysesCopy = SOS_DistributionToCreate.Analyses.ToList();
            var trackedAnalyses = new List<SOSAnalysis>();

            for (int j = 0; j < analysesCopy.Count; j++)
            {
                var Analysis = analysesCopy[j];
                var localMasterEntry = _context.SOSAnalyses.Local.FirstOrDefault(entry => entry.SOSAnalysisId == Analysis.SOSAnalysisId);

                if (localMasterEntry != null)
                {
                    trackedAnalyses.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(Analysis).State == EntityState.Detached)
                    {
                        // TODO: Consider handling concurrency if multiple threads/contexts attach the same entity
                        _context.SOSAnalyses.Attach(Analysis);
                    }
                    trackedAnalyses.Add(Analysis);
                }
            }
            SOS_DistributionToCreate.Analyses = trackedAnalyses;

            // +============ SEQUENCES =============+ \\
            // NOTE: Similar to Analyses, ensure Sequences are managed consistently by EF Core.
            var sequencesCopy = SOS_DistributionToCreate.Sequences.ToList();

            for (int j = 0; j < sequencesCopy.Count; j++)
            {
                var sequence = sequencesCopy[j];
                var localMasterEntry = _context.SOSSequences.Local.FirstOrDefault(entry => entry.SOSSequenceId == sequence.SOSSequenceId);

                if (localMasterEntry != null)
                {
                    // NOTE: Replace with tracked entity to avoid EF duplicate tracking issues
                    SOS_DistributionToCreate.Sequences.Remove(sequence);
                    SOS_DistributionToCreate.Sequences.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(sequence).State == EntityState.Detached)
                    {
                        // TODO: Consider handling concurrency if multiple threads/contexts attach the same entity
                        _context.SOSSequences.Attach(sequence);
                    }
                }
            }

            // +============ SOSHUBS COLLECTION =============+ \\
            // NOTE: Build a unique set of SOSHub IDs from Analyses and Sequences.
            var allSOSHubIds = new HashSet<int>();

            // NOTE: Collect SOSHub IDs from Analyses
            foreach (var analysis in SOS_DistributionToCreate.Analyses)
            {
                if (analysis.SOSHubId > 0)
                    allSOSHubIds.Add(analysis.SOSHubId);
            }

            // NOTE: Collect SOSHub IDs from Sequences
            foreach (var sequence in SOS_DistributionToCreate.Sequences)
            {
                if (sequence.SOSHubId > 0)
                    allSOSHubIds.Add(sequence.SOSHubId);
            }

            // NOTE: Load and assign unique SOSHubs with their sections, applied models, and times
            if (allSOSHubIds.Any())
            {
                // NOTE: Load SOSHubs with sections and applied models
                var sosHubs = await _context.SOSHubs
                    .Where(h => allSOSHubIds.Contains(h.SOSHubId))
                    .Include(h => h.Sections)
                    .Include(a => a.AppliedModels)
                    .ToListAsync();

                SOS_DistributionToCreate.SOSHubs = sosHubs;

                // NOTE: Prepare collection for SOSDistributionOperationSequence records
                var operationSequences = new List<SOSDistributionOperationSequence>();

                foreach (var sosHub in sosHubs)
                {
                    var hubSections = sosHub?.Sections?.ToList() ?? new List<Section>();
                    if (hubSections.Count == 0) continue;

                    List<SOSTime> allTimes = new();

                    // NOTE: Load times from first analysis if exists
                    if (sosHub?.SOSAnalysis?.Count > 0)
                    {
                        var analysis = sosHub.SOSAnalysis!.FirstOrDefault();
                        if (analysis != null)
                        {
                            var analysisComplete = await _context.SOSAnalyses.Include(t => t.Times).FirstOrDefaultAsync(a => a.SOSAnalysisId == analysis.SOSAnalysisId);
                            if (analysisComplete?.Times != null) allTimes = analysisComplete.Times.ToList();
                        }
                    }
                    else if (sosHub?.SOSSequence?.Count > 0)
                    {
                        var sequence = sosHub.SOSSequence!.FirstOrDefault();
                        if (sequence != null)
                        {
                            var sequenceComplete = await _context.SOSSequences.Include(t => t.Times).FirstOrDefaultAsync(s => s.SOSSequenceId == sequence.SOSSequenceId);
                            if (sequenceComplete?.Times != null) allTimes = sequenceComplete.Times.ToList();
                        }
                    }

                    // =============== TIMES FOR SECTION =============== \\
                    // NOTE: Build operation sequences for each section
                    foreach (var section in hubSections)
                    {
                        var sectionTime = allTimes.FirstOrDefault(s => s.SectionId == section.SectionId);

                        var timeOS = new string[5]; // NOTE: FORMAT -> "0§0§0§0§0"

                        // NOTE: Map times into positions based on applied models
                        if (sosHub?.AppliedModels?.Any(a => a.ProductId == 3) == true)
                            timeOS[0] = $"{sectionTime?.Time ?? "0"}";


                        if (sosHub?.AppliedModels?.Any(a => a.ProductId == 1) == true)
                            timeOS[1] = $"{sectionTime?.Time ?? "0"}";


                        if (sosHub?.AppliedModels?.Any(a => a.ProductId == 2) == true)
                            timeOS[2] = $"{sectionTime?.Time ?? "0"}";

                        var operationSequence = new SOSDistributionOperationSequence
                        {
                            SectionId = section.SectionId,
                            SequenceId = null,
                            Times = String.Join("§", timeOS),
                            IsActive = true
                        };

                        operationSequences.Add(operationSequence);
                    }
                }


                // =============== ASSIGN APPLIED MODELS =============== \\
                // NOTE: Build applied models flags (5 slots: X, P, N, etc.)
                var AppliedModels = sosHubs.SelectMany(d => d.AppliedModels!).Distinct();
                var listModels = new string[5];


                foreach (var model in AppliedModels)
                {
                    if (model.ProductId == 3) listModels[0] = "X";
                    if (model.ProductId == 1) listModels[1] = "P";
                    if (model.ProductId == 2) listModels[2] = "N";
                }

                SOS_DistributionToCreate.AplicationModels = String.Join("§", listModels);

                // =============== CYCLE TIME CALCULATION =============== \\
                // NOTE: Calculate global cycle time by summing all operation sequences
                double[] allTimesCycle = new double[5];
                foreach (var opSeq in operationSequences)
                {
                    SOS_DistributionToCreate.SOSDistributionOperationSequence.Add(opSeq);

                    var timesOpSeq = string.IsNullOrEmpty(opSeq.Times) ? new List<string>() : opSeq.Times.Split("§").Take(5).ToList();
                    // Pad with "0" if less than 5 elements
                    while (timesOpSeq.Count < 5)
                     {
                        timesOpSeq.Add("0");
                     }
                    for (int i = 0; i < allTimesCycle.Length; i++)
                    {
                        allTimesCycle[i] += double.TryParse(timesOpSeq[i], out var val) ? val : 0;
                    }
                }

                SOS_DistributionToCreate.CycleTime = String.Join("§", allTimesCycle);

            }

            _context.SOSDistributions.Add(SOS_DistributionToCreate);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> GetIdDistributionBySosHub(int IdSosHub)
        {
            var SOSDis = await _context.SOSHubs.Include(s => s.SOSDistribution.Where(d => d.IsActive == true)).Where(s => s.SOSHubId == IdSosHub).FirstOrDefaultAsync();
            if (SOSDis == null) return 0;

            if (SOSDis.SOSDistribution == null || !SOSDis.SOSDistribution.Any()) return 0;
            return SOSDis.SOSDistribution.FirstOrDefault(d => d.SOSHubId == IdSosHub)?.SOSDistributionId ?? 0;
        }

        public async Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {
            var query = _context.SOSDistributions.AsNoTracking()
                .Where(SOS => SOS.SOSDistributionId == SOSDistributionId && SOS.IsActive == true);

            var sosDistribution = await query.FirstOrDefaultAsync();

            if (sosDistribution != null)
            {
                await _context.Entry(sosDistribution).Collection(t => t.SOSDistributionOperationSequence).LoadAsync();

                foreach (var operationSequence in sosDistribution.SOSDistributionOperationSequence)
                {
                    await _context.Entry(operationSequence).Reference(t => t.Section).LoadAsync();
                    await _context.Entry(operationSequence?.Section).Collection(a => a.Analyses).LoadAsync();
                }

                if (includeImages)
                {
                    await _context.Entry(sosDistribution).Collection(d => d.Illustrations).LoadAsync();
                }

                if (includeNotes)
                {
                    await _context.Entry(sosDistribution).Collection(d => d.Notes).LoadAsync();
                }

                if (includeLogbooks)
                {
                    await _context.Entry(sosDistribution)
                        .Collection(d => d.DistributionLogbooks)
                        .LoadAsync();

                    foreach (var logbook in sosDistribution.DistributionLogbooks)
                    {
                        await _context.Entry(logbook).Reference(l => l.Approver).LoadAsync();
                        await _context.Entry(logbook).Reference(l => l.Reviewer).LoadAsync();
                    }
                }

                if (includeTimes)
                {
                    await _context.Entry(sosDistribution).Reference(t => t.SOSDistributionAdditionalTime).LoadAsync();
                }

                if (includeTurns)
                {
                    await _context.Entry(sosDistribution)
                        .Collection(d => d.Turns)
                        .LoadAsync();

                    foreach (var turn in sosDistribution.Turns)
                    {
                        await _context.Entry(turn).Reference(t => t.Supervisor).LoadAsync();
                        await _context.Entry(turn).Reference(t => t.Operator).LoadAsync();
                    }
                }

                if (includeSOS)
                {
                    await _context.Entry(sosDistribution).Collection(d => d.SOSHubs).LoadAsync();

                    if (sosDistribution.SOSHubs != null && sosDistribution.SOSHubs.Any())
                    {
                        foreach (var sosHub in sosDistribution.SOSHubs)
                        {
                            await _context.Entry(sosHub).Collection(s => s.Sections).LoadAsync();

                            foreach (var section in sosHub.Sections)
                            {
                                await _context.Entry(section).Collection(s => s.Analyses).LoadAsync();
                            }

                            await _context.Entry(sosHub).Collection(s => s.AppliedModels).LoadAsync();
                            await _context.Entry(sosHub).Collection(s => s.ToolsUsed).LoadAsync();
                            foreach (var toolUsed in sosHub.ToolsUsed)
                            {
                                await _context.Entry(toolUsed).Reference(t => t.Tool).LoadAsync();
                            }

                            await _context.Entry(sosHub).Collection(s => s.MaterialsUsed).LoadAsync();
                            foreach (var materialUsed in sosHub.MaterialsUsed)
                            {
                                await _context.Entry(materialUsed).Reference(m => m.Material).LoadAsync();
                            }

                            await _context.Entry(sosHub).Collection(s => s.SafetyEquipment).LoadAsync();
                            await _context.Entry(sosHub).Reference(s => s.Plant).LoadAsync();
                            await _context.Entry(sosHub).Reference(s => s.Department).LoadAsync();
                            await _context.Entry(sosHub).Reference(s => s.Creator).LoadAsync();
                            await _context.Entry(sosHub).Collection(s => s.ApproverOwners).LoadAsync();
                            await _context.Entry(sosHub).Collection(s => s.ReviewerEditors).LoadAsync();
                        }
                    }
                }

                if (includeImagesSOS && sosDistribution.SOSHubs != null)
                {
                    //await _context.Entry(sosDistribution.SOSHubs).Collection(s => s.Images).LoadAsync();
                }

                if (includeCollections)
                {
                    sosDistribution.Sequences = await _context.SOSSequences
                        .Where(s => s.Distributions.Any(d => d.SOSDistributionId == SOSDistributionId))
                        .Include(sh => sh.SOSHub).ThenInclude(shs => shs.Creator)
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();

                    sosDistribution.Analyses = await _context.SOSAnalyses
                        .Where(s => s.Distributions.Any(d => d.SOSDistributionId == SOSDistributionId))
                        .Include(sh => sh.SOSHub).ThenInclude(shs => shs.Creator)
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();

                    // Fix for existing distributions: Create missing operation sequences
                    if (sosDistribution.SOSDistributionOperationSequence?.Count() == 0 && sosDistribution.SOSHubs?.Any() == true)
                    {
                        Console.WriteLine($"DEBUG GetSOSDistribution: No operation sequences found, creating them for distribution {SOSDistributionId}");

                        var newOperationSequences = new List<SOSDistributionOperationSequence>();

                        foreach (var sosHub in sosDistribution.SOSHubs)
                        {
                            if (sosHub.Sections != null)
                            {
                                foreach (var section in sosHub.Sections)
                                {
                                    var operationSequence = new SOSDistributionOperationSequence
                                    {
                                        SectionId = section.SectionId,
                                        SequenceId = null,
                                        Times = "",
                                        IsActive = true
                                    };

                                    sosDistribution.SOSDistributionOperationSequence.Add(operationSequence);
                                    _context.SOSDistributionOperationSequence.Add(operationSequence);
                                    newOperationSequences.Add(operationSequence);

                                    Console.WriteLine($"DEBUG GetSOSDistribution: Created OperationSequence for Section {section.SectionId}");
                                }
                            }
                        }

                        if (newOperationSequences.Any())
                        {
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"DEBUG GetSOSDistribution: Saved {newOperationSequences.Count} new operation sequences");

                            // Reload the operation sequences with their sections and analyses
                            await _context.Entry(sosDistribution).Collection(t => t.SOSDistributionOperationSequence).LoadAsync();
                            foreach (var operationSequence in sosDistribution.SOSDistributionOperationSequence)
                            {
                                await _context.Entry(operationSequence).Reference(t => t.Section).LoadAsync();
                                await _context.Entry(operationSequence?.Section).Collection(a => a.Analyses).LoadAsync();
                            }
                        }
                    }

                    // Load SOSHubs collection for the distribution
                    var allSOSHubIds = new HashSet<int>();

                    // Collect SOSHub IDs from Sequences
                    foreach (var sequence in sosDistribution.Sequences)
                    {
                        if (sequence.SOSHubId > 0)
                            allSOSHubIds.Add(sequence.SOSHubId);
                    }

                    // Collect SOSHub IDs from Analyses
                    foreach (var analysis in sosDistribution.Analyses)
                    {
                        if (analysis.SOSHubId > 0)
                            allSOSHubIds.Add(analysis.SOSHubId);
                    }

                    // Load unique SOSHubs with their related data
                    if (allSOSHubIds.Any())
                    {
                        sosDistribution.SOSHubs = await _context.SOSHubs
                            .Where(h => allSOSHubIds.Contains(h.SOSHubId))
                            .Include(s => s.Sections)
                            .ThenInclude(sec => sec.Analyses)
                            .Include(s => s.AppliedModels)
                            .Include(s => s.ToolsUsed)
                            .ThenInclude(t => t.Tool)
                            .Include(s => s.MaterialsUsed)
                            .ThenInclude(m => m.Material)
                            .Include(s => s.SafetyEquipment)
                            .Include(p => p.Plant)
                            .Include(d => d.Department)
                            .ToListAsync();

                        // Sync critical points from SOSHubs to SOSDistributionOperationSequence analyses
                        Console.WriteLine($"DEBUG: SOSHubs count: {sosDistribution.SOSHubs?.Count() ?? 0}");
                        Console.WriteLine($"DEBUG: SOSDistributionOperationSequence count: {sosDistribution.SOSDistributionOperationSequence?.Count() ?? 0}");

                        // First, log all available hub analyses
                        var allHubAnalyses = sosDistribution.SOSHubs
                            .SelectMany(h => h.Sections)
                            .SelectMany(s => s.Analyses)
                            .ToList();

                        Console.WriteLine($"DEBUG: Total hub analyses available: {allHubAnalyses.Count}");
                        foreach (var hubAnalysis in allHubAnalyses)
                        {
                            Console.WriteLine($"DEBUG: Hub Analysis ID: {hubAnalysis.AnalysisId}, Text: '{hubAnalysis.Text}', CriticalPoints: {hubAnalysis.CriticalPoints?.Count() ?? 0}");
                            if (hubAnalysis.CriticalPoints?.Any() == true)
                            {
                                Console.WriteLine($"DEBUG:   Critical Points: [{string.Join(", ", hubAnalysis.CriticalPoints)}]");
                            }
                        }

                        foreach (var operationSequence in sosDistribution.SOSDistributionOperationSequence)
                        {
                            Console.WriteLine($"DEBUG: Processing OpSeq {operationSequence.SOSDistributionOperationSequenceId}, SectionId: {operationSequence.SectionId}");
                            Console.WriteLine($"DEBUG: Section.Analyses count: {operationSequence.Section?.Analyses?.Count() ?? 0}");

                            if (operationSequence.Section?.Analyses != null)
                            {
                                foreach (var opAnalysis in operationSequence.Section.Analyses)
                                {
                                    Console.WriteLine($"DEBUG: Processing OpSeq Analysis ID: {opAnalysis.AnalysisId}, Text: '{opAnalysis.Text}', current CriticalPoints: {opAnalysis.CriticalPoints?.Count() ?? 0}");

                                    // Find the corresponding analysis in SOSHubs with critical points
                                    var hubAnalysis = allHubAnalyses.FirstOrDefault(a => a.AnalysisId == opAnalysis.AnalysisId);

                                    Console.WriteLine($"DEBUG: Found hubAnalysis: {hubAnalysis != null}");
                                    if (hubAnalysis != null)
                                    {
                                        Console.WriteLine($"DEBUG:   Hub Analysis Text: '{hubAnalysis.Text}', CriticalPoints: {hubAnalysis.CriticalPoints?.Count() ?? 0}");
                                        if (hubAnalysis.CriticalPoints?.Any() == true)
                                        {
                                            opAnalysis.CriticalPoints = hubAnalysis.CriticalPoints;
                                            opAnalysis.Reasons = hubAnalysis.Reasons;
                                            Console.WriteLine($"DEBUG: ✓ Successfully synchronized {hubAnalysis.CriticalPoints.Count} critical points to analysis {opAnalysis.AnalysisId}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"DEBUG: ✗ Hub analysis {hubAnalysis.AnalysisId} has no critical points to sync");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"DEBUG: ✗ No matching hub analysis found for OpSeq Analysis ID {opAnalysis.AnalysisId}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return sosDistribution;
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
                query = query.Include(m => m.SOSHubs);
            }

            var sosDistributions = await query.ToListAsync();

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
            // Recargar la colección para asegurar que está vacía y sincronizada
            await _context.Entry(DistributionEntity).Collection(d => d.SOSHubs).LoadAsync();

            try
            {
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

        public async Task<AsyncVoidMethodBuilder> AddSOSHubToSOSDistribution(SOSDistribution master, SOSHub slave)
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
                var localSlaveEntry = _context.SOSHubs.Local.FirstOrDefault(entry => entry.SOSHubId == slave.SOSHubId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSHubs.Attach(slave);
                    }
                }

                // Añadir el soshub a la colección de soshub del master
                if (master.SOSHubs == null)
                {
                    master.SOSHubs = new List<SOSHub>();
                }
                // Verificar si el soshub ya está en la colección
                if (!master.SOSHubs.Any(c => c.SOSHubId == slave.SOSHubId))
                {
                    master.SOSHubs.Add(slave);
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
                // Asegúrate de que el master está en el contexto
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

                // Recarga la colección para evitar duplicados en memoria
                await _context.Entry(master).Collection(m => m.Analyses).LoadAsync();

                // Asegúrate de que el slave está en el contexto
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

                // Añadir solo si no existe
                if (!master.Analyses.Any(c => c.SOSAnalysisId == slave.SOSAnalysisId))
                {
                    master.Analyses.Add(slave);
                    await _context.SaveChangesAsync();
                }
                // Si ya existe, no hace nada (no lanza excepción)
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the SOSDistribution: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> AddSequenceToSOSDistribution(SOSDistribution master, SOSSequence slave)
        {
            try
            {
                // Asegúrate de que el master está en el contexto
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

                // Recarga la colección para evitar duplicados en memoria
                await _context.Entry(master).Collection(m => m.Sequences).LoadAsync();

                // Asegúrate de que el slave está en el contexto
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

                // Añadir solo si no existe
                if (!master.Sequences.Any(c => c.SOSSequenceId == slave.SOSSequenceId))
                {
                    master.Sequences.Add(slave);
                    await _context.SaveChangesAsync();
                }
                // Si ya existe, no hace nada (no lanza excepción)
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the SOSDistribution: " + ex.Message);
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> AddSOSDistributionLogbookToSOSDistribution(SOSDistribution master, SOSDistributionLogbook slave)
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

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSHubsFromSOSDistribution(SOSDistribution master)
        {
            // Check if entity is already being tracked
            var localMaster = _context.SOSDistributions.Local
                .FirstOrDefault(d => d.SOSDistributionId == master.SOSDistributionId);

            if (localMaster != null)
            {
                // Use the tracked instance
                if (!_context.Entry(localMaster).Collection(d => d.SOSHubs).IsLoaded)
                    await _context.Entry(localMaster).Collection(d => d.SOSHubs).LoadAsync();

                localMaster.SOSHubs?.Clear();
                await _context.SaveChangesAsync();
            }
            else
            {
                // For untracked entities, use a completely separate context to avoid any conflicts
                // This isolates the operation from any existing tracking state
                var connectionString = _context.Database.GetConnectionString();
                var optionsBuilder = new DbContextOptionsBuilder<SupervisorMobilityContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var isolatedContext = new SupervisorMobilityContext(optionsBuilder.Options))
                {
                    // Load the distribution with only hubs in the clean context
                    var distributionWithHubs = await isolatedContext.SOSDistributions
                        .Include(d => d.SOSHubs)
                        .FirstOrDefaultAsync(d => d.SOSDistributionId == master.SOSDistributionId);

                    if (distributionWithHubs?.SOSHubs?.Any() == true)
                    {
                        // Clear the hubs in the isolated context
                        distributionWithHubs.SOSHubs.Clear();
                        await isolatedContext.SaveChangesAsync();
                    }
                }
            }

            return new AsyncVoidMethodBuilder();
        }

        private void DetachAllConflictingEntities()
        {
            // Detach all potentially conflicting entities
            var entriesToDetach = _context.ChangeTracker.Entries()
                .Where(e =>
                    e.Entity.GetType().Name.Contains("Section") ||
                    e.Entity.GetType().Name.Contains("SOSHub") ||
                    e.Entity.GetType().Name.Contains("SOSDistributionOperationSequence") ||
                    e.Entity.GetType().Name.Contains("Distribution") ||
                    e.Entity.GetType().Name.Contains("Operation") ||
                    e.Entity.GetType().Name.Contains("Area") ||
                    e.Entity.GetType().Name.Contains("Plant"))
                .ToList();

            foreach (var entry in entriesToDetach)
            {
                entry.State = EntityState.Detached;
            }
        }


        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(SOSDistribution Master)
        {
            Master.SOSDistributionAdditionalTime = null;
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSequencesFromSOSDistribution(SOSDistribution Master)
        {
            foreach (var sec in Master.Sequences)
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
        #region SOSDistributionOperationSequences
        //public async Task<SOSCombinationOperationSequence> GetSOSCombinationOperationSequencesById(int id)
        //{
        //    return await _context.SOSCombinationOperationSequences.AsNoTracking().Where(t => t.SOSCombinationOperationSequenceId == id).FirstOrDefaultAsync();

        //}
        //public async Task<int> UpdateSOSCombinationOperationSequences(SOSCombinationOperationSequenceForUpdateDto OperationSequenceForUpdate)
        //{
        //    try
        //    {
        //        var query = _context.SOSCombinationOperationSequences.Where(t => t.SOSCombinationOperationSequenceId == OperationSequenceForUpdate.SOSCombinationOperationSequenceId);

        //        SOSCombinationOperationSequence operationSequence = await query.FirstOrDefaultAsync();

        //        if (operationSequence == null)
        //        {
        //            throw new InvalidOperationException("operationSequence not found or is not active.");
        //        }

        //        var localEntry = _context.SOSCombinationOperationSequences.Local.FirstOrDefault(entry => entry.SOSCombinationOperationSequenceId == OperationSequenceForUpdate.SOSCombinationOperationSequenceId);
        //        if (localEntry != null)
        //        {
        //            _context.Entry(localEntry).CurrentValues.SetValues(OperationSequenceForUpdate);
        //        }
        //        else
        //        {
        //            if (_context.Entry(operationSequence).State == EntityState.Detached)
        //            {
        //                _context.SOSCombinationOperationSequences.Attach(operationSequence);
        //            }

        //            _mapper.Map(OperationSequenceForUpdate, operationSequence);
        //            _context.SOSCombinationOperationSequences.Update(operationSequence);
        //        }

        //        return await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("An error occurred while updating the operationSequence: " + ex.Message);
        //        return 0;
        //    }
        //}



        public async Task<SOSDistributionOperationSequence> GetSOSDistributionOperationSequencesById(int id)
        {
            return await _context.SOSDistributionOperationSequence.AsNoTracking().Where(t => t.SOSDistributionOperationSequenceId == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateSOSDistributionOperationSequences(SOSDistributionOperationSequenceForUpdateDto OperationSequenceForUpdate)
        {
            try
            {
                var query = _context.SOSDistributionOperationSequence.Where(t => t.SOSDistributionOperationSequenceId == OperationSequenceForUpdate.SOSDistributionOperationSequenceId);

                SOSDistributionOperationSequence operationSequence = await query.FirstOrDefaultAsync();

                if (operationSequence == null)
                {
                    throw new InvalidOperationException("operationSequence not found or is not active.");
                }

                var localEntry = _context.SOSDistributionOperationSequence.Local.FirstOrDefault(entry => entry.SOSDistributionOperationSequenceId == OperationSequenceForUpdate.SOSDistributionOperationSequenceId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(OperationSequenceForUpdate);
                }
                else
                {
                    if (_context.Entry(operationSequence).State == EntityState.Detached)
                    {
                        _context.SOSDistributionOperationSequence.Attach(operationSequence);
                    }

                    _mapper.Map(OperationSequenceForUpdate, operationSequence);
                    _context.SOSDistributionOperationSequence.Update(operationSequence);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the operationSequence: " + ex.Message);
                return 0;
            }
        }

        public async Task<AsyncVoidMethodBuilder> DeleteSOSDistributionOperationSequencesById(int OperationSequenceId)
        {
            try
            {
                var localEntry = _context.SOSDistributionOperationSequence.Local.FirstOrDefault(entry => entry.SOSDistributionOperationSequenceId == OperationSequenceId);

                if (localEntry != null)
                {
                    _context.SOSDistributionOperationSequence.Remove(localEntry);
                }
                else
                {
                    var dbEntry = await _context.SOSDistributionOperationSequence.FirstOrDefaultAsync(entry => entry.SOSDistributionOperationSequenceId == OperationSequenceId);
                    if (dbEntry != null) { _context.SOSDistributionOperationSequence.Remove(dbEntry); }
                }

                await _context.SaveChangesAsync();
                return new AsyncVoidMethodBuilder();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the operationSequence: " + ex.Message);
                return new AsyncVoidMethodBuilder();
            }
        }
        public async Task<List<SOSDistributionOperationSequence>> AddRangeSOSDistributionOperationSequences(List<SOSDistributionOperationSequence> SOSOperationSequencesToAdd)
        {
            _context.SOSDistributionOperationSequence.AddRange(SOSOperationSequencesToAdd);
            await _context.SaveChangesAsync();

            // Desvincular las nuevas combinationlogbook del contexto
            foreach (var OperationSequences in SOSOperationSequencesToAdd)
            {
                _context.Entry(OperationSequences).State = EntityState.Detached;
            }

            return SOSOperationSequencesToAdd;
        }

        public async Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSDistribution(SOSDistribution master, SOSDistributionOperationSequence slave)
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
                var localSlaveEntry = _context.SOSDistributionOperationSequence.Local.FirstOrDefault(entry => entry.SOSDistributionOperationSequenceId == slave.SOSDistributionOperationSequenceId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSDistributionOperationSequence.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.SOSDistributionOperationSequence == null)
                {
                    master.SOSDistributionOperationSequence = new List<SOSDistributionOperationSequence>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.SOSDistributionOperationSequence.Any(c => c.SOSDistributionOperationSequenceId == slave.SOSDistributionOperationSequenceId))
                {
                    master.SOSDistributionOperationSequence.Add(slave);
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
        #endregion
    }
}
