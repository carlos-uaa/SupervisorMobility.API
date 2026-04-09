using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRILinesController
    {
        private readonly IHRILinesService hRILinesService;

        public HRILinesController(IHRILinesService hRILinesService)
        {
            this.hRILinesService = hRILinesService;
        }

        [HttpGet("GetAllHRILinesAsync")]
        public async Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync()
        {
            return await hRILinesService.GetAllHRILinesAsync();
        }

        [HttpGet("GetSingleHRILineAsync/{id}")]
        public async Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int id)
        {
            return await hRILinesService.GetSingleHRILineAsync(id);
        }

        [HttpPost("CreateHRILineAsync")]
        public async Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines newLine)
        {
            return await hRILinesService.CreateHRILineAsync(newLine);
        }

        [HttpPut("UpdateHRILineAsync")]
        public async Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines updatedLine)
        {
            return await hRILinesService.UpdateHRILineAsync(updatedLine);
        }

        [HttpDelete("DeleteHRILineAsync/{id}")]
        public async Task<ServiceResponse<bool>> DeleteHRILineAsync(int id)
        {
            return await hRILinesService.DeleteHRILineAsync(id);
        }
    }
}
