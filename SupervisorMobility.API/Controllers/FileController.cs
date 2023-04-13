using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using SpreadsheetLight;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.DataAccess.Entities;
using Microsoft.AspNetCore.Cors;
using ClosedXML.Excel;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ReturnResults;

namespace SupervisorMobility.API.Controllers
{


    [Route("api/File")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public FileController(IWebHostEnvironment env, IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository,
            IAssyChartService assyChartService)
        {
            _assyChartService = assyChartService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _env = env;
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


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

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);


            uploadResult.FileName = untrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForFileStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;

            var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);

            return Ok(fileToReturn);
        }

        [HttpPost("UploadUsers")]
        public async Task<ActionResult<FileUploadGeneralDto>> UploadUsers(IFormFile file)
        {
            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\users", trustedFileNameForStorage);

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


        [HttpPost("UploadEvidences")]
        public async Task<ActionResult<FileUpload>> UploadEvidences(int lupId, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\evidence", trustedFileNameForStorage);

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


        [HttpPost("Data")]
        public async Task<ActionResult<FileUploadGeneralDto>> UpdateDataInServer(FileUploadGeneralDto FileInfo)
        {


            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\assycharts" + FileInfo.StorageFileName;
            List<AssyChartDataToBulk> DataList = new List<AssyChartDataToBulk>();

            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);

            Plant PlantInfo = new Plant("", "");

            if (regexcsv.IsMatch(FileInfo.StorageFileName))
            {
                try
                {
                    //load data from csv into the list

                    using (TextFieldParser csvParser = new TextFieldParser(file))
                    {

                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        bool FirstRow = true;
                        while (!csvParser.EndOfData)
                        {
                            string[] fields = csvParser.ReadFields();

                            if (FirstRow)
                            {
                                PlantInfo = new Plant(fields[4], fields[7]);

                                PlantInfo.PlantId = fields[1] != "" ? int.Parse(fields[1]) : -1;
                                //junp heades line
                                csvParser.ReadLine();
                                FirstRow = false;

                            }
                            else
                            {
                                var ToInsertIntoList = new AssyChartDataToBulk();

                                ToInsertIntoList.PlantId = PlantInfo.PlantId;
                                ToInsertIntoList.Plant = PlantInfo;

                                ToInsertIntoList.AssyChardId = fields[0] != "" ? int.Parse(fields[0]) : -1;
                                ToInsertIntoList.IsActive = fields[1] != "" ? bool.Parse(fields[1]) : true;

                                ToInsertIntoList.GOS = fields[2] != "" ? fields[2] : "";
                                ToInsertIntoList.CCP = fields[3] != "" ? fields[3] : "";
                                ToInsertIntoList.HOE = fields[4] != "" ? fields[4] : "";


                                try
                                {
                                    ToInsertIntoList.CreationDate = fields[5] != "" ? DateTime.Parse(fields[5]) : DateTime.Now;

                                }
                                catch (Exception ex)
                                {
                                    ToInsertIntoList.CreationDate = DateTime.Now;

                                }

                                try
                                {
                                    ToInsertIntoList.ModificationDate = fields[6] != "" ? DateTime.Parse(fields[6]) : DateTime.Now;

                                }
                                catch (Exception ex)
                                {
                                    ToInsertIntoList.ModificationDate = DateTime.Now;

                                }


                                ToInsertIntoList.ProductId = fields[7] != "" ? int.Parse(fields[7]) : -1;
                                ToInsertIntoList.ProductCode = fields[8] != "" ? fields[8] : "";
                                ToInsertIntoList.ProductDescription = fields[9] != "" ? fields[9] : "";
                                ToInsertIntoList.ProductIsActive = fields[10] != "" ? bool.Parse(fields[10]) : true;

                                ToInsertIntoList.AreaId = fields[11] != "" ? int.Parse(fields[11]) : -1;
                                ToInsertIntoList.AreaCode = fields[12] != "" ? fields[12] : "";
                                ToInsertIntoList.AreaDescription = fields[13] != "" ? fields[13] : "";
                                ToInsertIntoList.AreaIsActive = fields[14] != " " ? bool.Parse(fields[14]) : true;

                                ToInsertIntoList.OperationId = fields[15] != "" ? int.Parse(fields[15]) : -1;
                                ToInsertIntoList.OperationCode = fields[16] != "" ? fields[16] : "";
                                ToInsertIntoList.OperationDescription = fields[17] != "" ? fields[17] : "";
                                ToInsertIntoList.OperationIsActive = fields[18] != "" ? bool.Parse(fields[18]) : true;

                                ToInsertIntoList.DistributionId = fields[19] != "" ? int.Parse(fields[19]) : -1;
                                ToInsertIntoList.DistributionCode = fields[20] != "" ? fields[20] : "";
                                ToInsertIntoList.DistributionDescription = fields[21] != "" ? fields[21] : "";
                                ToInsertIntoList.DistributionIsActive = fields[22] != "" ? bool.Parse(fields[22]) : true;

                                DataList.Add(ToInsertIntoList);
                            }


                        }
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }//end input data from csv

            if (regexlsx.IsMatch(FileInfo.StorageFileName))
            {
                try
                {

                    using (var workBook = new XLWorkbook(file))
                    {
                        IXLWorksheet ws = workBook.Worksheet(1);

                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        bool SecondRow = true;
                        int i = 1;
                        foreach (IXLRow row in ws.Rows())

                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow && SecondRow)
                            {

                                PlantInfo = new Plant(ws.Cell(i, 5).Value.ToString(), ws.Cell(i, 8).Value.ToString());

                                PlantInfo.PlantId = ws.Cell(i, 2).GetString() != "" ? (int)ws.Cell(i, 2).Value : -1;
                                i++;
                                firstRow = false;
                            }
                            else if (SecondRow && !firstRow)
                            {
                                SecondRow = false;
                                i++;
                            }
                            else
                            {
                                if (!row.IsEmpty())
                                {

                                    var ToInsertIntoList = new AssyChartDataToBulk();

                                    ToInsertIntoList.PlantId = PlantInfo.PlantId;
                                    ToInsertIntoList.Plant = PlantInfo;

                                    ToInsertIntoList.AssyChardId = ws.Cell(i, 1).GetString() != "" ? ws.Cell(i, 1).GetValue<int>() : -1;
                                    ToInsertIntoList.IsActive = ws.Cell(i, 2).GetString() != "" ? ws.Cell(i, 2).GetValue<bool>() : true;

                                    ToInsertIntoList.GOS = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                    ToInsertIntoList.CCP = ws.Cell(i, 4).GetString() != "" ? ws.Cell(i, 4).GetValue<string>() : "";
                                    ToInsertIntoList.HOE = ws.Cell(i, 5).GetString() != "" ? ws.Cell(i, 5).GetValue<string>() : "";
                                    try
                                    {
                                        ToInsertIntoList.CreationDate = ws.Cell(i, 6).GetString() != "" ? DateTime.Parse(ws.Cell(i, 6).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        ToInsertIntoList.CreationDate = DateTime.Now;
                                    }
                                    try
                                    {
                                        ToInsertIntoList.ModificationDate = ws.Cell(i, 7).GetString() != "" ? DateTime.Parse(ws.Cell(i, 6).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        ToInsertIntoList.ModificationDate = DateTime.Now;
                                    }
                                    ToInsertIntoList.ProductId = ws.Cell(i, 8).GetString() != "" ? ws.Cell(i, 8).GetValue<int>() : -1;
                                    ToInsertIntoList.ProductCode = ws.Cell(i, 9).GetString() != "" ? ws.Cell(i, 9).GetValue<string>() : "";
                                    ToInsertIntoList.ProductDescription = ws.Cell(i, 10).GetString() != "" ? ws.Cell(i, 10).GetValue<string>() : "";
                                    ToInsertIntoList.ProductIsActive = ws.Cell(i, 11).GetString() != "" ? ws.Cell(i, 11).GetValue<bool>() : true;

                                    ToInsertIntoList.AreaId = ws.Cell(i, 12).GetString() != "" ? ws.Cell(i, 12).GetValue<int>() : -1;
                                    ToInsertIntoList.AreaCode = ws.Cell(i, 13).GetString() != "" ? ws.Cell(i, 13).GetValue<string>() : "";
                                    ToInsertIntoList.AreaDescription = ws.Cell(i, 14).GetString() != "" ? ws.Cell(i, 14).GetValue<string>() : "";
                                    ToInsertIntoList.AreaIsActive = ws.Cell(i, 15).GetString() != "" ? ws.Cell(i, 15).GetValue<bool>() : true;

                                    ToInsertIntoList.OperationId = ws.Cell(i, 16).GetString() != "" ? ws.Cell(i, 16).GetValue<int>() : -1;
                                    ToInsertIntoList.OperationCode = ws.Cell(i, 17).GetString() != "" ? ws.Cell(i, 17).GetValue<string>() : "";
                                    ToInsertIntoList.OperationDescription = ws.Cell(i, 18).GetString() != "" ? ws.Cell(i, 18).GetValue<string>() : "";
                                    ToInsertIntoList.OperationIsActive = ws.Cell(i, 19).GetString() != "" ? ws.Cell(i, 19).GetValue<bool>() : true;

                                    ToInsertIntoList.DistributionId = ws.Cell(i, 20).GetString() != "" ? ws.Cell(i, 20).GetValue<int>() : -1;
                                    ToInsertIntoList.DistributionCode = ws.Cell(i, 21).GetString() != "" ? ws.Cell(i, 21).GetValue<string>() : "";
                                    ToInsertIntoList.DistributionDescription = ws.Cell(i, 22).GetString() != "" ? ws.Cell(i, 22).GetValue<string>() : "";
                                    ToInsertIntoList.DistributionIsActive = ws.Cell(i, 23).GetString() != "" ? ws.Cell(i, 23).GetValue<bool>() : true;

                                    DataList.Add(ToInsertIntoList);
                                    i++;
                                }
                            }

                        }//end foreach

                    }//end using

                    Debug.WriteLine($"plant id {PlantInfo.PlantId} code {PlantInfo.Code} description: {PlantInfo.Description}");


                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }//end trycatch to add excel to list
            }//end read data from excel file


            //*****************************************************//

            //created result array to return
            UploadAssyChartResult ResumeActionsResultsToReturn = new UploadAssyChartResult();



            //verify if plant is a new plant or update
            if (PlantInfo.PlantId == -1)
            {
                //a new plant of a missing field id in doc
                //check if plant exist in case of user not input id, but exist code and description
                if (PlantInfo.Description != "" && PlantInfo.Code != "")
                {
                    //have description and code, plant exist, get by this info
                    if (!await _supervisorMobilityRepository.PlantExistByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description))
                    {
                        //chek if not exist plant, if not exis, 
                        ResumeActionsResultsToReturn.PlantCreate++;
                        //creating plant
                        var plantForCreate = _mapper.Map<PlantForCreationDto>(PlantInfo);
                        var finalPlant = await _assyChartService.CreatePlantAsync(plantForCreate);
                        //save create plant
                        await _supervisorMobilityRepository.SaveChangesAsync();
                        //get id to use in updates and creates
                        PlantInfo.PlantId = finalPlant.PlantId;
                    }
                    else
                    {

                        //the plant exists, and they deleted the id in the document
                        var GetInfoPlantBecauseExist = await _supervisorMobilityRepository.GetPlantByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description);
                        //get id of the plant
                        PlantInfo = GetInfoPlantBecauseExist;
                        Debug.WriteLine($"Debug: Plantid : {PlantInfo.PlantId}");
                    }
                }
                else if (PlantInfo.Description == "" && PlantInfo.Code == "")
                {
                    var result = new BadRequestObjectResult("Error, Missing fields plant in documents, pls fix it");
                    result.StatusCode = StatusCodes.Status409Conflict;
                    return result;
                }
                else
                {
                    var result = new BadRequestObjectResult("Error, please consult your add mannager");
                    result.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return result;
                }
            }
            else // else plant, have id
            {
                //get plant to check any change in code and description
                Plant? plantEntityInDataBase = await _supervisorMobilityRepository.GetPlantAsync(PlantInfo.PlantId, false);
                //is not null plant
                if (plantEntityInDataBase != null)
                {
                    //check if any field match, to verify that it is not another plant 
                    if (plantEntityInDataBase.Code != PlantInfo.Code && plantEntityInDataBase.Description != PlantInfo.Description)
                    {
                        //its a diferent plant try to search between code and description
                        if (!await _supervisorMobilityRepository.PlantExistByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description))
                        {
                            //plant not exist, add 1 to create in result 
                            ResumeActionsResultsToReturn.PlantCreate++;
                            //creating plant
                            var plantForCreate = _mapper.Map<PlantForCreationDto>(PlantInfo);
                            var finalPlant = await _assyChartService.CreatePlantAsync(plantForCreate);
                            //save changes in db create plant
                            await _supervisorMobilityRepository.SaveChangesAsync();
                            //get id to use in updates and creates
                            PlantInfo.PlantId = finalPlant.PlantId;
                        }
                        else
                        {
                            //get plant whit code and description
                            var GetInfoPlantBecauseExist = await _supervisorMobilityRepository.GetPlantByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description);
                            PlantInfo = GetInfoPlantBecauseExist;
                            Debug.WriteLine($"Debug 2 - plant have id, not exist #id in db, exist whit code: Plantid : {PlantInfo.PlantId}");
                        }
                    }
                }
                else
                {
                    //try to verify existence between code and description 
                    if (!await _supervisorMobilityRepository.PlantExistByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description))
                    {
                        //plant not exist, add 1 to create in result 
                        ResumeActionsResultsToReturn.PlantCreate++;
                        //creating plant
                        var plantForCreate = _mapper.Map<PlantForCreationDto>(PlantInfo);
                        var finalPlant = await _assyChartService.CreatePlantAsync(plantForCreate);
                        //save changes in db create plant
                        await _supervisorMobilityRepository.SaveChangesAsync();
                        //get id to use in updates and creates
                        PlantInfo.PlantId = finalPlant.PlantId;
                    }
                    else
                    {
                        //get plant whit code and description
                        var GetInfoPlantBecauseExist = await _supervisorMobilityRepository.GetPlantByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description);
                        PlantInfo = GetInfoPlantBecauseExist;
                        Debug.WriteLine($"Debug 2 - plant have id, not exist #id in db, exist whit code: Plantid : {PlantInfo.PlantId}");
                    }
                    //else create plant                   
                }
            }//end plant login update 4.0  :'v 

            try
            {
                //foreach toget all assy charts
                foreach (AssyChartDataToBulk item in DataList)
                {
                    //var indexarea = -1;
                    //var indexdistribution = -1;
                    //var indexoperacion = -1;
                    //assign values for assy chart to update or crate
                    AssyChartWithoutNavigationProperties finalAssyChart = new AssyChartWithoutNavigationProperties()
                    {
                        AssyChardId = item.AssyChardId ?? -1,
                        GOS = item.GOS ?? "",
                        CCP = item.CCP ?? "",
                        HOE = item.HOE ?? "",
                        ProductId = item.ProductId ?? -1,
                        PlantId = PlantInfo.PlantId,
                        AreaId = item.AreaId ?? -1,
                        DistributionId = item.DistributionId ?? -1,
                        OperationId = item.OperationId ?? -1,
                        CreationDate = item.CreationDate ?? DateTime.Now,
                        ModificationDate = item.ModificationDate ?? DateTime.Now,
                    };

                    if (finalAssyChart.AreaId == -1)
                    {
                        //check if area exist in case of user not input id, but exist code and description
                        if (item.AreaDescription != "" && item.AreaCode != "")
                        {
                            //have description and code, area maybe exist, try to get by this info
                            if (!await _supervisorMobilityRepository.AreaExistByCodeAndDescriptionInPlantAsync(item.AreaCode, item.AreaDescription, finalAssyChart.PlantId))
                            {
                                ResumeActionsResultsToReturn.AreasCreated++;
                                var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = item.AreaCode, Description = item.AreaDescription, IsActive = item.AreaIsActive ?? true });
                                var finalArea = _mapper.Map<Area>(areaForCreate);
                                await _supervisorMobilityRepository.AddAreaForPlantAsync(PlantInfo.PlantId, finalArea);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                var createdAreaToSave = _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);
                                finalAssyChart.AreaId = createdAreaToSave.AreaId;
                            }
                            else
                            {
                                //the area exists, and they deleted the id in the document
                                var GetInfoForArea = await _supervisorMobilityRepository.GetAreaForPlantByCodeAndDescriptionAsync(finalAssyChart.PlantId, item.AreaCode, item.AreaDescription);
                                finalAssyChart.AreaId = GetInfoForArea.AreaId;
                            }
                        }
                        else if (item.AreaCode == "" && item.AreaDescription == "")
                        {
                            var result = new BadRequestObjectResult("Error, Missing fields Area in documents, pls fix it");
                            result.StatusCode = StatusCodes.Status409Conflict;
                            return result;
                        }
                        else
                        {
                            var result = new BadRequestObjectResult("Error, please consult your add mannager");
                            result.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return result;
                        }
                    }
                    else //area have id
                    {
                        //get area whit id
                        Area? areaEntityInDataBase = await _supervisorMobilityRepository.GetAreaForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, false);
                        //is not null plant
                        if (areaEntityInDataBase != null)
                        {
                            //verified that it is not an area of another plant.
                            if (areaEntityInDataBase.PlantId != finalAssyChart.PlantId)
                            {
                                //its a diferent area, try to search between code and description in plant
                                if (!await _supervisorMobilityRepository.AreaExistByCodeAndDescriptionInPlantAsync(item.AreaCode, item.AreaDescription, finalAssyChart.PlantId))
                                {
                                    //area not exist, add 1 to create in result 
                                    ResumeActionsResultsToReturn.AreasCreated++;
                                    var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = item.AreaCode, Description = item.AreaDescription, IsActive = item.AreaIsActive ?? true });
                                    var finalArea = _mapper.Map<Area>(areaForCreate);
                                    await _supervisorMobilityRepository.AddAreaForPlantAsync(PlantInfo.PlantId, finalArea);
                                    await _supervisorMobilityRepository.SaveChangesAsync();
                                    finalAssyChart.AreaId = finalArea.AreaId;
                                }
                                else
                                {
                                    //get area whit code and description
                                    var GetInfoForArea = await _supervisorMobilityRepository.GetAreaForPlantByCodeAndDescriptionAsync(finalAssyChart.PlantId, item.AreaCode, item.AreaDescription);
                                    finalAssyChart.AreaId = GetInfoForArea.AreaId;
                                    Debug.WriteLine($"Debug 2 AREA - area have id, not exist #id in db, exist whit code: area : {finalAssyChart.AreaId}");
                                }
                            }
                        }
                        else //have id but entity is null
                        {
                            //try to verify existence between code and description if id not match
                            if (!await _supervisorMobilityRepository.AreaExistByCodeAndDescriptionInPlantAsync(item.AreaCode, item.AreaDescription, finalAssyChart.PlantId))
                            {
                                ResumeActionsResultsToReturn.AreasCreated++;
                                var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = item.AreaCode, Description = item.AreaDescription, IsActive = item.AreaIsActive ?? true });
                                var finalArea = _mapper.Map<Area>(areaForCreate);
                                await _supervisorMobilityRepository.AddAreaForPlantAsync(PlantInfo.PlantId, finalArea);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                finalAssyChart.AreaId = finalArea.AreaId;
                            }
                            else
                            {
                                var GetInfoForArea = await _supervisorMobilityRepository.GetAreaForPlantByCodeAndDescriptionAsync(finalAssyChart.PlantId, item.AreaCode, item.AreaDescription);
                                finalAssyChart.AreaId = GetInfoForArea.AreaId;
                                Debug.WriteLine($"Debug 2AREA - area have id, not exist #id in db, exist whit code: areaide : {finalAssyChart.AreaId}");
                            }
                        }

                    }//end if area 

                    //Distribucion
                    if (finalAssyChart.DistributionId == -1)
                    {
                        //check if distribution exist in case of user not input id, but exist code and description
                        if (item.DistributionCode != "" && item.DistributionDescription != "")
                        {
                            //have description and code, area maybe exist, try to get by this info
                            if (!await _supervisorMobilityRepository.DistributionExistsByCodeandDescriptionInAreaAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionCode))
                            {
                                ResumeActionsResultsToReturn.DistributionCreated++;
                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = item.DistributionIsActive ?? true });
                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                                await _supervisorMobilityRepository.AddDistributionForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, finalDistribution);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                finalAssyChart.DistributionId = finalDistribution.DistributionId;
                            }
                            else
                            {
                                //the distribution exists, and they deleted the id in the document
                                var GetInfoForDistributiom = await _supervisorMobilityRepository.GetDistributionForAreaByCodeAndDescriptionAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription);
                                finalAssyChart.DistributionId = GetInfoForDistributiom.DistributionId;
                            }
                        }
                        else if (item.DistributionCode == "" && item.DistributionDescription == "")
                        {
                            var result = new BadRequestObjectResult("Error, Missing fields distribution in documents, pls fix it");
                            result.StatusCode = StatusCodes.Status409Conflict;
                            return result;
                        }
                        else
                        {
                            var result = new BadRequestObjectResult("Error, please consult your admin mannager");
                            result.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return result;
                        }
                    }
                    else //distribution have id
                    {
                        //get distribution whit id in area
                        Distribution? distributionEntityInDataBase = await _supervisorMobilityRepository.GetDistributionForAreaAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId);
                        //is not null distribution
                        if (distributionEntityInDataBase != null)
                        {

                            //verified that it is not an diferent distribution whit same id
                            if (distributionEntityInDataBase.Code != item.DistributionCode && distributionEntityInDataBase.Description != item.DistributionDescription)
                            {
                                //its a diferent distribution, try to search between code and description in plant
                                if (!await _supervisorMobilityRepository.DistributionExistsByCodeandDescriptionInAreaAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription))
                                {
                                    //distribution not exist, add 1 to create in result 
                                    ResumeActionsResultsToReturn.DistributionCreated++;
                                    var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = item.DistributionIsActive ?? true });
                                    var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                                    await _supervisorMobilityRepository.AddDistributionForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, finalDistribution);
                                    await _supervisorMobilityRepository.SaveChangesAsync();

                                    finalAssyChart.DistributionId = finalDistribution.DistributionId;
                                }
                                else
                                {
                                    //the distribution exists, and they deleted the id in the document
                                    var GetInfoForDistributiom = await _supervisorMobilityRepository.GetDistributionForAreaByCodeAndDescriptionAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription);
                                    finalAssyChart.DistributionId = GetInfoForDistributiom.DistributionId;
                                }
                            }
                        }
                        else //have id but entity is null
                        {
                            //try to verify existence between code and description if id not match
                            if (!await _supervisorMobilityRepository.DistributionExistsByCodeandDescriptionInAreaAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription))
                            {
                                //distribution not exist, add 1 to create in result 
                                ResumeActionsResultsToReturn.DistributionCreated++;
                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = item.DistributionIsActive ?? true });
                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                                await _supervisorMobilityRepository.AddDistributionForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, finalDistribution);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                finalAssyChart.DistributionId = finalDistribution.DistributionId;
                            }
                            else
                            {
                                //the distribution exists, and they deleted the id in the document
                                var GetInfoForDistributiom = await _supervisorMobilityRepository.GetDistributionForAreaByCodeAndDescriptionAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription);
                                finalAssyChart.DistributionId = GetInfoForDistributiom.DistributionId;
                            }
                        }
                    }//end if distribution 

                    //operacion
                    if (finalAssyChart.OperationId == -1)
                    {
                        //check if operation exist in case of user not input id, but exist code and description, 
                        //verify doc have information in fields
                        if (item.OperationCode != "" && item.OperationDescription != "")
                        {
                            //have description and code, area maybe exist, try to get by this info
                            if (!await _supervisorMobilityRepository.OperationExistsByCodeAndDescriptionInDistributionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription))
                            {
                                ResumeActionsResultsToReturn.OperationCreated++;
                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = item.OperationCode, Description = item.OperationDescription, IsActive = item.OperationIsActive ?? true });
                                var finalOperation = _mapper.Map<Entities.Operation>(operationForCreate);
                                await _supervisorMobilityRepository.AddOperationForDistributionAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId, finalOperation);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                finalAssyChart.OperationId = finalOperation.OperationId;
                            }
                            else
                            {
                                //the operation exists, and they deleted the id in the document
                                var GetInfoForOperation = await _supervisorMobilityRepository.GetOperationForDistributionByCodeAndDescriptionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription);
                                finalAssyChart.OperationId = GetInfoForOperation.OperationId;
                            }
                        }
                        else if (item.OperationCode == "" && item.OperationDescription == "")
                        {
                            var result = new BadRequestObjectResult("Error, Missing fields operation in documents, pls fix it");
                            result.StatusCode = StatusCodes.Status409Conflict;
                            return result;
                        }
                        else
                        {
                            var result = new BadRequestObjectResult("Error, please consult your admin mannager");
                            result.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return result;
                        }
                    }
                    else //operation have id
                    {
                        //get operation whit id in distribution
                        Entities.Operation? operationEntityInDataBase = await _supervisorMobilityRepository.GetOperationForDistributionAsync(finalAssyChart.DistributionId, finalAssyChart.OperationId);
                        //is not null distribution
                        if (operationEntityInDataBase != null)
                        {
                            //verified that it is not an diferent operationwhit same id
                            if (operationEntityInDataBase.Code != item.OperationCode && operationEntityInDataBase.Description != item.OperationDescription)
                            {
                                //its a diferent operationwhit, try to search between code and description
                                if (!await _supervisorMobilityRepository.OperationExistsByCodeAndDescriptionInDistributionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription))
                                {
                                    //operation not exist, add 1 to create in result 
                                    ResumeActionsResultsToReturn.OperationCreated++;
                                    var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = item.OperationCode, Description = item.OperationDescription, IsActive = item.OperationIsActive ?? true });
                                    var finalOperation = _mapper.Map<Entities.Operation>(operationForCreate);
                                    await _supervisorMobilityRepository.AddOperationForDistributionAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId, finalOperation);
                                    await _supervisorMobilityRepository.SaveChangesAsync();

                                    finalAssyChart.OperationId = finalOperation.OperationId;
                                }
                                else
                                {
                                    //the distribution exists, and they deleted the id in the document
                                    var GetInfoForOperation = await _supervisorMobilityRepository.GetOperationForDistributionByCodeAndDescriptionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription);
                                    finalAssyChart.OperationId = GetInfoForOperation.OperationId;
                                }
                            }
                        }
                        else //have id but entity is null
                        {
                            //try to verify existence between code and description if id not match
                            if (!await _supervisorMobilityRepository.OperationExistsByCodeAndDescriptionInDistributionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription))
                            {
                                //operation not exist, add 1 to create in result 
                                ResumeActionsResultsToReturn.OperationCreated++;
                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = item.OperationCode, Description = item.OperationDescription, IsActive = item.OperationIsActive ?? true });
                                var finalOperation = _mapper.Map<Entities.Operation>(operationForCreate);
                                await _supervisorMobilityRepository.AddOperationForDistributionAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId, finalOperation);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                finalAssyChart.OperationId = finalOperation.OperationId;
                            }
                            else
                            {
                                //the operation exists, and they deleted the id in the document
                                var GetInfoForOperation = await _supervisorMobilityRepository.GetOperationForDistributionByCodeAndDescriptionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription);
                                finalAssyChart.OperationId = GetInfoForOperation.OperationId;
                            }
                        }
                    }//end if operation 

                    ////PRODUCTO
                    if (finalAssyChart.ProductId == -1)
                    {
                        //check if product exist in case of user not input id, but exist code and description, 
                        //verify doc have information in fields
                        if (item.ProductCode != "" && item.ProductDescription != "")
                        {
                            //have description and code, product maybe exist, try to get by this info
                            if (!await _supervisorMobilityRepository.ProductExistByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription))
                            {
                                ResumeActionsResultsToReturn.ProductCreated++;
                                var productForCreate = _mapper.Map<ProductForCreationDto>(new ProductForCreationDto() { Code = item.ProductCode, Description = item.ProductDescription, IsActive = item.ProductIsActive ?? true });
                                var finalProduct = _mapper.Map<Product>(productForCreate);
                                _supervisorMobilityRepository.AddProduct(finalProduct);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                finalAssyChart.ProductId = finalProduct.ProductId;
                            }
                            else
                            {
                                //the operation exists, and they deleted the id in the document
                                var GetInfoForProduct = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
                                finalAssyChart.ProductId = GetInfoForProduct.ProductId;
                            }
                        }
                        else if (item.ProductCode == "" && item.ProductDescription == "")
                        {
                            var result = new BadRequestObjectResult("Error, Missing fields product in documents, pls fix it");
                            result.StatusCode = StatusCodes.Status409Conflict;
                            return result;
                        }
                        else
                        {
                            var result = new BadRequestObjectResult("Error, please consult your admin mannager");
                            result.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return result;
                        }
                    }
                    else //product have id
                    {
                        //get product    whit id in distribution
                        Product? productEntityInDataBase = await _supervisorMobilityRepository.GetProductAsync((int)item.ProductId);
                        //is not null product
                        if (productEntityInDataBase != null)
                        {
                            //verified that it is not an diferent product whit same id
                            if (productEntityInDataBase.Code != item.ProductCode && productEntityInDataBase.Description != item.ProductDescription)
                            {
                                //its a diferent product, try to search between code and description in distribution
                                if (!await _supervisorMobilityRepository.ProductExistByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription))
                                {
                                    ResumeActionsResultsToReturn.ProductCreated++;
                                    var productForCreate = _mapper.Map<ProductForCreationDto>(new ProductForCreationDto() { Code = item.ProductCode, Description = item.ProductDescription, IsActive = item.ProductIsActive ?? true });
                                    var finalProduct = _mapper.Map<Product>(productForCreate);
                                    _supervisorMobilityRepository.AddProduct(finalProduct);
                                    await _supervisorMobilityRepository.SaveChangesAsync();
                                    finalAssyChart.ProductId = finalProduct.ProductId;
                                }
                                else
                                {
                                    //the operation exists, and they deleted the id in the document
                                    var GetInfoForProduct = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
                                    finalAssyChart.ProductId = GetInfoForProduct.ProductId;
                                }
                            }
                        }
                        else //have id but entity is null
                        {
                            //try to verify existence between code and description if id not match
                            if (!await _supervisorMobilityRepository.ProductExistByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription))
                            {
                                ResumeActionsResultsToReturn.ProductCreated++;
                                var productForCreate = _mapper.Map<ProductForCreationDto>(new ProductForCreationDto() { Code = item.ProductCode, Description = item.ProductDescription, IsActive = item.ProductIsActive ?? true });
                                var finalProduct = _mapper.Map<Product>(productForCreate);
                                _supervisorMobilityRepository.AddProduct(finalProduct);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                finalAssyChart.ProductId = finalProduct.ProductId;
                            }
                            else
                            {
                                //the operation exists, and they deleted the id in the document
                                var GetInfoForProduct = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
                                finalAssyChart.ProductId = GetInfoForProduct.ProductId;
                            }
                        }
                    }//end if distribution 



                    //Assychart
                    if (finalAssyChart.AssyChardId == -1)
                    {
                        //try to search if assy chart exist whit parametes

                        if (!await _supervisorMobilityRepository.AssyChartExistAdvanceAsync(finalAssyChart.GOS, finalAssyChart.CCP, finalAssyChart.HOE, finalAssyChart.PlantId, finalAssyChart.AreaId, finalAssyChart.DistributionId, finalAssyChart.OperationId, finalAssyChart.ProductId))
                        {
                            Debug.WriteLine($"New assy id {finalAssyChart.AssyChardId} plantid {finalAssyChart.PlantId} areaid {finalAssyChart.AreaId} distributionid {finalAssyChart.DistributionId} operation {finalAssyChart.OperationId}  product {finalAssyChart.ProductId}");
                            finalAssyChart.CreationDate = DateTime.Now;
                            finalAssyChart.ModificationDate = DateTime.Now;

                            AssyChartForCreation assychartForCreate = new AssyChartForCreation()
                            {
                                GOS = finalAssyChart.GOS,
                                CCP = finalAssyChart.CCP,
                                HOE = finalAssyChart.HOE,
                                ProductId = finalAssyChart.ProductId,
                                PlantId = finalAssyChart.PlantId,
                                AreaId = finalAssyChart.AreaId,
                                DistributionId = finalAssyChart.DistributionId,
                                OperationId = finalAssyChart.OperationId,
                                CreationDate = finalAssyChart.CreationDate,
                                ModificationDate = finalAssyChart.CreationDate
                            };

                            var itemUser = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                            if (itemUser != null)
                            {
                                Debug.WriteLine($"update assy id {itemUser.AssyChardId} plantid {finalAssyChart.PlantId} areaid {finalAssyChart.AreaId} distributionid {finalAssyChart.DistributionId} operation {finalAssyChart.OperationId} product {finalAssyChart.ProductId}");

                                ResumeActionsResultsToReturn.AssyChartCreated++;
                            }
                        }
                        else
                        {
                            ResumeActionsResultsToReturn.AssyChartUpdated++;
                        }
                    }
                    else
                    {

                        //update assy chart

                        var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync(item.AssyChardId ?? -1);

                        if (assyChartEntity != null)
                        {
                            Debug.WriteLine($"dif null si id assy id {finalAssyChart.AssyChardId} plantid {finalAssyChart.PlantId} areaid {finalAssyChart.AreaId} distributionid {finalAssyChart.DistributionId} operation {finalAssyChart.OperationId} product {finalAssyChart.ProductId}");

                            ResumeActionsResultsToReturn.AssyChartUpdated++;
                            finalAssyChart.ModificationDate = DateTime.Now;

                            _mapper.Map(finalAssyChart, assyChartEntity);

                            await _supervisorMobilityRepository.SaveChangesAsync();
                        }
                        else
                        {

                            if (!await _supervisorMobilityRepository.AssyChartExistAdvanceAsync(finalAssyChart.GOS, finalAssyChart.CCP, finalAssyChart.HOE, finalAssyChart.PlantId, finalAssyChart.AreaId, finalAssyChart.DistributionId, finalAssyChart.OperationId, finalAssyChart.ProductId))
                            {
                                finalAssyChart.CreationDate = DateTime.Now;
                                finalAssyChart.ModificationDate = DateTime.Now;

                                AssyChartForCreation assychartForCreate = new AssyChartForCreation()
                                {
                                    GOS = finalAssyChart.GOS,
                                    CCP = finalAssyChart.CCP,
                                    HOE = finalAssyChart.HOE,
                                    ProductId = finalAssyChart.ProductId,
                                    PlantId = finalAssyChart.PlantId,
                                    AreaId = finalAssyChart.AreaId,
                                    DistributionId = finalAssyChart.DistributionId,
                                    OperationId = finalAssyChart.OperationId,
                                    CreationDate = finalAssyChart.CreationDate,
                                    ModificationDate = finalAssyChart.CreationDate
                                };

                                var itemUser = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                                if (itemUser != null)
                                {
                                    Debug.WriteLine($"update assy id {itemUser.AssyChardId} plantid {finalAssyChart.PlantId} areaid {finalAssyChart.AreaId} distributionid {finalAssyChart.DistributionId} operation {finalAssyChart.OperationId} product {finalAssyChart.ProductId}");

                                    ResumeActionsResultsToReturn.AssyChartCreated++;
                                }
                            }
                            else
                            {
                                ResumeActionsResultsToReturn.AssyChartUpdated++;
                            }

                        }

                    }


                }//end foreach


            }//end trycatch
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return Ok(ResumeActionsResultsToReturn);

        }

        [EnableCors("Cors")]
        [HttpGet("Bulk/ByPlantId/{plantId}")]
        public async Task<IActionResult> DownloadFile(int plantId)
        {
            List<AssyChartWhitInfo> assyChartsForPlant = _mapper.Map<List<AssyChartWhitInfo>>(await _supervisorMobilityRepository.GetAssyChartByPlantAsync(plantId));

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
                    ws.SetCellValue($"C{row}", itemUser.GOS ?? "");
                    ws.SetCellValue($"D{row}", itemUser.CCP ?? "");
                    ws.SetCellValue($"E{row}", itemUser.HOE);
                    ws.SetCellValue($"F{row}", itemUser.CreationDate.ToString() ?? "");
                    ws.SetCellValue($"G{row}", itemUser.ModificationDate.ToString() ?? "");
                    ws.SetCellValue($"H{row}", itemUser.Product?.ProductId.ToString() ?? "");
                    ws.SetCellValue($"I{row}", itemUser.Product?.Code ?? "");
                    ws.SetCellValue($"J{row}", itemUser.Product?.Description ?? "");
                    ws.SetCellValue($"K{row}", itemUser.Product?.IsActive?.ToString() ?? "");
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

        [EnableCors("Cors")]
        [HttpGet("Bulk/ByPlantId")]
        public async Task<IActionResult> DownloadFileAllPlants()
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

                var assyChartsEntitys = await _supervisorMobilityRepository.GetAssyChartByPlantAsync(plant.PlantId);
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
                        ws.SetCellValue($"C{row}", itemUser.GOS ?? "");
                        ws.SetCellValue($"D{row}", itemUser.CCP ?? "");
                        ws.SetCellValue($"E{row}", itemUser.HOE);
                        ws.SetCellValue($"F{row}", itemUser.CreationDate.ToString() ?? "");
                        ws.SetCellValue($"G{row}", itemUser.ModificationDate.ToString() ?? "");
                        ws.SetCellValue($"H{row}", itemUser.Product?.ProductId.ToString() ?? "");
                        ws.SetCellValue($"I{row}", itemUser.Product?.Code ?? "");
                        ws.SetCellValue($"J{row}", itemUser.Product?.Description ?? "");
                        ws.SetCellValue($"K{row}", itemUser.Product?.IsActive?.ToString() ?? "");
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


        [EnableCors("Cors")]
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
                return File(memory, FileInfo.ContentType, Path.GetFileName(path));


            }
            return NotFound("Error File download");

        }

        [EnableCors("Cors")]
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
                return File(memory, FileInfo.ContentType, Path.GetFileName(path));


            }
            return NotFound("Error File download");

        }


        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadUsers")]
        public async Task<IActionResult> DownloadAllUsers()
        {
            List<UsersWithNavigationDetails> allUsersList = _mapper.Map<List<UsersWithNavigationDetails>>(await _supervisorMobilityRepository.GetAllUsersWhitPlantAreaAndGroupAsync());


            if (allUsersList.Count == 0)
            {
                return BadRequest("No Users");
            }

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();


            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "UserId");
            ws.SetCellValue("B1", "Payroll");
            ws.SetCellValue("C1", "Name");
            ws.SetCellValue("D1", "Plant");
            ws.SetCellValue("E1", "Area");
            ws.SetCellValue("F1", "Group");
            ws.SetCellValue("G1", "Admin permission");
            ws.SetCellValue("H1", "Supervisor permission");
            ws.SetCellValue("I1", "Operator permission");
            ws.SetCellValue("J1", "Date Create User");
            ws.SetCellValue("K1", "Date Update User");
            ws.SetCellValue("L1", "Date Disable User");
            ws.SetCellValue("M1", "User Is Active");



            int row = 2;
            foreach (var itemUser in allUsersList)
            {
                ws.SetCellValue($"A{row}", itemUser.UserId.ToString() ?? "");
                ws.SetCellValue($"B{row}", itemUser.Payroll.ToString() ?? "");
                ws.SetCellValue($"C{row}", itemUser.Name.ToString() ?? ""); ;
                ws.SetCellValue($"D{row}", itemUser.PlantId.ToString() ?? ""); ;
                ws.SetCellValue($"E{row}", itemUser.AreaId.ToString() ?? "");
                ws.SetCellValue($"F{row}", itemUser.GroupId.ToString() ?? "");
                ws.SetCellValue($"G{row}", itemUser.IsAdmin.ToString() ?? "");
                ws.SetCellValue($"H{row}", itemUser.IsSupervisor.ToString() ?? "");
                ws.SetCellValue($"I{row}", itemUser.IsOperator.ToString() ?? "");
                ws.SetCellValue($"J{row}", itemUser.CreatedDate.ToString() ?? "");
                ws.SetCellValue($"K{row}", itemUser.LastUpdated.ToString() ?? "");
                ws.SetCellValue($"L{row}", itemUser.DisabledDate.ToString() ?? "");
                ws.SetCellValue($"M{row}", itemUser.IsActive.ToString() ?? "");
                row++;
            }



            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersBulk.xlsx");
            res.EnableRangeProcessing = true;
            return res;



        }//end download file function 

    }
}
