 using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIExcelDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;
using SupervisorMobility.API.Models.NotificationDtos;
using System.Drawing;


namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIRepository : IHRIRepository
    {
        private readonly SupervisorMobilityContext _context;
        private IHRIRevisionItemRepository _hriRevisionItemRepository;
        private IHRICyclesRepository _hriCyclesRepository;
        private IHRIHourmeterRevisionRepository _hriHourmeterRepository;
        private IHRIRevisionCyclesRepository _hriRevisionCyclesRepository;
        private IHRImagesService _hrimagesService;
        private readonly IMapper _mapper;

        private readonly INotificationService _notificationService;
        public HRIRepository(
            SupervisorMobilityContext context, IMapper mapper, IHRIRevisionItemRepository hriRevisionItemRepository,
            IHRICyclesRepository hriCyclesRepository,
            IHRIHourmeterRevisionRepository hriHourmeterRepository,
            IHRIRevisionCyclesRepository hriRevisionCyclesRepository,
            IHRImagesService hrimagesService,
            INotificationService notificationService
        )
        {
            _context = context;
            _mapper = mapper;
            _hriRevisionItemRepository = hriRevisionItemRepository;
            _hriCyclesRepository = hriCyclesRepository;
            _hriHourmeterRepository = hriHourmeterRepository;
            _hrimagesService = hrimagesService;
            _hriRevisionCyclesRepository = hriRevisionCyclesRepository;
            _notificationService = notificationService;
        }
        public async Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto newHRI)
        {
            var serviceResponse = new ServiceResponse<GetHRIDto>();
            try
            {
                //creamos el hri con los datos basicos y luego lo guardamos para obtener el HriId generado por la base de datos,
                //esto es necesario para relacionar las tablas de imagenes, items revisados, etc.
                var hri = new HRI
                {
                    HRILinesId = newHRI.HRILinesId,
                    HRIItemId = newHRI.HRIItemId,
                    ControlNumber = newHRI.ControlNumber,
                    HRIDockId = newHRI.HRIDockId,
                    Department = newHRI.Department,
                    SupervisorUserId = newHRI.SupervisorUserId,
                    SSVUserId = newHRI.SSVUserId,
                    PlantId = newHRI.PlantId,
                    AreaId = newHRI.AreaId,
                    IsActive = true,
                    CreationDate = DateTime.UtcNow
                };
                await _context.HRIs.AddAsync(hri);
                await _context.SaveChangesAsync();

                //agregamos las imagenes relacionadas al hri
                if (newHRI.Images != null && newHRI.Images.Count > 0)
                {
                    newHRI.Images.ForEach(img => img.HriId = hri.HriId);
                    await _hrimagesService.CreateHRImagesAsync(newHRI.Images);
                }


                //agregamos los items revisados relacionados al hri
                var numOfCycles = newHRI.HriCycles != null ? newHRI.HriCycles.Count : 0;
                if (newHRI.ItemsRevised != null && newHRI.ItemsRevised.Count > 0)
                {
                    var res = await _hriRevisionItemRepository.CreateHRIREvisionItemsByHRIId(hri.HriId, newHRI.ItemsRevised, numOfCycles);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Revision Items: {res.Message}";
                        return serviceResponse;
                    }

                }
                //agregamos las revisiones semanales relacionadas al hri
                if (newHRI.WeeklyRevisions != null && newHRI.WeeklyRevisions.Count > 0)
                {
                    var res = await CreateNewWeeeklyRevisions(newHRI.WeeklyRevisions);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Weekly Revisions: {res.Message}";
                        return serviceResponse;
                    }
                }

                //agregamos los ciclos relacionados al hri
                if (newHRI.HriCycles != null && newHRI.HriCycles.Count > 0)
                {
                    var res = await _hriCyclesRepository.CreateHRICyclesByHRIId(hri.HriId, newHRI.HriCycles);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Cycles: {res.Message}";
                        return serviceResponse;
                    }

                }

                //agregamos la revision del hourmeter relacionada al hri
                if (newHRI.HourmeterRevision != null)
                {
                    var hourmeterRevision = new CreateHourMeterRevisionDto
                    {
                        HriId = hri.HriId,
                        IsActive = true
                    };
                    var res = await _hriHourmeterRepository.AddHourmeterRevision(hourmeterRevision);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Hourmeter Revision: {res.Message}";
                        return serviceResponse;
                    }


                }
                //agregamos una accion al historial del hri indicando que se creo el hri
                var newHistoryItem = new HRIHistoryItemDto
                {
                    HRIid = hri.HriId,
                    Action = "HRI Created",
                    ActionDate = DateTime.UtcNow,
                    ResponsibleUserId = newHRI.SupervisorUserId ?? newHRI.SSVUserId,
                    ActionType = "CREATE"
                };
                await SendHistoryAction(newHistoryItem);


                // Create notification fot the supervisor
                var dto = new NotificationToCreateDto
                {
                    MadeBy = "System",
                    NotificationType = "Created HRI",
                    NotificationText = "A new HRI has been created with control number: " + hri.ControlNumber,
                    UserId = hri.SupervisorUserId ?? 1,
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

        public async Task<ServiceResponse<bool>> CreateNewWeeeklyRevisions(List<CreateWeeklyRevisionDto> weeklyRevisions)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                foreach (var weeklyRevision in weeklyRevisions)
                {
                    var newWeeklys = _mapper.Map<WeeklyRevisions>(weeklyRevision);
                    await _context.WeeklyRevisions.AddAsync(newWeeklys);
                }
                await _context.SaveChangesAsync();

                //agregamos una accion al historial del hri indicando que se crearon las revisiones semanales
                var HRIId = weeklyRevisions.First().HriId;
                var historyItem = new HRIHistoryItemDto
                {
                    HRIid = weeklyRevisions.First().HriId,
                    Action = "Weekly Revisions Created",
                    ActionDate = DateTime.UtcNow,
                    ResponsibleUserId = weeklyRevisions.First().UserId,
                    ActionType = "UPDATE"
                };
                await SendHistoryAction(historyItem);

                // Create notification if needed
                var weekly = weeklyRevisions.First();
                if (weekly.Notification == true)
                {
                    var dto = new NotificationToCreateDto
                    {
                        MadeBy = "System",
                        NotificationType = weekly.Title ?? "Weekly Revision with NG",
                        NotificationText = weekly.Message ?? "A new weekly revision has been created.",
                        UserId = weekly.To ?? 1,
                        IsAccepted = true,
                        IsActive = true,
                        EntryDate = DateTime.Now,
                        TargetRelation = HRIId
                    };
                    SpecialOptionsNotification options = new SpecialOptionsNotification
                    {
                        Email = weekly.IsUrgent ? true : false,
                        WhatsApp = weekly.IsUrgent ? true : false,
                        MicrosoftTeams = false,
                        type = "RevisionWithNG"
                    };
                    var created = await _notificationService.CreateNotificationAsync(dto, options, weekly.CCPEmails);
                }

                response.Success = true;
                response.Message = "Weekly revisions created successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating weekly revisions: {ex.Message}";
                response.Data = false;
            }
            return response;
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



                // Create notification fot the supervisor
                var dto = new NotificationToCreateDto
                {
                    MadeBy = "System",
                    NotificationType = "Deleted HRI",
                    NotificationText = "An HRI has been deleted with control number: " + hri.ControlNumber,
                    UserId = hri.SupervisorUserId ?? 1,
                    IsAccepted = true,
                    IsActive = true,
                    EntryDate = DateTime.Now
                };
                SpecialOptionsNotification options = new SpecialOptionsNotification
                {
                    Email = false,
                    WhatsApp = false,
                    MicrosoftTeams = false,
                    type = "Deleted HRI"
                };
                var created = await _notificationService.CreateNotificationAsync(dto, options);


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
                var hris = await _context.HRIs.AsNoTracking().Include(h => h.Line)
                    .Include(h => h.NameOfItem)
                    .Include(h => h.Dock)
                    .Include(h => h.Images)
                    .Include(h => h.ItemsRevised!)
                        .ThenInclude(ir => ir.Frequency)
                    .Include(h => h.ItemsRevised!.Where(ir => ir.IsActive == true))
                        .ThenInclude(ir => ir.Veredict)
                    .Include(h => h.ItemsRevised!.Where(ir => ir.IsActive == true))
                        .ThenInclude(ir => ir.RevisionMethod)
                    .Include(h => h.ItemsRevised!.Where(ir => ir.IsActive == true))
                        .ThenInclude(ir => ir.RevisionCycles!.Where(rc => rc.IsActive == true))
                            .ThenInclude(rc => rc.DailyRevisions!)
                                .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.WeeklyRevisions!.Where(wr => wr.IsActive == true))
                    .Include(h => h.HriCycles!.Where(hc => hc.IsActive == true))
                        .ThenInclude(c => c.DailyRevisions!)
                            .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.HriCycles!.Where(hc => hc.IsActive == true))
                        .ThenInclude(c => c.Operator)
                    .Include(h => h.HriCycles!.Where(hc => hc.IsActive == true))
                        .ThenInclude(c => c.Supervisor)
                    .Include(h => h.HourmeterRevision)
                        .ThenInclude(hr => hr.DailyRevisions!)
                            .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.Supervisor)
                    .Include(h => h.SSV)
                    .Include(h => h.Plant)
                    .Include(h => h.Area)
                    .ToListAsync();
                response.Data = hris.Select(h => _mapper.Map<GetHRIDto>(h)).ToList();
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
            var today = DateTime.Now;
            var dateQ = new DateTime(today.Year, today.Month, 1);
            
            try
            {
                var hri = await _context.HRIs.AsNoTracking().Include(h => h.Line)
                    .Include(h => h.NameOfItem)
                    .Include(h => h.Dock)
                    .Include(h => h.Images)
                    .Include(h => h.ItemsRevised!.Where(ir =>
                        ir.CreationDate.HasValue &&
                        ir.CreationDate < dateQ.AddMonths(1) &&
                        (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                    ))
                        .ThenInclude(ir => ir.Frequency)
                    .Include(h => h.ItemsRevised!.Where(ir =>                        
                        ir.CreationDate.HasValue &&
                        ir.CreationDate < dateQ.AddMonths(1) &&
                        (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                    ))
                        .ThenInclude(ir => ir.Veredict)
                    .Include(h => h.ItemsRevised!.Where(ir =>                        
                        ir.CreationDate.HasValue &&
                        ir.CreationDate < dateQ.AddMonths(1) &&
                        (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                    ))
                        .ThenInclude(ir => ir.RevisionMethod)
                    .Include(h => h.ItemsRevised!.Where(ir =>
                        ir.CreationDate.HasValue &&
                        ir.CreationDate < dateQ.AddMonths(1) &&
                        (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                    ))
                        .ThenInclude(ir => ir.RevisionCycles!.Where(rc => rc.IsActive == true))
                            .ThenInclude(rc => rc.DailyRevisions!.Where(dr=>dr.Month == DateTime.Now.Month && dr.Year == DateTime.Now.Year))
                                .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.WeeklyRevisions!.Where(wr => wr.IsActive == true && wr.Month == DateTime.Now.Month && wr.Year == DateTime.Now.Year))
                    .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                               ))
                        .ThenInclude(c => c.DailyRevisions!.Where(dr=>dr.Month == DateTime.Now.Month && dr.Year == DateTime.Now.Year))
                            .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                               ))
                        .ThenInclude(c => c.Operator)
                    .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                               ))
                        .ThenInclude(c => c.Supervisor)
                    .Include(h => h.HourmeterRevision)
                        .ThenInclude(hr => hr.DailyRevisions!.Where(dr=>dr.Month == DateTime.Now.Month && dr.Year == DateTime.Now.Year))
                            .ThenInclude(dr => dr.Responsible)
                    .Include(h => h.Supervisor)
                    .Include(h => h.SSV)
                    .Include(h => h.Plant)
                    .Include(h => h.Area)
                    .FirstOrDefaultAsync(h => h.HriId == id);
                if (hri == null)
                {
                    response.Success = false;
                    response.Message = "HRI not found.";
                    return response;
                }
                //filtramoos los ciclos para traer solo los activos, asi como los items revisados y las revisiones diarias relacionadas a esos ciclos e items

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


        public async Task<ServiceResponse<GetHRIDto>> GetDailyByMonthAndYear(int hriId, int month, int year)
        {
            var response = new ServiceResponse<GetHRIDto>();
            var dateQ = new DateTime(year, month, 1);
            try
            {
                var hri = await _context.HRIs.AsNoTracking()
                   .Include(h => h.Line)
                   .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(ir => ir.Frequency)
                   .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(ir => ir.Veredict)
                   .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(ir => ir.RevisionMethod)
                   .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(ir => ir.RevisionCycles!.Where(rc => rc.IsActive == true))
                           .ThenInclude(rc => rc.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                               .ThenInclude(dr => dr.Responsible)
                   .Include(h => h.WeeklyRevisions!.Where(wr => wr.IsActive == true && wr.Month == month && wr.Year == year))
                   .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(c => c.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                           .ThenInclude(dr => dr.Responsible)
                   .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(c => c.Operator)
                   .Include(h => h.HriCycles!.Where(hc =>
                                                    hc.CreationDate.HasValue &&
                                                    hc.CreationDate < dateQ.AddMonths(1) &&
                                                    (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                                ))
                       .ThenInclude(c => c.Supervisor)
                   .Include(h => h.HourmeterRevision)
                       .ThenInclude(hr => hr.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                           .ThenInclude(dr => dr.Responsible)
                   .FirstOrDefaultAsync(h => h.HriId == hriId);

                if (hri == null)
                {
                    response.Success = false;
                    response.Message = "HRI not found.";
                    return response;
                }
                //filtramoos los ciclos para traer solo los activos, asi como los items revisados y las revisiones diarias relacionadas a esos ciclos e items

                response.Data = _mapper.Map<GetHRIDto>(hri);
                response.Success = true;
                response.Message = "HRI retrieved successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving daily revisions: {ex.Message}";
            }
            return response;
        }

        public async Task<GetHRIDto>GetHRIByMonthAndYearFilter(int hriId,int month,int year)
        {
            var dateQ = new DateTime(year, month, 1);
            var hri = await _context.HRIs.AsNoTracking().Include(h => h.Line)
                 .Include(h => h.NameOfItem)
                 .Include(h => h.Dock)
                 .Include(h => h.Images)
                 .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                     .ThenInclude(ir => ir.Frequency)
                 .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                     .ThenInclude(ir => ir.Veredict)
                 .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                     .ThenInclude(ir => ir.RevisionMethod)
                 .Include(h => h.ItemsRevised!.Where(ir =>
                                                    ir.CreationDate.HasValue &&
                                                    ir.CreationDate < dateQ.AddMonths(1) &&
                                                    (!ir.DeletedDate.HasValue || ir.DeletedDate > dateQ)
                                                ))
                     .ThenInclude(ir => ir.RevisionCycles!.Where(rc => rc.IsActive == true))
                         .ThenInclude(rc => rc.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                             .ThenInclude(dr => dr.Responsible)
                 .Include(h => h.WeeklyRevisions!.Where(wr => wr.IsActive == true && wr.Month == month && wr.Year == year))
                 .Include(h => h.HriCycles!.Where(hc =>
                                                hc.CreationDate.HasValue &&
                                                hc.CreationDate < dateQ.AddMonths(1) &&
                                                (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                            ))
                     .ThenInclude(c => c.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                         .ThenInclude(dr => dr.Responsible)
                 .Include(h => h.HriCycles!.Where(hc =>
                                                hc.CreationDate.HasValue &&
                                                hc.CreationDate < dateQ.AddMonths(1) &&
                                                (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                            ))
                     .ThenInclude(c => c.Operator)
                 .Include(h => h.HriCycles!.Where(hc =>
                                                hc.CreationDate.HasValue &&
                                                hc.CreationDate < dateQ.AddMonths(1) &&
                                                (!hc.DeletedDate.HasValue || hc.DeletedDate > dateQ)
                                            ))
                     .ThenInclude(c => c.Supervisor)
                 .Include(h => h.HourmeterRevision)
                     .ThenInclude(hr => hr.DailyRevisions!.Where(dr => dr.Month == month && dr.Year == year))
                         .ThenInclude(dr => dr.Responsible)
                 .Include(h => h.Supervisor)
                 .Include(h => h.SSV)
                 .Include(h => h.Plant)
                 .Include(h => h.Area)
                 .FirstOrDefaultAsync(h => h.HriId == hriId);

                  return _mapper.Map<GetHRIDto>(hri);
        }

        public async Task<ServiceResponse<List<GetHRIToTableDto>>> GetAllHRITable()
        {
            var response = new ServiceResponse<List<GetHRIToTableDto>>();
            var hriTableList = new List<GetHRIToTableDto>();
            try
            {
                var hris = await _context.HRIs.AsNoTracking().Include(h => h.Line)
                    .Include(h => h.NameOfItem)
                    .Include(h => h.ItemsRevised!.Where(ir => ir.IsActive == true))
                    .Include(h => h.Images)
                    .Where(h => h.IsActive)
                    .ToListAsync();

                foreach (var hri in hris)
                {
                    var hriTableDto = new GetHRIToTableDto
                    {
                        HriId = hri.HriId,
                        Line = hri.Line,
                        NameOfItem = hri.NameOfItem,
                        ControlNumber = hri.ControlNumber,
                        Department = hri.Department,
                        RevisedItemsCount = hri.ItemsRevised != null ? hri.ItemsRevised.Count : 0,
                        ImagesCount = hri.Images != null ? hri.Images.Count : 0,
                        IsActive = hri.IsActive,
                        CreationDate = hri.CreationDate
                    };
                    hriTableList.Add(hriTableDto);
                }
                response.Data = hriTableList;
                response.Success = true;
                response.Message = "HRIs for table retrieved successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving HRIs for table: {ex.Message}";
            }
            return response;

        }

        public async Task<ServiceResponse<bool>> UpdateHRI(int id, UpdateHRIDto updatedHRI)
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
                hri.HRILinesId = updatedHRI.HRILinesId;
                hri.HRIItemId = updatedHRI.HRIItemId;
                hri.ControlNumber = updatedHRI.ControlNumber;
                hri.HRIDockId = updatedHRI.HRIDockId;
                hri.Department = updatedHRI.Department;
                hri.SupervisorUserId = updatedHRI.SupervisorUserId;
                hri.SSVUserId = updatedHRI.SSVUserId;

                await _context.SaveChangesAsync();

                //actualizamos los ciclos relacionados al hri
                if (updatedHRI.HRICycles != null && updatedHRI.HRICycles.Count > 0)
                {
                    foreach (var cycle in updatedHRI.HRICycles)
                    {
                        //si el ciclo tiene un id, significa que ya existe en la base de datos y solo se actualiza, si no tiene id, se crea uno nuevo, si el ciclo tiene el campo Deleted en true, se elimina de la base de datos
                        if (cycle.Deleted == true && cycle.CycleId != 0)
                        {
                            var res = await _hriCyclesRepository.DeleteHRICycle(cycle.CycleId);
                            if (res.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error deleting HRI Cycle with id {cycle.CycleId}: {res.Message}";
                                response.Data = false;
                                return response;
                            }
                            var res2 = await _hriRevisionCyclesRepository.DeleteRevisionCycleByHriId(hri.HriId, cycle.Cycle);
                            if (res2.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error deleting HRI Revision Cycle with id {cycle.CycleId}: {res2.Message}";
                                response.Data = false;
                                return response;
                            }

                            //agregamos una accion al historial del hri indicando que se elimino el ciclo
                            var historyItem = new HRIHistoryItemDto
                            {
                                HRIid = hri.HriId,
                                Action = $"HRI Cycle {cycle.Cycle} deleted",
                                ActionDate = DateTime.Now,
                                ResponsibleUserId = updatedHRI.SupervisorUserId ?? updatedHRI.SSVUserId,
                                ActionType = "DELETE"
                            };
                            await SendHistoryAction(historyItem);

                            continue;
                        }
                        //si el id es diferente de 0 y el campo deleted es null o false actualizamos el ciclo
                        else if (cycle.CycleId != 0 && (cycle.Deleted == null || cycle.Deleted == false))
                        {
                            var cycleToUpdate = _mapper.Map<UpdateHRICycleDto>(cycle);
                            var res = await _hriCyclesRepository.UpdateHRICycle(cycle.CycleId, cycleToUpdate);
                            if (res.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error updating HRI Cycle with id {cycle.CycleId}: {res.Message}";
                                response.Data = false;
                                return response;
                            }
                            //agregamos una accion al historial del hri indicando que se actualizo el ciclo
                            var historyItem = new HRIHistoryItemDto
                            {
                                HRIid = hri.HriId,
                                Action = $"HRI Cycle {cycle.Cycle} updated",
                                ActionDate = DateTime.Now,
                                ResponsibleUserId = updatedHRI.SupervisorUserId ?? updatedHRI.SSVUserId,
                                ActionType = "UPDATE"
                            };
                            continue;
                        }
                        //si el id es 0 creamos un nuevo ciclo relacionado al hri
                        else if (cycle.CycleId == 0 && cycle.Deleted != true)
                        {
                            var cycleToCreate = _mapper.Map<CreateHRICyclesDto>(cycle);
                            cycleToCreate.HriId = hri.HriId;
                            cycleToCreate.IsActive = true;
                            var res = await _hriCyclesRepository.CreateHRICycle(cycleToCreate);
                            if (res.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error creating HRI Cycle: {res.Message}";
                                response.Data = false;
                                return response;
                            }
                            var res2 = await _hriRevisionCyclesRepository.AddNewRevisionCycleToRevisionsItems(hri.HriId, new CreateRevisionCyclesDto { Cycle = cycle.Cycle, IsActive = true });
                            if (res2.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error adding new revision cycle to related revision items: {res2.Message}";
                                response.Data = false;
                                return response;
                            }

                            continue;
                        }
                    }
                }

                //actualizamos los items revisados relacionados al hri
                if (updatedHRI.RevisionItems != null && updatedHRI.RevisionItems.Count > 0)
                {
                    foreach (var item in updatedHRI.RevisionItems)
                    {
                        // si el item tiene el campo Deleted en true, se elimina de la base de datos
                        if (item.Deleted == true)
                        {
                            var res = await _hriRevisionItemRepository.DeleteHRIRevisionItem(item.ItemId);
                            if (res.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error deleting HRI Revision Item with id {item.ItemId}: {res.Message}";
                                response.Data = false;
                                return response;
                            }
                            //agregamos una accion al historial del hri indicando que se elimino el item revisado
                            var historyItem = new HRIHistoryItemDto
                            {
                                HRIid = hri.HriId,
                                Action = $"HRI Revision Item {item.RevisionPoint} deleted",
                                ActionDate = DateTime.Now,
                                ResponsibleUserId = updatedHRI.SupervisorUserId ?? updatedHRI.SSVUserId,
                                ActionType = "DELETE"
                            };
                            await SendHistoryAction(historyItem);
                            continue;

                        }
                        //si el id es diferente de 0 y el campo deleted es null o false actualizamos el item
                        else if (item.ItemId != 0 && (item.Deleted == null || item.Deleted == false))
                        {
                            var itemToUpdate = _mapper.Map<UpdateHRIRevisionItemDto>(item);
                            var validationRes = await _hriRevisionItemRepository.ValidateItemForUpdate(item.ItemId, itemToUpdate);
                            if (validationRes.Success == true)
                            {
                                var res = await _hriRevisionItemRepository.UpdateHRIRevisionItem(item.ItemId, itemToUpdate);
                                if (res.Success == false)
                                {
                                    response.Success = false;
                                    response.Message = $"Error updating HRI Revision Item with id {item.ItemId}: {res.Message}";
                                    response.Data = false;
                                    return response;
                                }
                                //agregamos una accion al historial del hri indicando que se actualizo el item revisado
                                var historyItem = new HRIHistoryItemDto
                                {
                                    HRIid = hri.HriId,
                                    Action = $"HRI Revision Item {item.RevisionPoint} updated",
                                    ActionDate = DateTime.Now,
                                    ResponsibleUserId = updatedHRI.SupervisorUserId ?? updatedHRI.SSVUserId,
                                    ActionType = "UPDATE"
                                };
                                await SendHistoryAction(historyItem);
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //si el id es 0 creamos un nuevo item relacionado al hri
                        else if (item.ItemId == 0)
                        {
                            var itemToCreate = _mapper.Map<CreateHRIRevisionItemDto>(item);
                            itemToCreate.HriId = hri.HriId;
                            itemToCreate.IsActive = true;
                            var res = await _hriRevisionItemRepository.CreateHRIRevisionItem(itemToCreate);
                            if (res.Success == false)
                            {
                                response.Success = false;
                                response.Message = $"Error creating HRI Revision Item: {res.Message}";
                                response.Data = false;
                                return response;
                            }

                            continue;
                        }


                    }
                }

                // actualizamos las imagenes
                if (updatedHRI.Images != null && updatedHRI.Images.Count > 0)
                {
                    updatedHRI.Images.ForEach(img => img.HriId = hri.HriId);
                    await _hrimagesService.UpdateHRImageAsync(updatedHRI.Images);
                }
                

                // Create notification fot the supervisor
                var dto = new NotificationToCreateDto
                {
                    MadeBy = "System",
                    NotificationType = "Update HRI",
                    NotificationText = "An HRI has been updated with control number: " + hri.ControlNumber,
                    UserId = hri.SupervisorUserId ?? 1,
                    IsAccepted = true,
                    IsActive = true,
                    EntryDate = DateTime.Now
                };
                SpecialOptionsNotification options = new SpecialOptionsNotification
                {
                    Email = false,
                    WhatsApp = false,
                    MicrosoftTeams = false,
                    type = "Update HRI"
                };
                var created = await _notificationService.CreateNotificationAsync(dto, options);
                
                response.Success = true;
                response.Message = "HRI updated successfully.";
                response.Data = true;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating HRI: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
                response.Data = false;
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

        public async Task<ServiceResponse<List<GetHRIHistoryActionDto>>> GetHRIHistory(int hriId)
        {
            var response = new ServiceResponse<List<GetHRIHistoryActionDto>>();
            try
            {
                var historyActions = await _context.HRIHistoryActions.AsNoTracking()
                    .Where(ha => ha.HRIid == hriId)
                    .Include(ha => ha.Responsible)
                    .OrderByDescending(ha => ha.ActionDate)
                    .ToListAsync();
                response.Data = historyActions.Select(ha => _mapper.Map<GetHRIHistoryActionDto>(ha)).ToList();
                response.Success = true;
                response.Message = "HRI history retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving HRI history: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
            }
            return response;
        }

        // Endpoints para el Dashboard del HRI
        public async Task<ServiceResponse<HriKpis>> GetHriKPIs()
        {
            var response = new ServiceResponse<HriKpis>();
            try
            {
                HriKpis hriKpis = new HriKpis();
                hriKpis.TotalHri = await _context.HRIs.CountAsync(h => h.IsActive);
                hriKpis.TodayRevisions = await _context.DailyRevisions.CountAsync(ri => ri.IsActive.Value && ri.Day == DateTime.Now.Day && ri.Month == DateTime.Now.Month);

                int cycle1NGCount = await _context.HRICycles.Where(hc => hc.Cycle == 1).SelectMany(hc => hc.DailyRevisions).CountAsync(dr => dr.Status == "NG");
                int cycle2NGCount = await _context.HRICycles.Where(hc => hc.Cycle == 2).SelectMany(hc => hc.DailyRevisions).CountAsync(dr => dr.Status == "NG");
                int cycle3NGCount = await _context.HRICycles.Where(hc => hc.Cycle == 3).SelectMany(hc => hc.DailyRevisions).CountAsync(dr => dr.Status == "NG");
                hriKpis.CriticCycle = Math.Max(cycle1NGCount, Math.Max(cycle2NGCount, cycle3NGCount));

                hriKpis.GlobalHealth =
                    await _context.DailyRevisions.CountAsync(dr => dr.HourmeterRevisionId == null && dr.Status.ToUpper() == "OK") * 100.0 /
                    await _context.DailyRevisions.CountAsync(dr => dr.HourmeterRevisionId == null);

                response.Data = hriKpis;
                response.Success = true;
                response.Message = "HRI KPIs retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving HRI KPIs: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
            }
            return response;
        }

        public async Task<ServiceResponse<LinesChartData>> GetLinesChartData(int areaId)
        {
            var response = new ServiceResponse<LinesChartData>();
            try
            {
                // Traer todos los HRI con sus ciclos y revisiones
                var hris = await _context.HRIs
                    .Include(h => h.Line)
                    .Include(h => h.Area)
                    .Include(h => h.HriCycles)
                        .ThenInclude(c => c.DailyRevisions)
                    .Include(h => h.ItemsRevised)
                        .ThenInclude(ir => ir.RevisionCycles)
                            .ThenInclude(rc => rc.DailyRevisions)
                    .Where(h => areaId == 0 || h.Area.AreaId == areaId)
                    .ToListAsync();

                // Labels: nombres de línea únicos
                var _barChartLabels = hris
                    .Select(h => h.Line?.LineName)
                    .Distinct()
                    .ToArray();

                // Agrupamos por LineName y contamos por Status
                var groupedData = hris
                    .GroupBy(h => h.Line?.LineName ?? "Sin Línea")
                    .Select(g => new
                    {
                        LineName = g.Key,
                        StatusCounts = g.SelectMany(h =>
                            // Ruta 1: Revisiones generales del ciclo HRI
                            h.HriCycles.SelectMany(c => c.DailyRevisions)
                            .Concat(
                                // Ruta 2: Revisiones específicas de los ítems
                                h.ItemsRevised.SelectMany(ir => ir.RevisionCycles.SelectMany(rc => rc.DailyRevisions))
                            )
                        )
                        // Opcional: Filtrar por IsActive si quieres que coincida con tu KPI
                        .Where(dr => dr.IsActive == true)
                        .GroupBy(dr => dr.Status)
                        .ToDictionary(x => x.Key ?? "Pendiente", x => x.Count())
                    })
                    .ToList();

                // Construimos las series para cada Status esperado
                var _barChartSeries = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = "OK",
                        Data = groupedData.Select(g => g.StatusCounts.ContainsKey("Ok") ? (double)g.StatusCounts["Ok"] : 0).ToArray()
                    },
                    new ChartSeries
                    {
                        Name = "NG",
                        Data = groupedData.Select(g => g.StatusCounts.ContainsKey("NG") ? (double)g.StatusCounts["NG"] : 0).ToArray()
                    },
                    new ChartSeries
                    {
                        Name = "NA",
                        Data = groupedData.Select(g => g.StatusCounts.ContainsKey("NA") ? (double)g.StatusCounts["NA"] : 0).ToArray()
                    }
                };

                var linesChartData = new LinesChartData
                {
                    Labels = _barChartLabels,
                    Series = _barChartSeries
                };

                response.Data = linesChartData;
                response.Success = true;
                response.Message = "Lines Chart Data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving Data for the Lines Chart: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
            }
            return response;
        }

        public async Task<ServiceResponse<GeneralStatusChartData>> GetGeneralStatusChartData(int areaId)
        {
            var response = new ServiceResponse<GeneralStatusChartData>();
            try
            {
                // Traer todas las revisiones
                var allRevisions = await _context.DailyRevisions
                    .Where(dr => dr.HourmeterRevisionId == null)
                    .Select(dr => new { dr.Status })
                    .ToListAsync();

                // Contar por Status
                var statusCounts = allRevisions
                    .GroupBy(r => r.Status?.ToUpper() ?? "PENDIENTE")
                    .ToDictionary(g => g.Key, g => g.Count());

                // Armar los datos para el donut
                var _donutData = new double[]
                {
                    statusCounts.GetValueOrDefault("OK", 0),
                    statusCounts.GetValueOrDefault("NG", 0),
                    statusCounts.GetValueOrDefault("NA", 0)
                };

                var _donutLabels = new string[]
                {
                    "Correcto (OK)",
                    "Incorrecto (NG)",
                    "No Aplica (NA)"
                };

                var generalStatusChartData = new GeneralStatusChartData
                {
                    Data = _donutData,
                    Labels = _donutLabels
                };

                response.Success = true;
                response.Data = generalStatusChartData;
                response.Message = "General Status Chart Data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving Data for the General Status Chart: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<HriRecentRevisionsDto>>> GetRecentRevisions(int areaId, string? filter)
        {
            var response = new ServiceResponse<List<HriRecentRevisionsDto>>();
            try
            {
                var hris = await _context.HRIs
                    .Include(h => h.Line)
                    .Include(h => h.Area)
                    .Include(h => h.HriCycles)
                        .ThenInclude(c => c.DailyRevisions)
                    .Include(h => h.ItemsRevised)
                        .ThenInclude(ir => ir.RevisionCycles)
                            .ThenInclude(rc => rc.DailyRevisions)
                    .Where(h => (areaId == 0 || h.Area.AreaId == areaId) &&
                                (string.IsNullOrEmpty(filter) || h.Line.LineName.Contains(filter)))
                    .AsNoTracking().ToListAsync();

                var revisionsFromCycles = hris.SelectMany(h => h.HriCycles?
                    .SelectMany(c => c.DailyRevisions?.Select(r => new HriRecentRevisionsDto
                    {
                        HriId = h.HriId,
                        HRIName = h.ControlNumber,
                        RevisionPointName = "N/A",
                        Line = h.Line?.LineName ?? "N/A",
                        Cycle = c.Cycle,
                        Day = r.Day,
                        Month = GetMonth(r.Month),
                        Status = r.Status ?? string.Empty,
                        RevisionId = r.RevisionId
                    }) ?? new List<HriRecentRevisionsDto>()) ?? new List<HriRecentRevisionsDto>());

                // RUTA 2: Revisiones desde los ítems (ItemsRevised -> RevisionCycles)
                var revisionsFromItems = hris.SelectMany(h => h.ItemsRevised?
                    .SelectMany(ir => ir.RevisionCycles?
                        .SelectMany(rc => rc.DailyRevisions?.Select(r => new HriRecentRevisionsDto
                        {
                            HriId = h.HriId,
                            HRIName = h.ControlNumber,
                            RevisionPointName = ir.RevisionPoint,
                            Line = h.Line?.LineName ?? "N/A",
                            Cycle = rc.Cycle,
                            Day = r.Day,
                            Month = GetMonth(r.Month),
                            Status = r.Status ?? string.Empty,
                            RevisionId = r.RevisionId
                        }) ?? new List<HriRecentRevisionsDto>()) ?? new List<HriRecentRevisionsDto>()) ?? new List<HriRecentRevisionsDto>());

                // Unimos ambas listas, ordenamos y asignamos a la respuesta
                response.Data = revisionsFromCycles
                    .Concat(revisionsFromItems)
                    .OrderByDescending(x => x.RevisionId)
                    .Take(10).ToList();
                response.Success = true;
                response.Message = "History action sent successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending history action: {ex.Message}");
                response.Success = false;
                response.Message = $"Error sending history action: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";
            }
            return response;
        }

        private string GetMonth(int monthNumber)
        {
            return monthNumber switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => "Mes desconocido"
            };
        }
        
        public async Task<ServiceResponse<byte[]>> CreateExcelHriFile(int hriId, int month, int year)
        {
            var serviceResponse = new ServiceResponse<byte[]>();
            try
            {
                //obtenemos el hri con su informacion completa
                var hriResponse = await GetHRIByMonthAndYearFilter(hriId, month, year);

                ExcelPackage.License.SetNonCommercialPersonal("SupervisorMobility");
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("HRI");
                int diasMes = DateTime.DaysInMonth(year, month);
                //establecemos el ancho de todas las columnas
                ws.Column(1).Width = 5.29; //A 
                ws.Column(2).Width = 29.29; //B 
                ws.Column(3).Width = 24.29; //C
                ws.Column(4).Width = 24.29; //D
                ws.Column(5).Width = 20; //E
                ws.Column(6).Width = 5.27; //F
                ws.Column(7).Width = 7.29; //G

                //primero le damos el ancho de las columnas de los dias
                for (int i = 1; i <= diasMes; i++)
                {
                    ws.Column(7 + i).Width = 3.29; //colocamos el ancho de las columnas de los dias
                }

                
                CreateRevisionItemSection(ws, hriResponse, month, year);
                CreateHeaderSection(ws, hriResponse, diasMes);
                

                serviceResponse.Data = package.GetAsByteArray();
                serviceResponse.Success = true;
                serviceResponse.Message = "Excel file created successfully.";

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error creating Excel file: {ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")}";

            }

            return serviceResponse;
        }

        private void CreateHeaderSection(ExcelWorksheet ws, GetHRIDto hriData, int diasMes)
        {
            ws.Cells["A1:B3"].Merge = true;

            // Ejemplo: insertar imagen desde archivo local
            var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "icon", "logo.png");
            if (File.Exists(iconPath))
            {
                var picture = ws.Drawings.AddPicture("LogoEmpresa", new FileInfo(iconPath));

                // Posición: fila 1, columna 1 (base 0 internamente)
                picture.SetPosition(0, 0, 0, 0);

                // Tamaño en pixeles
                picture.SetSize(240, 70);
            }


            ws.Cells["C3:E4"].Merge = true;
            ws.Cells["C3:E4"].Value = "PROCEDIMIENTO DE REVISION INICIAL Y HOJA DE REGISTRO";
            ws.Cells["C3:E4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["C3:E4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            ws.Cells["C3:E4"].Style.Font.UnderLineType = OfficeOpenXml.Style.ExcelUnderLineType.Single;
            ws.Cells["C3:E4"].Style.Font.UnderLine = true;
            ws.Cells["C3:E4"].Style.Font.Bold = true;
            ws.Cells["C3:E4"].Style.Font.Size = 14;


            ws.Cells["F1:H2"].Merge = true;
            ws.Cells["F1:H2"].Value = "Nombre del equipo";
            ws.Cells["F1:H2"].Style.Font.Size = 10;
            ws.Cells["F1:H2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["F1:H2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["F1:H2"]);

            ws.Cells["I1:M2"].Merge = true;
            ws.Cells["I1:M2"].Value = hriData.NameOfItem != null ? hriData.NameOfItem.Name : "N/A";
            ws.Cells["I1:M2"].Style.Font.Size = 10;
            ws.Cells["I1:M2"].Style.Font.Bold = true;
            ws.Cells["I1:M2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["I1:M2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["I1:M2"]);

            ws.Cells["N1:Q2"].Merge = true;
            ws.Cells["N1:Q2"].Value = "Numero";
            ws.Cells["N1:Q2"].Style.Font.Size = 10;
            ws.Cells["N1:Q2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["N1:Q2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["N1:Q2"]);

            ws.Cells["R1:AB2"].Merge = true;
            ws.Cells["R1:AB2"].Value = hriData.ControlNumber != null ? hriData.ControlNumber : "N/A";
            ws.Cells["R1:AB2"].Style.Font.Size = 24;
            ws.Cells["R1:AB2"].Style.Font.Bold = true;
            ws.Cells["R1:AB2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["R1:AB2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["R1:AB2"]);


            ws.Cells["AD1:AK1"].Merge = true;
            ws.Cells["AD1:AK1"].Value = "Departamento que realiza";
            ws.Cells["AD1:AK1"].Style.Font.Size = 11;
            ws.Cells["AD1:AK1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["AD1:AK1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["AD1:AK1"]);

            ws.Cells["AD2:AK2"].Merge = true;
            ws.Cells["AD2:AK2"].Value = hriData.Department != null ? hriData.Department : "N/A";
            ws.Cells["AD2:AK2"].Style.Font.Size = 11;
            ws.Cells["AD2:AK2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["AD2:AK2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["AD2:AK2"]);

            ws.Cells["AD3:AK3"].Merge = true;
            ws.Cells["AD3:AK3"].Value = "Supervisor";
            ws.Cells["AD3:AK3"].Style.Font.Size = 11;
            ws.Cells["AD3:AK3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["AD3:AK3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["AD3:AK3"]);

            ws.Cells["AD4:AK5"].Merge = true;
            ws.Cells["AD4:AK5"].Value = hriData.SSV != null ? hriData.SSV.Name : "N/A";
            ws.Cells["AD4:AK5"].Style.Font.Size = 11;
            ws.Cells["AD4:AK5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["AD4:AK5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["AD4:AK5"]);


            ws.Cells["F3:G3"].Merge = true;
            ws.Cells["F3:G3"].Value = "Nombre de linea";
            ws.Cells["F3:G3"].Style.Font.Size = 10;
            ws.Cells["F3:G3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["F3:G3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["F3:G3"]);

            ws.Cells["H3:Q3"].Merge = true;
            ws.Cells["H3:Q3"].Value = hriData.Line != null ? hriData.Line.LineName : "N/A";
            ws.Cells["H3:Q3"].Style.Font.Size = 16;
            ws.Cells["H3:Q3"].Style.Font.Bold = true;
            ws.Cells["H3:Q3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["H3:Q3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["H3:Q3"]);

            ws.Cells["R3:W3"].Merge = true;
            ws.Cells["R3:W3"].Value = "Dock asignado";
            ws.Cells["R3:W3"].Style.Font.Size = 12;
            ws.Cells["R3:W3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["R3:W3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["R3:W3"]);

            ws.Cells["X3:AB3"].Merge = true;
            ws.Cells["X3:AB3"].Value = hriData.Dock != null ? hriData.Dock.DockName : "N/A";
            ws.Cells["X3:AB3"].Style.Font.Size = 18;
            ws.Cells["X3:AB3"].Style.Font.Bold = true;
            ws.Cells["X3:AB3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["X3:AB3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            setAllBorders(ws.Cells["X3:AB3"]);


            // Unimos las celadas para las imagenes
            var lastCol = ConvertirNumeroALetraColumna(diasMes + 7); // Columna AK
            ws.Cells[$"A9:{lastCol}24"].Merge = true;
            ws.Cells[$"A9:{lastCol}24"].Value = "";
            setOutBorders(ws.Cells[$"A9:{lastCol}26"]);

            // Ejemplo: insertar imagen desde archivo local
            var imageAmount = hriData.Images != null ? hriData.Images.Count : 0; // default 9
            var rowStart = 13; // Fila 9
            var colStart = 1;
            var colEnd = diasMes + 7; // Columna AK
            var maxWidth = 200;
            var maxHeight = 158;
            var gap = 10;

            PositionarImagenesEnRango(ws, rowStart, colStart, colEnd, imageAmount, maxWidth, maxHeight, gap, hriData.Images);
        }
        
        private void PositionarImagenesEnRango(ExcelWorksheet ws, int rowStart, 
            int colStart, int colEnd, int imageAmount, int maxWidth, int maxHeight, int gap, List<HRImages> images)
        {
            // Calcular ancho total disponible en píxeles
            double anchoTotal = 0;
            for (int i = colStart; i <= colEnd; i++)
            {
                anchoTotal += ws.Column(i).Width;
            }
            double pixelesTotal = anchoTotal * 7;

            // Ancho por imagen (reducido por los gaps entre imágenes)
            double tamanoPorImagen = (pixelesTotal - (gap * (imageAmount - 1))) / imageAmount;
            if (tamanoPorImagen > maxWidth) tamanoPorImagen = maxWidth;

            
            // Posicionar cada imagen
            for (int i = 0; i < imageAmount; i++)
            {
                var imgsrc = images != null && images.Count > i ? images[i].ImageUrl : null;
                if (string.IsNullOrEmpty(imgsrc)) continue;
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), imgsrc);
                if (!File.Exists(imagePath))
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "icon", "logo.png");   
                }

                // Posición X acumulada en píxeles
                double posicionXTotal = i * (tamanoPorImagen + gap);
                
                // Convertir píxeles totales a columna + offsetX
                double pixelesAcumulados = 0;
                int columnaActual = colStart;
                
                for (int col = colStart; col <= colEnd; col++)
                {
                    double anchoColumna = ws.Column(col).Width * 7;
                    
                    if (pixelesAcumulados + anchoColumna > posicionXTotal)
                    {
                        columnaActual = col;
                        break;
                    }
                    
                    pixelesAcumulados += anchoColumna;
                }
                
                double offsetX = posicionXTotal - pixelesAcumulados;
                
                var picture = ws.Drawings.AddPicture($"Imagen{i}", new FileInfo(imagePath));
                picture.SetPosition(rowStart - 1, 0, columnaActual - 1, (int)offsetX);
                picture.SetSize((int)tamanoPorImagen, (tamanoPorImagen > maxHeight) ? maxHeight : (int)tamanoPorImagen);
            }
        }

        private void CreateRevisionItemSection(ExcelWorksheet ws, GetHRIDto hriData, int month, int year)
        {

            var varMes = new DateTime(year, month, 1).ToString("MMMM").ToUpper();
            var varAnio = year;
            var weeksOfMonth = GetMonthWeeks(month, varAnio);
            var satAndSunDays = GetSaturdaysAndSundays(month, varAnio);
            //calcular el dia en que empezo este mes
            var fecha = new DateTime(varAnio, month, 1);
            var diaInicio = fecha.DayOfWeek;   // Friday, Monday, etc.

            var totalLineas = 1;
            var totalItemsRevision = hriData.ItemsRevised!.Count;
            var totalTurnos = hriData.HriCycles!.Count;

            var bodyFontSize = 10;
            var headerFontSize = 11;

            #region titulos de tabla
            //colocamos el nombre de cada columna
            //item
            var item = ws.Cells["A27:A28"];
            item.Merge = true;
            item.Value = "ITEM";
            item.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            item.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            item.Style.Font.Bold = true;
            item.Style.Font.Size = headerFontSize;

            //puntos de revision
            var puntosRevision = ws.Cells["B27:B28"];
            puntosRevision.Merge = true;
            puntosRevision.Value = "PUNTOS DE REVISION";
            puntosRevision.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            puntosRevision.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            puntosRevision.Style.Font.Bold = true;
            puntosRevision.Style.Font.Size = headerFontSize;

            //metodo de revision
            var metodoRevision = ws.Cells["C27:C28"];
            metodoRevision.Merge = true;
            metodoRevision.Value = "METODO DE REVISION";
            metodoRevision.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            metodoRevision.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            metodoRevision.Style.Font.Bold = true;
            metodoRevision.Style.Font.Size = headerFontSize;

            //criterio
            var criterio = ws.Cells["D27:D28"];
            criterio.Merge = true;
            criterio.Value = "CRITERIO";
            criterio.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            criterio.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            criterio.Style.Font.Bold = true;
            criterio.Style.Font.Size = headerFontSize;

            //frecuencia
            var frecuencia = ws.Cells["E27:E28"];
            frecuencia.Merge = true;
            frecuencia.Value = "FRECUENCIA";
            frecuencia.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            frecuencia.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            frecuencia.Style.Font.Bold = true;
            frecuencia.Style.Font.Size = headerFontSize;

            // mes
            var mes = ws.Cells["F27"];
            mes.Value = "Mes";
            mes.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            mes.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            mes.Style.Font.Bold = true;
            mes.Style.Font.Size = headerFontSize;

            //dia 
            var dia = ws.Cells["F28"];
            dia.Merge = true;
            dia.Value = "Dia";
            dia.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            dia.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            dia.Style.Font.Bold = true;
            dia.Style.Font.Size = headerFontSize;

            //turno
            var turno = ws.Cells["G27:G28"];
            turno.Merge = true;
            turno.Value = "TURNO";
            turno.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            turno.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            turno.Style.Font.Bold = true;
            turno.Style.Font.Size = headerFontSize;

            //colocamos el encabezado del mes y las celdas de los dias
            int diasMes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var encabezadoMes = ws.Cells[27, 8, 27, 8 + (diasMes - 1)];
            encabezadoMes.Merge = true;
            encabezadoMes.Value = $"REGISTRO DE REALIZACION DE REVISION DE MES :{varMes} ANIO {varAnio}";
            encabezadoMes.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            encabezadoMes.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            encabezadoMes.Style.Font.Bold = true;
            encabezadoMes.Style.Font.Size = headerFontSize;

            for (int i = 1; i <= diasMes; i++)
            {
                var diaCell = ws.Cells[28, 7 + i];
                diaCell.Value = i;
                diaCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                diaCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                diaCell.Style.Font.Bold = true;
                diaCell.Style.Font.Size = headerFontSize;
            }

            #endregion

            //colocamos los items de revision y los turnos
            var filaInicioItems = 29;

            for (int i = 1; i <= totalItemsRevision; i++)
            {
                //numero del item
                var itemCell = ws.Cells[$"A{filaInicioItems}:A{filaInicioItems + totalTurnos - 1}"];
                itemCell.Merge = true;
                itemCell.Value = $"{hriData.ItemsRevised![i - 1].ItemNumber}";
                itemCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                itemCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                itemCell.Style.Font.Size = bodyFontSize;
                //punto de revision
                var itemRevision = ws.Cells[$"B{filaInicioItems}:B{filaInicioItems + totalTurnos - 1}"];
                itemRevision.Merge = true;
                itemRevision.Value = $"{hriData.ItemsRevised![i - 1].RevisionPoint}";
                itemRevision.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                itemRevision.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                itemRevision.Style.Font.Size = bodyFontSize;
                //metodo de revision
                var itemMetodoRevision = ws.Cells[$"C{filaInicioItems}:C{filaInicioItems + totalTurnos - 1}"];
                itemMetodoRevision.Merge = true;
                itemMetodoRevision.Value = $"{hriData.ItemsRevised![i - 1].RevisionMethod!.Description}";
                itemMetodoRevision.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                itemMetodoRevision.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                itemMetodoRevision.Style.Font.Size = bodyFontSize;
                //criterio
                var itemCriterio = ws.Cells[$"D{filaInicioItems}:D{filaInicioItems + totalTurnos - 1}"];
                itemCriterio.Merge = true;
                itemCriterio.Value = $"{hriData.ItemsRevised![i - 1].Veredict!.Description}";
                itemCriterio.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                itemCriterio.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                itemCriterio.Style.Font.Size = bodyFontSize;
                //frecuencia
                var itemFrecuencia = ws.Cells[$"E{filaInicioItems}:F{filaInicioItems + totalTurnos - 1}"];
                itemFrecuencia.Merge = true;
                itemFrecuencia.Value = $"{hriData.ItemsRevised![i - 1].Frequency!.Description}";
                itemFrecuencia.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                itemFrecuencia.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                itemFrecuencia.Style.Font.Size = bodyFontSize;
                for (int j = 1; j <= totalTurnos; j++)
                {
                    var itemCiclo = ws.Cells[$"G{filaInicioItems}"];
                    itemCiclo.Value = j.ToString();
                    itemCiclo.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    itemCiclo.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    itemCiclo.Style.Font.Size = bodyFontSize;

                    //pintamos las celdas de gris si el dia es sabado o domingo
                       foreach (var SatOrSun in satAndSunDays)
                        {
                            var cell = ws.Cells[filaInicioItems, 7 + SatOrSun];
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#787575"));
                        }



                        //si el item en el ciclo actual tiene revisiones diarias, colocamos el simbolo en la columna
                        //correspondiente al dia de la revision
                        if (hriData.ItemsRevised![i - 1].RevisionCycles![j-1].DailyRevisions!=null && hriData.ItemsRevised![i - 1].RevisionCycles![j-1].DailyRevisions!.Count > 0)
                    {
                        var dailyRevisions = hriData.ItemsRevised![i - 1].RevisionCycles![j-1].DailyRevisions!;
                        foreach (var dailyRevision in dailyRevisions)
                        {
                            var diaRevision = dailyRevision.Day;
                            var valorCelda = GetSimbolRevision(dailyRevision.Status!=null ? dailyRevision.Status : ""); //por ahora colocamos O, luego se debe colocar el valor correspondiente a cada revision diaria
                            var cell = ws.Cells[filaInicioItems, 7 + diaRevision];
                            cell.Value = valorCelda;
                            cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            cell.Style.Font.Size = bodyFontSize;
                        }
                    }

                    filaInicioItems++;
                }


            }
            totalLineas += totalItemsRevision * totalTurnos;

            //colocamos la simbologia de las revisiones
            var simbologia = ws.Cells[$"B{filaInicioItems + 2}"];
            simbologia.Value = "SIMBOLOGIA";
            var simbologiaOk = ws.Cells[$"B{filaInicioItems + 3}"];
            simbologiaOk.Value = " O = OK";
            var simbologiaNG = ws.Cells[$"B{filaInicioItems + 4}"];
            simbologiaNG.Value = " X = NG";
            var simbologiaNA = ws.Cells[$"B{filaInicioItems + 5}"];
            simbologiaNA.Value = " N/A = NO APLICA";
            var simbologiaMsm = ws.Cells[$"B{filaInicioItems + 6}"];
            simbologiaMsm.Value = "COLOCAR VALOR NUMERICO";
            var simbologiaMsm2 = ws.Cells[$"B{filaInicioItems + 7}"];
            simbologiaMsm2.Value = "DONDE APLIQUE";

            var stiloSimbologia = ws.Cells[$"B{filaInicioItems + 2}:B{filaInicioItems + 7}"];
            stiloSimbologia.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            stiloSimbologia.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            stiloSimbologia.Style.Font.Size = bodyFontSize;
            stiloSimbologia.Style.Font.Bold = true;

            //colocamos la segunda tabla 
            var filaInicioTab2Border = filaInicioItems;
            var filaInicioTab2 = filaInicioItems; //fila donde termina la tabla de items de revision
            var totalLineasTab2 = 3; //horometro + 2 lineas de dias

            //horometro
            var horometro = ws.Cells[$"D{filaInicioTab2}:F{filaInicioTab2 + 2}"];
            horometro.Merge = true;
            horometro.Value = "HOROMETRO 1 ER T";
            horometro.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            horometro.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            horometro.Style.Font.Size = headerFontSize;
            horometro.Style.Font.Bold = true;

            var turnoVacioHorometro = ws.Cells[$"G{filaInicioTab2}:G{filaInicioTab2 + 2}"];
            turnoVacioHorometro.Merge = true;
            var hourmeter = hriData.HourmeterRevision;
            if(hourmeter != null && (hourmeter.DailyRevisions!=null && hourmeter.DailyRevisions.Count > 0))
            {
                for (int i = 1; i <= diasMes; i++)
                {
                    var dailyHourmeter = hourmeter.DailyRevisions!.FirstOrDefault(dr => dr.Day == i);


                    var horometroDia = ws.Cells[filaInicioTab2, 7 + i, filaInicioTab2 + 2, 7 + i];
                    horometroDia.Merge = true;
                    if(satAndSunDays.Contains(i))
                    {
                        horometroDia.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        horometroDia.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#787575"));
                    }


                    //si existe una revision diaria para el dia actual colocamos el simbolo correspondiente en la celda
                    if (dailyHourmeter != null)
                    {
                        horometroDia.Value = GetSimbolRevision(dailyHourmeter.Status!=null ? dailyHourmeter.Status : "");
                        horometroDia.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        horometroDia.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        horometroDia.Style.Font.Size = bodyFontSize;
                    }

                }
            }
            else
            {
                for (int i = 1; i <= diasMes; i++)
                {
                    var horometroDia = ws.Cells[filaInicioTab2, 7 + i, filaInicioTab2 + 2, 7 + i];
                    horometroDia.Merge = true;
                    if (satAndSunDays.Contains(i))
                    {
                        horometroDia.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        horometroDia.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#787575"));
                    }
                }
            }




            
            filaInicioTab2 += totalLineasTab2;
            var filaInicioTab2Aux = filaInicioTab2;
            
            
            for (int i = 1; i <= totalTurnos; i++)
            {
                var dailyRevisions = hriData.HriCycles[i - 1].DailyRevisions;

                var revisorOp = ws.Cells[$"D{filaInicioTab2Aux}:F{filaInicioTab2Aux}"];
                revisorOp.Merge = true;
                revisorOp.Value = $"REVISO OP {i}ER T";
                revisorOp.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                revisorOp.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                revisorOp.Style.Font.Size = headerFontSize;
                revisorOp.Style.Font.Bold = true;
                
                var revisionSv = ws.Cells[$"D{filaInicioTab2Aux + totalTurnos}:F{filaInicioTab2Aux + totalTurnos}"];
                revisionSv.Merge = true;
                revisionSv.Value = $"REVISO SV {i}ER T";
                revisionSv.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                revisionSv.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                revisionSv.Style.Font.Size = headerFontSize;
                revisionSv.Style.Font.Bold = true;

                //pintamos las celdas de gris si el dia es sabado o domingo
                foreach (var SatOrSun in satAndSunDays)
                {
                    var cellOp = ws.Cells[filaInicioTab2Aux, 7 + SatOrSun];
                    cellOp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cellOp.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#787575"));

                    var cellSv = ws.Cells[filaInicioTab2Aux + totalTurnos, 7 + SatOrSun];
                    cellSv.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cellSv.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#787575"));
                }

                if (dailyRevisions != null && dailyRevisions.Count > 0)
                {   //optenemos el id del operador y el sv del ciclo con ello filtramos las revisiones diarias
                    var operatorId = hriData.HriCycles[i - 1].Operator != null ? hriData.HriCycles[i - 1].OperatorUserId : 0;
                    var svId = hriData.HriCycles[i - 1].Supervisor != null ? hriData.HriCycles[i - 1].SupervisorUserId : 0;
                    if(operatorId!=0 && dailyRevisions.Any(dr=>dr.UserId == operatorId))
                    {
                        //colocamos el simbolo de cada revision diaria del operador en la columna del dia
                        var dailyRevisionOp = dailyRevisions.Where(dr => dr.UserId == operatorId).ToList();
                        for (int j = 0; j < dailyRevisionOp.Count; j++)
                        {
                            var valorRev = ws.Cells[filaInicioTab2Aux, 7 + dailyRevisionOp[j].Day]; //aqui colocaremos el resultado de cada revision diaria, por ahora lo dejamos vacio
                            valorRev.Value = GetSimbolRevision(dailyRevisionOp[j].Status!=null ? dailyRevisionOp[j].Status : "");
                            valorRev.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            valorRev.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            valorRev.Style.Font.Size = bodyFontSize;
                        }
                    }
                    
                    if(svId!=0 && dailyRevisions.Any(dr => dr.UserId == svId))
                    {
                        //colocamos el simbolo de cada revision diaria del sv en la columna del dia
                        var dailyRevisionSv = dailyRevisions.Where(dr => dr.UserId == svId).ToList();
                        for (int j2 = 0; j2 < dailyRevisionSv.Count; j2++)
                        {
                            var valorRev = ws.Cells[filaInicioTab2Aux + totalTurnos, 7 + dailyRevisionSv[j2].Day]; //aqui colocaremos el resultado de cada revision diaria, por ahora lo dejamos vacio
                            valorRev.Value = GetSimbolRevision(dailyRevisionSv[j2].Status!=null ? dailyRevisionSv[j2].Status : "");
                            valorRev.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            valorRev.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            valorRev.Style.Font.Size = bodyFontSize;
                        }
                    }
                    
                }

                
                filaInicioTab2Aux++;
                filaInicioTab2 += 2;
                totalLineasTab2 += 2;
            }

            //Revision semanal

            var revisionSemanal = ws.Cells[$"D{filaInicioTab2}:F{filaInicioTab2 + 1}"];
            revisionSemanal.Merge = true;
            revisionSemanal.Value = "REVISION SEMANAL SSV";
            revisionSemanal.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            revisionSemanal.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            revisionSemanal.Style.Font.Size = headerFontSize;
            revisionSemanal.Style.Font.Bold = true;
            var auxColumnStart = 7;
            for (int i = 1; i <= weeksOfMonth.Count; i++)
            {

                if (i == 1)
                {
                    var valorRevSemanal = ws.Cells[filaInicioTab2, auxColumnStart, filaInicioTab2 + 1, auxColumnStart + weeksOfMonth[i - 1].TotalDays]; //aqui colocaremos el resultado de la revision semanal del SSV, por ahora lo dejamos vacio
                    valorRevSemanal.Merge = true;
                    //si existe una revision semanal para la semana actual colocamos el simbolo correspondiente en la celda
                    if (hriData.WeeklyRevisions != null && (hriData.WeeklyRevisions.Count > 0 && hriData.WeeklyRevisions.Any(wr=>wr.Week==i)))
                    {
                        valorRevSemanal.Value = GetSimbolRevision(hriData.WeeklyRevisions.First(wr=>wr.Week==i).Status!=null ? hriData.WeeklyRevisions.First(wr=>wr.Week==i).Status! : "");
                    }
                    else
                    {
                        valorRevSemanal.Value = "";
                    }
                    valorRevSemanal.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    valorRevSemanal.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    valorRevSemanal.Style.Font.Size = bodyFontSize;
                    auxColumnStart += weeksOfMonth[i - 1].TotalDays;

                }
                else
                {
                    var valorRevSemanal = ws.Cells[filaInicioTab2, auxColumnStart + 1, filaInicioTab2 + 1, auxColumnStart + weeksOfMonth[i - 1].TotalDays]; //aqui colocaremos el resultado de la revision semanal del SSV, por ahora lo dejamos vacio
                    valorRevSemanal.Merge = true;
                    //si existe una revision semanal para la semana actual colocamos el simbolo correspondiente en la celda
                    if (hriData.WeeklyRevisions != null && (hriData.WeeklyRevisions.Count > 0 && hriData.WeeklyRevisions.Any(wr => wr.Week == i)))
                    {
                        valorRevSemanal.Value = GetSimbolRevision(hriData.WeeklyRevisions.First(wr => wr.Week == i).Status != null ? hriData.WeeklyRevisions.First(wr => wr.Week == i).Status! : "");
                    }
                    else
                    {
                        valorRevSemanal.Value = "";
                    }
                    valorRevSemanal.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    valorRevSemanal.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    valorRevSemanal.Style.Font.Size = bodyFontSize;
                    auxColumnStart += weeksOfMonth[i - 1].TotalDays;
                }
            }
            filaInicioTab2 += 1;
            totalLineasTab2 += 2;


            // dibujar el borde de la tabla de items de revision
            var borderRange = ws.Cells[27, 1, 27 + totalLineas, 8 + (diasMes - 1)];//linea inicio,columna inicio,linea final,columna final
            borderRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            borderRange.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            borderRange.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
            borderRange.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            borderRange.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

            // dibujar el borde de la tabla de horometro
            var borderRangeTab2 = ws.Cells[filaInicioTab2Border, 4, filaInicioTab2Border + (totalLineasTab2 - 1), 8 + (diasMes - 1)];
            borderRangeTab2.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRangeTab2.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRangeTab2.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            borderRangeTab2.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            borderRangeTab2.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            borderRangeTab2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
            borderRangeTab2.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            borderRangeTab2.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

        }

        private List<WeeksOfMonthDto> GetMonthWeeks(int mes, int anio)

        {
            //definir los dias que tendra cada semana en nuestro caso la semana empieza en viernes y termina en jueves
            var weeksOfMonth = new List<WeeksOfMonthDto>();
            int diasMes = DateTime.DaysInMonth(anio, mes);
            var diasRestantes = diasMes;
            var diaInicioSemana = (int)new DateTime(anio, mes, 1).DayOfWeek;
            switch (diaInicioSemana)
            {
                case 0: //domingo
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 1 });
                    diasRestantes -= 1;
                    break;
                case 1: //lunes
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 7 });
                    diasRestantes -= 7;
                    break;
                case 2: //martes
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 6 });
                    diasRestantes -= 6;
                    break;
                case 3: //miercoles
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 5 });
                    diasRestantes -= 5;
                    break;
                case 4: //jueves
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 4 });
                    diasRestantes -= 4;
                    break;
                case 5: //viernes
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 3 });
                    diasRestantes -= 3;
                    break;
                case 6: //sabado
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = 1, TotalDays = 2 });
                    diasRestantes -= 2;
                    break;
            }
            if (diasRestantes % 7 != 0)

            {
                var total = 1;
                var semanasCompletas = (int)(diasRestantes / 7);
                total += semanasCompletas + 1;
                for (int i = 0; i < semanasCompletas; i++)
                {
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = i + 2, TotalDays = 7 });

                }
                weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = total, TotalDays = diasRestantes % 7 });
            }
            else
            {

                var semanasCompletas = (int)(diasRestantes / 7);
                for (int i = 0; i < semanasCompletas; i++)
                {
                    weeksOfMonth.Add(new WeeksOfMonthDto { WeekNumber = i + 2, TotalDays = 7 });

                }
            }
            return weeksOfMonth;
        }
        
        private List<int> GetSaturdaysAndSundays(int mes, int anio)
        {
            var diasMes = DateTime.DaysInMonth(anio, mes);
            var dias = new List<int>();
            for (int i = 1; i <= diasMes; i++)
            {
                var diaSemana = new DateTime(anio, mes, i).DayOfWeek;
                if (diaSemana == DayOfWeek.Sunday || diaSemana == DayOfWeek.Saturday)
                {
                    dias.Add(i);
                }
            }
            return dias;
        }
        
        private string GetSimbolRevision(string valor)
        {
            switch (valor)
            {
                case "Ok":
                    return "O";
                case "NG":
                    return "X";
                case "NA":
                    return "N/A";
                default:
                    return "";
            }
        }

        private void setAllBorders(ExcelRange range)
        {
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        private void setOutBorders(ExcelRange range)
        {
            var startRow = range.EntireRow.StartRow;
            var endRow = range.EntireRow.EndRow;
            var startCol = range.EntireColumn.StartColumn;
            var endCol = range.EntireColumn.EndColumn;

            for (int col = startCol; col <= endCol; col++)
            {
                range.Worksheet.Cells[startRow, col].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Worksheet.Cells[endRow, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            for (int row = startRow; row <= endRow; row++)
            {
                range.Worksheet.Cells[row, startCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Worksheet.Cells[row, endCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
        }
        
        private string ConvertirNumeroALetraColumna(int numero)
        {
            string columna = "";

            while (numero > 0)
            {
                numero--;
                columna = (char)('A' + numero % 26) + columna;
                numero /= 26;
            }

            return columna;
        }

        private int ConvertirLetraColumnaANumero(string columna)
        {
            int numero = 0;
            
            foreach (char c in columna.ToUpper())
            {
                numero = numero * 26 + (c - 'A' + 1);
            }
            
            return numero;
        }
    }
}