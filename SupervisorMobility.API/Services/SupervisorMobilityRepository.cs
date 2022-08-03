using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Services
{
    public class SupervisorMobilityRepository : ISupervisorMobilityRepository
    {
        private readonly SupervisorMobilityContext _context;

        public SupervisorMobilityRepository(SupervisorMobilityContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region ChecklistCategoryOperations

        public void AddChecklistCategory(ChecklistCategory checklistCategory)
        {
            _context.ChecklistCategories.Add(checklistCategory);
        }

        public async Task<bool> ChecklistCategoryExistAsync(int checklistCategoryId)
        {
            return await _context.ChecklistCategories.AnyAsync(c => c.ChecklistCategoryId == checklistCategoryId);
        }

        public void DeleteChecklistCategory(ChecklistCategory checklistCategory)
        {
            _context.ChecklistCategories.Remove(checklistCategory);
        }

        public async Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync()
        {
            return await _context.ChecklistCategories
                .OrderBy(c => c.ChecklistCategoryId).ToListAsync();
        }

        public async Task<ChecklistCategory?> GetChecklistCategoryAsync(int categoryId, bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.ChecklistCategories.Include(cq => cq.ChecklistQuestions)
                    .Where(c => c.ChecklistCategoryId == categoryId).FirstOrDefaultAsync();
            }

            return await _context.ChecklistCategories
                .Where(c => c.ChecklistCategoryId == categoryId).FirstOrDefaultAsync();
        }

        #endregion
        #region JobObservationTypesOperations
        public async Task<IEnumerable<JobObservationType>> GetJobObservationTypesAsync()
        {
            return await _context.JobObservationTypes
                .OrderBy(c => c.JobObservationTypeId).ToListAsync();
        }

        public async Task<JobObservationType?> GetJobObservationTypeAsync(int id, bool includeConfigs = false)
        {
            if (includeConfigs)
            {
                return await _context.JobObservationTypes.Include(jot => jot.JobObservationConfigs)
                    .Where(c => c.JobObservationTypeId == id).FirstOrDefaultAsync();
            }

            return await _context.JobObservationTypes
                .Where(c => c.JobObservationTypeId == id).FirstOrDefaultAsync();
        }

        public void AddJobObservationType(JobObservationType jobObservationType)
        {
            _context.JobObservationTypes.Add(jobObservationType);
        }

        public void DeleteJobObservationType(JobObservationType jobObservationType)
        {
            _context.JobObservationTypes.Remove(jobObservationType);
        }

        #endregion
        #region QuestionTypeOperations

        public async Task<IEnumerable<QuestionType>> GetQuestionTypesAsync()
        {
            return await _context.QuestionTypes.OrderBy(q => q.QuestionTypeId).ToListAsync();
        }

        public async Task<QuestionType?> GetQuestionTypeAsync(int questionTypeId, bool includeChecklistQuestions = false)
        {
            if (includeChecklistQuestions)
            {
                return await _context.QuestionTypes.Include(cq => cq.ChecklistQuestions)
                    .Where(q => q.QuestionTypeId == questionTypeId).FirstOrDefaultAsync();
            }

            return await _context.QuestionTypes
                .Where(c => c.QuestionTypeId == questionTypeId).FirstOrDefaultAsync();
        }


        #endregion
        #region ChecklistQuestionOperations

        public async Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForCategoryAsync(int categoryId)
        {
            return await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId).ToListAsync();
        }
        public async Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int questionId)
        {
            return await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId && cq.QuestionID == questionId)
                .FirstOrDefaultAsync();
        }
        public async Task AddChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestion checklistQuestion)
        {
            var checklistCategory = await GetChecklistCategoryAsync(categoryId, false);
            if (checklistCategory != null)
            {
                checklistCategory.ChecklistQuestions.Add(checklistQuestion);
            }
        }
        public void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion)
        {
            _context.ChecklistQuestions.Remove(checklistQuestion);
        }
        #endregion
        #region CommonOperations
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        } 
        #endregion
    }
}
