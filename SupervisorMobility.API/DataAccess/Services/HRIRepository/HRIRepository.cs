using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRIRepository : IHRIRepository
    {
        private readonly SupervisorMobilityContext _context;
        private IHRIRevisionItemRepository _hriRevisionItemRepository;
        private IHRICyclesRepository _hriCyclesRepository;
        private IHRIHourmeterRevisionRepository _hriHourmeterRepository;
        private readonly IMapper _mapper;
        public HRIRepository(SupervisorMobilityContext context, IMapper mapper, IHRIRevisionItemRepository hriRevisionItemRepository, IHRICyclesRepository hriCyclesRepository, IHRIHourmeterRevisionRepository hriHourmeterRepository)
        {
            _context = context;
            _mapper = mapper;
            _hriRevisionItemRepository = hriRevisionItemRepository;
            _hriCyclesRepository = hriCyclesRepository;
            _hriHourmeterRepository = hriHourmeterRepository;
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


                //agregamos los items revisados relacionados al hri
                if(newHRI.ItemsRevised != null && newHRI.ItemsRevised.Count > 0)
                {
                   var res = await _hriRevisionItemRepository.CreateHRIREvisionItemsByHRIId(hri.HriId, newHRI.ItemsRevised);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Revision Items: {res.Message}";
                    }
                    return serviceResponse;
                }
                //agregamos las revisiones semanales relacionadas al hri
                if(newHRI.WeeklyRevisions != null && newHRI.WeeklyRevisions.Count > 0)
                {
                    foreach(var weeklyRevision in newHRI.WeeklyRevisions)
                    {
                        weeklyRevision.HriId = hri.HriId;
                        await _context.WeeklyRevisions.AddAsync(weeklyRevision);
                    }
                    await _context.SaveChangesAsync();
                }

                //agregamos los ciclos relacionados al hri
                if(newHRI.HriCycles != null && newHRI.HriCycles.Count > 0)
                {
                    var res = await _hriCyclesRepository.CreateHRICyclesByHRIId(hri.HriId, newHRI.HriCycles);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Cycles: {res.Message}";
                    }
                    return serviceResponse;
                }

                //agregamos la revision del hourmeter relacionada al hri
                if(newHRI.HourmeterRevision != null)
                {
                    var res = await _hriHourmeterRepository.AddHourmeterRevision(newHRI.HourmeterRevision);
                    if (res.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Error creating HRI Hourmeter Revision: {res.Message}";
                    }
                    return serviceResponse;

                }

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
                var hris = await _context.HRIs.Include(h => h.Line)
                    .Include(h => h.NameOfItem)
                    .Include(h => h.Dock)
                    .Include(h => h.Images)
                    .Include(h => h.ItemsRevised)
                    .Include(h => h.WeeklyRevisions)
                    .Include(h => h.HriCycles)
                    .Include(h => h.HourmeterRevision)
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
                var hri = await _context.HRIs.Include(h => h.Line)
                    .Include(h => h.NameOfItem)
                    .Include(h => h.Dock)
                    .Include(h => h.Images)
                    .Include(h => h.ItemsRevised)
                    .Include(h => h.WeeklyRevisions)
                    .Include(h => h.HriCycles)
                    .Include(h => h.HourmeterRevision)
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
