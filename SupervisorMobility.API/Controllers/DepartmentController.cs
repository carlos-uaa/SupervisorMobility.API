using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.DepartmentDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public DepartmentController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departmentEntity = await _supervisorMobilityRepository.GetDepartmentsAsync();
            return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departmentEntity));
        }

        [HttpGet("{departmentId}", Name = "GetDepartment")]
        public async Task<ActionResult> GetDepartment(int departmentId)
        {
            //Find Job Observation type
            var department = await _supervisorMobilityRepository
                .GetDepartmentAsync(departmentId);
            if (department == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DepartmentDto>(department));
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(
            DepartmentForCreationDto department)
        {
            //Mpa the pbject
            var finalDepartment = _mapper.Map<Entities.Department>(department);
            _supervisorMobilityRepository.AddDepartment(finalDepartment);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createDepartmentToReturn =
                _mapper.Map<DepartmentDto>(finalDepartment);

            return CreatedAtRoute("GetDepartment",
                new
                {
                    departmentId = createDepartmentToReturn.DepartmentId
                },
                createDepartmentToReturn);
        }


        [HttpPut("{departmentID}")]
        public async Task<ActionResult> UpdateDepartment(int departmentId,
            DepartmentForUpdateDto deparment)
        {
            var departmentEntity = await _supervisorMobilityRepository.GetDepartmentAsync(departmentId);
            if (departmentEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(deparment, departmentEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }


        [HttpDelete("{departmentId}")]
        public async Task<ActionResult> DeleteDepartment(int departmentId)
        {
            var departmentEntity = await _supervisorMobilityRepository.GetDepartmentAsync(departmentId);
            if (departmentEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteDepartment(departmentEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
