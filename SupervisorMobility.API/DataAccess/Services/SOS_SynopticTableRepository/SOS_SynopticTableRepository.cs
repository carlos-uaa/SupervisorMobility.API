using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Dtos;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using System.Runtime.CompilerServices;
using AutoMapper;
using SupervisorMobility.API.Context;

namespace SupervisorMobility.API.DataAccess.Services.SOS_SynopticTableRepository
{
    public class SOS_SynopticTableRepository : ISOS_SynopticTableRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        public SOS_SynopticTableRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //SynopticTableofOperatingRequirements

        #region SOSSynopticTableofOperatingRequirements
        public async Task<int> CreateSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements SOS_SynopticTableofOperatingRequirementsToCreate)
        {
            _context.SOSSynopticTableofOperatingRequirements.Add(SOS_SynopticTableofOperatingRequirementsToCreate);
            
            // NOTE: Mark related SOSHubs as Unchanged - they already exist in DB
            if (SOS_SynopticTableofOperatingRequirementsToCreate.SOSHubs != null)
            {
                foreach (var sosHub in SOS_SynopticTableofOperatingRequirementsToCreate.SOSHubs)
                {
                    _context.Entry(sosHub).State = EntityState.Unchanged;
                }
            }
            
            return _context.SaveChanges();
        }
        public async Task<SOSSynopticTableofOperatingRequirements> GetSOSSynopticTableofOperatingRequirements(int SOSSynopticTableofOperatingRequirementsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var query = _context.SOSSynopticTableofOperatingRequirements.AsNoTracking().Where(SOS => SOS.SOSSynopticTableofOperatingRequirementsId == SOSSynopticTableofOperatingRequirementsId && SOS.IsActive == true);

            var sosSynopticRequirements = await query.Include(d => d.RequirementDifficulties).Include(k => k.SOSSTROKnowledge).Include(s => s.SOSSTROSkill).Include(e => e.EstablishedConditions).Include(i => i.InsuranceFeatures).Include(o => o.OperationMachine).FirstOrDefaultAsync();

            if (sosSynopticRequirements != null)
            {
                await _context.Entry(sosSynopticRequirements).Collection(t => t.SOSSynopticRequirementsOperationSequence).LoadAsync();

                foreach (var operationSequence in sosSynopticRequirements.SOSSynopticRequirementsOperationSequence)
                {
                    await _context.Entry(operationSequence).Reference(t => t.Section).LoadAsync();
                    await _context.Entry(operationSequence?.Section).Collection(a => a.Analyses).LoadAsync();
                }


                if (includeLogbooks)
                {
                    await _context.Entry(sosSynopticRequirements)
                        .Collection(d => d.SynopticRequirementsLogbooks)
                        .LoadAsync();

                    foreach (var logbook in sosSynopticRequirements.SynopticRequirementsLogbooks)
                    {
                        await _context.Entry(logbook).Reference(l => l.Approver).LoadAsync();
                        await _context.Entry(logbook).Reference(l => l.Reviewer).LoadAsync();
                    }
                }

                if (includeSOS)
                {
                    await _context.Entry(sosSynopticRequirements).Collection(d => d.SOSHubs).LoadAsync();
                }

                if (includeCollections)
                {
                    await _context.Entry(sosSynopticRequirements).Reference(d => d.Reviewer).LoadAsync();
                    await _context.Entry(sosSynopticRequirements).Reference(d => d.Approver).LoadAsync();
                    await _context.Entry(sosSynopticRequirements).Reference(d => d.Creator).LoadAsync();



                    sosSynopticRequirements.Sequences = await _context.SOSSequences
                        .Where(s => s.SOSSynopticOperatingRequirements.Any(d => d.SOSSynopticTableofOperatingRequirementsId == SOSSynopticTableofOperatingRequirementsId))
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();

                    sosSynopticRequirements.Analyses = await _context.SOSAnalyses
                        .Where(s => s.SOSSynopticOperatingRequirements.Any(d => d.SOSSynopticTableofOperatingRequirementsId == SOSSynopticTableofOperatingRequirementsId))
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();
                }
            }

            return sosSynopticRequirements;
        }
        public async Task<IEnumerable<SOSSynopticTableofOperatingRequirements>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var query = _context.SOSSynopticTableofOperatingRequirements.AsNoTracking().Where(SOS => SOS.IsActive == true);


            if (includeLogbooks)
            {
                query = query.Include(t => t.SynopticRequirementsLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHubs);
            }

            var sosSynopticRequirements = await query.ToListAsync();



            if (includeLogbooks)
            {
                foreach (var SOSSynoptic in sosSynopticRequirements)
                {
                    SOSSynoptic.SynopticRequirementsLogbooks = SOSSynoptic.SynopticRequirementsLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }


            return sosSynopticRequirements;
        }

        /// <summary>
        /// Updates an existing <c>SOSSynopticTableofOperatingRequirements</c> entity with the provided DTO data.
        /// Handles updates to related SOS hubs, requirement difficulties, knowledge, skills, and established conditions.
        /// </summary>
        /// <param name="sosSynopticTableofOperatingRequirements_Id">The ID of the STRO entity to update.</param>
        /// <param name="STROUpdate">The DTO containing updated fields and related entities.</param>
        /// <returns>The updated <see cref="SOSSynopticTableofOperatingRequirements"/> entity.</returns>
        /// <exception cref="Exception">Thrown if the specified STRO entity is not found.</exception>
        public async Task<SOSSynopticTableofOperatingRequirements> UpdateSOSSynopticTableofOperatingRequirements(int sosSynopticTableofOperatingRequirements_Id, SOSSynopticTableofOperatingRequirementsForUpdateDto STROUpdate)
        {
            // NOTE: Load the STRO entity with all related collections to ensure full update
            var entity = await _context.SOSSynopticTableofOperatingRequirements
                .Include(s => s.SOSHubs)
                .Include(e => e.RequirementDifficulties)
                .Include(k => k.SOSSTROKnowledge)
                .Include(s => s.SOSSTROSkill)
                .Include(c => c.EstablishedConditions)
                .Include(s => s.SOSSynopticRequirementsOperationSequence)
                .Include(i => i.InsuranceFeatures)
                .Include(o => o.OperationMachine)
                .FirstOrDefaultAsync(e => e.SOSSynopticTableofOperatingRequirementsId == sosSynopticTableofOperatingRequirements_Id);


            if (entity == null) throw new Exception("STRO not found");

            // NOTE: Update main entity properties
            _context.Entry(entity).CurrentValues.SetValues(STROUpdate);

            //+============ SOS HUBS UPDATE =============+\\
            if (STROUpdate.SOSHubIds != null)
            {
                var currentSosHubs = entity.SOSHubs.Select(a => a.SOSHubId).ToList();

                // NOTE: Remove hubs no longer present
                var RemoveSOSHubs = entity.SOSHubs.Where(h => !STROUpdate.SOSHubIds.Contains(h.SOSHubId)).ToList();

                foreach (var hub in RemoveSOSHubs) entity.SOSHubs.Remove(hub);

                // NOTE: Add new hubs
                var ToAddNewSOSHubs = STROUpdate.SOSHubIds.Except(currentSosHubs);

                foreach (var HubId in ToAddNewSOSHubs)
                {
                    var FindHub = await _context.SOSHubs.FindAsync(HubId);
                    if (FindHub != null) entity.SOSHubs.Add(FindHub);
                }
            }

            //+============ REQUIREMENT DIFFICULTIES UPDATE =============+\\
            if (STROUpdate.RequirementDifficulties != null)
            {
                var currentRD = entity.RequirementDifficulties.ToList();

                // NOTE: Remove difficulties no longer present
                var ToRemoveRD = currentRD.Where(d => !STROUpdate.RequirementDifficulties.Any(dd => dd.SOSHubId == d.SOSHubId)).ToList();
                _context.SOSSynopticTableRequirementOperationDifficulty.RemoveRange(ToRemoveRD);

                // NOTE: Update existing or add new difficulties
                foreach (var requirementDiff in STROUpdate.RequirementDifficulties)
                {
                    var existing = currentRD.FirstOrDefault(sd => sd.SOSHubId == requirementDiff.SOSHubId);

                    if (existing == null)
                    {
                        var addRequirementDiff = new SOSSynopticTableRequirementOperationDifficulty
                        {
                            SOSSynopticTableofOperatingRequirementsId = entity.SOSSynopticTableofOperatingRequirementsId,
                            SOSHubId = requirementDiff.SOSHubId,
                            DifficultyLevel = requirementDiff.DifficultyLevel
                        };

                        entity.RequirementDifficulties!.Add(addRequirementDiff);
                    }
                    else
                    {
                        existing.DifficultyLevel = requirementDiff.DifficultyLevel;
                    }
                }

            }

            //+============ KNOWLEDGE UPDATE =============+\\
            if (STROUpdate.SOSSTROKnowledge != null)
            {
                var currentK = entity.SOSSTROKnowledge?.Where(sk => sk.SOSSynopticTableofOperatingRequirementsId == entity.SOSSynopticTableofOperatingRequirementsId).ToList() ?? new List<SOSSTROKnowledgeHub>();

                // NOTE: Remove knowledge entries no longer present
                var toRemove = currentK.Where(d => !STROUpdate.SOSSTROKnowledge.Any(k => k.SOSHubId == d.SOSHubId && k.KnowledgeId == d.KnowledgeId)).ToList();
                _context.SOSSTROKnowledgeHub.RemoveRange(toRemove);

                // NOTE: Add new knowledge entries if they don't exist
                foreach (var Knowledge in STROUpdate.SOSSTROKnowledge)
                {
                    var existing = currentK.FirstOrDefault(k => k.KnowledgeId == Knowledge.KnowledgeId && k.SOSHubId == Knowledge.SOSHubId);
                    if (existing == null)
                    {
                        var addKnowledge = new SOSSTROKnowledgeHub
                        {
                            SOSSynopticTableofOperatingRequirementsId = entity.SOSSynopticTableofOperatingRequirementsId,
                            KnowledgeId = Knowledge.KnowledgeId,
                            SOSHubId = Knowledge.SOSHubId
                        };

                        entity.SOSSTROKnowledge?.Add(addKnowledge);
                    }
                }
            }

            //+============ SKILL UPDATE =============+\\
            if (STROUpdate.SOSSTROSkill != null)
            {
                var currentS = entity.SOSSTROSkill?.Where(sk => sk.SOSSynopticTableofOperatingRequirementsId == entity.SOSSynopticTableofOperatingRequirementsId).ToList() ?? new List<SOSSTROSkillHub>();

                // NOTE: Remove skill entries no longer present
                var ToRemoveS = currentS.Where(s => !STROUpdate.SOSSTROSkill.Any(ss => ss.SOSHubId == s.SOSHubId && ss.SkillId == s.SkillId));
                _context.SOSSTROSkillHub.RemoveRange(ToRemoveS);

                // NOTE: Add new skill entries if they don't exist
                foreach (var Skill in STROUpdate.SOSSTROSkill)
                {
                    var existing = currentS.FirstOrDefault(k => k.SkillId == Skill.SkillId && k.SOSHubId == Skill.SOSHubId);
                    if (existing == null)
                    {
                        var addSkill = new SOSSTROSkillHub
                        {
                            SOSSynopticTableofOperatingRequirementsId = entity.SOSSynopticTableofOperatingRequirementsId,
                            SkillId = Skill.SkillId,
                            SOSHubId = Skill.SOSHubId
                        };

                        entity.SOSSTROSkill?.Add(addSkill);
                    }
                }
            }

            //+======= ESTABLISHED CONDITIONS ========+\\
            if (STROUpdate.EstablishedConditions != null)
            {
                var currentEC = entity.EstablishedConditions.Where(e => e.SOSSynopticTableofOperatingRequirementsId == entity.SOSSynopticTableofOperatingRequirementsId).ToList() ?? new List<EstablishedConditions>();

                var ToRemoveEC = currentEC.Where(e => !STROUpdate.EstablishedConditions.Any(ec => ec.Id == e.Id && ec.Id != 0)).ToList();
                _context.EstablishedConditions.RemoveRange(ToRemoveEC);

                foreach (var EstaCon in STROUpdate.EstablishedConditions)
                {
                    var existing = currentEC.FirstOrDefault(e => e.Id == EstaCon.Id);
                    if (existing == null)
                    {
                        var AddEstablishedCondition = new EstablishedConditions
                        {
                            Condition = EstaCon.Condition,
                            SectionId = EstaCon.SectionId,
                            SOSSynopticTableofOperatingRequirementsId = EstaCon.SOSSynopticTableofOperatingRequirementsId
                        };

                        entity.EstablishedConditions?.Add(AddEstablishedCondition);
                    }
                    else
                    {
                        existing.Condition = EstaCon.Condition;
                    }
                }
            }

            //+======= ESTABLISHED CONDITIONS ========+\\
            if (STROUpdate.InsuranceFeatures != null)
            {
                var currentEC = entity.InsuranceFeatures.Where(e => e.SOSSynopticTableofOperatingRequirementsId == entity.SOSSynopticTableofOperatingRequirementsId).ToList() ?? new List<InsuranceFeatures>();

                var ToRemoveEC = currentEC.Where(e => !STROUpdate.InsuranceFeatures.Any(ec => ec.Id == e.Id && ec.Id != 0)).ToList();
                _context.InsuranceFeatures.RemoveRange(ToRemoveEC);

                foreach (var EstaCon in STROUpdate.InsuranceFeatures)
                {
                    var existing = currentEC.FirstOrDefault(e => e.Id == EstaCon.Id);
                    if (existing == null)
                    {
                        var AddInsuranceFeatures = new InsuranceFeatures
                        {
                            Insurance = EstaCon.Insurance,
                            SectionId = EstaCon.SectionId,
                            SOSSynopticTableofOperatingRequirementsId = EstaCon.SOSSynopticTableofOperatingRequirementsId
                        };

                        entity.InsuranceFeatures?.Add(AddInsuranceFeatures);
                    }
                    else
                    {
                        existing.Insurance = EstaCon.Insurance;
                    }
                }
            }

            //+======= OPERATIONS MACHINE ========+\\
            if (STROUpdate.OperationMachine != null)
            {
                var currentEC = entity.OperationMachine.Where(e => e.SOSSynopticTableofOperatingRequirementsId == entity.SOSSynopticTableofOperatingRequirementsId).ToList() ?? new List<OperationMachine>();

                var ToRemoveEC = currentEC.Where(e => !STROUpdate.OperationMachine.Any(ec => ec.Id == e.Id && ec.Id != 0)).ToList();
                _context.OperationMachine.RemoveRange(ToRemoveEC);

                foreach (var EstaCon in STROUpdate.OperationMachine)
                {
                    var existing = currentEC.FirstOrDefault(e => e.Id == EstaCon.Id);
                    if (existing == null)
                    {
                        var AddOperationMachine = new OperationMachine
                        {
                            Operation = EstaCon.Operation,
                            SectionId = EstaCon.SectionId,
                            SOSSynopticTableofOperatingRequirementsId = EstaCon.SOSSynopticTableofOperatingRequirementsId
                        };

                        entity.OperationMachine?.Add(AddOperationMachine);
                    }
                    else
                    {
                        existing.Operation = EstaCon.Operation;
                    }
                }
            }

            //+=========== STRO SEQUENCES ============+\\
            foreach (var updatedItem in STROUpdate.SOSSynopticRequirementsOperationSequence!)
            {
                // NOTE: Add new sequence if it does not exist (ID = 0)
                if (updatedItem.SOSSynopticRequirementsOperationSequenceId == 0)
                {
                    var AddSTROSequence = new SOSSynopticRequirementsOperationSequence
                    {
                        Sequence = updatedItem.Sequence,
                        SectionId = updatedItem.SectionId,
                        SosHubId = updatedItem.SosHubId,
                        OperationPersonText = updatedItem?.OperationPersonText,
                        OperationMachineText = updatedItem?.OperationMachineText,
                        IsOperationPersonRequired = updatedItem?.IsOperationPersonRequired,
                        IsOperationMachineRequired = updatedItem?.IsOperationMachineRequired,
                        IsActive = true,
                        SOSSynopticTableofOperatingRequirementsId = entity.SOSSynopticTableofOperatingRequirementsId
                    };

                    // NOTE: Link sequence to the current Synoptic Table of Operating Requirements
                    entity.SOSSynopticRequirementsOperationSequence!.Add(AddSTROSequence);
                }
                else
                {
                    // NOTE: Update existing sequence if it already exists
                    var existingItem = entity.SOSSynopticRequirementsOperationSequence!.FirstOrDefault(x => x.SOSSynopticRequirementsOperationSequenceId == updatedItem.SOSSynopticRequirementsOperationSequenceId);
                    if (existingItem != null)
                    {
                        // NOTE: Update entity values with the updated DTO values
                        _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
                    }
                }
            }



            // NOTE: Save all changes to the database
            await _context.SaveChangesAsync();

            return entity;

        }
        public async Task<int> RemoveSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id)
        {
            var entity = await _context.SOSSynopticTableofOperatingRequirements
                .FirstOrDefaultAsync(e => e.SOSSynopticTableofOperatingRequirementsId == SOS_SynopticTableofOperatingRequirements_id);
            if (entity == null) return 0;
            entity.IsActive = false;
            _context.SOSSynopticTableofOperatingRequirements.Update(entity);
            return await _context.SaveChangesAsync();
        }
        public async Task AddIlustrationToSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id, FileUpload evidence)
        {
            throw new NotImplementedException();
        }
        public async Task<int> RemoveIlustrationFromSOSSynopticTableofOperatingRequirements(int SOS_SynopticTableofOperatingRequirements_id, int ImageFile_id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Add To Sos SynopticTableofOperatingRequirements
        public async Task<AsyncVoidMethodBuilder> AddSOSHubToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSHub slave)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> AddSOSSynopticRequirementsLogbookToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, SOSSynopticRequirementsLogbook Slave)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> AddNoteToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, Commentary Slave)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> AddAnalysisToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSAnalysis slave)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> AddSequenceToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements master, SOSSequence slave)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, SOSSynopticRequirementsOperationSequence Slave)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SOSSynopticRequirementsLogbook>> AddRangeSOSSynopticRequirementsLogbook(List<SOSSynopticRequirementsLogbook> SOSSynopticRequirementsLogbooksToAdd)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Remove from SosSynopticTableofOperatingRequirements
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSynopticRequirementsLogbookFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllNotesFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSHubsFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSequencesFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllAnalysisFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> SOSDataRemoveAllSOSSynopticTableofOperatingRequirementsAdditionalTimeFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region SOSSynopticRequirementsLogbook
        public async Task<SOSSynopticRequirementsLogbook> GetSOSSynopticRequirementsLogbookById(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<int> UpdateSynopticRequirementsLogbook(SOSSynopticRequirementsLogbookForUpdateDto SynopticTableofOperatingRequirementsForUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateSOSSynopticRequirementsLogbook(SOSSynopticRequirementsLogbook LogBook_ToCreate)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SOSSynopticRequirementsOperationSequences
        public async Task<SOSSynopticRequirementsOperationSequence> GetSOSSynopticRequirementsOperationSequencesById(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<int> UpdateSOSSynopticRequirementsOperationSequences(SOSSynopticRequirementsOperationSequenceForUpdateDto OperationSequenceForUpdate)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SOSSynopticRequirementsOperationSequence>> AddRangeSOSSynopticRequirementsOperationSequences(List<SOSSynopticRequirementsOperationSequence> SOSOperationSequencesToAdd)
        {
            throw new NotImplementedException();
        }
        public async Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirements Master, List<SOSSynopticRequirementsOperationSequence> operationSequences)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SOSSynopticTableofControlPoints
        public async Task<int> CreateSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints SOS_SynopticTableofControlPointsToCreate)
        {
            Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] Creating CSPC with ProcessName='{SOS_SynopticTableofControlPointsToCreate.ProcessName}', SOSHubId={SOS_SynopticTableofControlPointsToCreate.SOSHubId}");
            Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] Analyses count: {SOS_SynopticTableofControlPointsToCreate.Analyses?.Count() ?? 0}");
            Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] Sequences count: {SOS_SynopticTableofControlPointsToCreate.Sequences?.Count() ?? 0}");
            
            var analysesCopy = SOS_SynopticTableofControlPointsToCreate.Analyses.ToList();

            for (int j = 0; j < analysesCopy.Count; j++)
            {
                var Analysis = analysesCopy[j];
                var localMasterEntry = _context.SOSAnalyses.Local
                    .FirstOrDefault(entry => entry.SOSAnalysisId == Analysis.SOSAnalysisId);

                if (localMasterEntry != null)
                {
                    SOS_SynopticTableofControlPointsToCreate.Analyses.Remove(Analysis);
                    SOS_SynopticTableofControlPointsToCreate.Analyses.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(Analysis).State == EntityState.Detached)
                    {
                        _context.SOSAnalyses.Attach(Analysis);
                    }
                }
            }

            var sequencesCopy = SOS_SynopticTableofControlPointsToCreate.Sequences.ToList();

            for (int j = 0; j < sequencesCopy.Count; j++)
            {
                var sequence = sequencesCopy[j];
                var localMasterEntry = _context.SOSSequences.Local
                    .FirstOrDefault(entry => entry.SOSSequenceId == sequence.SOSSequenceId);

                if (localMasterEntry != null)
                {
                    SOS_SynopticTableofControlPointsToCreate.Sequences.Remove(sequence);
                    SOS_SynopticTableofControlPointsToCreate.Sequences.Add(localMasterEntry);
                }
                else
                {
                    if (_context.Entry(sequence).State == EntityState.Detached)
                    {
                        _context.SOSSequences.Attach(sequence);
                    }
                }
            }

            _context.SOSSynopticTableofControlPoints.Add(SOS_SynopticTableofControlPointsToCreate);
            
            // CRITICAL FIX: Establecer la relación many-to-many con el Hub
            // Cargar el Hub y agregarlo a la colección para que EF Core cree la entrada en la tabla intermedia
            var hub = await _context.SOSHubs.FindAsync(SOS_SynopticTableofControlPointsToCreate.SOSHubId);
            if (hub != null)
            {
                // Cargar la colección de CSPC del Hub si no está cargada
                await _context.Entry(hub).Collection(h => h.SOSSynopticControlPoints).LoadAsync();
                
                // Agregar el CSPC a la colección del Hub (esto crea la relación many-to-many)
                hub.SOSSynopticControlPoints.Add(SOS_SynopticTableofControlPointsToCreate);
                
                Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] Added CSPC to Hub collection. Hub now has {hub.SOSSynopticControlPoints.Count} CSPC items");
            }
            else
            {
                Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] ERROR: Hub {SOS_SynopticTableofControlPointsToCreate.SOSHubId} not found!");
            }
            
            var savedCount = _context.SaveChanges();
            Console.WriteLine($"[CreateSOSSynopticTableofControlPoints] SaveChanges returned: {savedCount}");
            
            return savedCount;
        }

        public async Task<SOSSynopticTableofControlPoints> GetSOSSynopticTableofControlPoints(int SOSSynopticTableofControlPointsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var query = _context.SOSSynopticTableofControlPoints.AsNoTracking()
               .Where(SOS => SOS.SOSSynopticTableofControlPointsId == SOSSynopticTableofControlPointsId && SOS.IsActive == true);

            var sosSynopticControlPoints = await query.FirstOrDefaultAsync();

            if (sosSynopticControlPoints != null)
            {
                await _context.Entry(sosSynopticControlPoints).Collection(t => t.SOSSynopticPointsOperationSequence).LoadAsync();

                foreach (var operationSequence in sosSynopticControlPoints.SOSSynopticPointsOperationSequence)
                {
                    await _context.Entry(operationSequence).Reference(t => t.Section).LoadAsync();
                    await _context.Entry(operationSequence?.Section).Collection(a => a.Analyses).LoadAsync();
                }


                if (includeLogbooks)
                {
                    await _context.Entry(sosSynopticControlPoints)
                        .Collection(d => d.SynopticPointsLogbooks)
                        .LoadAsync();

                    foreach (var logbook in sosSynopticControlPoints.SynopticPointsLogbooks)
                    {
                        await _context.Entry(logbook).Reference(l => l.Approver).LoadAsync();
                    }
                }

                if (includeSOS)
                {
                    await _context.Entry(sosSynopticControlPoints).Collection(d => d.SOSHubs).LoadAsync();
                }

                if (includeCollections)
                {
                    await _context.Entry(sosSynopticControlPoints).Reference(d => d.Reviewer).LoadAsync();
                    await _context.Entry(sosSynopticControlPoints).Reference(d => d.Approver).LoadAsync();
                    await _context.Entry(sosSynopticControlPoints).Reference(d => d.Creator).LoadAsync();



                    sosSynopticControlPoints.Sequences = await _context.SOSSequences
                        .Where(s => s.SOSSynopticControlPoints.Any(d => d.SOSSynopticTableofControlPointsId == SOSSynopticTableofControlPointsId))
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();

                    sosSynopticControlPoints.Analyses = await _context.SOSAnalyses
                        .Where(s => s.SOSSynopticControlPoints.Any(d => d.SOSSynopticTableofControlPointsId == SOSSynopticTableofControlPointsId))
                        .Include(sh => sh.SOSHub)
                        .ThenInclude(shs => shs.Sections)
                        .ThenInclude(shsa => shsa.Analyses)
                        .ToListAsync();
                }
            }

            return sosSynopticControlPoints;
        }

        public async Task<IEnumerable<SOSSynopticTableofControlPoints>> GetAllSOSSynopticTableofControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var query = _context.SOSSynopticTableofControlPoints.AsNoTracking().Where(SOS => SOS.IsActive == true);


            if (includeLogbooks)
            {
                query = query.Include(t => t.SynopticPointsLogbooks);
            }



            if (includeSOS)
            {
                query = query.Include(m => m.SOSHubs);
            }

            var sosSynopticControlPoints = await query.ToListAsync();



            if (includeLogbooks)
            {
                foreach (var SOSSynoptic in sosSynopticControlPoints)
                {
                    SOSSynoptic.SynopticPointsLogbooks = SOSSynoptic.SynopticPointsLogbooks.Where(t => t.IsActive == true).ToList();
                }
            }


            return sosSynopticControlPoints;
        }

        public async Task<SOSSynopticTableofControlPoints> UpdateSOSSynopticTableofControlPoints(int sosSynopticTableofControlPoints_Id, SOSSynopticTableofControlPointsForUpdateDto STCPUpdate)
        {
            var entity = await _context.SOSSynopticTableofControlPoints
                .FirstOrDefaultAsync(e => e.SOSSynopticTableofControlPointsId == sosSynopticTableofControlPoints_Id);

            if (entity == null) throw new Exception("SOSSynopticTableofControlPoints not found");

            entity.ProcessName = STCPUpdate.ProcessName ?? entity.ProcessName;
            entity.InternalControlNumber = STCPUpdate.InternalControlNumber ?? entity.InternalControlNumber;
            entity.CreatorId = STCPUpdate.CreatorId ?? entity.CreatorId;
            entity.ReviewerId = STCPUpdate.ReviewerId ?? entity.ReviewerId;
            entity.ApproverId = STCPUpdate.ApproverId ?? entity.ApproverId;
            entity.SOSHubId = STCPUpdate.SOSHubId ?? entity.SOSHubId;

            _context.SOSSynopticTableofControlPoints.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        #endregion

        public async Task<int> RemoveSOSSynopticTableofControlPoints(int SOS_SynopticTableofControlPoints_id)
        {
            var entity = await _context.SOSSynopticTableofControlPoints
                .FirstOrDefaultAsync(e => e.SOSSynopticTableofControlPointsId == SOS_SynopticTableofControlPoints_id);
            if (entity == null) return 0;
            entity.IsActive = false;
            _context.SOSSynopticTableofControlPoints.Update(entity);
            return await _context.SaveChangesAsync();
        }

        #region Add To Sos SynopticTableofControlPoints
        public async Task<AsyncVoidMethodBuilder> AddSOSSynopticPointsLogbookToSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints Master, SOSSynopticPointsLogbook Slave)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SOSSynopticPointsLogbook
        public async Task<int> CreateSOSSynopticPointsLogbook(SOSSynopticPointsLogbook LogBook_ToCreate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
