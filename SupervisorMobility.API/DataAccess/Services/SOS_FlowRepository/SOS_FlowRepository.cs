using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services.SOS_FlowRepository
{
    public class SOS_FlowRepository : ISOS_FlowRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;


        public SOS_FlowRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Flow
        #region SOSFlow
        public async Task<int> CreateSOSFlow(SOSFlow SOS_FlowToCreate)
        {
            _context.SOSFlows.Add(SOS_FlowToCreate);
            return _context.SaveChanges();
        }

        public async Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includePeople = false)
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
            if (includePeople)
            {
                query = query.Include(p => p.Approver);
                query = query.Include(p => p.ReviewerHS);
            }

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
                query = query.Include(m => m.SOSHub).ThenInclude(a => a.Area);
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
    }
}
