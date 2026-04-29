using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.NotificationDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRICyclesRepository : IHRICyclesRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public HRICyclesRepository(SupervisorMobilityContext context, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
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
                response.Message = ex.Message + ex.InnerException.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> CreateHRICyclesByHRIId(int hriId, List<CreateHRICyclesDto> createHRICycles)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var newHRICycles = createHRICycles.Select(c => _mapper.Map<HRICycles>(c)).ToList();
                foreach (var hriCycle in newHRICycles)
                {
                    hriCycle.HriId = hriId;
                    await _context.HRICycles.AddAsync(hriCycle);
                }
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "HRICycles created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
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
                    CycleId = createDaily.EntityRelationId,
                    Day = createDaily.Day,
                    Month = createDaily.Month,
                    Status = createDaily.Status,
                    UserId = createDaily.UserId,
                    UserType = createDaily.UserType,
                    IsActive = true

                };
                await _context.DailyRevisions.AddAsync(newDaily);
                await _context.SaveChangesAsync();


                // Create notification if needed
                if (createDaily.Notification == true)
                {
                    var dto = new NotificationToCreateDto
                    {
                        MadeBy = "System",
                        NotificationType = createDaily.Title ?? "Revision with NG",
                        NotificationText = createDaily.Message ?? "A new daily revision has been created.",
                        UserId = createDaily.To ?? 1,
                        IsAccepted = true,
                        IsActive = true,
                        EntryDate = DateTime.Now
                    };
                    SpecialOptionsNotification options = new SpecialOptionsNotification
                    {
                        Email = false,
                        WhatsApp = false,
                        MicrosoftTeams = false,
                        type = "Created HRI"
                    };
                    var created = await _notificationService.CreateNotificationAsync(dto, options);
                }

                response.Data = true;
                response.Success = true;
                response.Message = "Daily revision created successfully.";
            }
            catch(Exception ex)
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
                // Remove associated daily revisions first
                var dailyRevisions = _context.DailyRevisions.Where(d => d.CycleId == id);
                foreach (var daily in dailyRevisions)
                {
                    daily.IsActive = false; // Soft delete
                }
                hriCycle.IsActive = false; // Soft delete
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Success = true;
                response.Message = "HRICycle deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message + (ex.InnerException != null ? " Inner Exception: " + ex.InnerException.Message : "");
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id)
        {
            var response = new ServiceResponse<GetHRICyclesDto>();
            try
            {
                var hriCycle = await _context.HRICycles.Include(h=>h.Supervisor).Include(h=>h.Operator).Include(h => h.DailyRevisions).FirstOrDefaultAsync(h => h.CycleId == id && h.IsActive == true);
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
                var hriCycles = await _context.HRICycles.Include(h=>h.Supervisor).Include(h=>h.Operator).Include(h => h.DailyRevisions).Where(h => h.IsActive == true).ToListAsync();
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
    

