using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.SOS_Combination;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutoMapper;
using SupervisorMobility.API.Context;

namespace SupervisorMobility.API.DataAccess.Services.SOS_CombinationRepository
{
    public class SOS_CombinationRepository : ISOS_CombinationRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public SOS_CombinationRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Combination
        #region SOSCombination
        public async Task<int> CreateSOSCombination(SOSCombination SOS_CombinationToCreate)
        {
            _context.SOSCombinations.Add(SOS_CombinationToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeProcess = false)
        {
            // Consulta inicial para encontrar la combinación
            var sosCombination = await _context.SOSCombinations.AsNoTracking()
                .Where(c => c.SOSCombinationId == SOSCombinationId && c.IsActive == true)
                .FirstOrDefaultAsync();


            // Verificar si sosCombination es nulo antes de cargar relaciones
            if (sosCombination != null)
            {
                // Incluir imágenes relacionadas
                if (includeImages)
                {
                    await _context.Entry(sosCombination).Collection(c => c.Illustrations).LoadAsync();
                    sosCombination.Illustrations = sosCombination.Illustrations.Where(i => i.IsActive == true).ToList();
                }

                // Incluir notas relacionadas (descomentado si es necesario en el futuro)
                //if (includeNotes)
                //{
                //    await _context.Entry(sosCombination).Collection(c => c.Notes).LoadAsync();
                //    sosCombination.Notes = sosCombination.Notes.Where(n => n.IsActive == true).ToList();
                //}

                // Incluir registros (logbooks) y sus referencias
                if (includeLogbooks)
                {
                    await _context.Entry(sosCombination).Collection(c => c.CombinationLogbooks).LoadAsync();
                    foreach (var logbook in sosCombination.CombinationLogbooks)
                    {
                        await _context.Entry(logbook).Reference(l => l.Approver).LoadAsync();
                        await _context.Entry(logbook).Reference(l => l.Reviewer).LoadAsync();
                    }

                    await _context.Entry(sosCombination).Reference(c => c.ReviewerHS).LoadAsync();
                }

                if (includeSOS)
                {
                    await _context.Entry(sosCombination).Reference(d => d.SOSHub).LoadAsync();

                    if (sosCombination.SOSHub != null)
                    {
                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.Sections).LoadAsync();

                        foreach (var section in sosCombination.SOSHub.Sections)
                        {
                            await _context.Entry(section).Collection(s => s.Analyses).LoadAsync();
                        }

                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.AppliedModels).LoadAsync();
                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.ToolsUsed).LoadAsync();
                        foreach (var toolUsed in sosCombination.SOSHub.ToolsUsed)
                        {
                            await _context.Entry(toolUsed).Reference(t => t.Tool).LoadAsync();
                        }

                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.MaterialsUsed).LoadAsync();
                        foreach (var materialUsed in sosCombination.SOSHub.MaterialsUsed)
                        {
                            await _context.Entry(materialUsed).Reference(m => m.Material).LoadAsync();
                        }

                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.SafetyEquipment).LoadAsync();
                        await _context.Entry(sosCombination.SOSHub).Reference(s => s.Plant).LoadAsync();
                        await _context.Entry(sosCombination.SOSHub).Reference(s => s.Department).LoadAsync();
                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.ApproverOwners).LoadAsync();
                        await _context.Entry(sosCombination.SOSHub).Collection(s => s.ReviewerEditors).LoadAsync();
                    }
                }

                // Incluir imágenes específicas del SOS
                if (includeImagesSOS)
                {
                    var sosHub = sosCombination.SOSHub;
                    if (sosHub != null)
                    {
                        await _context.Entry(sosHub).Collection(s => s.Images).LoadAsync();
                    }
                }

                // Incluir procesos relacionados
                if (includeProcess)
                {
                    await _context.Entry(sosCombination).Collection(c => c.SOSCombinationOperationSequence).LoadAsync();
                }
            }

            return sosCombination;
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
        public async Task<List<SOSCombinationOperationSequence>> AddRangeSOSCombinationOperationSequences(List<SOSCombinationOperationSequence> SOSOperationSequencesToAdd)
        {
            _context.SOSCombinationOperationSequences.AddRange(SOSOperationSequencesToAdd);
            await _context.SaveChangesAsync();

            // Desvincular las nuevas combinationlogbook del contexto
            foreach (var OperationSequences in SOSOperationSequencesToAdd)
            {
                _context.Entry(OperationSequences).State = EntityState.Detached;
            }

            return SOSOperationSequencesToAdd;
        }

        public async Task<AsyncVoidMethodBuilder> RemoveAllOperationsSequenceFromSOSCombination(SOSCombination Master)
        {
            Master.SOSCombinationOperationSequence?.Clear();
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
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

        public async Task<AsyncVoidMethodBuilder> AddOperationSequenceToSOSCombination(SOSCombination master, SOSCombinationOperationSequence slave)
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
                var localSlaveEntry = _context.SOSCombinationOperationSequences.Local.FirstOrDefault(entry => entry.SOSCombinationOperationSequenceId == slave.SOSCombinationOperationSequenceId);
                if (localSlaveEntry != null)
                {
                    slave = localSlaveEntry;
                }
                else
                {
                    if (_context.Entry(slave).State == EntityState.Detached)
                    {
                        _context.SOSCombinationOperationSequences.Attach(slave);
                    }
                }

                // Añadir el comentario a la colección de ProcessSheetCommentary del master
                if (master.SOSCombinationOperationSequence == null)
                {
                    master.SOSCombinationOperationSequence = new List<SOSCombinationOperationSequence>();
                }

                // Verificar si el comentario ya está en la colección
                if (!master.SOSCombinationOperationSequence.Any(c => c.SOSCombinationOperationSequenceId == slave.SOSCombinationOperationSequenceId))
                {
                    master.SOSCombinationOperationSequence.Add(slave);
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
        #region SOSCombinationOperationSequences
        public async Task<SOSCombinationOperationSequence> GetSOSCombinationOperationSequencesById(int id)
        {
            return await _context.SOSCombinationOperationSequences.AsNoTracking().Where(t => t.SOSCombinationOperationSequenceId == id).FirstOrDefaultAsync();

        }
        public async Task<int> UpdateSOSCombinationOperationSequences(SOSCombinationOperationSequenceForUpdateDto OperationSequenceForUpdate)
        {
            try
            {
                var query = _context.SOSCombinationOperationSequences.Where(t => t.SOSCombinationOperationSequenceId == OperationSequenceForUpdate.SOSCombinationOperationSequenceId);

                SOSCombinationOperationSequence operationSequence = await query.FirstOrDefaultAsync();

                if (operationSequence == null)
                {
                    throw new InvalidOperationException("operationSequence not found or is not active.");
                }

                var localEntry = _context.SOSCombinationOperationSequences.Local.FirstOrDefault(entry => entry.SOSCombinationOperationSequenceId == OperationSequenceForUpdate.SOSCombinationOperationSequenceId);
                if (localEntry != null)
                {
                    _context.Entry(localEntry).CurrentValues.SetValues(OperationSequenceForUpdate);
                }
                else
                {
                    if (_context.Entry(operationSequence).State == EntityState.Detached)
                    {
                        _context.SOSCombinationOperationSequences.Attach(operationSequence);
                    }

                    _mapper.Map(OperationSequenceForUpdate, operationSequence);
                    _context.SOSCombinationOperationSequences.Update(operationSequence);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while updating the operationSequence: " + ex.Message);
                return 0;
            }
        }
        #endregion
    }
}
