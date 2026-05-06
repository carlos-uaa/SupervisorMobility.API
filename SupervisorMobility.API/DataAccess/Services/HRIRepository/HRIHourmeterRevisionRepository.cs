using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.NotificationDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIHourmeterRevisionRepository : IHRIHourmeterRevisionRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public HRIHourmeterRevisionRepository(
            SupervisorMobilityContext context,
            IMapper mapper,
            INotificationService notificationService
        )
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions()
        {
            var serviceResponse = new ServiceResponse<List<GetHourmeterRevisionDto>>();
            try
            {
                var hourmeterRevisions = await _context.HourmeterRevisions
                    .Include(hr => hr.DailyRevisions!).ThenInclude(dr=>dr.Responsible).Where(hr => hr.IsActive == true)
                    .ToListAsync();
                serviceResponse.Data = hourmeterRevisions.Select(hr => _mapper.Map<GetHourmeterRevisionDto>(hr)).ToList();
                serviceResponse.Message = "Hourmeter revisions retrieved successfully.";
                serviceResponse.Success = true;
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
                var hourmeterRevision = await _context.HourmeterRevisions.Include(hr => hr.DailyRevisions!).ThenInclude(dr=>dr.Responsible).Where(hr => hr.IsActive == true)
                    .FirstOrDefaultAsync(hr => hr.HriId == Hrid);
                serviceResponse.Data = _mapper.Map<GetHourmeterRevisionDto>(hourmeterRevision);
                serviceResponse.Message = hourmeterRevision != null ? "Hourmeter revision found." : "Hourmeter revision not found.";
                serviceResponse.Success = true;
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
                    .Include(hr => hr.DailyRevisions!)
                    .ThenInclude(dr=>dr.Responsible)
                    .FirstOrDefaultAsync(hr => hr.Id == id);
                serviceResponse.Data = _mapper.Map<GetHourmeterRevisionDto>(hourmeterRevision);
                serviceResponse.Message = hourmeterRevision != null ? "Hourmeter revision found." : "Hourmeter revision not found.";
                serviceResponse.Success = true;
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

        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var newDaily = new DailyRevisions
                {
                    HourmeterRevisionId = createDaily.EntityRelationId,
                    Day = createDaily.Day,
                    Month = createDaily.Month,
                    UserId = createDaily.UserId,
                    UserType = createDaily.UserType,
                    Status = createDaily.Status,
                    IsActive = true
                };
                await _context.DailyRevisions.AddAsync(newDaily);
                await _context.SaveChangesAsync();

                //creamos un nuevo registro en HRIHistoryActions para el historial de acciones
                var HRIId = (int)await _context.HourmeterRevisions.Where(h => h.Id == createDaily.EntityRelationId).Select(h => h.HriId).FirstOrDefaultAsync();
                var historyItem = new HRIHistoryItemDto
                {
                    Action = $"Created daily revision for Hourmeter, Day: {createDaily.Day}, Month: {createDaily.Month}, Status: {createDaily.Status}",
                    ActionDate = DateTime.Now,
                    ResponsibleUserId = createDaily.UserId,
                    HRIid = HRIId,
                    ActionType = "UPDATE"
                };
                await SendHistoryAction(historyItem);


                // Create notification if needed
                if (createDaily.Notification == true)
                {
                    var dto = new NotificationToCreateDto
                    {
                        MadeBy = "System",
                        NotificationType = createDaily.Title ?? "Revision of Hourmeter with NG",
                        NotificationText = createDaily.Message ?? "A new daily revision has been created.",
                        UserId = createDaily.To ?? 1,
                        IsAccepted = true,
                        IsActive = true,
                        EntryDate = DateTime.Now,
                        TargetRelation = HRIId
                    };
                    SpecialOptionsNotification options = new SpecialOptionsNotification
                    {
                        Email = createDaily.IsUrgent ? true : false,
                        WhatsApp = createDaily.IsUrgent ? true : false,
                        MicrosoftTeams = false,
                        type = "RevisionWithNG"
                    };
                    var created = await _notificationService.CreateNotificationAsync(dto, options);
                }


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

        public async Task<ServiceResponse<bool>> DeleteHourmeterRevision(int id)
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                var hourmeterRevision = await _context.HourmeterRevisions
                    .FirstOrDefaultAsync(hr => hr.Id == id);
                if (hourmeterRevision == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Hourmeter revision not found.";
                    return serviceResponse;
                }

                //soft delete related daily revisions
                var dailyRevisions = await _context.DailyRevisions
                    .Where(dr => dr.HourmeterRevisionId == id && dr.IsActive == true)
                    .ToListAsync();
                foreach (var daily in dailyRevisions)
                {
                    daily.IsActive = false; // Soft delete
                }
                // Soft delete by setting IsActive to false
                hourmeterRevision.IsActive = false;
                await _context.SaveChangesAsync();
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Hourmeter revision deleted successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> SendHistoryAction(HRIHistoryItemDto HRIHistoryItemDto)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var historyItem = _mapper.Map<HRIHistoryActions>(HRIHistoryItemDto);
                await _context.HRIHistoryActions.AddAsync(historyItem);
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "History action sent successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"Error sending history action: {ex.Message}");
                response.Success = false;
                response.Message = $"Error sending history action: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
                response.Data = false;
            }
            return response;
        }
    }
}
