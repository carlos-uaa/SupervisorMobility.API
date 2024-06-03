using Microsoft.AspNetCore.DataProtection;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface IStampingRepository
    {

        #region DataPanel
        Task<int> AddDataPanel(DataPanel dataPanelForCreate);
        Task<IEnumerable<DataPanel>> getAllDataPanels(bool includeSpecifications = false);
        Task<DataPanel?> getDataPanel(int DataPanel_id, bool includeSpecifications = false);

        Task<int> UpdateDataPanel(DataPanelForUpdateDto _DataPanelForUpdate, DataPanel _DataPanelEntity);

        Task<AsyncVoidMethodBuilder> AddRangeDataPanelSpecifications(List<DataPanelSpecification> dataPanelSpecifications);
        Task<int?> removeDataPanel(DataPanel entityDataPanel);

        Task<int> DataPanelMaxItemOrderAsync();
        Task<int> DataPanelSpecificationMaxItemOrderAsync(int dp_id);
        Task<int> UpdateDataPanelsSequenceAsync(DataPanelForUpdateSequenceDto newDataPanelSequence, DataPanel DataPanelEntity);
        Task<IEnumerable<DataPanel>> GetDataPanelForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId);
        #endregion

        #region DataPanelSpecification
        Task<int> AddDataPanelSpecification(DataPanel dataPanel, DataPanelSpecification specforCreate);

        public Task<IEnumerable<DataPanelSpecification>> getAllDataPanelSpecificationFromDataPanel(int DataPanel_id);
        public Task<DataPanelSpecification?> getDataPanelSpecification(int DataPanelSpecification_id);
        Task<int> UpdateDataPanelSpecificationSequenceAsync(DataPanelSpecificationForUpdateSequenceDto newDataPanelSequence, DataPanelSpecification DataPanelEntity);
        Task<IEnumerable<DataPanelSpecification>> GetDataPanelSpecificationForUpdateSequenceAsync(int currentSequence, int oldSequence, int categoryId, int panelid);
        #endregion

        #region Part

        Task<int> AddPart(Part partToAdd);
        Task<Part> GetPart(int part_id, bool includeScketes = false);
        Task<IEnumerable<Part>> GetAllParts(bool includeScketes = false);
        Task<int> UpdatePart(PartForUpdateDto partForUpdate, Part partentity);
        Task<int> DeletePart(Part partentity);
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task AddPartSketch(int part_id, FileUpload evidence);
        #endregion

        #region ProblemDefect
        Task<int> ProblemDefectMaxItemOrderAsync();

        Task<int> AddProblemDefect(ProblemDefect ProblemDefectToAdd);
        Task<ProblemDefect> GetProblemDefect(int ProblemDefect_id);
        Task<IEnumerable<ProblemDefect>> GetAllProblemDefects();
        Task<int> UpdateProblemDefect(ProblemDefectForUpdateDto ProblemDefectForUpdate, ProblemDefect ProblemDefectentity);
        Task<int> DeleteProblemDefect(ProblemDefect ProblemDefectentity);

        #endregion

        Task<FileUpload?> FetchFileAsync(int fileid);
       
        Task<bool> SaveChangesAsync();
    }
}
