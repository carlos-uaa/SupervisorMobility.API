using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRIItemController
    {
        private readonly IHRIItemsService hRIItemsService;

        public HRIItemController(IHRIItemsService hRIItemsService )
        {
            this.hRIItemsService = hRIItemsService;
        }

        [HttpGet("GetAllHRIItemsAsync")]
        public async Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync()
        {
            return await hRIItemsService.GetAllHRIItemsAsync();
        }

        [HttpGet("GetSingleHRIItemAsync/{id}")]
        public async Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int id)
        {
            return await hRIItemsService.GetSingleHRIItemAsync(id);
        }

        [HttpPost("CreateHRIItemAsync")]
        public async Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem newItem)
        {
            return await hRIItemsService.CreateHRIItemAsync(newItem);
        }

        [HttpPut("UpdateHRIItemAsync")]
        public async Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem updatedItem)
        {
            return await hRIItemsService.UpdateHRIItemAsync(updatedItem);
        }

        [HttpDelete("DeleteHRIItemAsync/{id}")]
        public async Task<ServiceResponse<bool>> DeleteHRIItemAsync(int id)
        {
            return await hRIItemsService.DeleteHRIItemAsync(id);
        }
    }
}
