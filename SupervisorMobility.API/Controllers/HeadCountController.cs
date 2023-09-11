using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{

    [Route("api/HeadCount")]
    [ApiController]
    public class HeadCountController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;


        public HeadCountController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
        IAssyChartService assyChartService)
        {
            _assyChartService = assyChartService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _env = env;
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("Upload")]
        public async Task<ActionResult<FileUpload>> UploadFileFromMassiveUpload(IFormFile file, int UserIdUpload)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            trustedFileNameForStorage = System.IO.Path.ChangeExtension(trustedFileNameForStorage, ".xlsx");
            var path = Path.Combine(_env.ContentRootPath, "uploads\\headcount", trustedFileNameForStorage);

            await using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                // Utiliza "await" para asegurarte de que se complete la copia del archivo antes de continuar
                await file.CopyToAsync(fs);
            }

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            await _supervisorMobilityRepository.SaveChangesAsync();


            //Start Massive Upload 
            string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\headcount\\" + trustedFileNameForStorage;
            try
            {
                using (var workBook = new XLWorkbook(filepath))
                {
                    var pages = workBook.Worksheets.Count - 1;

                    //for (int p = 1; p <= pages; p++)
                    //{
                    IXLWorksheet ws = workBook.Worksheet(1);


                    bool firstRow = true;
                    int i = 1;
                    foreach (IXLRow row in ws.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        HeadCount _headCount = new HeadCount();

                        if (firstRow)
                        {
                            firstRow = false;
                        }
                        else
                        {
                            if (!row.IsEmpty())
                            {
                                int maxRetries = 5; // Número máximo de intentos
                                TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                int retries = 0;

                                while (retries < maxRetries)
                                {
                                    try
                                    {
                                        //procedimiento
                                        try
                                        {
                                           _headCount.Codigo = ws.Cell(i, 1).GetString() != "" ? (int)ws.Cell(i, 1).Value : -1;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.CO = ws.Cell(i, 2).GetString() != "" ? ws.Cell(i, 2).GetValue<string>() : "";
        //                                  ToInsertIntoList.GOS = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            var valuesArea = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                            var splitedArea = valuesArea.Split("-");

                                            try
                                            {
                                                _headCount.ID_Area = int.Parse(splitedArea[0]);
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                _headCount.Nombre_Area = splitedArea[1];
                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            var valueDepartament = ws.Cell(i, 4).GetString() != "" ? ws.Cell(i, 4).GetValue<string>() : "";
                                            var CostDepartament = valueDepartament.Split("_");
                                            var splitedDepartament = CostDepartament[0].Split("-");
                                            try
                                            {
                                                _headCount.Cost_center = int.Parse(splitedDepartament[0]);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.ID_Departamento = splitedDepartament[1];
                                            }
                                            catch (Exception ex)
                                            {

                                            } try
                                            {
                                                _headCount.Nombre_Departamento = CostDepartament[1];
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            var valueFunctionDescription = ws.Cell(i, 5).GetString() != "" ? ws.Cell(i, 5).GetValue<string>() : "";

                                            try
                                            {
                                                _headCount.ID_subarea = i;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.nombre_subarea = $"Subarea {i}";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.Nivel = ws.Cell(i, 6).GetString() != "" ? ws.Cell(i, 6).GetValue<string>() : "";

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.Group = ws.Cell(i, 7).GetString() != "" ? ws.Cell(i, 7).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.BUDGET = ws.Cell(i, 8).GetString() != "" ? ws.Cell(i, 8).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.RTO = ws.Cell(i, 9).GetString() != "" ? ws.Cell(i, 9).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            var valueHC = ws.Cell(i, 1).GetString() != "" ? ws.Cell(i, 1).Value.ToString() : "";
                                            try
                                            {
                                                _headCount.HC = int.Parse(valueHC);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.Comentarios = ws.Cell(i, 11).GetString() != "" ? ws.Cell(i, 11).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.LABOR_TYPE = ws.Cell(i, 12).GetString() != "" ? ws.Cell(i, 12).GetValue<string>() : "";
                                        }
                                        catch (Exception ex)
                                        {

                                        } 
                                        try
                                        {
                                            _headCount.Fecha_de_alta = DateTime.Now;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            _headCount.UserUploadId = UserIdUpload;
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        try
                                        {
                                                User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);
                                                _headCount.Usuario_de_alta = userEntity.Name;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        await _supervisorMobilityRepository.AddHeadCoutAsync(_headCount);


                                        retries = 0;
                                        // Si la operación tiene éxito, puedes salir del bucle
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Maneja la excepción aquí, si es necesario
                                        Console.WriteLine($"Intento {retries + 1} falló: {ex.Message}");

                                        // Incrementa el número de intentos
                                        retries++;

                                        // Espera el intervalo de tiempo antes de volver a intentarlo
                                        await Task.Delay(retryInterval);
                                    }



                                }

                            }//end is not empety row
                        }//end else first roe
                        i++;
                    }//end foreach
                    await _supervisorMobilityRepository.SaveChangesAsync();

                    //}//for de paginas

                }//end using



            }//end try
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }//end trycatch to add excel to list






            return Ok();

        }

    }
}
