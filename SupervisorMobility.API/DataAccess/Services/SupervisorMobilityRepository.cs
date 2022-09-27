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
                .OrderBy(c => c.Sequence).ToListAsync();
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

        public async Task<int> GetChecklistCategoriesMaxSequenceAsync()
        {
            return await _context.ChecklistCategories.MaxAsync(cc => cc.Sequence) + 1;
        }

        public async Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesForUpdateSequenceAsync(
            int currentSequence, int oldSequence, int categoryId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.ChecklistCategories
                        .Where(c => c.Sequence >= lowerValue
                            && c.Sequence <= upperValue
                            && c.ChecklistCategoryId != categoryId)
                        .OrderBy(c => c.Sequence).ToListAsync();
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

        public async Task<bool> JobObservationTypeExistAsync(int jobObservationTypeId)
        {
            return await _context.JobObservationTypes.AnyAsync(c => c.JobObservationTypeId == jobObservationTypeId);
        }
        #endregion
        #region GroupOperations
        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            return await _context.Groups
                .OrderBy(c => c.GroupId).ToListAsync();
        }

        public async Task<Group?> GetGroupAsync(int groupId)
        {
            return await _context.Groups
                .Where(c => c.GroupId == groupId).FirstOrDefaultAsync();
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void DeleteGroup(Group group)
        {
            _context.Groups.Remove(group);
        }
        #endregion
        #region PlantOperations
        public async Task<IEnumerable<Plant>> GetPlantsAsync()
        {
            return await _context.Plants
                .OrderBy(c => c.PlantId).ToListAsync();
        }

        public async Task<Plant?> GetPlantAsync(int plantId)
        {
            return await _context.Plants
                .Where(c => c.PlantId == plantId).FirstOrDefaultAsync();
        }

        public void AddPlant(Plant plant)
        {
            _context.Plants.Add(plant);
        }

        public void DeletePlant(Plant plant)
        {
            _context.Plants.Remove(plant);
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

        public async Task<int> GetChecklistQuestionMaxCategorySequenceAsync(int categoryId)
        {
            var sequence = await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId)
                .MaxAsync(cq => (int?)cq.CategorySequence) ?? 0;
            return  sequence + 1;
        }

        public void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion)
        {
            _context.ChecklistQuestions.Remove(checklistQuestion);
        }

        public async Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForUpdateSequenceAsync(
                int currentSequence, int oldSequence, int categoryId, int checklistQuestionId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.ChecklistQuestions
                        .Where(c => c.ChecklistCategoryId == categoryId
                            && c.CategorySequence >= lowerValue
                            && c.CategorySequence <= upperValue
                            && c.QuestionID != checklistQuestionId)
                        .OrderBy(c => c.CategorySequence).ToListAsync();
        }
        #endregion
        #region JobObservationConfigOperations
        public async Task<IEnumerable<JobObservationConfig>> GetJobOperationConfigsForJobOperationTypeAsync(int jobObservationTypeId)
        {
            return await _context.JobObservationConfigs
                .Where(joc => joc.JobObservationTypeId == jobObservationTypeId).ToListAsync();
        }
        public async Task<JobObservationConfig?> GetJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId,
            int jobObservationConfigId)
        {
            return await _context.JobObservationConfigs
                .Where(joc => joc.JobObservationTypeId == jobObservationTypeId 
                           && joc.JobObservationConfigId == jobObservationConfigId)
                .FirstOrDefaultAsync();
        }
        public async Task AddJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId, JobObservationConfig jobObservationConfig)
        {
            var jobOperationType = await GetJobObservationTypeAsync(jobObservationTypeId);
            if(jobOperationType != null)
            {
                jobOperationType.JobObservationConfigs.Add(jobObservationConfig);
            }
        }
        public void DeleteJobOperationConfig(JobObservationConfig jobObservationConfig)
        {
            _context.JobObservationConfigs.Remove(jobObservationConfig);
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
