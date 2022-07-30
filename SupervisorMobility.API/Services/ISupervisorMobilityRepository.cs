using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Services
{
    public interface ISupervisorMobilityRepository
    {
        #region ChecklistCategoryOperations

        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync();
        Task<ChecklistCategory?> GetChecklistCategoryAsync(int categoryId, bool includeChecklistQuestion = false);
        Task<bool> ChecklistCategoryExistAsync(int cityId);
        void AddChecklistCategory(ChecklistCategory checklistCategory);
        void DeleteChecklistCategory(ChecklistCategory checklistCategory);
        #endregion

        #region QuestionTypeOperations
        Task<IEnumerable<QuestionType>> GetQuestionTypesAsync();
        Task<QuestionType?> GetQuestionTypeAsync(int id, bool includeChecklistQuestions = false);
        #endregion

        #region ChecklistQuestionOperations

        Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForCategoryAsync(int categoryId);
        Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int checklistQuestion);
        Task AddChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestion checklistQuestion);
        void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion);
        #endregion

        #region CommonOperations

        Task<bool> SaveChangesAsync();

        #endregion
    }
}
