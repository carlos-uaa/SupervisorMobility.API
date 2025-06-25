using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using DuoVia.FuzzyStrings;
using FuzzyString;
using Slugify;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Math;
using System.ComponentModel;
using SupervisorMobility.API.DataAccess.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace SupervisorMobility.API.Controllers
{

    //[EnableCors]
    //[EnableCors("CorsPolicy")]
    [Route("api/File")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ITreeService _treeService;
        private readonly HttpClient _bridgeHttpClient;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly BackgroundProcessingService _backProcessingService;

        public FileController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
            IAssyChartService assyChartService, ITreeService treeService, CustomHttpClientService customHttp, BackgroundProcessingService backProcessingServic)
        {
            _backProcessingService = backProcessingServic;
            _bridgeHttpClient = customHttp.GetBridgeHttpClient();
            _treeService = treeService;
            _assyChartService = assyChartService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _env = env;
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        //[EnableCors("CorsPolicy")]
        [HttpPost]
        public async Task<ActionResult<FileUploadGeneralDto>> UploadFile(IFormFile file)
        {
            FileUploadForCreationDto uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;

            var trsutedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);


            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);

            if (regexcsv.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "csv");
            else if (regexlsx.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "xlsx");
            else
                trustedFileNameForFileStorage = Path.GetRandomFileName();

            //trustedFileNameForFileStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\assycharts", trustedFileNameForFileStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);


            uploadResult.FileName = untrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForFileStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            return Ok(fileToReturn);
        }
        //[EnableCors("CorsPolicy")]
        [HttpPost("UploadUsers")]
        public async Task<ActionResult<FileUploadGeneralDto>> UploadUsers(IFormFile file)
        {
            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\users", trustedFileNameForStorage);

            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            return Ok(fileToReturn);

        }


        [HttpPost("UploadGuide")]
        public async Task<ActionResult<FileUpload>> UploadGuide(IFormFile file)
        {
            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\guides", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            return Ok(fileToReturn);
        }

        //[EnableCors("CorsPolicy")]
        [HttpPost("UploadEvidences")]
        public async Task<ActionResult<FileUpload>> UploadEvidences(int lupId, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\evidence", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;


            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
            await _supervisorMobilityRepository.AddEvidenceForLupAsync(lupId, fileToReturn);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        //[EnableCors("CorsPolicy")]
        [HttpPost("UploadOperatorSignature")]
        public async Task<ActionResult<FileUpload>> UploadOperatorSignature(int jobObservationId, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\operatorSignature", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;


            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
            await _supervisorMobilityRepository.AddOperatorSignatureForJobObservationAsync(jobObservationId, fileToReturn);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        //[EnableCors("CorsPolicy")]
        [HttpPost("UploadPreviousEvidence")]
        public async Task<ActionResult<FileUpload>> UploadPreviousEvidence(int kaizenId, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\previousEvidence", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;


            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
            await _supervisorMobilityRepository.AddPreviousEvidenceForKaizen(kaizenId, fileToReturn);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        [HttpPost("UploadThenEvidence")]
        public async Task<ActionResult<FileUpload>> UploadThenEvidence(int kaizenId, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\thenEvidence", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;


            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
            await _supervisorMobilityRepository.AddThenEvidenceForKaizen(kaizenId, fileToReturn);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

   
        [HttpGet("Headcount/example")]
        public async Task<IActionResult> DownloadHeadcountExample()
        {
            string filePath = _env.ContentRootPath + "\\Documents\\HC_Example.xlsx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));


        }//end download file function 

        [HttpGet("Bulk/ByPlantId/{plantId}")]
        public async Task<IActionResult> DownloadBulkOnePlant(int plantId)
        {
            List<AssyChartWhitInfo> assyChartsForPlant = _mapper.Map<List<AssyChartWhitInfo>>(await _supervisorMobilityRepository.GetAllAssyChartsByPlantAsync(plantId));

            if (assyChartsForPlant.Count == 0)
            {
                return BadRequest("No data In Plant");
            }

            MemoryStream ms = new MemoryStream();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                // Crear el Workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Crear la hoja
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Vincular hoja al Workbook
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                };
                sheets.Append(sheet);

                // Obtener la hoja
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Fila de encabezados
                Row headerRow1 = new Row();
                headerRow1.Append(
                    new Cell() { CellValue = new CellValue("PlantId"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue(assyChartsForPlant[0].PlantId.ToString()), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("PlantCode"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue(assyChartsForPlant[0].Plant.Code), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("PlantDescription"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue(assyChartsForPlant[0].Plant.Description), DataType = CellValues.String }
                );
                sheetData.Append(headerRow1);

                Row headerRow2 = new Row();
                headerRow2.Append(
                    new Cell() { CellValue = new CellValue("AssyChartId"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("isActive"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("GOS"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("CCP"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("HOE"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("CreationDate"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ModificationDate"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ProductId"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ProductCode"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("ProductDescription"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow2);

                // Agregar datos de las filas
                foreach (var itemUser in assyChartsForPlant)
                {
                    Row dataRow = new Row();
                    dataRow.Append(
                        new Cell() { CellValue = new CellValue(itemUser.AssyChardId.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.IsActive.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.CreationDate.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.ModificationDate.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Area?.AreaId.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Area?.Code), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Area?.Description), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Operation?.OperationId.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Operation?.Code), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Distribution?.DistributionId.ToString()), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(itemUser.Distribution?.Code), DataType = CellValues.String }
                    );
                    sheetData.Append(dataRow);
                }

                workbookPart.Workbook.Save();
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{assyChartsForPlant[0].Plant.Description}.xlsx" ?? "ReportOnePlant.xlsx");
            res.EnableRangeProcessing = true;
            return res;


        }//end download file function

        //falta actualizar esta descarga de la informacion generandola al formato de carga de trabajo
        //[EnableCors("CorsPolicy")]
        [HttpGet("Bulk/ByPlantId")]
        public async Task<IActionResult> DownloadBulkAllPlants()
        {
            List<PlantDto> allPlants = _mapper.Map<List<PlantDto>>(await _supervisorMobilityRepository.GetPlantsAsync());

            Debug.WriteLine($"{allPlants.Count}  {allPlants[0].Code}");


            if (allPlants.Count == 0)
            {
                return BadRequest("No Plants");
            }

            MemoryStream ms = new MemoryStream(6000 * 65536);

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                bool firstSheet = true;

                foreach (var plant in allPlants)
                {
                    // Crear una nueva hoja
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    string sheetName = plant.Description ?? (firstSheet ? "Primer Planta" : "Planta Siguiente");
                    Sheet sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = (uint)(sheets.ChildElements.Count + 1),
                        Name = sheetName
                    };
                    sheets.Append(sheet);

                    // Obtener el objeto SheetData
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Encabezados de la tabla
                    Row headerRow1 = new Row();
                    headerRow1.Append(
                        new Cell() { CellValue = new CellValue("PlantId"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(plant.PlantId.ToString() ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("PlantCode"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(plant.Code ?? ""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("PlantDescription"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(plant.Description ?? ""), DataType = CellValues.String }
                    );
                    sheetData.Append(headerRow1);

                    Row headerRow2 = new Row();
                    headerRow2.Append(
                        new Cell() { CellValue = new CellValue("AssyChartId"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("isActive"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("GOS"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("CCP"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("HOE"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("CreationDate"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("ModificationDate"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("AreaId"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("AreaCode"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("AreaDescription"), DataType = CellValues.String }
                    );
                    sheetData.Append(headerRow2);

                    // Obtener datos de la planta
                    var assyChartsEntitys = await _supervisorMobilityRepository.GetAllAssyChartsByPlantAsync(plant.PlantId);
                    List<AssyChartWhitInfo> assyChartsForPlant = assyChartsEntitys != null
                        ? _mapper.Map<List<AssyChartWhitInfo>>(assyChartsEntitys)
                        : new List<AssyChartWhitInfo>();

                    if (assyChartsForPlant.Count > 0)
                    {
                        int rowIndex = 3;
                        foreach (var itemUser in assyChartsForPlant)
                        {
                            Row dataRow = new Row();
                            dataRow.Append(
                                new Cell() { CellValue = new CellValue(itemUser.AssyChardId.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.IsActive.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.CreationDate.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.ModificationDate.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Area?.AreaId.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Area?.Code ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Area?.Description ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Operation?.OperationId.ToString() ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Operation?.Code ?? ""), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(itemUser.Distribution?.DistributionId.ToString() ?? ""), DataType = CellValues.String }
                            );
                            sheetData.Append(dataRow);
                            rowIndex++;
                        }
                    }

                    firstSheet = false;
                }

                workbookPart.Workbook.Save();
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportAllPlants.xlsx");
            res.EnableRangeProcessing = true;
            return res;



        }//end download file function 


        //[EnableCors("CorsPolicy")]
        [HttpGet("Guide/{fileid}")]
        public async Task<IActionResult> DownloadGuide(int fileid)
        {
            var FileInfo = await _assyChartService.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\guides", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;


                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;


            }
            return NotFound("Error File download");

        }

        //[EnableCors("CorsPolicy")]
        [HttpGet("Users/DownloadAllExample")]
        public async Task<IActionResult> DownloadAllExample()
        {
            string filePath = _env.ContentRootPath + "\\Documents\\All_Example.xlsx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));

            //var path = Path.Combine(_env.ContentRootPath, "\\Documents\\All_Example.xlsx");

            //var memory = new MemoryStream();
            //using (var stream = new FileStream(path, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;


            //var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            //result.EnableRangeProcessing = true;

            //return result;

        }//end download file function 

        //[EnableCors("CorsPolicy")]
        [HttpGet("Users/DownloadSSVExample")]
        public async Task<IActionResult> DownloadSSVExample()
        {
            var path = Path.Combine(_env.ContentRootPath, "\\Documents\\SSV_Example.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            result.EnableRangeProcessing = true;

            return result;

        }//end download file function 

        //[EnableCors("CorsPolicy")]
        [HttpGet("Users/DownloadSupervisorExample")]
        public async Task<IActionResult> DownloadSupervisorExample()
        {

            var path = Path.Combine(_env.ContentRootPath, "\\Documents\\SV_Example.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            result.EnableRangeProcessing = true;

            return result;
        }//end download file function 

        //[EnableCors("CorsPolicy")]
        [HttpGet("Users/DownloadOperatorsExample")]
        public async Task<IActionResult> DownloadOperatorsExample()
        {
            var path = Path.Combine(_env.ContentRootPath, "\\Documents\\Operators_Exmaple.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;


            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            result.EnableRangeProcessing = true;

            return result;

        }//end download file function 


        //[EnableCors("CorsPolicy")]
        [HttpGet("Evidence/{fileid}")]
        public async Task<IActionResult> DownloadEvidence(int fileid)
        {
            var FileInfo = await _assyChartService.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\evidence", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");

        }

        [HttpGet("PreviousEvidence/{fileid}")]
        public async Task<IActionResult> DownloadPreviousEvidence(int fileid)
        {
            var FileInfo = await _assyChartService.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\previousEvidence", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using 
                    (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");

        }

        [HttpGet("ThenEvidence/{fileid}")]
        public async Task<IActionResult> DownloadThenEvidence(int fileid)
        {
            var FileInfo = await _assyChartService.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\thenEvidence", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");

        }

        [HttpGet("Signatures/{fileid}")]
        public async Task<IActionResult> DownloadOperatorSignature(int fileid)
        {
            var FileInfo = await _assyChartService.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\operatorSignature", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");

        }



        //[EnableCors("CorsPolicy")]
        [HttpGet("Bulk/DownloadUsers")]
        public async Task<IActionResult> DownloadAllUsers()
        {
            IEnumerable<User> allUsersList = await _supervisorMobilityRepository.GetAllUsersAsync();


            if (allUsersList.ToList().Count == 0)
            {
                return BadRequest("No Users");
            }

            MemoryStream ms = new MemoryStream();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                // Crear Workbook y Worksheet
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Crear hojas en el Workbook
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Users Bulk"
                };
                sheets.Append(sheet);

                // Acceder a SheetData
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Crear fila de encabezados
                Row headerRow = new Row();
                string[] headers = new[]
                {
            "UserId", "UserName@compasdcpcs.local", "Payroll", "Name", "Email",
            "UserType", "SuperiorId", "SubordinadosId's", "Plant", "Area", "Group", "Distribution"
        };

                foreach (var header in headers)
                {
                    Cell cell = new Cell()
                    {
                        CellValue = new CellValue(header),
                        DataType = CellValues.String
                    };
                    headerRow.Append(cell);
                }
                sheetData.Append(headerRow);

                // Agregar datos de los usuarios
                foreach (var itemUser in allUsersList)
                {
                    Row dataRow = new Row();

                    dataRow.Append(CreateTextCell(itemUser.UserId.ToString()));
                    dataRow.Append(CreateTextCell(itemUser.ObjectId?.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.Payroll.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.Name.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.Email?.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.UserType.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.SuperiorId.ToString() ?? ""));

                    // Subordinados
                    string subs = string.Join(",", itemUser.Subordinates?.Select(s => s.UserId.ToString()) ?? Enumerable.Empty<string>());
                    dataRow.Append(CreateTextCell(subs));

                    dataRow.Append(CreateTextCell(itemUser.PlantId?.ToString() ?? ""));

                    // Áreas
                    if (itemUser.UserType == 2)
                    {
                        string areas = string.Join(",", itemUser.Areas?.Select(a => a.AreaId.ToString()) ?? Enumerable.Empty<string>());
                        dataRow.Append(CreateTextCell(areas));
                    }
                    else
                    {
                        dataRow.Append(CreateTextCell(itemUser.AreaId?.ToString() ?? ""));
                    }

                    dataRow.Append(CreateTextCell(itemUser.GroupId?.ToString() ?? ""));
                    dataRow.Append(CreateTextCell(itemUser.DistributionId?.ToString() ?? ""));

                    sheetData.Append(dataRow);
                }

                workbookPart.Workbook.Save();
            }

            // Aquí puedes usar el MemoryStream (`ms`) según sea necesario.
            ms.Seek(0, SeekOrigin.Begin);


            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersBulk.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

        Cell CreateTextCell(string text)
        {
            return new Cell()
            {
                CellValue = new CellValue(text),
                DataType = CellValues.String
            };
        }

        [HttpPost("MassiveUploadTreeData")]
        public async Task<ActionResult<FileUpload>> MassiveUploadTreeData(IFormFile file, int plantnameid, int userId)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            trustedFileNameForStorage = System.IO.Path.ChangeExtension(trustedFileNameForStorage, ".xlsx");
            var path = Path.Combine(_env.ContentRootPath, "uploads\\massive", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
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

            await _backProcessingService.StartAsync(trustedFileNameForStorage, userId, 2, CancellationToken.None, plantnameid);


            return Ok(fileToReturn);

        }

        //[EnableCors("CorsPolicy")]
        [HttpGet("MassiveUploadTreeDataExample")]
        public async Task<IActionResult> MassiveDownloadDocumentTreeDataTemplate()
        {
            var path = Path.Combine(_env.ContentRootPath, "\\Documents\\TreeDataExample.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            result.EnableRangeProcessing = true;

            return result;

        }//end download file function 


        //[EnableCors("Cors")]
        [HttpPost("MassivePaths")]
        public async Task<ActionResult<FileUpload>> UploadFileAndProccessMassivePaths(IFormFile file, int userId)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            trustedFileNameForStorage = System.IO.Path.ChangeExtension(trustedFileNameForStorage, ".xlsx");
            var path = Path.Combine(_env.ContentRootPath, "uploads\\massive", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                await using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    // Utiliza "await" para asegurarte de que se complete la copia del archivo antes de continuar
                    await file.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            await _supervisorMobilityRepository.SaveChangesAsync();

            await _backProcessingService.StartAsync(trustedFileNameForStorage, userId, 3, CancellationToken.None);


            return Ok(fileToReturn);
        }

        // [EnableCors("CorsPolicy")]
        [HttpGet("MassivePathsExample")]
        public async Task<IActionResult> MassivePathsDownloadDocumentTemplate()
        {
            var path = Path.Combine(_env.ContentRootPath, "\\Documents\\PathsExample.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            result.EnableRangeProcessing = true;

            return result;

        }//end download file function 





    }
}
