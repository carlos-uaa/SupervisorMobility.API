using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using SpreadsheetLight;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUpload;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.ProductDtos;
using Azure;
using SupervisorMobility.API.DataAccess.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SupervisorMobility.API.Controllers
{

    public class DataAreaSaved
    {
        public AreaWithoutNavigationPropertiesDto areaInfo { get; set; }
        public List<DataDistributionsSaved> DistributionsInArea { get; set; } = new List<DataDistributionsSaved>();
    }

    public class DataDistributionsSaved
    {
        public DistributionWithoutNavigationPropertiesDto distributionInfo { get; set; }
        public List<OperationWithoutNavigationPropertiesDto> OperationsInDistribution { get; set; } = new List<OperationWithoutNavigationPropertiesDto>();
    }

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
        public async Task<ActionResult<UploadResult>> UploadFile(IFormFile file)
        {
            UploadResult uploadResult = new UploadResult();
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
            var path = Path.Combine(_env.ContentRootPath, "uploads", trustedFileNameForFileStorage);

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.StorageFileName = trustedFileNameForFileStorage;

            return Ok(uploadResult);
        }

        [HttpPost("Data")]
        public async Task<ActionResult<UploadDataResult>> UpdateDataInServer(UploadResult FileInfo)
        {


            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\" + FileInfo.StorageFileName;
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

                                ToInsertIntoList.CreationDate = DateTime.Parse(fields[5]);
                                ToInsertIntoList.ModificationDate = DateTime.Parse(fields[6]);

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
                    //load a file whit sldocument
                    SLDocument sl = new SLDocument(file);

                    //start in row 2


                    SLWorksheetStatistics stats = sl.GetWorksheetStatistics();

                    for (int i = 1; i < stats.EndRowIndex - 1; i++)
                    {

                        if (i == 1)
                        {
                            //assign data to plant
                            PlantInfo = new Plant
                            (
                                 sl.GetCellValueAsString(i, 5), sl.GetCellValueAsString(i, 8)
                            );

                            PlantInfo.PlantId = sl.HasCellValue(i, 1) ? sl.GetCellValueAsInt32(i, 2) : -1;
                            i++;
                        }
                        else
                        {
                            var ToInsertIntoList = new AssyChartDataToBulk();

                            ToInsertIntoList.PlantId = PlantInfo.PlantId;
                            ToInsertIntoList.Plant = PlantInfo;

                            ToInsertIntoList.AssyChardId = sl.HasCellValue(i, 1) ? sl.GetCellValueAsInt32(i, 1) : -1;
                            ToInsertIntoList.IsActive = sl.HasCellValue(i, 2) ? sl.GetCellValueAsBoolean(i, 2) : true;

                            ToInsertIntoList.GOS = sl.HasCellValue(i, 3) ? sl.GetCellValueAsString(i, 3) : "";
                            ToInsertIntoList.CCP = sl.HasCellValue(i, 4) ? sl.GetCellValueAsString(i, 4) : "";
                            ToInsertIntoList.HOE = sl.HasCellValue(i, 5) ? sl.GetCellValueAsString(i, 5) : "";

                            ToInsertIntoList.CreationDate = DateTime.Parse(sl.GetCellValueAsString(i, 6));
                            ToInsertIntoList.ModificationDate = DateTime.Parse(sl.GetCellValueAsString(i, 7));


                            ToInsertIntoList.ProductId = sl.HasCellValue(i, 8) ? sl.GetCellValueAsInt32(i, 8) : -1;
                            ToInsertIntoList.ProductCode = sl.HasCellValue(i, 9) ? sl.GetCellValueAsString(i, 9) : "";
                            ToInsertIntoList.ProductDescription = sl.HasCellValue(i, 10) ? sl.GetCellValueAsString(i, 10) : "";
                            ToInsertIntoList.ProductIsActive = sl.HasCellValue(i, 11) ? sl.GetCellValueAsBoolean(i, 11) : true;


                            ToInsertIntoList.AreaId = sl.HasCellValue(i, 12) ? sl.GetCellValueAsInt32(i, 12) : -1;
                            ToInsertIntoList.AreaCode = sl.HasCellValue(i, 13) ? sl.GetCellValueAsString(i, 13) : "";
                            ToInsertIntoList.AreaDescription = sl.HasCellValue(i, 14) ? sl.GetCellValueAsString(i, 14) : "";
                            ToInsertIntoList.AreaIsActive = sl.HasCellValue(i, 15) ? sl.GetCellValueAsBoolean(i, 15) : true;

                            ToInsertIntoList.OperationId = sl.HasCellValue(i, 16) ? sl.GetCellValueAsInt32(i, 16) : -1;
                            ToInsertIntoList.OperationCode = sl.HasCellValue(i, 17) ? sl.GetCellValueAsString(i, 17) : "";
                            ToInsertIntoList.OperationDescription = sl.HasCellValue(i, 18) ? sl.GetCellValueAsString(i, 18) : "";
                            ToInsertIntoList.OperationIsActive = sl.HasCellValue(i, 19) ? sl.GetCellValueAsBoolean(i, 19) : true;

                            ToInsertIntoList.DistributionId = sl.HasCellValue(i, 20) ? sl.GetCellValueAsInt32(i, 20) : -1;
                            ToInsertIntoList.DistributionCode = sl.HasCellValue(i, 21) ? sl.GetCellValueAsString(i, 21) : "";
                            ToInsertIntoList.DistributionDescription = sl.HasCellValue(i, 22) ? sl.GetCellValueAsString(i, 22) : "";
                            ToInsertIntoList.DistributionIsActive = sl.HasCellValue(i, 23) ? sl.GetCellValueAsBoolean(i, 23) : true;

                            DataList.Add(ToInsertIntoList);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }//end trycatch to add excel to list
            }//end read data from excel file

            //created result array to return
            UploadDataResult ResumeActionsResultsToReturn = new UploadDataResult();


            //Lists of info to evit using await's 
            List<ProductDto> SaveProducts = new List<ProductDto>();
            List<DataAreaSaved> SaveAreas = new List<DataAreaSaved>();


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
                    var result = new BadRequestObjectResult("Error, Missing fields in documents, pls fix it");
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
                            //verified that it is not an diferent distribution whit same id
                            if (operationEntityInDataBase.Code != item.OperationCode && operationEntityInDataBase.Description != item.OperationDescription)
                            {
                                //its a diferent distribution, try to search between code and description in distribution
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
                                var GetInfoForProduct = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync( item.ProductCode, item.ProductDescription);
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
                        Product? productEntityInDataBase = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
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

                        if(! await _supervisorMobilityRepository.AssyChartExistAdvanceAsync(finalAssyChart.GOS, finalAssyChart.CCP, finalAssyChart.HOE, finalAssyChart.PlantId, finalAssyChart.AreaId, finalAssyChart.DistributionId, finalAssyChart.OperationId, finalAssyChart.ProductId))
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

                            var element = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                            if (element != null)
                            {
                                ResumeActionsResultsToReturn.AssyChartCreated++;
                            }
                        }
                        else
                        {
                            //get assy chart by advance
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"update assy id {finalAssyChart.AssyChardId} plantid {finalAssyChart.PlantId} areaid {finalAssyChart.AreaId} distributionid {finalAssyChart.DistributionId} operation {finalAssyChart.OperationId} product {finalAssyChart.ProductId}");

                        //update assy chart

                        var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync(item.AssyChardId ?? -1);

                        if (assyChartEntity != null)
                        {
                            ResumeActionsResultsToReturn.AssyChartUpdated++;
                            finalAssyChart.ModificationDate = DateTime.Now;

                            _mapper.Map(finalAssyChart, assyChartEntity);

                            await _supervisorMobilityRepository.SaveChangesAsync();
                        }
                        else
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



                            var element = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                            if (element != null)
                            {
                                ResumeActionsResultsToReturn.AssyChartCreated++;


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
            using (SLDocument sl = new SLDocument())
            {
                //Plant ROW data
                sl.SetCellValue("A1", "PlantId");
                sl.SetCellValue("B1", assyChartsForPlant[0].PlantId);

                sl.SetCellValue("D1", "PlantCode");
                sl.SetCellValue("E1", assyChartsForPlant[0].Plant.Code);

                sl.SetCellValue("G1", "PlantId");
                sl.SetCellValue("H1", assyChartsForPlant[0].Plant.Description);

                //ROW Data identificators

                sl.SetCellValue("A2", "AssyChartId");
                sl.SetCellValue("B2", "isActive");
                sl.SetCellValue("C2", "GOS");
                sl.SetCellValue("D2", "CCP");
                sl.SetCellValue("E2", "HOE");
                sl.SetCellValue("F2", "CreationDate");
                sl.SetCellValue("G2", "ModificationDate");
                sl.SetCellValue("H2", "ProductId");
                sl.SetCellValue("I2", "ProductCode");
                sl.SetCellValue("J2", "ProductDescription");
                sl.SetCellValue("K2", "ProductIsActive");
                sl.SetCellValue("L2", "AreaId");
                sl.SetCellValue("M2", "AreaCode");
                sl.SetCellValue("N2", "AreaDescription");
                sl.SetCellValue("O2", "AreaIsActive");
                sl.SetCellValue("P2", "OperationId");
                sl.SetCellValue("Q2", "OperationCode");
                sl.SetCellValue("R2", "OperationDescription");
                sl.SetCellValue("S2", "OperationIsActive");
                sl.SetCellValue("T2", "DistributionId");
                sl.SetCellValue("U2", "DistributionCode");
                sl.SetCellValue("V2", "DistributionDescription");
                sl.SetCellValue("W2", "DistributionIsActive");

                int row = 3;
                foreach (var element in assyChartsForPlant)
                {

                    sl.SetCellValue($"A{row}", element.AssyChardId.ToString() ?? "");
                    sl.SetCellValue($"B{row}", element.IsActive.ToString() ?? "");
                    sl.SetCellValue($"C{row}", element.GOS ?? "");
                    sl.SetCellValue($"D{row}", element.CCP ?? "");
                    sl.SetCellValue($"E{row}", element.HOE);
                    sl.SetCellValue($"F{row}", element.CreationDate.ToString() ?? "");
                    sl.SetCellValue($"G{row}", element.ModificationDate.ToString() ?? "");
                    sl.SetCellValue($"H{row}", element.Product?.ProductId.ToString() ?? "");
                    sl.SetCellValue($"I{row}", element.Product?.Code ?? "");
                    sl.SetCellValue($"J{row}", element.Product?.Description ?? "");
                    sl.SetCellValue($"K{row}", element.Product?.IsActive?.ToString() ?? "");
                    sl.SetCellValue($"L{row}", element.Area?.AreaId.ToString() ?? "");
                    sl.SetCellValue($"M{row}", element.Area?.Code ?? "");
                    sl.SetCellValue($"N{row}", element.Area?.Description ?? "");
                    sl.SetCellValue($"O{row}", element.Area?.IsActive?.ToString() ?? "");
                    sl.SetCellValue($"P{row}", element.Operation?.OperationId.ToString() ?? "");
                    sl.SetCellValue($"Q{row}", element.Operation?.Code ?? "");
                    sl.SetCellValue($"R{row}", element.Operation?.Description ?? "");
                    sl.SetCellValue($"S{row}", element.Operation?.IsActive.ToString() ?? "");
                    sl.SetCellValue($"T{row}", element.Distribution?.DistributionId.ToString() ?? "");
                    sl.SetCellValue($"U{row}", element.Distribution?.Code ?? "");
                    sl.SetCellValue($"V{row}", element.Distribution?.Description ?? "");
                    sl.SetCellValue($"W{row}", element.Distribution?.IsActive?.ToString() ?? "");
                    row++;
                }

                sl.SaveAs(ms);
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
            SLDocument sl = new SLDocument();
            bool firstSheet = true;

            foreach (var plant in allPlants)
            {
                if (firstSheet)
                {
                    sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, plant.Description ?? "Primer Planta");
                    firstSheet = false;
                }
                else
                {
                    sl.AddWorksheet(plant.Description ?? "Planta Siguiente");
                }

                //Plant ROW data
                sl.SetCellValue("A1", "PlantId");
                sl.SetCellValue("B1", plant.PlantId.ToString() ?? "");

                sl.SetCellValue("D1", "PlantCode");
                sl.SetCellValue("E1", plant.Code ?? "");

                sl.SetCellValue("G1", "PlantId");
                sl.SetCellValue("H1", plant.Description ?? "");

                //ROW Data identificators

                sl.SetCellValue("A2", "AssyChartId");
                sl.SetCellValue("B2", "isActive");
                sl.SetCellValue("C2", "GOS");
                sl.SetCellValue("D2", "CCP");
                sl.SetCellValue("E2", "HOE");
                sl.SetCellValue("F2", "CreationDate");
                sl.SetCellValue("G2", "ModificationDate");
                sl.SetCellValue("H2", "ProductId");
                sl.SetCellValue("I2", "ProductCode");
                sl.SetCellValue("J2", "ProductDescription");
                sl.SetCellValue("K2", "ProductIsActive");
                sl.SetCellValue("L2", "AreaId");
                sl.SetCellValue("M2", "AreaCode");
                sl.SetCellValue("N2", "AreaDescription");
                sl.SetCellValue("O2", "AreaIsActive");
                sl.SetCellValue("P2", "OperationId");
                sl.SetCellValue("Q2", "OperationCode");
                sl.SetCellValue("R2", "OperationDescription");
                sl.SetCellValue("S2", "OperationIsActive");
                sl.SetCellValue("T2", "DistributionId");
                sl.SetCellValue("U2", "DistributionCode");
                sl.SetCellValue("V2", "DistributionDescription");
                sl.SetCellValue("W2", "DistributionIsActive");

                var assyChartsEntitys = await _supervisorMobilityRepository.GetAssyChartByPlantAsync(plant.PlantId);
                List<AssyChartWhitInfo> assyChartsForPlant = new List<AssyChartWhitInfo>();
                if (assyChartsEntitys != null)
                {
                    assyChartsForPlant = _mapper.Map<List<AssyChartWhitInfo>>(assyChartsEntitys);
                }


                if (assyChartsForPlant.Count != 0)
                {

                    int row = 3;
                    foreach (var element in assyChartsForPlant)
                    {
                        sl.SetCellValue($"A{row}", element.AssyChardId.ToString() ?? "");
                        sl.SetCellValue($"B{row}", element.IsActive.ToString() ?? "");
                        sl.SetCellValue($"C{row}", element.GOS ?? "");
                        sl.SetCellValue($"D{row}", element.CCP ?? "");
                        sl.SetCellValue($"E{row}", element.HOE);
                        sl.SetCellValue($"F{row}", element.CreationDate.ToString() ?? "");
                        sl.SetCellValue($"G{row}", element.ModificationDate.ToString() ?? "");
                        sl.SetCellValue($"H{row}", element.Product?.ProductId.ToString() ?? "");
                        sl.SetCellValue($"I{row}", element.Product?.Code ?? "");
                        sl.SetCellValue($"J{row}", element.Product?.Description ?? "");
                        sl.SetCellValue($"K{row}", element.Product?.IsActive?.ToString() ?? "");
                        sl.SetCellValue($"L{row}", element.Area?.AreaId.ToString() ?? "");
                        sl.SetCellValue($"M{row}", element.Area?.Code ?? "");
                        sl.SetCellValue($"N{row}", element.Area?.Description ?? "");
                        sl.SetCellValue($"O{row}", element.Area?.IsActive?.ToString() ?? "");
                        sl.SetCellValue($"P{row}", element.Operation?.OperationId.ToString() ?? "");
                        sl.SetCellValue($"Q{row}", element.Operation?.Code ?? "");
                        sl.SetCellValue($"R{row}", element.Operation?.Description ?? "");
                        sl.SetCellValue($"S{row}", element.Operation?.IsActive.ToString() ?? "");
                        sl.SetCellValue($"T{row}", element.Distribution?.DistributionId.ToString() ?? "");
                        sl.SetCellValue($"U{row}", element.Distribution?.Code ?? "");
                        sl.SetCellValue($"V{row}", element.Distribution?.Description ?? "");
                        sl.SetCellValue($"W{row}", element.Distribution?.IsActive?.ToString() ?? "");
                        row++;
                    }
                }
            }



            string path = Directory.GetCurrentDirectory().ToString() + "\\report.xlsx";

            sl.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportAllPlants.xlsx");
            res.EnableRangeProcessing = true;
            return res;



        }//end download file function 

    }


}
