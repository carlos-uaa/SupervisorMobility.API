using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Business
{
    public interface IChecklistCategoryService
    {
        Task<ChecklistCategory?> FetchChecklistCategoryAsync(int categoryId);
        Task<int> FetchChecklistCategoriesMaxSequenceAsync();
        Task UpdateChecklistCategoriesSequenceAsync(
            ChecklistCategorySequenceForUpdateDto newChecklistCategorySequence,
            ChecklistCategory checklistCategoryEntity);
        Task DeleteChecklistCategoryAsync(ChecklistCategory checklistCategory);
        Task<bool> CheckChecklistCategoryExistAsync(int categoryId);
        Task<ChecklistQuestion?> FetchChecklistQuestionForCategoryAsync(int categoryId, int questionId);
        Task<int> FetchChecklistQuestionMaxSequenceAsync(int cateogryId);
        Task UpdateChecklistQuestionsSequenceAsync(
            ChecklistQuestionSequenceForUpdateDto newChecklistQuestionSequence,
            ChecklistQuestion checklistQuestionEntity,
            int categoryId);
        Task DeleteChecklistQuestionAsync(ChecklistQuestion checklistQuestion);
    }
}
