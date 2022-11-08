using Microsoft.EntityFrameworkCore.ChangeTracking;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Services
{
    public interface ISupervisorMobilityRepository
    {
        #region ChecklistCategoryOperations

        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync();
        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesForUpdateSequenceAsync(
            int currentSequence, int oldSequence, int categoryId);
        Task<ChecklistCategory?> GetChecklistCategoryAsync(
            int categoryId, bool includeChecklistQuestion = false);
        Task<bool> ChecklistCategoryExistAsync(int categoryId);
        Task<int> GetChecklistCategoriesMaxSequenceAsync();
        void AddChecklistCategory(ChecklistCategory checklistCategory);
        void DeleteChecklistCategory(ChecklistCategory checklistCategory);
        #endregion
        #region JobObservationTypesOperations
        Task<IEnumerable<JobObservationType>> GetJobObservationTypesAsync();
        Task<JobObservationType?> GetJobObservationTypeAsync(int id, bool includeConfigs = false);
        void AddJobObservationType (JobObservationType jobObservationType);
        void DeleteJobObservationType (JobObservationType jobObservationType);
        Task<bool> JobObservationTypeExistAsync(int jobObservationTypeId);
        #endregion
        #region GroupOperations
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<Group?> GetGroupAsync(int groupId);
        void AddGroup(Group group);
        void DeleteGroup(Group group);
        #endregion
        #region PlantOperations
        Task<IEnumerable<Plant>> GetPlantsAsync();
        Task<Plant?> GetPlantAsync(int plantId, bool includeAreas = false);
        Task<bool> PlantExistAsync(int plantId);
        void AddPlant(Plant plant);
        void DeletePlant(Plant plant);
        #endregion
        #region AreaOperations
        Task<IEnumerable<Area>> GetAreasForPlantAsync(int plantId);
        Task<Area?> GetAreaForPlantAsync(int plantId,
            int areaId, bool includeOperations = false);
        Task<bool> AreaExistAsync(int areaId);
        Task AddAreaForPlantAsync(int plantId, Area area);
        void DeleteArea(Area area);
        #endregion
        #region OperationOperations
        Task<IEnumerable<Operation>> GetOperationsForAreaAsync(int areaId);
        Task<Operation?> GetOperationForAreaAsync(int areaId, int operationId);
        Task AddOperationForPlantAsync(int plantId, int areaId, Operation operation);
        void DeleteOperation (Operation operation);
        #endregion
        #region QuestionTypeOperations
        Task<IEnumerable<QuestionType>> GetQuestionTypesAsync();
        Task<QuestionType?> GetQuestionTypeAsync(int id, bool includeChecklistQuestions = false);
        #endregion
        #region ChecklistQuestionOperations

        Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForCategoryAsync(int categoryId);
        Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int checklistQuestionId);
        Task AddChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestion checklistQuestion);
        Task<int> GetChecklistQuestionMaxCategorySequenceAsync(int categoryId);
        Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForUpdateSequenceAsync(
                int currentSequence, int oldSequence, int categoryId, int checklistQuestionId);
        void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion);
        #endregion
        #region JobObservationConfigOperations
        Task<IEnumerable<JobObservationConfig>> GetJobOperationConfigsForJobOperationTypeAsync(int jobObservationTypeId);
        Task<JobObservationConfig?> GetJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId,
            int jobObservationConfigId);
        Task AddJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId, JobObservationConfig jobObservationConfig);
        void DeleteJobOperationConfig(JobObservationConfig jobObservationConfig);
        #endregion
        #region SupportDocumentTypeOperations
        Task<IEnumerable<SupportDocumentType>> GetSupportDocumentTypesAsync();
        Task<SupportDocumentType?> GetSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<bool> SupportDocumentTypeExistAsync(int supportDocumentTypeId);
        void AddSupportDocumentType(SupportDocumentType supportDocumentType);
        void DeleteSupportDocumentType(SupportDocumentType supportDocumentType);
        #endregion
        #region CommonOperations

        Task<bool> SaveChangesAsync();

        #endregion
    }
}
