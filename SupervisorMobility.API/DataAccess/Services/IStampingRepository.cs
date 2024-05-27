using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface IStampingRepository
    {

        #region DataPanel
        public Task<int> AddDataPanel(DataPanel dataPanelForCreate);
        public Task<IEnumerable<DataPanel>> getAllDataPanels(bool includeSpecifications = false);
        public Task<DataPanel?> getDataPanel(int DataPanel_id, bool includeSpecifications = false);

        public Task<int?> removeDataPanel(DataPanel entityDataPanel);
        #endregion 
        
        #region DataPanelSpecification
        public Task<IEnumerable<DataPanelSpecification>> getAllDataPanelSpecificationFromDataPanel(int DataPanel_id, bool includeDataPanel = false);
        public Task<DataPanelSpecification?> getDataPanelSpecification(int DataPanelSpecification_id, bool includeDataPanel = false);
        #endregion


    }
}
