using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIItemsService : IHRIItemsService
    {
        private readonly IHRIItemsRepository _HRIItemsRepository;

        public HRIItemsService(IHRIItemsRepository HRIItemsRepository)
        {
            _HRIItemsRepository = HRIItemsRepository;
        }

        public async Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync()
        {
            try
            {
                return await _HRIItemsRepository.GetAllHRIItemsAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIItem>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Items in Service: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int id)
        {
            try
            {
                return await _HRIItemsRepository.GetSingleHRIItemAsync(id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error trying to obtain the HRI Item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem newItem)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(newItem.ControlNumber))
                    return new ServiceResponse<HRIItem> { Success = false, Message = "The field Control Number is Required." };

                if (string.IsNullOrWhiteSpace(newItem.Name))
                    return new ServiceResponse<HRIItem> { Success = false, Message = "The field Name is Required." };
                return await _HRIItemsRepository.CreateHRIItemAsync(newItem);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error creating the HRI Item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem updatedItem)
        {
            try
            {
                // Validaciones
                if (updatedItem.Id <= 0)
                    return new ServiceResponse<HRIItem> { Success = false, Message = "The ID provided is invalid." };

                if (string.IsNullOrWhiteSpace(updatedItem.ControlNumber))
                    return new ServiceResponse<HRIItem> { Success = false, Message = "The field Control Number is required." };
                if (string.IsNullOrWhiteSpace(updatedItem.Name))
                    return new ServiceResponse<HRIItem> { Success = false, Message = "The field Name is required." };

                var existing = await _HRIItemsRepository.GetSingleHRIItemAsync(updatedItem.Id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<HRIItem> { Success = false, Message = $"There's no an existing Item with ID {updatedItem.Id}" };

                return await _HRIItemsRepository.UpdateHRIItemAsync(updatedItem);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error updating the HRI Item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRIItemAsync(int id)
        {
            try
            {
                var existing = await _HRIItemsRepository.GetSingleHRIItemAsync(id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<bool> { Success = false, Message = $"There's no an existing Item with ID {id}" };

                return await _HRIItemsRepository.DeleteHRIItemAsync(id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting the HRI Item: {ex.Message}"
                };
            }
        }
    }
}
