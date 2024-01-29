using AutoMapper;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Business
{
    public class JobObservationService : IJobObservationService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public JobObservationService(ISupervisorMobilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region JobCategoryStructure
        public async Task<IEnumerable<JobCategoryStructure>> FetchChecklistCategoriesAsync(bool includeChecklistQuestions = false)
        {
            return await _repository.GetChecklistCategoriesAsync(includeChecklistQuestions);
        }

        public async Task<JobCategoryStructure> CreateChecklistCategoryAsync(JobCategoryStructureForCreationDto checklistCategory)
        {
            var finalChecklistCategory = _mapper.Map<JobCategoryStructure>(checklistCategory);
            finalChecklistCategory.Sequence = await _repository.GetChecklistCategoriesMaxSequenceAsync();
            _repository.AddChecklistCategory(finalChecklistCategory);
            await _repository.SaveChangesAsync();

            return finalChecklistCategory;
        }

        public async Task UpdateChecklistCategoryAsync(JobCategoryStructureForUpdateDto checklistCategoryUpdate, JobCategoryStructure checklistCategory)
        {
            _mapper.Map(checklistCategoryUpdate, checklistCategory);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteChecklistCategoryAsync(JobCategoryStructure checklistCategory)
        {
            _repository.DeleteChecklistCategory(checklistCategory);
            await _repository.SaveChangesAsync();
        }

        public async Task<int> FetchChecklistCategoriesMaxSequenceAsync()
        {
            return await _repository.GetChecklistCategoriesMaxSequenceAsync();
        }

        public async Task<JobCategoryStructure?> FetchChecklistCategoryAsync(int categoryId, bool includeChecklistQuestions = false)
        {
            return await _repository.GetChecklistCategoryAsync(categoryId, includeChecklistQuestions);
        }

        public async Task UpdateChecklistCategoriesSequenceAsync(JobCategoryStructureSequenceForUpdateDto newChecklistCategorySequence, JobCategoryStructure checklistCategoryEntity)
        {
            //So we need to update the checklist categories sequence between desiered and old one.
            var currentSequence =
                newChecklistCategorySequence.Sequence < checklistCategoryEntity.Sequence
                ? newChecklistCategorySequence.Sequence
                : checklistCategoryEntity.Sequence - 1;

            var checklistCategoryEntities =
                await _repository.GetChecklistCategoriesForUpdateSequenceAsync(
                       newChecklistCategorySequence.Sequence,
                       checklistCategoryEntity.Sequence,
                       checklistCategoryEntity.JobCategoryStructureId);

            foreach (var checklistCategoryEntityForUpdate in checklistCategoryEntities)
            {
                currentSequence += 1;
                checklistCategoryEntityForUpdate.Sequence = currentSequence;
            }

            _mapper.Map(newChecklistCategorySequence, checklistCategoryEntity);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> CheckChecklistCategoryExistAsync(int categoryId)
        {
            return await _repository.ChecklistCategoryExistAsync(categoryId);
        }
        #endregion
        #region Question
        public async Task<IEnumerable<ChecklistQuestion>> FetchChecklistQuestionsForCategoryAsync(int categoryId)
        {
            return await _repository.GetChecklistQuestionsForCategoryAsync(categoryId);
        }

        public async Task DeleteChecklistQuestionAsync(ChecklistQuestion checklistQuestion)
        {
            _repository.DeleteChecklistQuestions(checklistQuestion);
            await _repository.SaveChangesAsync();
        }

        public async Task<int> FetchChecklistQuestionMaxSequenceAsync(int categoryId)
        {
            return await _repository.GetChecklistQuestionMaxCategorySequenceAsync(categoryId);
        }

        public async Task<ChecklistQuestion?> FetchChecklistQuestionForCategoryAsync(int categoryId, int questionId)
        {
            return await _repository.GetChecklistQuestionForCategoryAsync(categoryId, questionId);
        }

        public async Task<ChecklistQuestion> CreateChecklistQuestionForCategoryAsync(int categoryId,
            ChecklistQuestionForCreationDto checklistQuestion)
        {
            var finalChecklistQuestion = _mapper.Map<Entities.ChecklistQuestion>(checklistQuestion);
            finalChecklistQuestion.CategorySequence = await
                _repository.GetChecklistQuestionMaxCategorySequenceAsync(categoryId);
            await _repository.AddChecklistQuestionForCategoryAsync(
                categoryId, finalChecklistQuestion);
            await _repository.SaveChangesAsync();

            return finalChecklistQuestion;
        }

        public async Task UpdateChecklistQuestionForCategoryAsync(ChecklistQuestionForUpdateDto checklistQuestionForUpdate, ChecklistQuestion checklistQuestion)
        {
            //Check if the category is different, if it is the checklist category will be send to the buttom.
            if (checklistQuestionForUpdate.JobCategoryStructureId != checklistQuestion.JobCategoryStructureId)
            {
                //Send this question to the end
                var checklistQuestionSequence = new ChecklistQuestionSequenceForUpdateDto();
                checklistQuestionSequence.CategorySequence = await FetchChecklistQuestionMaxSequenceAsync(checklistQuestion.JobCategoryStructureId) - 1;
                await UpdateChecklistQuestionsSequenceAsync(checklistQuestionSequence, checklistQuestion, checklistQuestion.JobCategoryStructureId);

                checklistQuestion.CategorySequence =
                    await FetchChecklistQuestionMaxSequenceAsync(checklistQuestionForUpdate.JobCategoryStructureId);
            }

            _mapper.Map(checklistQuestionForUpdate, checklistQuestion);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateChecklistQuestionsSequenceAsync(ChecklistQuestionSequenceForUpdateDto newChecklistQuestionSequence, ChecklistQuestion checklistQuestionEntity, int categoryId)
        {
            //Update just the checklist questions sequences between the desired and the old one.
            var currentSequence =
                newChecklistQuestionSequence.CategorySequence < checklistQuestionEntity.CategorySequence
                ? newChecklistQuestionSequence.CategorySequence
                : checklistQuestionEntity.CategorySequence - 1;

            var checklistQuestionsEntities = await
                _repository.GetChecklistQuestionsForUpdateSequenceAsync(
                newChecklistQuestionSequence.CategorySequence,
                checklistQuestionEntity.CategorySequence,
                categoryId, checklistQuestionEntity.QuestionID);

            foreach (var checklistQuestionsEntityForUpdate in checklistQuestionsEntities)
            {
                currentSequence += 1;
                checklistQuestionsEntityForUpdate.CategorySequence = currentSequence;
            }

            _mapper.Map(newChecklistQuestionSequence, checklistQuestionEntity);
            await _repository.SaveChangesAsync();
        }
        #endregion
      
    }
}
