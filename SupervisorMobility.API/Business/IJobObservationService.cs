using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Models.JobObservationTypeDtos;

namespace SupervisorMobility.API.Business
{
    public interface IJobObservationService
    {
        #region Category
        Task<IEnumerable<ChecklistCategory>> FetchChecklistCategoriesAsync();
        Task<ChecklistCategory?> FetchChecklistCategoryAsync(int categoryId, bool includeChecklistQuestions = false);
        Task<ChecklistCategory> CreateChecklistCategoryAsync(ChecklistCategoryForCreationDto checklistCategory);
        Task<int> FetchChecklistCategoriesMaxSequenceAsync();
        Task UpdateChecklistCategoryAsync(ChecklistCategoryForUpdateDto checklistCategoryUpdate, ChecklistCategory checklistCategory);
        Task UpdateChecklistCategoriesSequenceAsync(
            ChecklistCategorySequenceForUpdateDto newChecklistCategorySequence,
            ChecklistCategory checklistCategory);
        Task DeleteChecklistCategoryAsync(ChecklistCategory checklistCategory);
        Task<bool> CheckChecklistCategoryExistAsync(int categoryId);
        #endregion
        #region Question
        Task<IEnumerable<ChecklistQuestion>> FetchChecklistQuestionsForCategoryAsync(int categoryId);
        Task<ChecklistQuestion?> FetchChecklistQuestionForCategoryAsync(int categoryId, int questionId);
        Task<int> FetchChecklistQuestionMaxSequenceAsync(int cateogryId);
        Task<ChecklistQuestion> CreateChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestionForCreationDto checklistQuestion);
        Task UpdateChecklistQuestionForCategoryAsync(ChecklistQuestionForUpdateDto checklistQuestionForUpdate, ChecklistQuestion checklistQuestion);
        Task UpdateChecklistQuestionsSequenceAsync(
            ChecklistQuestionSequenceForUpdateDto newChecklistQuestionSequence,
            ChecklistQuestion checklistQuestionEntity,
            int categoryId);
        Task DeleteChecklistQuestionAsync(ChecklistQuestion checklistQuestion);
        #endregion
        #region JobObservation
        Task<IEnumerable<JobObservationType>> FetchJobObservationTypesAsync();
        Task<JobObservationType?> FetchJobObservationTypeAsync(int jobObservationTypeId, bool includeConfigs = false);
        Task<JobObservationType> CreateJobObservationTypeAsync(JobObservationTypeForCreationDto jobObservationType);
        Task UpdateJobObservationTypeAsync(JobObservationTypeForUpdateDto jobObservationTypeUpdate, JobObservationType jobObservationType);
        Task DeleteJobObservationTypeAsync(JobObservationType jobObservationType);
        #endregion
    }
}
