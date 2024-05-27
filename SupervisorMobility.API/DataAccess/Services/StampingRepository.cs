using AutoMapper;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Services;

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

        #endregion

        #region DataPanelSpecification
        public async Task<IEnumerable<DataPanelSpecification>> getAllDataPanelSpecificationFromDataPanel(int DataPanel_id, bool includeDataPanel = false)
        {
            var query = _context.DataPanelSpecifications.Where(dps => dps.DataPanelId == DataPanel_id && dps.IsActive == true);

            if (includeDataPanel)
            {
                query = query.Include(dp => dp.DataPanel);
            }

            return await query.OrderBy(c => c.DataPanelSpecificationId).ToListAsync();
        }

        public async Task<DataPanelSpecification?> getDataPanelSpecification(int DataPanelSpecification_id, bool includeDataPanel = false)
        {
            var query = _context.DataPanelSpecifications.Where(dps => dps.DataPanelSpecificationId == DataPanelSpecification_id && dps.IsActive == true);

            if (includeDataPanel)
            {
                query = query.Include(dp => dp.DataPanel);
            }

            return await query.FirstOrDefaultAsync(); ;
        }
        #endregion
    }
}
