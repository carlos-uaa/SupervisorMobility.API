 using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;


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
        public HRIRepository(SupervisorMobilityContext context, IMapper mapper, IHRIRevisionItemRepository hriRevisionItemRepository, IHRICyclesRepository hriCyclesRepository, IHRIHourmeterRevisionRepository hriHourmeterRepository, IHRIRevisionCyclesRepository hriRevisionCyclesRepository, IHRImagesService hrimagesService)
        {
            _context = context;
            _mapper = mapper;
            _hriRevisionItemRepository = hriRevisionItemRepository;
            _hriCyclesRepository = hriCyclesRepository;
            _hriHourmeterRepository = hriHourmeterRepository;
            _hrimagesService = hrimagesService;
            _hriRevisionCyclesRepository = hriRevisionCyclesRepository;
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
                var historyItem = new HRIHistoryItemDto
                {
                    HRIid = weeklyRevisions.First().HriId,
                    Action = "Weekly Revisions Created",
                    ActionDate = DateTime.UtcNow,
                    ResponsibleUserId = weeklyRevisions.First().UserId,
                    ActionType = "UPDATE"
                };
                await SendHistoryAction(historyItem);

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
            try
            {
                var hri = await _context.HRIs.AsNoTracking().Include(h => h.Line)
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
    }
}