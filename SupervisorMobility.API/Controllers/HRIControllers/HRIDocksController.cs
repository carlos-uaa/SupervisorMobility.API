using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRIDocksController : ControllerBase
    {
        private readonly IHRIDocksService hRIDocksService;

        public HRIDocksController(IHRIDocksService hRIDocksService)
        {
            this.hRIDocksService = hRIDocksService;
        }

        [HttpGet("GetAllHRIDocksAsync")]
        public async Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync()
        {
            return await hRIDocksService.GetAllHRIDocksAsync();
        }

        [HttpGet("GetSingleHRIDockAsync/{id}")]
        public async Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int id)
        {
            return await hRIDocksService.GetSingleHRIDockAsync(id);
        }

        [HttpPost("CreateHRIDockAsync")]
        public async Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock newDock)
        {
            return await hRIDocksService.CreateHRIDockAsync(newDock);
        }

        [HttpPut("UpdateHRIDockAsync")]
        public async Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock updatedDock)
        {
            return await hRIDocksService.UpdateHRIDockAsync(updatedDock);
        }

        [HttpDelete("DeleteHRIDockAsync/{id}")]
        public async Task<ServiceResponse<bool>> DeleteHRIDockAsync(int id)
        {
            return await hRIDocksService.DeleteHRIDockAsync(id);
        }
    }
}
