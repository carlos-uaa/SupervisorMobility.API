using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRILinesService : IHRILinesService
    {
        private readonly IHRILinesRepository _HRILinesRepository;

        public HRILinesService(IHRILinesRepository HRILinesRepository)
        {
            _HRILinesRepository = HRILinesRepository;
        }

        public async Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync()
        {
            try
            {
                return await _HRILinesRepository.GetAllHRILinesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRILines>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Lines in Service: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int id)
        {
            try
            {
                return await _HRILinesRepository.GetSingleHRILineAsync(id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error trying to obtain the Line: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines newLine)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(newLine.Code))
                    return new ServiceResponse<HRILines> { Success = false, Message = "The field Code is Required." };

                if (string.IsNullOrWhiteSpace(newLine.LineName))
                    return new ServiceResponse<HRILines> { Success = false, Message = "The field LineName is Required." };

                return await _HRILinesRepository.CreateHRILineAsync(newLine);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error creating the Line: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines updatedLine)
        {
            try
            {
                // Validaciones
                if (updatedLine.Id <= 0)
                    return new ServiceResponse<HRILines> { Success = false, Message = "The ID provided is invalid." };

                if (string.IsNullOrWhiteSpace(updatedLine.Code))
                    return new ServiceResponse<HRILines> { Success = false, Message = "The field Code is required." };

                if (string.IsNullOrWhiteSpace(updatedLine.LineName))
                    return new ServiceResponse<HRILines> { Success = false, Message = "The field Line Name is required." };

                var existing = await _HRILinesRepository.GetSingleHRILineAsync(updatedLine.Id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<HRILines> { Success = false, Message = $"There's no an existing Line with ID {updatedLine.Id}" };

                return await _HRILinesRepository.UpdateHRILineAsync(updatedLine);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error al actualizar la línea: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRILineAsync(int id)
        {
            try
            {
                var existing = await _HRILinesRepository.GetSingleHRILineAsync(id);
                if (existing == null || existing.Data == null || !existing.Success)
                    return new ServiceResponse<bool> { Success = false, Message = $"There's no an existing Line with ID {id}" };

                return await _HRILinesRepository.DeleteHRILineAsync(id);
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
