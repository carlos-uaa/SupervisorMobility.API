using AutoMapper;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Business
{
    public class ChecklistCategoryService : IChecklistCategoryService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public ChecklistCategoryService(ISupervisorMobilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task DeleteChecklistCategoryAsync(ChecklistCategory checklistCategory)
        {
            _repository.DeleteChecklistCategory(checklistCategory);
            await _repository.SaveChangesAsync();
        }

        public async Task<int> FetchChecklistCategoriesMaxSequenceAsync()
        {
            return await _repository.GetChecklistCategoriesMaxSequenceAsync();
        }

        public async Task<ChecklistCategory?> FetchChecklistCategoryAsync(int categoryId)
        {
            return await _repository.GetChecklistCategoryAsync(categoryId);
        }

        public async Task UpdateChecklistCategoriesSequenceAsync(ChecklistCategorySequenceForUpdateDto newChecklistCategorySequence, ChecklistCategory checklistCategoryEntity)
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
                       checklistCategoryEntity.ChecklistCategoryId);

            foreach (var checklistCategoryEntityForUpdate in checklistCategoryEntities)
            {
                currentSequence += 1;
                checklistCategoryEntityForUpdate.Sequence = currentSequence;
            }

            _mapper.Map(newChecklistCategorySequence, checklistCategoryEntity);
            await _repository.SaveChangesAsync();
        }
    }
}
