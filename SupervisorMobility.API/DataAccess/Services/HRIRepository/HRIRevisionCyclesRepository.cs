using AutoMapper;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using Microsoft.EntityFrameworkCore;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIRevisionCyclesRepository : IHRIRevisionCyclesRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public HRIRevisionCyclesRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto createRevisionCyclesDto)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto updateRevisionCycleDto)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            throw new NotImplementedException();
        }
    }
}
