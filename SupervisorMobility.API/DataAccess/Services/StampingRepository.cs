using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;
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

            return await query.FirstOrDefaultAsync(); ;
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

        public async Task<FileUpload?> FetchFileAsync(int fileid)
        {

            return await _context.Files
                .Where(p => p.FileUploadId == fileid).FirstOrDefaultAsync();
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

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
