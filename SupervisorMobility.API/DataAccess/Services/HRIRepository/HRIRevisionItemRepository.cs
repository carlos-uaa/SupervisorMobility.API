using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIRevisionItemRepository : IHRIRevisionItemRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IHRIRevisionCyclesRepository _hriRevisionCyclesRepository;
        private readonly IMapper _mapper;
        public HRIRevisionItemRepository(SupervisorMobilityContext supervisorMobilityContext, IHRIRevisionCyclesRepository hriRevisionCyclesRepository, IMapper mapper)
        {
            _context = supervisorMobilityContext;
            _hriRevisionCyclesRepository = hriRevisionCyclesRepository;
            _mapper = mapper;
        }
        #region HRIRevisionItem
        public async Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems()
        {
            var response = new ServiceResponse<List<GetHRIRevisionItemDto>>();
            try
            {
                var revisionItems = await _context.HRIRevisionItems.Include(ri => ri.HRI)
                                                                   .Include(ri => ri.RevisionMethod)
                                                                   .Include(ri => ri.Veredict)
                                                                   .Include(ri => ri.Frequency)
                                                                   .Include(ri => ri.RevisionCycles!).ThenInclude(rc => rc.DailyRevisions)
                                                                   .ToListAsync();
                if(revisionItems.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No HRI Revision Items found.";
                    return response;
                }
                response.Data = revisionItems.Select(ri => _mapper.Map<GetHRIRevisionItemDto>(ri)).ToList();
                response.Success = true;
                response.Message = "HRI Revision Items retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id)
        {
            var response = new ServiceResponse<GetHRIRevisionItemDto>();
            try
            {
                var revisionItem = await _context.HRIRevisionItems.Include(ri => ri.HRI)
                                                                   .Include(ri => ri.RevisionMethod)
                                                                   .Include(ri => ri.Veredict)
                                                                   .Include(ri => ri.Frequency)
                                                                   .Include(ri=>ri.RevisionCycles!).ThenInclude(rc=>rc.DailyRevisions)
                                                                   .FirstOrDefaultAsync(ri => ri.ItemId == id);
                if (revisionItem == null)
                {
                    response.Success = false;
                    response.Message = "HRI Revision Item not found.";
                }
                else
                {
                    response.Data = _mapper.Map<GetHRIRevisionItemDto>(revisionItem);
                    response.Success = true;
                    response.Message = "HRI Revision Item retrieved successfully.";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto createHRIRevisionItemDto)
        {
            var response = new ServiceResponse<GetHRIRevisionItemDto>();
            try
            {
                var revisionItem = _mapper.Map<HRIRevisionItems>(createHRIRevisionItemDto);
                revisionItem.CreationDate = DateTime.Now;
                await _context.HRIRevisionItems.AddAsync(revisionItem);
                await _context.SaveChangesAsync();

                // Crear los ciclos de revisión para el item creado
                var cyclesCount = await _context.HRICycles.Where(rc => rc.HriId == revisionItem.HriId && rc.IsActive==true).CountAsync();
                if(cyclesCount > 0)
                {
                    var listOfCycles = new List<CreateRevisionCyclesDto>();
                    for (int i = 1; i <= cyclesCount; i++)
                    {
                        var revisionCycle = new CreateRevisionCyclesDto
                        {
                            Cycle = i,
                            IsActive = true
                        };
                        listOfCycles.Add(revisionCycle);
                    }
                    await _hriRevisionCyclesRepository.CreateRevisionCyclesByRevisionItemId(revisionItem.ItemId, listOfCycles);
                }
                //agregamos una accion al historial del hri indicando que se creo un nuevo item revisado
                var historyItem = new HRIHistoryItemDto
                {
                    HRIid = revisionItem.HriId,
                    Action = $"HRI Revision Item {revisionItem.RevisionPoint} created",
                    ActionDate = DateTime.Now,
                    ResponsibleUserId = await _context.HRIRevisionItems.Where(ri => ri.ItemId == revisionItem.ItemId)
                                                        .Select(ri => ri.HRI.SupervisorUserId ?? ri.HRI.SSVUserId)
                                                        .FirstOrDefaultAsync(),
                    ActionType = "UPDATE"
                };
                await SendHistoryAction(historyItem);
                response.Data = _mapper.Map<GetHRIRevisionItemDto>(revisionItem);
                response.Success = true;
                response.Message = "HRI Revision Item created successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message + ex.InnerException.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>>CreateHRIREvisionItemsByHRIId(int hriId, List<CreateHRIRevisionItemDto> createHRIRevisionItemDtos, int numOfCycles)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var hri = await _context.HRIs.FindAsync(hriId);
                if (hri == null)
                {
                    response.Success = false;
                    response.Message = "HRI not found.";
                    return response;
                }
                var revisionItems = createHRIRevisionItemDtos.Select(dto => _mapper.Map<HRIRevisionItems>(dto)).ToList();
                foreach (var item in revisionItems)
                {
                    
                    item.HriId = hriId; // Asignamos el HRIId a cada item
                    item.CreationDate = DateTime.Now;
                    await _context.HRIRevisionItems.AddAsync(item);
                    await _context.SaveChangesAsync();
                    // Crear los ciclos de revisión para cada item
                    if(numOfCycles > 0)
                    {
                        var listOfCycles = new List<CreateRevisionCyclesDto>();
                        for (int i = 1; i <= numOfCycles; i++)
                        {

                            var revisionCycle = new CreateRevisionCyclesDto
                            {
                                Cycle = i,
                                IsActive = true
                            };
                            listOfCycles.Add(revisionCycle);
                        }
                        await _hriRevisionCyclesRepository.CreateRevisionCyclesByRevisionItemId(item.ItemId, listOfCycles);
                    }
                    
                    
                }

                response.Data = true;
                response.Success = true;
                response.Message = "HRI Revision Items created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto updateHRIRevisionItemDto)
        {
            var response = new ServiceResponse<GetHRIRevisionItemDto>();
            try
            {
                var revisionItem = await _context.HRIRevisionItems.FindAsync(id);
                if (revisionItem == null)
                {
                    response.Success = false;
                    response.Message = "HRI Revision Item not found.";
                }
                else
                {
                    _mapper.Map(updateHRIRevisionItemDto, revisionItem);
                    _context.HRIRevisionItems.Update(revisionItem);
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<GetHRIRevisionItemDto>(revisionItem);
                    response.Success = true;
                    response.Message = "HRI Revision Item updated successfully.";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var revisionItem = await _context.HRIRevisionItems.FindAsync(id);
                if (revisionItem == null)
                {
                    response.Success = false;
                    response.Message = "HRI Revision Item not found.";
                }
                else
                {
                    // soft delete
                    revisionItem.IsActive = false;
                    revisionItem.DeletedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Eliminar los ciclos de revisión asociados
                    //var revisionCycles = await _hriRevisionCyclesRepository.GetAllRevisionCyclesByRevisionItemId(revisionItem.ItemId);
                    //if (revisionCycles.Data != null)
                    //{
                    //    foreach (var cycle in revisionCycles.Data)
                    //    {
                    //        await _hriRevisionCyclesRepository.DeleteRevisionCycle(cycle.RevisionCycleId);
                    //    }
                    //}
                    response.Data = true;
                    response.Success = true;
                    response.Message = "HRI Revision Item deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItemsByHRIId(int hriId)
        {
            var response = new ServiceResponse<List<GetHRIRevisionItemDto>>();
            try
            {
                var revisionItems = await _context.HRIRevisionItems.Where(ri => ri.HriId == hriId && ri.IsActive==true)
                                                                   .Include(ri => ri.RevisionMethod)
                                                                   .Include(ri => ri.Veredict)
                                                                   .Include(ri => ri.Frequency)                                                      
                                                                   .ToListAsync();
                if (revisionItems.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No HRI Revision Items found for the specified HRI.";
                    return response;
                }
                response.Data = revisionItems.Select(ri => _mapper.Map<GetHRIRevisionItemDto>(ri)).ToList();
                response.Success = true;
                response.Message = "HRI Revision Items retrieved successfully for the specified HRI.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
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

        public async Task<ServiceResponse<bool>> ValidateItemForUpdate(int itemId, UpdateHRIRevisionItemDto itemToUpdate)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var item = await _context.HRIRevisionItems.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == itemId);
                //validamos si tiene algun campo diferente al que ya tiene en la base de datos, si no tiene ninguno, no se actualiza y se le notifica al usuario
                if(item != null)
                {
                    // Si el item existe, comparamos los campos
                    if(item.ItemNumber != itemToUpdate.ItemNumber)
                    {
                        // Si hay diferencias, se puede proceder con la actualización
                        response.Data = true;
                        response.Success = true;
                        response.Message = "Item can be updated.";
                        return response;
                    }
                    if(item.RevisionPoint != itemToUpdate.RevisionPoint)
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Message = "Item can be updated.";
                        return response;
                    }
                    if(item.RevisionMethodId != itemToUpdate.RevisionMethodId)
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Message = "Item can be updated.";
                        return response;
                    }
                    if(item.VeredictId != itemToUpdate.VeredictId)
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Message = "Item can be updated.";
                        return response;
                    }
                    if(item.FrequencyId != itemToUpdate.FrequencyId)
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Message = "Item can be updated.";
                        return response;
                    }
                    
                }
                response.Data = false;
                response.Success = false;
                response.Message = "No changes detected. Item cannot be updated.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = false;

            }
            return response;
        }
        #endregion

        #region frequency
        public async Task<ServiceResponse<List<GetFrequencyDto>>> GetAllFrequencies()
        {
            var response = new ServiceResponse<List<GetFrequencyDto>>();
            try
            {
                var frequencies = await _context.Frequencies.Where(f => f.IsActive==true).ToListAsync();
                if(frequencies.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No frequencies found.";
                    return response;
                }
                response.Data = frequencies.Select(f => _mapper.Map<GetFrequencyDto>(f)).ToList();
                response.Success = true;
                response.Message = "Frequencies retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetFrequencyDto>> GetFrequencyById(int id)
        {
            var response = new ServiceResponse<GetFrequencyDto>();
            try
            {
                var frequency = await _context.Frequencies.Where(f => f.Id == id && f.IsActive==true).FirstOrDefaultAsync();
                if (frequency == null)
                {
                    response.Success = false;
                    response.Message = "Frequency not found.";
                }
                else
                {
                    response.Data = _mapper.Map<GetFrequencyDto>(frequency);
                    response.Success = true;
                    response.Message = "Frequency retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetFrequencyDto>> CreateFrequency(CreateFrequencyDto createFrequencyDto)
        {
            var response = new ServiceResponse<GetFrequencyDto>();
            try
            {
                var frequency = _mapper.Map<Frequency>(createFrequencyDto);
                await _context.Frequencies.AddAsync(frequency);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetFrequencyDto>(frequency);
                response.Success = true;
                response.Message = "Frequency created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetFrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto updateFrequencyDto)
        {
           var response = new ServiceResponse<GetFrequencyDto>();
            try
            {
                var frequency = await _context.Frequencies.FindAsync(id);
                if (frequency == null)
                {
                    response.Success = false;
                    response.Message = "Frequency not found.";
                }
                else
                {
                    _mapper.Map(updateFrequencyDto, frequency);
                    _context.Frequencies.Update(frequency);
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<GetFrequencyDto>(frequency);
                    response.Success = true;
                    response.Message = "Frequency updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteFrequency(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var frequency = await _context.Frequencies.FindAsync(id);
                if (frequency == null)
                {
                    response.Success = false;
                    response.Message = "Frequency not found.";
                }
                else
                {
                    frequency.IsActive = false;
                    await _context.SaveChangesAsync();
                    response.Data = true;
                    response.Success = true;
                    response.Message = "Frequency deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region veredict
        public async Task<ServiceResponse<List<GetVeredictDto>>> GetAllVeredicts()
        {
            var response = new ServiceResponse<List<GetVeredictDto>>();
            try
            {
                var veredicts = await _context.Veredicts.Where(v => v.IsActive == true).ToListAsync();
                if(veredicts.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No veredicts found.";
                    return response;
                }
                response.Data = veredicts.Select(v => _mapper.Map<GetVeredictDto>(v)).ToList();
                response.Success = true;
                response.Message = "Veredicts retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetVeredictDto>> GetVeredictById(int id)
        {
            var response = new ServiceResponse<GetVeredictDto>();
            try
            {
                var veredict = await _context.Veredicts.Where(v => v.Id == id && v.IsActive == true).FirstOrDefaultAsync();
                if (veredict == null)
                {
                    response.Success = false;
                    response.Message = "Veredict not found.";
                }
                else
                {
                    response.Data = _mapper.Map<GetVeredictDto>(veredict);
                    response.Success = true;
                    response.Message = "Veredict retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetVeredictDto>> CreateVeredict(CreateVeredictDto createVeredictDto)
        {
            var response = new ServiceResponse<GetVeredictDto>();
            try
            {
                var veredict = _mapper.Map<Veredict>(createVeredictDto);
                await _context.Veredicts.AddAsync(veredict);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetVeredictDto>(veredict);
                response.Success = true;
                response.Message = "Veredict created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetVeredictDto>> UpdateVeredict(int id, UpdateVeredictDto updateVeredictDto)
        {
            var response = new ServiceResponse<GetVeredictDto>();
            try
            {
                var veredict = await _context.Veredicts.FindAsync(id);
                if (veredict == null)
                {
                    response.Success = false;
                    response.Message = "Veredict not found.";
                }
                else
                {
                    _mapper.Map(updateVeredictDto, veredict);
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<GetVeredictDto>(veredict);
                    response.Success = true;
                    response.Message = "Veredict updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteVeredict(int id)
        {
             var response = new ServiceResponse<bool>();
            try
            {
                var veredict = await _context.Veredicts.FindAsync(id);
                if (veredict == null)
                {
                    response.Success = false;
                    response.Message = "Veredict not found.";
                }
                else
                {
                    veredict.IsActive = false;
                    await _context.SaveChangesAsync();
                    response.Data = true;
                    response.Success = true;
                    response.Message = "Veredict deleted successfully.";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region revision method
        public async Task<ServiceResponse<List<GetRevisionMethodDto>>> GetAllRevisionMethods()
        {
            var response = new ServiceResponse<List<GetRevisionMethodDto>>();
            try
            {
                var revisionMethods = await _context.RevisionMethods.Where(rm => rm.IsActive == true).ToListAsync();
                if (revisionMethods.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No revision methods found.";
                    return response;
                }
                response.Data = revisionMethods.Select(rm => _mapper.Map<GetRevisionMethodDto>(rm)).ToList();
                response.Success = true;
                response.Message = "Revision methods retrieved successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> GetRevisionMethodById(int id)
        {
            var response = new ServiceResponse<GetRevisionMethodDto>();
            try
            {
                var revisionMethod = await _context.RevisionMethods.Where(rm => rm.Id == id && rm.IsActive == true).FirstOrDefaultAsync();
                if (revisionMethod == null)
                {
                    response.Success = false;
                    response.Message = "Revision method not found.";
                }
                else
                {
                    response.Data = _mapper.Map<GetRevisionMethodDto>(revisionMethod);
                    response.Success = true;
                    response.Message = "Revision method retrieved successfully.";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> CreateRevisionMethod(CreateRevisionMethodDto createRevisionMethodDto)
        {
            var response = new ServiceResponse<GetRevisionMethodDto>();
            try
            {
                var revisionMethod = _mapper.Map<RevisionMethod>(createRevisionMethodDto);
                await _context.RevisionMethods.AddAsync(revisionMethod);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetRevisionMethodDto>(revisionMethod);
                response.Success = true;
                response.Message = "Revision method created successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto updateRevisionMethodDto)
        {
           var response = new ServiceResponse<GetRevisionMethodDto>();
            try
            {
                var revisionMethod = await _context.RevisionMethods.FindAsync(id);
                if (revisionMethod == null)
                {
                    response.Success = false;
                    response.Message = "Revision method not found.";
                }
                else
                {
                    _mapper.Map(updateRevisionMethodDto, revisionMethod);
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<GetRevisionMethodDto>(revisionMethod);
                    response.Success = true;
                    response.Message = "Revision method updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteRevisionMethod(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var revisionMethod = await _context.RevisionMethods.FindAsync(id);
                if (revisionMethod == null)
                {
                    response.Success = false;
                    response.Message = "Revision method not found.";
                }
                else
                {
                    revisionMethod.IsActive = false;
                    await _context.SaveChangesAsync();
                    response.Data = true;
                    response.Success = true;
                    response.Message = "Revision method deleted successfully.";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion
    }
}
