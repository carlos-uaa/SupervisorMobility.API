using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIHourmeterRevisionRepository : IHRIHourmeterRevisionRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public HRIHourmeterRevisionRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions()
        {
            var serviceResponse = new ServiceResponse<List<GetHourmeterRevisionDto>>();
            try
            {
                var hourmeterRevisions = await _context.HourmeterRevisions
                    .Include(hr => hr.DailyRevisions)
                    .ToListAsync();
                serviceResponse.Data = hourmeterRevisions.Select(hr => _mapper.Map<GetHourmeterRevisionDto>(hr)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int Hrid)
        {
            var serviceResponse = new ServiceResponse<GetHourmeterRevisionDto>();
            try
            {
                var hourmeterRevision = await _context.HourmeterRevisions.Include(hr => hr.DailyRevisions)
                    .FirstOrDefaultAsync(hr => hr.HriId == Hrid);
                serviceResponse.Data = _mapper.Map<GetHourmeterRevisionDto>(hourmeterRevision);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id)
        {
            var serviceResponse = new ServiceResponse<GetHourmeterRevisionDto>();
            try
            {
                var hourmeterRevision = await _context.HourmeterRevisions
                    .Include(hr => hr.DailyRevisions)
                    .FirstOrDefaultAsync(hr => hr.Id == id);
                serviceResponse.Data = _mapper.Map<GetHourmeterRevisionDto>(hourmeterRevision);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision)
        {
            var serviceResponse = new ServiceResponse<GetHourmeterRevisionDto>();
            try
            {
                var hourmeterRevision = _mapper.Map<HourmeterRevision>(newHourmeterRevision);
                await _context.HourmeterRevisions.AddAsync(hourmeterRevision);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetHourmeterRevisionDto>(hourmeterRevision);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetHourmeterRevisionDto>>> DeleteHourmeterRevision(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetHourmeterRevisionDto>>();
            try
            {
                var hourmeterRevision = await _context.HourmeterRevisions
                    .FirstOrDefaultAsync(hr => hr.Id == id);
                if (hourmeterRevision != null)
                {
                    _context.HourmeterRevisions.Remove(hourmeterRevision);
                    await _context.SaveChangesAsync();
                }
                var remainingRevisions = await _context.HourmeterRevisions.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<GetHourmeterRevisionDto>>(remainingRevisions);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}
