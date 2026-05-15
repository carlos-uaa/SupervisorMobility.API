using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.NotificationDtos;


namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIRevisionCyclesRepository : IHRIRevisionCyclesRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public HRIRevisionCyclesRepository(SupervisorMobilityContext context, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
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
                    Year = createDaily.Year,
                    RevisionDate = new DateTime(createDaily.Year, createDaily.Month, createDaily.Day),
                    UserId = createDaily.UserId,
                    UserType = createDaily.UserType,
                    Status = createDaily.Status,
                    IsActive = true
                };
                await _context.DailyRevisions.AddAsync(newDaily);
                await _context.SaveChangesAsync();

                //creamos un nuevo registro en la tabla de historial de acciones para esta revisión diaria
                var revisionItem = await _context.RevisionCycles.AsNoTracking().Include(rc => rc.HRIRevisionItems).FirstOrDefaultAsync(rc => rc.RevisionCycleId == createDaily.EntityRelationId);
                var HRIId = (int)revisionItem!.HRIRevisionItems!.HriId;
                var historyItem = new HRIHistoryItemDto
                {
                    Action = $"Created daily revision for Item {revisionItem!.HRIRevisionItems!.RevisionPoint}  On Revision Shift: {revisionItem.Cycle}, Day: {createDaily.Day}, Month: {createDaily.Month}, Status: {createDaily.Status}",
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
                        NotificationType = createDaily.Title ?? "Revision with NG",
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
                    var created = await _notificationService.CreateNotificationAsync(dto, options, createDaily.CCPEmails);
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

        public async Task<ServiceResponse<bool>>DeleteRevisionCycleByHriId(int hriId,int cycle)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var revisionItems = await _context.HRIRevisionItems.AsNoTracking().Where(ri => ri.HriId == hriId).Select(ri => ri.ItemId).ToListAsync();
                foreach (var itemId in revisionItems)
                {
                    var revisionCycle = await _context.RevisionCycles.FirstOrDefaultAsync(rc => rc.HRIRevisionItemsId == itemId && rc.Cycle == cycle);
                    if (revisionCycle != null)
                    {
                        //soft delete related daily revisions
                        var relatedDailyRevisions = await _context.DailyRevisions.Where(dr => dr.RevisionCycleId == revisionCycle.RevisionCycleId).ToListAsync();
                        if(relatedDailyRevisions.Any())
                        {
                            foreach (var dailyRevision in relatedDailyRevisions)
                            {
                                dailyRevision.IsActive = false;
                            }
                        }
                       
                        // Soft delete by setting IsActive to false
                        revisionCycle.IsActive = false;
                    }
                }
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "Revision cycle(s) deleted successfully.";
                return response;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while deleting the revision cycle: {ex.Message}";
            }
            return response;
        } 

        public async Task<ServiceResponse<bool>> AddNewRevisionCycleToRevisionsItems(int hriId, CreateRevisionCyclesDto newRevisionCycle)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var revisionItems = await _context.HRIRevisionItems.AsNoTracking().Where(ri => ri.HriId == hriId).Select(ri => ri.ItemId).ToListAsync();
                foreach (var itemId in revisionItems)
                {
                    var revisionCycle = _mapper.Map<RevisionCycles>(newRevisionCycle);
                    revisionCycle.HRIRevisionItemsId = itemId;
                    await _context.RevisionCycles.AddAsync(revisionCycle);
                }
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "New revision cycle added to all related revision items successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while adding the new revision cycle: {ex.Message}";
            }
            return response;
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
