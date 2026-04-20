using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;


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
        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
            var response = new ServiceResponse<List<GetRevisionCyclesDto>>();
            try
            {
                var revisionCycles = await _context.RevisionCycles.Include(rc => rc.DailyRevisions!).ThenInclude(dr => dr.Responsible).Where(rc => rc.IsActive == true).ToListAsync();
                if(revisionCycles == null || revisionCycles.Count == 0) 
                {
                    response.Success = false;
                    response.Message = "No revision cycles found.";
                    return response;
                }
                response.Data = revisionCycles.Select(rc => _mapper.Map<GetRevisionCyclesDto>(rc)).ToList();
                response.Success = true;
                response.Message = "Revision cycles retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while retrieving revision cycles: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            var response = new ServiceResponse<List<GetRevisionCyclesDto>>();
            try
            {
                var revisionCycles = await _context.RevisionCycles.Include(rc => rc.DailyRevisions!).ThenInclude(dr => dr.Responsible).Where(rc => rc.HRIRevisionItemsId == itemId && rc.IsActive == true).ToListAsync();
                if(revisionCycles == null || revisionCycles.Count == 0) 
                {
                    response.Success = false;
                    response.Message = "No revision cycles found for the specified item.";
                    return response;
                }
                response.Data = revisionCycles.Select(rc => _mapper.Map<GetRevisionCyclesDto>(rc)).ToList();
                response.Success = true;
                response.Message = "Revision cycles retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while retrieving revision cycles: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            var response = new ServiceResponse<GetRevisionCyclesDto>();
            try
            {
                var revisionCycle = await _context.RevisionCycles.Include(rc => rc.DailyRevisions!).ThenInclude(dr => dr.Responsible).FirstOrDefaultAsync(rc => rc.RevisionCycleId == id);
                if(revisionCycle == null) 
                {
                    response.Success = false;
                    response.Message = "Revision cycle not found.";
                    return response;
                }
                response.Data = _mapper.Map<GetRevisionCyclesDto>(revisionCycle);
                response.Success = true;
                response.Message = "Revision cycle retrieved successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while retrieving the revision cycle: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto createRevisionCyclesDto)
        {
            var response = new ServiceResponse<GetRevisionCyclesDto>();
            try
            {
                var revisionCycle = _mapper.Map<RevisionCycles>(createRevisionCyclesDto);
                revisionCycle.HRIRevisionItemsId = itemId;
                await _context.RevisionCycles.AddAsync(revisionCycle);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetRevisionCyclesDto>(revisionCycle);
                response.Success = true;
                response.Message = "Revision cycle created successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while creating the revision cycle: {ex.Message}";
            }
            return response;
        }

        public async  Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                foreach (var createRevisionCyclesDto in listOfRevisionsCycles)
                {
                    var revisionCycle = _mapper.Map<RevisionCycles>(createRevisionCyclesDto);
                    revisionCycle.HRIRevisionItemsId = itemId;
                    await _context.RevisionCycles.AddAsync(revisionCycle);
                }
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "Revision cycles created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while creating the revision cycles: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var newDaily = new DailyRevisions
                {
                    RevisionCycleId = createDaily.EntityRelationId,
                    Day = createDaily.Day,
                    Month = createDaily.Month,
                    UserId = createDaily.UserId,
                    UserType = createDaily.UserType,
                    Status = createDaily.Status,
                    IsActive = true
                };
                await _context.DailyRevisions.AddAsync(newDaily);
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "Daily revision created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        public async  Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto updateRevisionCycleDto)
        {
            var response = new ServiceResponse<GetRevisionCyclesDto>();
            try
            {
                var revisionCycle = await _context.RevisionCycles.FirstOrDefaultAsync(rc => rc.RevisionCycleId == id);
                if(revisionCycle == null) 
                {
                    response.Success = false;
                    response.Message = "Revision cycle not found.";
                    return response;
                }
                _mapper.Map(updateRevisionCycleDto, revisionCycle);
                _context.RevisionCycles.Update(revisionCycle);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetRevisionCyclesDto>(revisionCycle);
                response.Success = true;
                response.Message = "Revision cycle updated successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while updating the revision cycle: {ex.Message}";
            }
            return response;
        }

        public async  Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var revisionCycle = await _context.RevisionCycles.FirstOrDefaultAsync(rc => rc.RevisionCycleId == id);
                if(revisionCycle == null) 
                {
                    response.Success = false;
                    response.Message = "Revision cycle not found.";
                    return response;
                }

                //soft delete related daily revisions
                var relatedDailyRevisions = await _context.DailyRevisions.Where(dr => dr.RevisionCycleId == id).ToListAsync();
                foreach (var dailyRevision in relatedDailyRevisions)
                {
                    dailyRevision.IsActive = false;
                }
                // Soft delete by setting IsActive to false
                revisionCycle.IsActive = false;
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "Revision cycle deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while deleting the revision cycle: {ex.Message}";
            }
            return response;
        }


    }
}
