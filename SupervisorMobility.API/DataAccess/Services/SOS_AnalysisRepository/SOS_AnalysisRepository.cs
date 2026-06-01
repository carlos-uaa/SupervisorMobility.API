using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using System.Diagnostics;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using System.Runtime.CompilerServices;
using AutoMapper;
using SupervisorMobility.API.Context;

namespace SupervisorMobility.API.DataAccess.Services.SOS_AnalysisRepository
{
    public class SOS_AnalysisRepository : ISOS_AnalysisRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public SOS_AnalysisRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

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

            var query = _context.SOSAnalyses.AsNoTracking()
                  .Where(analysis => analysis.SOSHub.DistributionId == Distribution_Id && analysis.IsActive == true);

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


            return sosAnalyses;
        }

        //Get SOS Analysis by Areas
        public async Task<IEnumerable<SOSAnalysis>> GetAllSOSAnalysisByArea(int area, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            try
            {
                var query = _context.SOSAnalyses.AsNoTracking()
                  .Where(analysis => area == (int)analysis.SOSHub.AreaId && analysis.IsActive == true);

                if (includeImages) query = query.Include(i => i.Illustrations);
                if (includeNotes)  query = query.Include(query => query.Notes);
                if (includeLogbooks) query = query.Include(t => t.AnalysisLogbooks);
                if (includeSOS) query = query.Include(m => m.SOSHub).ThenInclude(ms => ms.Sections).ThenInclude(msa => msa.Analyses);

                var sosAnalyses = await query.OrderBy(s => s.SOSHubId).ToListAsync();
                return sosAnalyses;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while retrieving SOS Analyses by Areas: " + ex.Message);
                return new List<SOSAnalysis>();
            }
        }

        public async Task<int> UpdateSOSAnalysis(SOSAnalysisForUpdateDto AnalysisUpdate, SOSAnalysis AnalysisEntity)
        {
            try
            {
                SOSHub? SOShub = AnalysisUpdate.SOSHub;
                if (AnalysisUpdate.SOSHubId == null)
                {
                    throw new Exception("SOSHubId is required");
                }

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
    }
}
