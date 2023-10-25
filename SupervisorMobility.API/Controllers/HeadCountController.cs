using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.HeadCount;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;
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


        [EnableCors("Cors")]
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
            await _supervisorMobilityRepository.RemoveAllHeadCountRegisters();


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

                                            bool contieneEspacio = valueFunctionDescription.Contains(" ");

                                            if (contieneEspacio)
                                            {
                                                bool contieneNumero = valueFunctionDescription.Any(char.IsDigit);

                                                string[] resultado = valueFunctionDescription.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                                // 0 es la funcion y el resto de area
                                                if (contieneNumero)
                                                {
                                                    //Tiene id de subarea,  extraemos numero
                                                    string numeroString = new string(resultado[1].Where(char.IsDigit).ToArray());

                                                    //convertimos
                                                    if (int.TryParse(numeroString, out int numero))
                                                    {
                                                        //guaramos id
                                                        _headCount.ID_subarea = numero;
                                                        
                                                        try
                                                        {
                                                            _headCount.nombre_subarea = resultado[1].Replace(numeroString, "");
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {
                                                        //fallo el numero asignamos default
                                                        _headCount.ID_subarea = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    //No tiene id de subarea
                                                    _headCount.ID_subarea = 0;

                                                    try
                                                    {
                                                        _headCount.nombre_subarea = resultado[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }

                                                }

                                                try
                                                {
                                                    _headCount.Fuction_Type = resultado[0];
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                               

                                            }
                                            else
                                            {
                                                _headCount.ID_subarea = 0;
                                                _headCount.nombre_subarea = "N/a";
                                                _headCount.Fuction_Type = valueFunctionDescription;
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

                                        Console.WriteLine($"Intento {retries + 1} Linea Position [{i}]");

                                        // Si la operación tiene éxito, puedes salir del bucle
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Maneja la excepción aquí, si es necesario
                                        Console.WriteLine($"Intento {retries + 1} Linea Position [{i}] falló: {ex.Message}");

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HeadCountDto>>> GetAllData()
        {
            var data = await _supervisorMobilityRepository.GetAllHeadCountsDataAsync();

            return Ok(_mapper.Map<IEnumerable<HeadCountDto>>(data));
        }

        [HttpPut("{HeadId}")]
        public async Task<ActionResult> UpdateArea(int HeadId, HeadCountDto ForUpdate)
        {
           
            var HeadCounEntity = await _supervisorMobilityRepository.GetHeadCountByIdAsync(HeadId);
            if (HeadCounEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(ForUpdate, HeadCounEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Bulk/GetData")]
        public async Task<IActionResult> DownloadAllHeadCountRegisters()
        {
            var data = await _supervisorMobilityRepository.GetAllHeadCountsDataAsync();

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");


            ws.SetCellValue("A1", "IdSupervisorMobility");
            ws.SetCellValue("B1", "Codigo");
            ws.SetCellValue("C1", "CO");
            ws.SetCellValue("D1", "ID_AREA");
            ws.SetCellValue("E1", "NOMBRE_AREA");
            ws.SetCellValue("F1", "COST_CENTER");
            ws.SetCellValue("G1", "ID_DEPARTAMENT");
            ws.SetCellValue("H1", "FUNCTION");
            ws.SetCellValue("I1", "ID_SUBAREA");
            ws.SetCellValue("J1", "SUBAREA");
            ws.SetCellValue("K1", "NIVEL");
            ws.SetCellValue("L1", "GRUPO");
            ws.SetCellValue("M1", "BUDGET");
            ws.SetCellValue("N1", "RTO");
            ws.SetCellValue("O1", "HC");
            ws.SetCellValue("P1", "COMENTARIOS");
            ws.SetCellValue("Q1", "LABORTYPE");
            ws.SetCellValue("R1", "FECHADEALTA");
            ws.SetCellValue("S1", "USUARIODEALTA");
            ws.SetCellValue("T1", "USRIdSupervisorMobility");


            int row = 2;
            foreach(var element in data)
            {
                ws.SetCellValue($"A{row}", element.HeadCountId.ToString() ?? "");
                ws.SetCellValue($"B{row}", element.Codigo.ToString() ?? "");
                ws.SetCellValue($"C{row}", element.CO ?? "");
                ws.SetCellValue($"D{row}", element.ID_Area.ToString() ?? "");
                ws.SetCellValue($"E{row}", element.Nombre_Area ?? "");
                ws.SetCellValue($"F{row}", element.Cost_center.ToString() ?? "");
                ws.SetCellValue($"G{row}", element.ID_Departamento.ToString() ?? "");
                ws.SetCellValue($"H{row}", element.Fuction_Type.ToString() ?? "");
                ws.SetCellValue($"I{row}", element.ID_subarea.ToString() ?? "");
                ws.SetCellValue($"J{row}", element.nombre_subarea.ToString() ?? "");
                ws.SetCellValue($"K{row}", element.Nivel.ToString() ?? "");
                ws.SetCellValue($"L{row}", element.Group.ToString() ?? "");
                ws.SetCellValue($"M{row}", element.BUDGET.ToString() ?? "");
                ws.SetCellValue($"N{row}", element.RTO.ToString() ?? "");
                ws.SetCellValue($"O{row}", element.HC.ToString() ?? "");
                ws.SetCellValue($"P{row}", element.Comentarios?.ToString() ?? "");
                ws.SetCellValue($"Q{row}", element.LABOR_TYPE.ToString() ?? "");
                ws.SetCellValue($"R{row}", element.Fecha_de_alta.ToString()  ?? "");
                ws.SetCellValue($"S{row}", element.Usuario_de_alta.ToString() ?? "");
                ws.SetCellValue($"T{row}", element.UserUploadId.ToString() ?? "");

                row++;
            }

            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllHeadCount.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

        [HttpPost("Process")]
        public async Task<ActionResult> CreateProcess(HeadCountProcessCreateUpdateDto HD_Process)
        {

            var finalProcess = _mapper.Map<HeadCountProcess>(HD_Process);

            var result = await _supervisorMobilityRepository.AddHeadCountProcess(finalProcess);

            if (result == 1)
            {
            return Ok(finalProcess);
            }

            return NotFound("No creado");

        }

        [HttpGet("Process")]
        public async Task<ActionResult> ReadAllProcess()
        {

            var allProcess = await _supervisorMobilityRepository.GetAllHeadCountProcess();

            return Ok(allProcess);
        }

        [HttpPut("Process/{id_process}")]
        public async Task<ActionResult> UpdateProcess(int id_process, HeadCountProcessCreateUpdateDto HD_Process)
        {
            var entity = await _supervisorMobilityRepository.GetHeadCountProcessById(id_process);

            var resp = await _supervisorMobilityRepository.UpdateHeadCountProcess(HD_Process, entity);

            if(resp == 1)
            {
            return Ok();
            }

            return NotFound("No actualizado");

        }

        [HttpDelete("Process/{HD_Process_Id}")]
        public async Task<ActionResult> DeleteProcess(int HD_Process_Id)
        {
            var entity = await _supervisorMobilityRepository.GetHeadCountProcessById(HD_Process_Id);

            var resp = await _supervisorMobilityRepository.DeleteHeadCountProcess(entity);

            if (resp == 1)
            {
                return Ok();
            }

            return NotFound("No removido");
        }



    }
}
