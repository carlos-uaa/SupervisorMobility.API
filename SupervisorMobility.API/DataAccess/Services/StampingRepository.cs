using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.KaizenDtos;
using SupervisorMobility.API.Services;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class StampingRepository : IStampingRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        public StampingRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region DataPanel
        public async Task<int> AddDataPanel(DataPanel dataPanelForCreate)
        {
            _context.DataPanels.Add(dataPanelForCreate);
            return _context.SaveChanges();
        }

        public async Task<IEnumerable<DataPanel>> getAllDataPanels(bool includeSpecifications = false)
        {
            var query = _context.DataPanels.Where(u => u.IsActive == true);

            if (includeSpecifications)
            {
                query = query.Include(dp => dp.Specifications);
            }

            return await query.OrderBy(c => c.DataPanelId).ToListAsync();
        }

        public async Task<DataPanel?> getDataPanel(int DataPanel_id, bool includeSpecifications = false)
        {
            var query = _context.DataPanels.Where(u => u.DataPanelId == DataPanel_id && u.IsActive == true);

            if (includeSpecifications)
            {
                query = query.Include(dp => dp.Specifications);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> UpdateDataPanel(DataPanelForUpdateDto _DataPanelForUpdate, DataPanel _DataPanelEntity)
        {
            _mapper.Map(_DataPanelForUpdate, _DataPanelEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<AsyncVoidMethodBuilder> AddRangeDataPanelSpecifications(List<DataPanelSpecification> dataPanelSpecifications)
        {
            _context.DataPanelSpecifications.AddRange(dataPanelSpecifications);
            _context.SaveChanges();

            return new AsyncVoidMethodBuilder();
        }

        public async Task<int?> removeDataPanel(DataPanel entityDataPanel)
        {

            if (entityDataPanel != null)
            {
                entityDataPanel.IsActive = false;
            }
            else
            {
                return 0;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DataPanelMaxItemOrderAsync()
        {
            if (await _context.ProblemDefects.AnyAsync())
            {
                return await _context.DataPanels.MaxAsync(cc => cc.ItemOrder) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task<int> DataPanelSpecificationMaxItemOrderAsync(int dp_id)
        {
            if (await _context.ProblemDefects.AnyAsync())
            {
                return await _context.DataPanelSpecifications.Where(dp => dp.DataPanelId == dp_id).MaxAsync(cc => cc.ItemOrder) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task<int> UpdateDataPanelsSequenceAsync(DataPanelForUpdateSequenceDto newDataPanelSequence, DataPanel DataPanelEntity)
        {
            //So we need to update the checklist categories ItemOrder between desiered and old one.
            var currentItemOrder =
                newDataPanelSequence.ItemOrder < DataPanelEntity.ItemOrder
                ? newDataPanelSequence.ItemOrder
                : DataPanelEntity.ItemOrder - 1;

            var checklistCategoryEntities = await GetDataPanelForUpdateSequenceAsync(
                       newDataPanelSequence.ItemOrder,
                       DataPanelEntity.ItemOrder,
                       DataPanelEntity.DataPanelId);

            foreach (var DataPanelEntityForUpdate in checklistCategoryEntities)
            {
                currentItemOrder += 1;
                DataPanelEntityForUpdate.ItemOrder = currentItemOrder;
            }

            _mapper.Map(newDataPanelSequence, DataPanelEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DataPanel>> GetDataPanelForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.DataPanels
                        .Where(c => c.ItemOrder >= lowerValue
                            && c.ItemOrder <= upperValue
                            && c.DataPanelId != categoryId
                            && c.IsActive == true)
                        .OrderBy(c => c.ItemOrder).ToListAsync();
        }

        #endregion

        #region DataPanelSpecification
        public async Task<int> AddDataPanelSpecification(DataPanel dataPanel, DataPanelSpecification specforCreate)
        {
            _context.DataPanelSpecifications.Add(specforCreate);

            dataPanel.Specifications?.Add(specforCreate);

            return _context.SaveChanges();
        }
        public async Task<IEnumerable<DataPanelSpecification>> getAllDataPanelSpecificationFromDataPanel(int DataPanel_id)
        {
            var query = _context.DataPanelSpecifications.Where(dps => dps.DataPanelId == DataPanel_id && dps.IsActive == true);

            return await query.OrderBy(c => c.DataPanelSpecificationId).ToListAsync();
        }

        public async Task<DataPanelSpecification?> getDataPanelSpecification(int DataPanelSpecification_id)
        {
            var query = _context.DataPanelSpecifications.Where(dps => dps.DataPanelSpecificationId == DataPanelSpecification_id && dps.IsActive == true);

            return await query.FirstOrDefaultAsync(); ;
        }

        public async Task<int> UpdateDataPanelSpecificationSequenceAsync(DataPanelSpecificationForUpdateSequenceDto newDataPanelSequence, DataPanelSpecification DataPanelEntity)
        {
            //So we need to update the checklist categories ItemOrder between desiered and old one.
            var currentItemOrder =
                newDataPanelSequence.ItemOrder < DataPanelEntity.ItemOrder
                ? newDataPanelSequence.ItemOrder
                : DataPanelEntity.ItemOrder - 1;

            var checklistCategoryEntities = await GetDataPanelSpecificationForUpdateSequenceAsync(
                       newDataPanelSequence.ItemOrder,
                       DataPanelEntity.ItemOrder,
                       (int)DataPanelEntity.DataPanelSpecificationId,
                       (int)DataPanelEntity.DataPanelId);

            foreach (var DataPanelEntityForUpdate in checklistCategoryEntities)
            {
                currentItemOrder += 1;
                DataPanelEntityForUpdate.ItemOrder = currentItemOrder;
            }

            _mapper.Map(newDataPanelSequence, DataPanelEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DataPanelSpecification>> GetDataPanelSpecificationForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId, int panelid)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.DataPanelSpecifications
                        .Where(c => c.ItemOrder >= lowerValue
                            && c.ItemOrder <= upperValue
                            && c.DataPanelSpecificationId != categoryId
                            && c.DataPanelId == panelid
                            && c.IsActive == true)
                        .OrderBy(c => c.ItemOrder).ToListAsync();
        }

        #endregion

        #region Part

        public async Task<int> AddPart(Part partToAdd)
        {
            _context.Parts.Add(partToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Part> GetPart(int part_id, bool includeScketes = false)
        {
            var query = _context.Parts.Where(p => p.PartId == part_id && p.IsActive == true);

            if (includeScketes)
            {
                query = query.Include(pi => pi.Sketches);
            }

            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Part>> GetAllParts(bool includeScketes = false)
        {
            var query = _context.Parts.Where(p => p.IsActive == true);

            if (includeScketes)
            {
                query = query.Include(pi => pi.Sketches);
            }

            return await query.OrderBy(c => c.PartId).ToListAsync();
        }
        public async Task<int> UpdatePart(PartForUpdateDto partForUpdate, Part partentity)
        {
            _mapper.Map(partForUpdate, partentity);

            _context.Parts.Update(partentity);

            return await _context.SaveChangesAsync();

        }
        public async Task<int> DeletePart(Part partentity)
        {
            partentity.IsActive = false;
            _context.Parts.Update(partentity);

            return await _context.SaveChangesAsync();
        }

        public async Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile)
        {
            var finalNewFile = _mapper.Map<FileUpload>(newFile);
            _context.Files.Add(finalNewFile);
            await _context.SaveChangesAsync();
            return finalNewFile;
        }

        public async Task AddPartSketch(int part_id, FileUpload evidence)
        {
            var partentity = await GetPart(part_id, true);

            if (partentity != null)
            {

                if (partentity.Sketches != null)
                {
                    partentity.Sketches.Add(evidence);
                }
                else
                {
                    partentity.Sketches = new List<FileUpload>
                    {
                        evidence
                    };

                }


            }

        }


        #endregion
        #region ProblemDefect
        public async Task<int> ProblemDefectMaxItemOrderAsync()
        {

            if (await _context.ProblemDefects.AnyAsync())
            {
                return await _context.ProblemDefects.MaxAsync(cc => cc.ItemOrder) + 1;
            }
            else
            {
                return 1;
            }
        }
        public async Task<int> AddProblemDefect(ProblemDefect ProblemDefectToAdd)
        {
            _context.ProblemDefects.Add(ProblemDefectToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<ProblemDefect> GetProblemDefect(int ProblemDefect_id)
        {
            var query = _context.ProblemDefects.Where(p => p.ProblemDefectId == ProblemDefect_id && p.IsActive == true);

            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<ProblemDefect>> GetAllProblemDefects()
        {
            var query = _context.ProblemDefects.Where(p => p.IsActive == true);


            return await query.OrderBy(c => c.ProblemDefectId).ToListAsync();
        }
        public async Task<int> UpdateProblemDefect(ProblemDefectForUpdateDto ProblemDefectForUpdate, ProblemDefect ProblemDefectentity)
        {
            _mapper.Map(ProblemDefectForUpdate, ProblemDefectentity);

            _context.ProblemDefects.Update(ProblemDefectentity);

            return await _context.SaveChangesAsync();

        }
        public async Task<int> DeleteProblemDefect(ProblemDefect ProblemDefectentity)
        {
            ProblemDefectentity.IsActive = false;
            _context.ProblemDefects.Update(ProblemDefectentity);

            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Checkpoint
        public async Task<int> AddCheckpoint(Checkpoint CheckpointForCreate)
        {
            _context.Checkpoints.Add(CheckpointForCreate);
            return _context.SaveChanges();
        }

        public async Task<IEnumerable<Checkpoint>> getAllCheckpoints(bool includeStandars = false, bool includeSketches = false, bool includeSketchesStandars = false)
        {
            var query = _context.Checkpoints.Where(u => u.IsActive == true);

            if (includeStandars)
            {
                if (includeSketchesStandars)
                {
                    query = query.Include(dp => dp.Standars).ThenInclude(d => d.Sketches);

                }
                else
                {
                    query = query.Include(dp => dp.Standars);
                }
            }

            if (includeSketches)
            {
                query = query.Include(dp => dp.Sketches);
            }

            return await query.OrderBy(c => c.CheckpointId).ToListAsync();
        }

        public async Task<Checkpoint?> getCheckpoint(int Checkpoint_id, bool includeStandars = false, bool includeSketches = false, bool includeSketchesStandars = false)
        {
            var query = _context.Checkpoints.Where(u => u.CheckpointId == Checkpoint_id && u.IsActive == true);

            if (includeStandars)
            {
                if (includeSketchesStandars)
                {
                    query = query.Include(dp => dp.Standars).ThenInclude(d => d.Sketches);

                }
                else
                {
                    query = query.Include(dp => dp.Standars);
                }
            }
            if (includeSketches)
            {
                query = query.Include(dp => dp.Sketches) ;
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCheckpoint(CheckpointForUpdateDto _CheckpointForUpdate, Checkpoint _CheckpointEntity)
        {
            _mapper.Map(_CheckpointForUpdate, _CheckpointEntity);
            _context.Checkpoints.Update(_CheckpointEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<AsyncVoidMethodBuilder> AddRangeCheckpointNorms(List<CheckpointNorm> CheckpointNorms)
        {
            _context.CheckpointsNorm.AddRange(CheckpointNorms);
            _context.SaveChanges();

            return new AsyncVoidMethodBuilder();
        }

        public async Task<int?> removeCheckpoint(Checkpoint entityCheckpoint)
        {
            entityCheckpoint.IsActive = false;
            _context.Checkpoints.Update(entityCheckpoint);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CheckpointMaxItemOrderAsync()
        {
            if (await _context.Checkpoints.AnyAsync())
            {
                return await _context.Checkpoints.MaxAsync(cc => cc.ItemOrder) + 1;
            }
            else
            {
                return 1;
            }
        }


        public async Task AddSketchCheckpoint(int checkpoint_id, FileUpload evidence)
        {
            var partentity = await getCheckpoint(checkpoint_id, includeSketches: true);

            if (partentity != null)
            {

                if (partentity.Sketches != null)
                {
                    partentity.Sketches.Add(evidence);
                }
                else
                {
                    partentity.Sketches = new List<FileUpload>
                    {
                        evidence
                    };

                }


            }

        }

        #endregion
        #region CheckpointNorm
        public async Task<int> AddCheckpointNorm(Checkpoint Checkpoint, CheckpointNorm specforCreate)
        {
            _context.CheckpointsNorm.Add(specforCreate);

            Checkpoint.Standars?.Add(specforCreate);

            return _context.SaveChanges();
        }
        //public async Task<IEnumerable<CheckpointNorm>> getAllCheckpointNormFromCheckpoint(int Checkpoint_id, bool includeSketches = false)
        //{
        //    var query = _context.CheckpointsNorm.Where(dps => dps.CheckpointId == Checkpoint_id && dps.IsActive == true);

        //    if (includeSketches)
        //    {
        //        query = query.Include(dps => dps.Sketches);
        //    }

        //    return await query.OrderBy(c => c.CheckpointNormId).ToListAsync();
        //}

        public async Task<CheckpointNorm?> getCheckpointNorm(int CheckpointNorm_id, bool includeSketches = false)
        {
            var query = _context.CheckpointsNorm.Include(ck => ck.Checkpoint).Where(dps => dps.CheckpointNormId == CheckpointNorm_id && dps.IsActive == true);
            if (includeSketches)
            {
                query = query.Include(dps => dps.Sketches);
            }
            return await query.FirstOrDefaultAsync(); ;
        }

        public async Task<int?> removeCheckpointNorm(CheckpointNorm entityCheckpoint)
        {
            entityCheckpoint.IsActive = false;
            _context.CheckpointsNorm.Update(entityCheckpoint);
            return await _context.SaveChangesAsync();
        }

        //public async Task<int> UpdateCheckpointNormSequenceAsync(CheckpointNormForUpdateSequenceDto newCheckpointSequence, CheckpointNorm CheckpointEntity)
        //{
        //    //So we need to update the checklist categories ItemOrder between desiered and old one.
        //    var currentItemOrder =
        //        newCheckpointSequence.ItemOrder < CheckpointEntity.ItemOrder
        //        ? newCheckpointSequence.ItemOrder
        //        : CheckpointEntity.ItemOrder - 1;

        //    var checklistCategoryEntities = await GetCheckpointNormForUpdateSequenceAsync(
        //               newCheckpointSequence.ItemOrder,
        //               CheckpointEntity.ItemOrder,
        //               (int)CheckpointEntity.CheckpointNormId,
        //               (int)CheckpointEntity.CheckpointId);

        //    foreach (var CheckpointEntityForUpdate in checklistCategoryEntities)
        //    {
        //        currentItemOrder += 1;
        //        CheckpointEntityForUpdate.ItemOrder = currentItemOrder;
        //    }

        //    _mapper.Map(newCheckpointSequence, CheckpointEntity);
        //    return await _context.SaveChangesAsync();
        //}

        //public async Task<IEnumerable<CheckpointNorm>> GetCheckpointNormForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId, int panelid)
        //{
        //    int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
        //    int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

        //    return await _context.CheckpointNorms
        //                .Where(c => c.ItemOrder >= lowerValue
        //                    && c.ItemOrder <= upperValue
        //                    && c.CheckpointNormId != categoryId
        //                    && c.CheckpointId == panelid
        //                    && c.IsActive == true)
        //                .OrderBy(c => c.ItemOrder).ToListAsync();
        //}

        public async Task AddSketchChekpointNorm(int norm_id, FileUpload evidence)
        {
            var partentity = await getCheckpointNorm(norm_id, true);

            if (partentity != null)
            {

                if (partentity.Sketches != null)
                {
                    partentity.Sketches.Add(evidence);
                }
                else
                {
                    partentity.Sketches = new List<FileUpload>
                    {
                        evidence
                    };

                }
            }

        }

        public async Task<int> CheckpointNormMaxItemOrderAsync(int chk_id)
        {

            if (await _context.CheckpointsNorm.Where(cn => cn.CheckpointId == chk_id).AnyAsync())
            {
                return await _context.CheckpointsNorm.Where(cn => cn.CheckpointId == chk_id).MaxAsync(cc => cc.ItemOrder) + 1;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        public async Task<FileUpload?> FetchFileAsync(int fileid)
        {
            return await _context.Files
                .Where(p => p.FileUploadId == fileid).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task RemoveSketchCheckPoint(int CheckpointId, int fileUploadId)
        {
            var CheckPoint = await getCheckpoint(CheckpointId, true, true);

            var Sketch = await FetchFileAsync(fileUploadId);
            if (Sketch != null)
            {
                if (CheckPoint.Sketches != null)
                {
                    //Remove evidence
                    CheckPoint.Sketches.Remove(item: CheckPoint.Sketches.ToList().Find(e => e.FileUploadId == fileUploadId));
                }
            }
        }

        public async Task RemoveSketchCheckPointNorm(int Checkpoint_NormId, int fileUploadId)
        {
            var CheckPointNorm = await getCheckpointNorm(Checkpoint_NormId, true);

            var Sketch = await FetchFileAsync(fileUploadId);
            if (Sketch != null)
            {
                if (CheckPointNorm.Sketches != null)
                {
                    //Remove evidence
                    CheckPointNorm.Sketches.Remove(item: CheckPointNorm.Sketches.ToList().Find(e => e.FileUploadId == fileUploadId));
                }
            }
        }
        public async Task RemoveSketchPart(int part_Id, int fileUploadId)
        {
            var _part = await GetPart(part_Id, true);

            var Sketch = await FetchFileAsync(fileUploadId);
            if (Sketch != null)
            {
                if (_part.Sketches != null)
                {
                    //Remove evidence
                    _part.Sketches.Remove(item: _part.Sketches.ToList().Find(e => e.FileUploadId == fileUploadId));
                }
            }
        }

        #region Appearance

        public async Task<int> AddAppearance(Appearance appearanceToAdd)
        {
            _context.AppearanceInspections.Add(appearanceToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<Appearance> GetAppearance(int appearance_id, bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {
            var query = _context.AppearanceInspections.Where(p => p.AppearanceId == appearance_id && p.IsActive == true);

            if (includeDataPanelItems)
            {
                query = query.Include(a => a.DataPanelItems);
            }
            if (includeProblemDefectItems)
            {
                query = query.Include(a => a.ProblemDefectItems);
            }
            if (includeLogBookAppearance)
            {
                query = query.Include(a => a.LogbooksAppearance);
            }

            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Appearance>> GetAllAppearances(bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {
            var query = _context.AppearanceInspections.Where(p => p.IsActive == true);

            if (includeDataPanelItems)
            {
                query = query.Include(a => a.DataPanelItems);
            }
            if (includeProblemDefectItems)
            {
                query = query.Include(a => a.ProblemDefectItems);
            }
            if (includeLogBookAppearance)
            {
                query = query.Include(a => a.LogbooksAppearance);
            }

            return await query.OrderBy(c => c.AppearanceId).ToListAsync();
        }
        public async Task<int> UpdateAppearance(AppearanceForUpdateDto appearanceForUpdate, Appearance appearanceEntity)
        {
            _mapper.Map(appearanceForUpdate, appearanceEntity);

            _context.AppearanceInspections.Update(appearanceEntity);

            return await _context.SaveChangesAsync();

        }
        public async Task<int> DeleteAppearance(Appearance appearanceEntity)
        {
            appearanceEntity.IsActive = false;
            _context.AppearanceInspections.Update(appearanceEntity);

            return await _context.SaveChangesAsync();
        }

        #endregion

        #region LogbookAppearance

        public async Task<int> AddLogbookAppearance(LogbookAppearance logbookAppearanceToAdd)
        {
            _context.LogbookAppearance.Add(logbookAppearanceToAdd);
            return await _context.SaveChangesAsync();
        }
        public async Task<LogbookAppearance> GetLogbookAppearance(int logbookAppearance_id, bool includePanelResults = false, bool includeProblemDefectResults = false)
        {
            var query = _context.LogbookAppearance.Where(p => p.LogbookAppearanceId == logbookAppearance_id && p.IsActive == true);

            if (includePanelResults)
            {
                query = query.Include(l => l.PanelResults);
            }
            if (includeProblemDefectResults)
            {
                query = query.Include(l => l.ProblemDefectResults);
            }
            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<LogbookAppearance>> GetAllLogbookAppearances(bool includePanelResults = false, bool includeProblemDefectResults = false)
        {
            var query = _context.LogbookAppearance.Where(p => p.IsActive == true);

            if (includePanelResults)
            {
                query = query.Include(l => l.PanelResults);
            }
            if (includeProblemDefectResults)
            {
                query = query.Include(l => l.ProblemDefectResults);
            }

            return await query.OrderBy(c => c.LogbookAppearanceId).ToListAsync();
        }
        public async Task<int> UpdateLogbookAppearance(LogbookAppearanceForUpdateDto logbookAppearanceForUpdate, LogbookAppearance logbookAppearanceEntity)
        {
            _mapper.Map(logbookAppearanceForUpdate, logbookAppearanceEntity);

            _context.LogbookAppearance.Update(logbookAppearanceEntity);

            return await _context.SaveChangesAsync();

        }
        public async Task<int> DeleteLogbookAppearance(LogbookAppearance logbookAppearanceEntity)
        {
            logbookAppearanceEntity.IsActive = false;
            _context.LogbookAppearance.Update(logbookAppearanceEntity);

            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}
