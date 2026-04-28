using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIDocksService : IHRIDocksService
    {
        private readonly IHRIDocksRepository _HRIDocksRepository;

        public HRIDocksService(IHRIDocksRepository HRIDocksRepository)
        {
            _HRIDocksRepository = HRIDocksRepository;
        }

        public async Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync()
        {
            try
            {
                return await _HRIDocksRepository.GetAllHRIDocksAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIDock>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Docks in Service: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int id)
        {
            try
            {
                return await _HRIDocksRepository.GetSingleHRIDockAsync(id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error trying to obtain the Dock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock newDock)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(newDock.Code))
                    return new ServiceResponse<HRIDock> { Success = false, Message = "The field Code is Required." };

                if (string.IsNullOrWhiteSpace(newDock.DockName))
                    return new ServiceResponse<HRIDock> { Success = false, Message = "The field DockName is Required." };
                return await _HRIDocksRepository.CreateHRIDockAsync(newDock);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error creating the Dock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock updatedDock)
        {
            try
            {
                // Validaciones
                if (updatedDock.Id <= 0)
                    return new ServiceResponse<HRIDock> { Success = false, Message = "The ID provided is invalid." };

                if (string.IsNullOrWhiteSpace(updatedDock.Code))
                    return new ServiceResponse<HRIDock> { Success = false, Message = "The field Code is required." };
                if (string.IsNullOrWhiteSpace(updatedDock.DockName))
                    return new ServiceResponse<HRIDock> { Success = false, Message = "The field Dock Name is required." };

                var existing = await _HRIDocksRepository.GetSingleHRIDockAsync(updatedDock.Id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<HRIDock> { Success = false, Message = $"There's no an existing Dock with ID {updatedDock.Id}" };
                return await _HRIDocksRepository.UpdateHRIDockAsync(updatedDock);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error updating the Dock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRIDockAsync(int id)
        {
            try
            {
                var existing = await _HRIDocksRepository.GetSingleHRIDockAsync(id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<bool> { Success = false, Message = $"There's no an existing Dock with ID {id}" };

                return await _HRIDocksRepository.DeleteHRIDockAsync(id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar la línea: {ex.Message}"
                };
            }
        }
    }
}
