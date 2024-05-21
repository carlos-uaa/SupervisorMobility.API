using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Business
{
    public interface IJobObservationService
    {
        #region Category
        Task<IEnumerable<JobCategoryStructure>> FetchChecklistCategoriesAsync(bool includeChecklistQuestions = false);
        Task<IEnumerable<JobCategoryStructure>> FetchAllChecklistCategoriesAsync(bool includeChecklistQuestions = false);
        Task<JobCategoryStructure?> FetchChecklistCategoryAsync(int categoryId, bool includeChecklistQuestions = false);
        Task<JobCategoryStructure> CreateChecklistCategoryAsync(JobCategoryStructureForCreationDto checklistCategory);
        Task<int> FetchChecklistCategoriesMaxSequenceAsync();
        Task UpdateChecklistCategoryAsync(JobCategoryStructureForUpdateDto checklistCategoryUpdate, JobCategoryStructure checklistCategory);
        Task UpdateChecklistCategoriesSequenceAsync(
            JobCategoryStructureSequenceForUpdateDto newChecklistCategorySequence,
            JobCategoryStructure checklistCategory);
        Task DeleteChecklistCategoryAsync(JobCategoryStructure checklistCategory);
        Task<bool> CheckChecklistCategoryExistAsync(int categoryId);

        Task<IEnumerable<Lup>> GetChecklistQuestionInsidences(int questionId, int sv_id);
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
       
    }
}
