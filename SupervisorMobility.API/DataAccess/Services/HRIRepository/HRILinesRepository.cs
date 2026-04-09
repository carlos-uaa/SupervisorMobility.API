
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRILinesRepository : IHRILinesRepository
    {
        private readonly SupervisorMobilityContext _context;

        public HRILinesRepository(SupervisorMobilityContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync()
        {
            try
            {
                var lines = _context.HRILines.Where(h => h.IsActive).ToList();
                string message = lines.Count > 0 ? "HRILines retrieved successfully." : "No active HRILines found.";
                return new ServiceResponse<List<HRILines>>
                {
                    Data = lines,
                    Success = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRILines>>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Lines in DB: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int Id)
        {
            try
            {
                var line = _context.HRILines.FirstOrDefault(h => h.Id == Id);
                string message = line != null ? "HRILine retrieved successfully." : "HRILine not found.";
                return new ServiceResponse<HRILines>
                {
                    Data = line,
                    Success = line != null,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving The Line: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines Line)
        {
            try
            {
                _context.HRILines.Add(Line);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRILines>
                {
                    Data = Line,
                    Success = true,
                    Message = "HRILine created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error creating HRILine: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines Line)
        {
            try
            {
                var existingLine = await _context.HRILines.FirstOrDefaultAsync(h => h.Id == Line.Id);
                if (existingLine == null)
                    return new ServiceResponse<HRILines>
                    {
                        Data = null,
                        Success = false,
                        Message = "HRILine not found."
                    };

                existingLine.Code = Line.Code;
                existingLine.LineName = Line.LineName;
                existingLine.IsActive = Line.IsActive;

                _context.HRILines.Update(existingLine);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRILines>
                {
                    Data = existingLine,
                    Success = true,
                    Message = "HRILine updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error updating HRILine: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<bool>> DeleteHRILineAsync(int Id)
        {
            try
            {
                var existingLine = await _context.HRILines.FirstOrDefaultAsync(h => h.Id == Id);
                if (existingLine == null)
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HRILine not found."
                    };

                existingLine.IsActive = false;

                _context.HRILines.Update(existingLine);
                await _context.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "HRILine Deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error deleting HRILine: {ex.Message}"
                };
            }
        }
    }
}
