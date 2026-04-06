using AutoMapper;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI;
using SupervisorMobility.API.Models.HRIDtos;
using Microsoft.EntityFrameworkCore;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIServices : IHRIServices
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        public HRIServices(SupervisorMobilityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto newHRI)
        {
            var serviceResponse = new ServiceResponse<GetHRIDto>();
            try
            {
                var hri = _mapper.Map<HRI>(newHRI);
                await _context.HRIs.AddAsync(hri);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetHRIDto>(hri);
                serviceResponse.Success = true;
                serviceResponse.Message = "HRI created successfully.";
               

            }
            catch (Exception ex)
            {

                serviceResponse.Success = false;
                serviceResponse.Message = $"Error creating HRI: {ex.Message}";
                
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> DeleteHRI(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var hri = await _context.HRIs.FirstOrDefaultAsync(h => h.HriId == id);
                if (hri == null)
                {
                    response.Success = false;
                    response.Message = "HRI not found.";
                    response.Data = false;
                    return response;

                }
                hri.IsActive = false;
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "HRI deleted successfully.";
                response.Data = true;
               

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting HRI: {ex.Message}";
                response.Data = false;
                
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI()
        {
            var response = new ServiceResponse<List<GetHRIDto>>();
            try
            {
                var hris = await _context.HRIs.ToListAsync();
                response.Data = _mapper.Map<List<GetHRIDto>>(hris);
                response.Success = true;
                response.Message = "HRIs retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving HRIs: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRIDto>> GetHRIById(int id)
        {
            var response = new ServiceResponse<GetHRIDto>();
            try
            {
                var hri = await _context.HRIs.FirstOrDefaultAsync(h => h.HriId == id);
                if (hri == null)
                {
                    response.Success = false;
                    response.Message = "HRI not found.";
                    return response;
                }
                response.Data = _mapper.Map<GetHRIDto>(hri);
                response.Success = true;
                response.Message = "HRI retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving HRI: {ex.Message}";
            }
            return response;
        }
    }
}
