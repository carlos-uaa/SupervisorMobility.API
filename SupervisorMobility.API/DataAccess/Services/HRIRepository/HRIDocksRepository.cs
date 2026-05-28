using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIDocksRepository : IHRIDocksRepository
    {
        private readonly SupervisorMobilityContext _context;

        public HRIDocksRepository(SupervisorMobilityContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync()
        {
            try
            {
                var docks = await _context.HRIDocks.Where(h => h.IsActive).ToListAsync();
                string message = docks.Count > 0 ? "HRIDocks retrieved successfully." : "No active HRIDocks found.";
                return new ServiceResponse<List<HRIDock>>
                {
                    Data = docks,
                    Success = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIDock>>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Docks in DB: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int Id)
        {
            try
            {
                var dock = _context.HRIDocks.FirstOrDefault(h => h.Id == Id);
                string message = dock != null ? "HRIDock retrieved successfully." : "HRIDock not found.";
                return new ServiceResponse<HRIDock>
                {
                    Data = dock,
                    Success = dock != null,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving the HRIDock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock Dock)
        {
            try
            {
                 await _context.HRIDocks.AddAsync(Dock);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRIDock>
                {
                    Data = Dock,
                    Success = true,
                    Message = "HRI Dock created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error creating HRIDock: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock Dock)
        {
            try
            {
                var existingDock = await _context.HRIDocks.FirstOrDefaultAsync(h => h.Id == Dock.Id);
                if (existingDock == null)
                    return new ServiceResponse<HRIDock>
                    {
                        Data = null,
                        Success = false,
                        Message = "HRIDock not found."
                    };

                existingDock.Code = Dock.Code;
                existingDock.DockName = Dock.DockName;
                existingDock.IsActive = Dock.IsActive;

                _context.HRIDocks.Update(existingDock);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRIDock>
                {
                    Data = existingDock,
                    Success = true,
                    Message = "HRIDock updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error updating HRIDock: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<bool>> DeleteHRIDockAsync(int Id)
        {
            try
            {
                var existingDock = await _context.HRIDocks.FirstOrDefaultAsync(h => h.Id == Id);
                if (existingDock == null)
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HRIDock not found."
                    };

                existingDock.IsActive = false;

                _context.HRIDocks.Update(existingDock);
                await _context.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "HRIDock Deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error deleting HRIDock: {ex.Message}"
                };
            }
        }
    }
}
