using AutoMapper;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI;
using SupervisorMobility.API.Models.HRICyclesDtos;
using Microsoft.EntityFrameworkCore;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRICyclesServices : IHRICyclesService
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public HRICyclesServices(SupervisorMobilityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto createHRICycle)
        {
            var response = new ServiceResponse<GetHRICyclesDto>();
            try
            {
                var newHRICycle = _mapper.Map<HRICycles>(createHRICycle);
                await _context.HRICycles.AddAsync(newHRICycle);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetHRICyclesDto>(newHRICycle);
                response.Success = true;
                response.Message = "HRICycle created successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteHRICycle(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var hriCycle = await _context.HRICycles.FindAsync(id);
                if (hriCycle == null)
                {
                    response.Success = false;
                    response.Message = "HRICycle not found.";
                    return response;
                }

                _context.HRICycles.Remove(hriCycle);
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "HRICycle deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id)
        {
            var response = new ServiceResponse<GetHRICyclesDto>();
            try
            {
                var hriCycle = await _context.HRICycles.Include(h => h.DailyRevisions).FirstOrDefaultAsync(h => h.CycleId == id);
                if (hriCycle == null)
                {
                    response.Success = false;
                    response.Message = "HRICycle not found.";
                    return response;
                }

                response.Data = _mapper.Map<GetHRICyclesDto>(hriCycle);
                response.Success = true;
                response.Message = "HRICycle retrieved successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetHRICyclesDto>>> GetHRICycles()
        {
            var response = new ServiceResponse<List<GetHRICyclesDto>>();    
            try
            {
                var hriCycles = await _context.HRICycles.Include(h => h.DailyRevisions).ToListAsync();
                response.Data = hriCycles.Select(h => _mapper.Map<GetHRICyclesDto>(h)).ToList();
                response.Success = true;
                response.Message = "HRICycles retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> UpdateHRICycle(int id, UpdateHRICycleDto updateHRICycle)
        {
            var response = new ServiceResponse<GetHRICyclesDto>();
            try
            {
                var hriCycle = _context.HRICycles.Find(id);
                if (hriCycle == null)
                {
                    response.Success = false;
                    response.Message = "HRICycle not found.";
                    return response;
                }
                _mapper.Map(updateHRICycle, hriCycle);
                _context.HRICycles.Update(hriCycle);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetHRICyclesDto>(hriCycle);
                response.Success = true;
                response.Message = "HRICycle updated successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
