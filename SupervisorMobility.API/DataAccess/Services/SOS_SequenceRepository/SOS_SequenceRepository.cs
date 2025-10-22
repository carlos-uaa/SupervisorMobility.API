using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SupervisorMobility.API.Context;
using AutoMapper;

namespace SupervisorMobility.API.DataAccess.Services.SOS_SequenceRepository
{
    public class SOS_SequenceRepository : ISOS_SequenceRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        public SOS_SequenceRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

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
            var query = _context.SOSSequences.AsNoTracking()
                   .Where(s => s.SOSHub.DistributionId == Distribution_Id && s.IsActive == true);

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
    }
}
