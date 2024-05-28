using Microsoft.AspNetCore.DataProtection;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface IStampingRepository
    {

        #region DataPanel
         Task<int> AddDataPanel(DataPanel dataPanelForCreate);
         Task<IEnumerable<DataPanel>> getAllDataPanels(bool includeSpecifications = false);
         Task<DataPanel?> getDataPanel(int DataPanel_id, bool includeSpecifications = false);

         Task<int?> removeDataPanel(DataPanel entityDataPanel);

         Task<int> DataPanelMaxItemOrderAsync();
        Task<int> UpdateDataPanelsSequenceAsync(DataPanelForUpdateSequenceDto newDataPanelSequence, DataPanel DataPanelEntity);
        Task<IEnumerable<DataPanel>> GetDataPanelForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId);
        #endregion

        #region DataPanelSpecification
        public Task<IEnumerable<DataPanelSpecification>> getAllDataPanelSpecificationFromDataPanel(int DataPanel_id, bool includeDataPanel = false);
        public Task<DataPanelSpecification?> getDataPanelSpecification(int DataPanelSpecification_id, bool includeDataPanel = false);
        #endregion


    }
}
