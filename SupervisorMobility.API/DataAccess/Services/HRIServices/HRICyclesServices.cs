using AutoMapper;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Models.HRICyclesDtos;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRICyclesServices : IHRICyclesService
    {
        private readonly IHRICyclesRepository _hriCyclesRepository;
        public HRICyclesServices(IHRICyclesRepository hriCyclesRepository)
        {
            _hriCyclesRepository = hriCyclesRepository;
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto createHRICycle)
        {
            return await _hriCyclesRepository.CreateHRICycle(createHRICycle);
        }

        public async Task<ServiceResponse<bool>> DeleteHRICycle(int id)
        {
            return await _hriCyclesRepository.DeleteHRICycle(id);
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id)
        {
            return await _hriCyclesRepository.GetHRICycleById(id);
        }

        public async Task<ServiceResponse<List<GetHRICyclesDto>>> GetHRICycles()
        {
            return await _hriCyclesRepository.GetHRICycles();
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> UpdateHRICycle(int id, UpdateHRICycleDto updateHRICycle)
        {
            return await _hriCyclesRepository.UpdateHRICycle(id, updateHRICycle);
        }
    }
}
