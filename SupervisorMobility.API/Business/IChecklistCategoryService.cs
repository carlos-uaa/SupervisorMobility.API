using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;

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
    }
}
