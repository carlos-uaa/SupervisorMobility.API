using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIItemsRepository : IHRIItemsRepository
    {
        private readonly SupervisorMobilityContext _context;

        public HRIItemsRepository(SupervisorMobilityContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync()
        {
            try
            {
                var items = _context.HRIItems.Where(h => h.IsActive).ToList();
                string message = items.Count > 0 ? "HRIItems retrieved successfully." : "No active HRIItems found.";
                return new ServiceResponse<List<HRIItem>>
                {
                    Data = items,
                    Success = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIItem>>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving HRI Items in DB: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int Id)
        {
            try
            {
                var item = _context.HRIItems.FirstOrDefault(h => h.Id == Id);
                string message = item != null ? "HRIItem retrieved successfully." : "HRIItem not found.";
                return new ServiceResponse<HRIItem>
                {
                    Data = item,
                    Success = item != null,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving the HRI Item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem Item)
        {
            try
            {
                _context.HRIItems.Add(Item);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRIItem>
                {
                    Data = Item,
                    Success = true,
                    Message = "HRIItem created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error creating HRIItem: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem Item)
        {
            try
            {
                var existingItem = await _context.HRIItems.FirstOrDefaultAsync(h => h.Id == Item.Id);
                if (existingItem == null)
                    return new ServiceResponse<HRIItem>
                    {
                        Data = null,
                        Success = false,
                        Message = "HRIItem not found."
                    };

                existingItem.ControlNumber = Item.ControlNumber;
                existingItem.Name = Item.Name;
                existingItem.IsActive = Item.IsActive;

                _context.HRIItems.Update(existingItem);
                await _context.SaveChangesAsync();

                return new ServiceResponse<HRIItem>
                {
                    Data = existingItem,
                    Success = true,
                    Message = "HRIItem updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error updating HRIItem: {ex.Message}"
                };
            }

        }

        public async Task<ServiceResponse<bool>> DeleteHRIItemAsync(int Id)
        {
            try
            {
                var existingItem = await _context.HRIItems.FirstOrDefaultAsync(h => h.Id == Id);
                if (existingItem == null)
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HRIItem not found."
                    };

                existingItem.IsActive = false;

                _context.HRIItems.Update(existingItem);
                await _context.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "HRI Item deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error deleting HRI Item: {ex.Message}"
                };
            }
        }
    }
}
