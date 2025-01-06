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

       
       

        //10 Oct hace falta actualizar esta descarga de la informacion generandola al formato de carga de trabajo

        [HttpGet("Bulk/ByPlantId/{plantId}")]
        public async Task<IActionResult> DownloadBulkOnePlant(int plantId)
        {
            List<AssyChartWhitInfo> assyChartsForPlant = _mapper.Map<List<AssyChartWhitInfo>>(await _supervisorMobilityRepository.GetAllAssyChartsByPlantAsync(plantId));

            if (assyChartsForPlant.Count == 0)
            {
                return BadRequest("No data In Plant");
            }

            MemoryStream ms = new MemoryStream();
            using (SLDocument ws = new SLDocument())
            {
                //Plant ROW data
                ws.SetCellValue("A1", "PlantId");
                ws.SetCellValue("B1", assyChartsForPlant[0].PlantId);

                ws.SetCellValue("D1", "PlantCode");
                ws.SetCellValue("E1", assyChartsForPlant[0].Plant.Code);

                ws.SetCellValue("G1", "PlantId");
                ws.SetCellValue("H1", assyChartsForPlant[0].Plant.Description);

                //ROW Data identificators

                ws.SetCellValue("A2", "AssyChartId");
                ws.SetCellValue("B2", "isActive");
                ws.SetCellValue("C2", "GOS");
                ws.SetCellValue("D2", "CCP");
                ws.SetCellValue("E2", "HOE");
                ws.SetCellValue("F2", "CreationDate");
                ws.SetCellValue("G2", "ModificationDate");
                ws.SetCellValue("H2", "ProductId");
                ws.SetCellValue("I2", "ProductCode");
                ws.SetCellValue("J2", "ProductDescription");
                ws.SetCellValue("K2", "ProductIsActive");
                ws.SetCellValue("L2", "AreaId");
                ws.SetCellValue("M2", "AreaCode");
                ws.SetCellValue("N2", "AreaDescription");
                ws.SetCellValue("O2", "AreaIsActive");
                ws.SetCellValue("P2", "OperationId");
                ws.SetCellValue("Q2", "OperationCode");
                ws.SetCellValue("R2", "OperationDescription");
                ws.SetCellValue("S2", "OperationIsActive");
                ws.SetCellValue("T2", "DistributionId");
                ws.SetCellValue("U2", "DistributionCode");
                ws.SetCellValue("V2", "DistributionDescription");
                ws.SetCellValue("W2", "DistributionIsActive");

                int row = 3;
                foreach (var itemUser in assyChartsForPlant)
                {

                    ws.SetCellValue($"A{row}", itemUser.AssyChardId.ToString() ?? "");
                    ws.SetCellValue($"B{row}", itemUser.IsActive.ToString() ?? "");
                    //ws.SetCellValue($"C{row}", itemUser.GOS ?? "");
                    //ws.SetCellValue($"D{row}", itemUser.CCP ?? "");
                    //ws.SetCellValue($"E{row}", itemUser.HOE);
                    ws.SetCellValue($"F{row}", itemUser.CreationDate.ToString() ?? "");
                    ws.SetCellValue($"G{row}", itemUser.ModificationDate.ToString() ?? "");
                    //ws.SetCellValue($"H{row}", itemUser.Product?.ProductId.ToString() ?? "");
                    //ws.SetCellValue($"I{row}", itemUser.Product?.Code ?? "");
                    //ws.SetCellValue($"J{row}", itemUser.Product?.Description ?? "");
                    //ws.SetCellValue($"K{row}", itemUser.Product?.IsActive?.ToString() ?? "");
                    ws.SetCellValue($"L{row}", itemUser.Area?.AreaId.ToString() ?? "");
                    ws.SetCellValue($"M{row}", itemUser.Area?.Code ?? "");
                    ws.SetCellValue($"N{row}", itemUser.Area?.Description ?? "");
                    ws.SetCellValue($"O{row}", itemUser.Area?.IsActive?.ToString() ?? "");
                    ws.SetCellValue($"P{row}", itemUser.Operation?.OperationId.ToString() ?? "");
                    ws.SetCellValue($"Q{row}", itemUser.Operation?.Code ?? "");
                    ws.SetCellValue($"R{row}", itemUser.Operation?.Description ?? "");
                    ws.SetCellValue($"S{row}", itemUser.Operation?.IsActive.ToString() ?? "");
                    ws.SetCellValue($"T{row}", itemUser.Distribution?.DistributionId.ToString() ?? "");
                    ws.SetCellValue($"U{row}", itemUser.Distribution?.Code ?? "");
                    ws.SetCellValue($"V{row}", itemUser.Distribution?.Description ?? "");
                    ws.SetCellValue($"W{row}", itemUser.Distribution?.IsActive?.ToString() ?? "");
                    row++;
                }

                ws.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
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
            SLDocument ws = new SLDocument();
            bool firstSheet = true;

            foreach (var plant in allPlants)
            {
                if (firstSheet)
                {
                    ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, plant.Description ?? "Primer Planta");
                    firstSheet = false;
                }
                else
                {
                    ws.AddWorksheet(plant.Description ?? "Planta Siguiente");
                }

                //Plant ROW data
                ws.SetCellValue("A1", "PlantId");
                ws.SetCellValue("B1", plant.PlantId.ToString() ?? "");

                ws.SetCellValue("D1", "PlantCode");
                ws.SetCellValue("E1", plant.Code ?? "");

                ws.SetCellValue("G1", "PlantId");
                ws.SetCellValue("H1", plant.Description ?? "");

                //ROW Data identificators

                ws.SetCellValue("A2", "AssyChartId");
                ws.SetCellValue("B2", "isActive");
                ws.SetCellValue("C2", "GOS");
                ws.SetCellValue("D2", "CCP");
                ws.SetCellValue("E2", "HOE");
                ws.SetCellValue("F2", "CreationDate");
                ws.SetCellValue("G2", "ModificationDate");
                ws.SetCellValue("H2", "ProductId");
                ws.SetCellValue("I2", "ProductCode");
                ws.SetCellValue("J2", "ProductDescription");
                ws.SetCellValue("K2", "ProductIsActive");
                ws.SetCellValue("L2", "AreaId");
                ws.SetCellValue("M2", "AreaCode");
                ws.SetCellValue("N2", "AreaDescription");
                ws.SetCellValue("O2", "AreaIsActive");
                ws.SetCellValue("P2", "OperationId");
                ws.SetCellValue("Q2", "OperationCode");
                ws.SetCellValue("R2", "OperationDescription");
                ws.SetCellValue("S2", "OperationIsActive");
                ws.SetCellValue("T2", "DistributionId");
                ws.SetCellValue("U2", "DistributionCode");
                ws.SetCellValue("V2", "DistributionDescription");
                ws.SetCellValue("W2", "DistributionIsActive");

                var assyChartsEntitys = await _supervisorMobilityRepository.GetAllAssyChartsByPlantAsync(plant.PlantId);
                List<AssyChartWhitInfo> assyChartsForPlant = new List<AssyChartWhitInfo>();
                if (assyChartsEntitys != null)
                {
                    assyChartsForPlant = _mapper.Map<List<AssyChartWhitInfo>>(assyChartsEntitys);
                }


                if (assyChartsForPlant.Count != 0)
                {

                    int row = 3;
                    foreach (var itemUser in assyChartsForPlant)
                    {
                        ws.SetCellValue($"A{row}", itemUser.AssyChardId.ToString() ?? "");
                        ws.SetCellValue($"B{row}", itemUser.IsActive.ToString() ?? "");
                        //ws.SetCellValue($"C{row}", itemUser.GOS ?? "");
                        //ws.SetCellValue($"D{row}", itemUser.CCP ?? "");
                        //ws.SetCellValue($"E{row}", itemUser.HOE);
                        ws.SetCellValue($"F{row}", itemUser.CreationDate.ToString() ?? "");
                        ws.SetCellValue($"G{row}", itemUser.ModificationDate.ToString() ?? "");
                        //ws.SetCellValue($"H{row}", itemUser.Product?.ProductId.ToString() ?? "");
                        //ws.SetCellValue($"I{row}", itemUser.Product?.Code ?? "");
                        //ws.SetCellValue($"J{row}", itemUser.Product?.Description ?? "");
                        //ws.SetCellValue($"K{row}", itemUser.Product?.IsActive?.ToString() ?? "");
                        ws.SetCellValue($"L{row}", itemUser.Area?.AreaId.ToString() ?? "");
                        ws.SetCellValue($"M{row}", itemUser.Area?.Code ?? "");
                        ws.SetCellValue($"N{row}", itemUser.Area?.Description ?? "");
                        ws.SetCellValue($"O{row}", itemUser.Area?.IsActive?.ToString() ?? "");
                        ws.SetCellValue($"P{row}", itemUser.Operation?.OperationId.ToString() ?? "");
                        ws.SetCellValue($"Q{row}", itemUser.Operation?.Code ?? "");
                        ws.SetCellValue($"R{row}", itemUser.Operation?.Description ?? "");
                        ws.SetCellValue($"S{row}", itemUser.Operation?.IsActive.ToString() ?? "");
                        ws.SetCellValue($"T{row}", itemUser.Distribution?.DistributionId.ToString() ?? "");
                        ws.SetCellValue($"U{row}", itemUser.Distribution?.Code ?? "");
                        ws.SetCellValue($"V{row}", itemUser.Distribution?.Description ?? "");
                        ws.SetCellValue($"W{row}", itemUser.Distribution?.IsActive?.ToString() ?? "");
                        row++;
                    }
                }
            }



            ws.SaveAs(ms);

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
            string filePath = _env.ContentRootPath + "Documents\\All_Example.xlsx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));

            //var path = Path.Combine(_env.ContentRootPath, "Documents\\All_Example.xlsx");

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
            var path = Path.Combine(_env.ContentRootPath, "Documents\\SSV_Example.xlsx");

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

            var path = Path.Combine(_env.ContentRootPath, "Documents\\SV_Example.xlsx");

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
            var path = Path.Combine(_env.ContentRootPath, "Documents\\Operators_Exmaple.xlsx");

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
            SLDocument ws = new SLDocument();


            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "UserId");
            ws.SetCellValue("B1", "UserName@compasdcpcs.local");
            ws.SetCellValue("C1", "Payroll");
            ws.SetCellValue("D1", "Name");
            ws.SetCellValue("E1", "Email");
            ws.SetCellValue("F1", "UserType");
            ws.SetCellValue("G1", "SuperiorId");
            ws.SetCellValue("H1", "SubordinadosId's");
            ws.SetCellValue("I1", "Plant");
            ws.SetCellValue("J1", "Area");
            ws.SetCellValue("K1", "Group");
            ws.SetCellValue("L1", "Distribution");



            int row = 2;
            foreach (var itemUser in allUsersList)
            {
                ws.SetCellValue("A1", "UserId");
                ws.SetCellValue("B1", "UserName@compasdcpcs.local");
                ws.SetCellValue("C1", "Payroll");
                ws.SetCellValue("D1", "Name");
                ws.SetCellValue("E1", "Email");
                ws.SetCellValue("F1", "UserType");
                ws.SetCellValue("G1", "SuperiorId");
                ws.SetCellValue("H1", "SubordinadosId's");
                ws.SetCellValue("I1", "Plant");
                ws.SetCellValue("J1", "Area");
                ws.SetCellValue("K1", "Group");
                ws.SetCellValue("L1", "Distribution");
                ws.SetCellValue($"A{row}", itemUser.UserId.ToString() ?? "");
                ws.SetCellValue($"B{row}", itemUser.ObjectId?.ToString() ?? "");
                ws.SetCellValue($"C{row}", itemUser.Payroll.ToString() ?? "");
                ws.SetCellValue($"D{row}", itemUser.Name.ToString() ?? "");
                ws.SetCellValue($"E{row}", itemUser.Email?.ToString() ?? ""); ;

                ws.SetCellValue($"F{row}", itemUser.UserType.ToString() ?? "");

                ws.SetCellValue($"G{row}", itemUser.SuperiorId.ToString() ?? "");
                var subs = "";
                if (itemUser.Subordinates?.Count > 0)
                {
                    foreach (var subitem in itemUser.Subordinates)
                    {
                        subs += $"{subitem.UserId},";
                    }
                    ws.SetCellValue($"H{row}", subs ?? "");

                }

                ws.SetCellValue($"I{row}", itemUser.PlantId?.ToString() ?? "");

                if (itemUser.UserType == 2)
                {
                    var areas = "";
                    if (itemUser.Areas?.Count > 0)
                    {
                        foreach (var itemArea in itemUser.Areas)
                        {
                            areas += $"{itemArea.AreaId},";
                        }
                    }
                    ws.SetCellValue($"J{row}", areas ?? "");

                }
                else
                {
                    ws.SetCellValue($"J{row}", itemUser.AreaId?.ToString() ?? "");

                }

                ws.SetCellValue($"K{row}", itemUser.GroupId?.ToString() ?? "");
                ws.SetCellValue($"L{row}", itemUser.DistributionId?.ToString() ?? "");
                row++;
            }



            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersBulk.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 
        

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
            var path = Path.Combine(_env.ContentRootPath, "Documents\\TreeDataExample.xlsx");

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
            var path = Path.Combine(_env.ContentRootPath, "Documents\\PathsExample.xlsx");

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
