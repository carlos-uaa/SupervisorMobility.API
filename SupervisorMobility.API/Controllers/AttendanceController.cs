using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AttendanceDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Globalization;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;

        public AttendanceController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
            IAssyChartService assyChartService)
        {
            _assyChartService = assyChartService;
            _env = env;
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("UploadAttendance")]
        public async Task<ActionResult<FileUploadGeneralDto>> UploadAttendance(IFormFile file)
        {
            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\attendance", trustedFileNameForStorage);

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            return Ok(fileToReturn);

        }

        [HttpGet]
        public async Task<ActionResult> GetAllAttendance(int idsuperior)
        {
            List<AttendanceWithNavigationDetailsDto> allattendance = _mapper.Map<List<AttendanceWithNavigationDetailsDto>>(await _assyChartService.GetAllAttendanceOfSupervisorAsync(idsuperior));

            return Ok(allattendance);
        }


        [HttpGet("Assign")]
        public async Task<ActionResult> AssignEmployees()
        {
            string filePath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\attendance\\attendance.csv";
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>();

            List<Attendance> allattendance = _mapper.Map<List<Attendance>>(await _assyChartService.GetAllAttendanceAsync());
            List<UsersWithoutNavigationDetails> alluser = _mapper.Map<List<UsersWithoutNavigationDetails>>(await _assyChartService.GetAllUsers());
            var fecha2 = DateTime.Now;

            List<Attendance> allattendanceadded = new List<Attendance>();


            foreach (var record in records)
            {
                int id = 0;

                if ((string)record.Id_Empleado != "")
                    id = int.Parse(record.Id_Empleado);
                else
                    continue;

                //string concepto = "";
                //if ((string)record.Concepto != "")
                //    concepto = (string)record.Concepto;
                //else
                //    continue;


                DateTime fecha = DateTime.Now;

                if ((string)record.Fecha != "")
                    fecha = DateTime.Parse((string)record.Fecha);
                else
                    continue;


                var user = alluser.Find(u => u.Payroll == id);
                if (user == null)
                {
                    continue;
                }

                var existingRecordInAttendance = allattendance.Find(a => a.User.Payroll == id);
                bool mismoDia = fecha.Day == fecha2.Day && fecha.Month == fecha2.Month && fecha.Year == fecha2.Year;

                if (!mismoDia)
                {
                    //es un dia diferente
                    if (existingRecordInAttendance != null)
                    {
                        //ya se encuentra en la lista
                        var updateRecord = new AttendanceForUpdateDto
                        {
                            UserId = user.UserId,
                            SuperiorId = user.SuperiorId,
                            CurrentdistributionId = user.DistributionId,
                            Compas = false,
                            Station = false
                        };

                        bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                        //se actualiza dado que es de un dia pasado y no estara en la planta 
                    }
                    else
                    {
                        //El registro no existe y es un dia diferente

                        var newRecorddifday = new AttendanceForCreationDto
                        {
                            UserId = user.UserId,
                            SuperiorId = user.SuperiorId,
                            CurrentdistributionId = user.DistributionId,
                            Compas = mismoDia,
                            Station = false
                        };


                        var processAttendanceYesterday = await _assyChartService.CreateAttendanceAsync(newRecorddifday);
                        allattendanceadded.Add(processAttendanceYesterday);
                    }

                    continue;

                }
                else
                {
                    //Es el mismo dia
                    DateTime inico = DateTime.Now;
                    DateTime fin = DateTime.Now;




                    //el inicio puede estar vacio por que es turno vespertino u horas extras
                    if ((string)record.Inicio == "")
                    {
                        //ya existe registro de el en tabla de asistencia
                        if ((string)record.Fin != "")
                        {
                            //Update, ya salio
                            var updateRecord = new AttendanceForUpdateDto
                            {
                                UserId = user.UserId,
                                SuperiorId = user.SuperiorId,
                                CurrentdistributionId = user.DistributionId,
                                Compas = false,
                                Station = false
                            };

                            bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                        continue;
                        }
                    }
                    else
                    {
                        if ((string)record.Fin == "")
                        {
                           //aun no sale, no se hace nada
                            if (existingRecordInAttendance != null)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //Update, ya salio
                            var updateRecord = new AttendanceForUpdateDto
                            {
                                UserId = user.UserId,
                                SuperiorId = user.SuperiorId,
                                CurrentdistributionId = user.DistributionId,
                                Compas = false,
                                Station = false
                            };

                            bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                            continue;

                        }
                    }

                    if (existingRecordInAttendance != null)
                    {
                        continue;
                    }
                }


                var newRecord = new AttendanceForCreationDto
                {
                    UserId = user.UserId,
                    SuperiorId = user.SuperiorId,
                    CurrentdistributionId = user.DistributionId,
                    Compas = mismoDia,
                    Station = false
                };


                var processAttendance = await _assyChartService.CreateAttendanceAsync(newRecord);
                allattendanceadded.Add(processAttendance);

            }

            return Ok(allattendanceadded);
        }

        [HttpPost("updatelist")]
        public async Task<ActionResult> updatelist(List<AttendanceWithoutDetailsDto> lista)
        {
            //update lista
            List<Attendance> allattendance = _mapper.Map<List<Attendance>>(await _assyChartService.GetAllAttendanceAsync());

            foreach (var item in lista)
            {
                var AttendaceforUpdate = allattendance.Find(e => e.AttendanceId == item.AttendanceId);

                if (AttendaceforUpdate != null)
                {
                    bool update = await _assyChartService.UpdateAttendanceAsync(_mapper.Map<AttendanceForUpdateDto>(item), AttendaceforUpdate);
                }

            }

            allattendance = _mapper.Map<List<Attendance>>(await _assyChartService.GetAllAttendanceAsync());

            return Ok(allattendance);
        }

    }
}
