using AutoMapper;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.KaizenDtos;
using SupervisorMobility.API.Services;
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
          
            if (entityDataPanel != null) { 
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
            return await _context.DataPanels.MaxAsync(cc => cc.ItemOrder) + 1;
        }

        public async Task<int> DataPanelSpecificationMaxItemOrderAsync()
        {
            return await _context.DataPanelSpecifications.MaxAsync(cc => cc.ItemOrder) + 1;
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
        #endregion
    }
}
