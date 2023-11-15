using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.HeadCount;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        private readonly IServiceProvider _serviceProvider;

        public HeadCountController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
        IAssyChartService assyChartService, IServiceProvider serviceProvider)
{
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

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

            try
            {
                await using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    // Utiliza "await" para asegurarte de que se complete la copia del archivo antes de continuar
                    await file.CopyToAsync(fs);
                }
            }catch(Exception ex)
            {
                return NotFound(ex.Message);
            }

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            await _supervisorMobilityRepository.SaveChangesAsync();

        
            /////procesamiento backgrpun

            var headCountProcessingService = new BackgroundProcessingService(_serviceProvider, trustedFileNameForStorage, UserIdUpload);

            // Iniciar el servicio en segundo plano
            await headCountProcessingService.StartAsync(CancellationToken.None);


            ///end background
            return Ok(fileToReturn);

        }

      

        private async Task<int> IsValidFunctionInRow(string process, List<HeadCountProcess> reglas)
        {
            string[] wordsInProcess = process.Split(' ');

            for (int i = 0; i < reglas.Count; i++)
            {
                if (reglas[i].Process.Split(' ').All(palabraG2 => wordsInProcess.Any(palabraG1 => palabraG1.Contains(palabraG2))))
                {
                    return i;
                }
            }

            return -1; // Si no cumple con ninguna regla

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
            foreach (var element in data)
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
                ws.SetCellValue($"R{row}", element.Fecha_de_alta.ToString() ?? "");
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

        //[HttpPost("Process")]
        //public async Task<ActionResult> CreateProcess(HeadCountProcessCreateUpdateDto HD_Process)
        //{

        //    var finalProcess = _mapper.Map<HeadCountProcess>(HD_Process);

        //    var result = await _supervisorMobilityRepository.AddHeadCountProcess(finalProcess);

        //    if (result == 1)
        //    {
        //        return Ok(finalProcess);
        //    }

        //    return NotFound("No creado");

        //}

        //[HttpGet("Process")]
        //public async Task<ActionResult> ReadAllProcess()
        //{

        //    var allProcess = await _supervisorMobilityRepository.GetAllHeadCountProcess();

        //    return Ok(allProcess);
        //}

        //[HttpPut("Process/{id_process}")]
        //public async Task<ActionResult> UpdateProcess(int id_process, HeadCountProcessCreateUpdateDto HD_Process)
        //{
        //    var entity = await _supervisorMobilityRepository.GetHeadCountProcessById(id_process);

        //    var resp = await _supervisorMobilityRepository.UpdateHeadCountProcess(HD_Process, entity);

        //    if (resp == 1)
        //    {
        //        return Ok();
        //    }

        //    return NotFound("No actualizado");

        //}

        //[HttpDelete("Process/{HD_Process_Id}")]
        //public async Task<ActionResult> DeleteProcess(int HD_Process_Id)
        //{
        //    var entity = await _supervisorMobilityRepository.GetHeadCountProcessById(HD_Process_Id);

        //    var resp = await _supervisorMobilityRepository.DeleteHeadCountProcess(entity);

        //    if (resp == 1)
        //    {
        //        return Ok();
        //    }

        //    return NotFound("No removido");
        //}



    }
}
