using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.HeadCount;
using SupervisorMobility.API.Services;


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
        private readonly BackgroundProcessingService _backgroundProcessingService;

        public HeadCountController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
        IAssyChartService assyChartService, IServiceProvider serviceProvider, BackgroundProcessingService backgroundProcessingService)
        {
            _backgroundProcessingService = backgroundProcessingService;
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
         
            await _backgroundProcessingService.StartAsync(trustedFileNameForStorage, UserIdUpload, 1 , CancellationToken.None);

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

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                // Crear una hoja
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheet sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Users Bulk"
                };
                sheets.Append(sheet);

                // Obtener el objeto SheetData
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Encabezados
                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { CellValue = new CellValue("IdSupervisorMobility"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Codigo"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("CO"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ID_AREA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("NOMBRE_AREA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("COST_CENTER"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ID_DEPARTAMENT"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("FUNCTION"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ID_SUBAREA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("SUBAREA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("NIVEL"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("GRUPO"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("BUDGET"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("RTO"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("HC"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("COMENTARIOS"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("LABORTYPE"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("FECHADEALTA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("USUARIODEALTA"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("USRIdSupervisorMobility"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                // Agregar datos
                int rowNumber = 2;
                foreach (var element in data)
                {
                    Row dataRow = new Row();
                    dataRow.Append(
                        new Cell() { CellValue = new CellValue(element.HeadCountId.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Codigo.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.CO ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.ID_Area?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Nombre_Area ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Cost_center?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.ID_Departamento?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Fuction_Type?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.ID_subarea?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.nombre_subarea?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Nivel?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Group?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.BUDGET?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.RTO?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.HC?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Comentarios?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.LABOR_TYPE?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Fecha_de_alta.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.Usuario_de_alta?.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(element.UserUploadId?.ToString() ?? ""), DataType = CellValues.String }
                    );
                    sheetData.Append(dataRow);
                    rowNumber++;
                }

                workbookPart.Workbook.Save();
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllHeadCount.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

       


    }
}
