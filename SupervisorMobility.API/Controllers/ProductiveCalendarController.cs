using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.ProductiveCalendarDtos;
using SupervisorMobility.API.Services;


namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductiveCalendarController : ControllerBase
    {
       
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;

        public ProductiveCalendarController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("GetHolidays/{year}")]
        public async Task<IActionResult> GetHolidays(int year)
        {
            try
            {
                var holidays = await _supervisorMobilityRepository.GetActiveHolidaysOfYearAsync(year);
                return Ok(holidays);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateOrCreateHolidays/{year}")]
        public async Task<IActionResult> UpdateOrCreateHolidays(List<HolidayForUpdateDto> holidaysForCreateOrUpdate, int year)
        {
            try
            {
                var existHolidays = await _supervisorMobilityRepository.GetHolidaysOfYearAsync(year);
                //scar diferencia entre exist holidays y holidays rcibidos

                // Obtener los HolidayId de los recibidos
                var receivedIds = holidaysForCreateOrUpdate.Where(h => h.HolidayId > 0).Select(h => h.HolidayId).ToHashSet();

                // Diferencia: feriados existentes que NO están en la lista recibida (posibles eliminaciones)
                var holidaysToRemove = existHolidays
                    .Where(eh => !receivedIds.Contains(eh.HolidayId))
                    .ToList();

                foreach (var holiday in holidaysToRemove)
                {
                    var existingHoliday = await _supervisorMobilityRepository.GetHolidayByIdAsync(holiday.HolidayId);

                    holiday.IsActive = false;
                    // si el existingholiday es diferente de item hay que actualizar
                    _ = await _supervisorMobilityRepository.UpdateHolidayAsync(existingHoliday, _mapper.Map<HolidayForUpdateDto>(holiday));

                }

                foreach (var item in holidaysForCreateOrUpdate)
                {
                    if (item == null)
                    {
                        return BadRequest("El objeto holiday no puede ser nulo.");
                    }

                    if (item.HolidayId > 0)
                    {
                        var existingHoliday = await _supervisorMobilityRepository.GetHolidayByIdAsync(item.HolidayId);

                        // si el existingholiday es diferente de item hay que actualizar
                        _ = await _supervisorMobilityRepository.UpdateHolidayAsync(existingHoliday, item);
                    }
                    else
                    {
                        //create

                        item.HolidayId = 0;
                        item.IsActive = true;
                        _ = await _supervisorMobilityRepository.AddHolidayAsync(item);
                    }

                }

                var Returnedholidays = await _supervisorMobilityRepository.GetActiveHolidaysOfYearAsync(year);
                return Ok(Returnedholidays);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}
