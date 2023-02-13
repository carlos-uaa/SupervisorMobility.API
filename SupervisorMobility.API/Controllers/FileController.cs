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
            if (PlantInfo.PlantId <= 0)
            {
                //a new plant of a missing field id in doc
                //check if plant exist in case of user not input id, but exist code and description
                if (!await _supervisorMobilityRepository.PlantExistByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description))
                {

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
                    ResumeActionsResultsToReturn.PlantUpdate++;
                    //exist, get data to update void fields in doc.
                    var GetInfoPlantBecauseExist = await _supervisorMobilityRepository.GetPlantByCodeAndDescriptionAsync(PlantInfo.Code, PlantInfo.Description);
                    PlantInfo = GetInfoPlantBecauseExist;
                }
            }
            else
            {
                //get plant to check any change in code and description
                Plant? plantEntityInDataBase = await _supervisorMobilityRepository.GetPlantAsync(PlantInfo.PlantId, false);

                if (plantEntityInDataBase != null)
                {
                    if (plantEntityInDataBase.Code != PlantInfo.Code || plantEntityInDataBase.Description != PlantInfo.Description)
                    {
                        //update plant 
                        ResumeActionsResultsToReturn.PlantUpdate++;
                        var PlantDataInDoc = _mapper.Map<PlantForUpdateDto>(PlantInfo);
                        await _assyChartService.UpdatePlantAsync(PlantDataInDoc, plantEntityInDataBase);
                    }
                }
                else
                {
                    ResumeActionsResultsToReturn.PlantCreate++;
                    //creating plant
                    var plantForCreate = _mapper.Map<PlantForCreationDto>(PlantInfo);

                    var finalPlant = await _assyChartService.CreatePlantAsync(plantForCreate);
                    //save create plant
                    await _supervisorMobilityRepository.SaveChangesAsync();
                    //get id to use in updates and creates
                    PlantInfo.PlantId = finalPlant.PlantId;
                }


            }

            //update or create info in plant
            try
            {

                //foreach to index list
                foreach (AssyChartDataToBulk item in DataList)
                {
                    var indexarea = -1;
                    var indexdistribution = -1;
                    var indexoperacion = -1;
                    AssyChartWithoutNavigationProperties finalAssyChart = new AssyChartWithoutNavigationProperties()
                    {
                        AssyChardId = (int)item.AssyChardId,
                        GOS = item.GOS,
                        CCP = item.CCP,
                        HOE = item.HOE,
                        ProductId = 0,
                        PlantId = PlantInfo.PlantId,
                        AreaId = 0,
                        DistributionId = 0,
                        OperationId = 0,
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                    };

                    if (item.AreaId == -1)
                    {
                        //this is a new area
                        //chek if now exist or created to use and save
                        indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);
                        if (indexarea >= 0)
                        {
                            //area exist in save areas
                            finalAssyChart.AreaId = SaveAreas[indexarea].areaInfo.AreaId;
                        }
                        else
                        {
                            //if not exist in data list of area saved, search in database whit code and description in plant
                            if (!await _supervisorMobilityRepository.AreaExistByCodeAndDescriptionInPlantAsync(item.AreaCode, item.AreaDescription, PlantInfo.PlantId))
                            {
                                //not exist, create 
                                ResumeActionsResultsToReturn.AreasCreated++;

                                var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = item.AreaCode, Description = item.AreaDescription, IsActive = (bool)item.AreaIsActive });

                                var finalArea = _mapper.Map<Area>(areaForCreate);

                                await _supervisorMobilityRepository.AddAreaForPlantAsync(
                                    PlantInfo.PlantId, finalArea);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdAreaToSave = _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);

                                SaveAreas.Add(new DataAreaSaved() { areaInfo = createdAreaToSave });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                                finalAssyChart.AreaId = createdAreaToSave.AreaId;

                            }
                            else
                            {
                                var AreaBecauseExist = await _supervisorMobilityRepository.GetAreaForPlantByCodeAndDescriptionAsync(finalAssyChart.PlantId, item.AreaCode, item.AreaDescription);
                                finalAssyChart.AreaId = AreaBecauseExist.AreaId;
                                SaveAreas.Add(new DataAreaSaved() { areaInfo = _mapper.Map<AreaWithoutNavigationPropertiesDto>(AreaBecauseExist) });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                                //exist area in plant, get info and save in list
                            }
                        }
                    }
                    else
                    {
                        //area exist, search area in plant
                        Area? areaEntityInDataBase = await _supervisorMobilityRepository.GetAreaForPlantAsync(finalAssyChart.PlantId, (int)item.AreaId);

                        if (areaEntityInDataBase != null)
                        {
                            //exist check if update any field
                            if (areaEntityInDataBase.Code != item.AreaCode || areaEntityInDataBase.Description != item.AreaDescription)
                            {
                                //update area 
                                ResumeActionsResultsToReturn.AreasUpdated++;
                                var AreaDataInDoc = _mapper.Map<AreaForUpdateDto>(new AreaForUpdateDto { Code = item.AreaCode, Description = item.AreaDescription, IsActive = (bool)item.AreaIsActive });

                                _mapper.Map(AreaDataInDoc, areaEntityInDataBase);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var areatosave = _mapper.Map<AreaWithoutNavigationPropertiesDto>(areaEntityInDataBase);
                                areatosave.Code = item.AreaCode;
                                areatosave.Description = item.AreaDescription;
                                areatosave.AreaId = (int)item.AreaId;

                                SaveAreas.Add(new DataAreaSaved() { areaInfo = areatosave });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                            }
                            else
                            {
                                SaveAreas.Add(new DataAreaSaved() { areaInfo = _mapper.Map<AreaWithoutNavigationPropertiesDto>(areaEntityInDataBase) });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                            }
                            finalAssyChart.AreaId = areaEntityInDataBase.AreaId;

                        }
                        else
                        {
                            //area has id but does not exist in plant, search in plant or create new
                            if (!await _supervisorMobilityRepository.AreaExistByCodeAndDescriptionInPlantAsync(item.AreaCode, item.AreaDescription, PlantInfo.PlantId))
                            {
                                //not exist, create 
                                ResumeActionsResultsToReturn.AreasCreated++;

                                var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = item.AreaCode, Description = item.AreaDescription, IsActive = (bool)item.AreaIsActive });

                                var finalArea = _mapper.Map<Area>(areaForCreate);

                                await _supervisorMobilityRepository.AddAreaForPlantAsync(
                                    PlantInfo.PlantId, finalArea);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdAreaToSave = _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);

                                SaveAreas.Add(new DataAreaSaved() { areaInfo = createdAreaToSave });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                                finalAssyChart.AreaId = createdAreaToSave.AreaId;

                            }
                            else
                            {
                                var AreaBecauseExist = await _supervisorMobilityRepository.GetAreaForPlantByCodeAndDescriptionAsync(finalAssyChart.PlantId, item.AreaCode, item.AreaDescription);
                                finalAssyChart.AreaId = AreaBecauseExist.AreaId;
                                SaveAreas.Add(new DataAreaSaved() { areaInfo = _mapper.Map<AreaWithoutNavigationPropertiesDto>(AreaBecauseExist) });
                                indexarea = SaveAreas.FindIndex(elementInList => elementInList.areaInfo.Code == item.AreaCode && elementInList.areaInfo.Description == item.AreaDescription);

                                //exist area in plant, get info and save in list
                            }


                        }
                    }//end if area 

                    //Distribucion
                    // indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.DistributionId == item.DistributionId);

                    if (item.DistributionId == -1)
                    {
                        //this is a new distribution
                        //chek if now exist or created to use and save

                        indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);

                        if (indexdistribution >= 0)
                        {
                            //area exist in save areas
                            finalAssyChart.DistributionId = SaveAreas[indexarea].DistributionsInArea[indexdistribution].distributionInfo.DistributionId;
                        }
                        else
                        {
                            //if not exist in data list of distributions saved, search in database whit code and description in area
                            if (!await _supervisorMobilityRepository.DistributionExistsByCodeandDescriptionInAreaAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription))
                            {
                                ResumeActionsResultsToReturn.DistributionCreated++;

                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = (bool)item.DistributionIsActive });

                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);

                                await _supervisorMobilityRepository.AddDistributionForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, finalDistribution);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdDistributionToSave = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(finalDistribution);

                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = createdDistributionToSave });

                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);

                                finalAssyChart.DistributionId = createdDistributionToSave.DistributionId;

                            }
                            else
                            {
                                var DistributionBecauseExist = await _supervisorMobilityRepository.GetDistributionForAreaByCodeAndDescriptionAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription);
                                finalAssyChart.DistributionId = DistributionBecauseExist.DistributionId;
                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(DistributionBecauseExist) });
                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);

                                //exist area in plant, get info and save in list
                            }
                        }
                    }
                    else
                    {
                        //distribution exist, search distribution in area
                        Distribution? distributionEntityInDataBase = await _supervisorMobilityRepository.GetDistributionForAreaAsync(finalAssyChart.AreaId, (int)item.DistributionId);

                        if (distributionEntityInDataBase != null)
                        {
                            //exist check if update any field
                            if (distributionEntityInDataBase.Code != item.DistributionCode || distributionEntityInDataBase.Description != item.DistributionDescription)
                            {
                                //update area 
                                ResumeActionsResultsToReturn.DistributionUpdated++;
                                var DistributionDataInDoc = _mapper.Map<DistributionForUpdateDto>(new DistributionForUpdateDto { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = (bool)item.DistributionIsActive });

                                _mapper.Map(DistributionDataInDoc, distributionEntityInDataBase);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var distributiontosave = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(distributionEntityInDataBase);
                                distributiontosave.Code = item.DistributionCode;
                                distributiontosave.Description = item.DistributionCode;
                                distributiontosave.DistributionId = (int)item.DistributionId;

                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = distributiontosave });
                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);


                            }
                            else
                            {
                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(distributionEntityInDataBase) });
                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);
                            }
                            finalAssyChart.DistributionId = distributionEntityInDataBase.DistributionId;

                        }
                        else
                        {
                            if (!await _supervisorMobilityRepository.DistributionExistsByCodeandDescriptionInAreaAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription))
                            {
                                ResumeActionsResultsToReturn.DistributionCreated++;

                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = item.DistributionCode, Description = item.DistributionDescription, IsActive = (bool)item.DistributionIsActive });

                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);

                                await _supervisorMobilityRepository.AddDistributionForPlantAsync(finalAssyChart.PlantId, finalAssyChart.AreaId, finalDistribution);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdDistributionToSave = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(finalDistribution);

                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = createdDistributionToSave });

                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);

                                finalAssyChart.DistributionId = createdDistributionToSave.DistributionId;

                            }
                            else
                            {
                                var DistributionBecauseExist = await _supervisorMobilityRepository.GetDistributionForAreaByCodeAndDescriptionAsync(finalAssyChart.AreaId, item.DistributionCode, item.DistributionDescription);
                                finalAssyChart.DistributionId = DistributionBecauseExist.DistributionId;
                                SaveAreas[indexarea].DistributionsInArea.Add(new DataDistributionsSaved() { distributionInfo = _mapper.Map<DistributionWithoutNavigationPropertiesDto>(DistributionBecauseExist) });
                                indexdistribution = SaveAreas[indexarea].DistributionsInArea.FindIndex(elementInList => elementInList.distributionInfo.Code == item.DistributionCode && elementInList.distributionInfo.Description == item.DistributionDescription);

                                //exist area in plant, get info and save in list
                            }
                        }
                    }//end if distribution


                    //operacion

                    if (item.OperationId == -1)
                    {
                        //this is a new operation
                        //chek if now exist or created to use and save
                        indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);

                        if (indexdistribution >= 0)
                        {
                            //area exist in save areas
                            finalAssyChart.OperationId = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution[indexdistribution].OperationId;
                        }
                        else
                        {
                            //if not exist in data list of operation saved in distribution, search in database whit code and description in distribution
                            if (!await _supervisorMobilityRepository.OperationExistsByCodeAndDescriptionInDistributionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription))
                            {
                                ResumeActionsResultsToReturn.OperationCreated++;

                                //operation not exist, create now
                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = item.OperationCode, Description = item.OperationDescription, IsActive = (bool)item.OperationIsActive });

                                var finalOperation = _mapper.Map<Entities.Operation>(operationForCreate);

                                await _supervisorMobilityRepository.AddOperationForDistributionAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId, finalOperation);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdOperationToSave = _mapper.Map<OperationWithoutNavigationPropertiesDto>(finalOperation);

                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(createdOperationToSave);
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);


                                finalAssyChart.OperationId = createdOperationToSave.OperationId;

                            }
                            else
                            {
                                //operation exist in database
                                var OperationBecauseExist = await _supervisorMobilityRepository.GetOperationForDistributionByCodeAndDescriptionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription);
                                finalAssyChart.OperationId = OperationBecauseExist.OperationId;

                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(_mapper.Map<OperationWithoutNavigationPropertiesDto>(OperationBecauseExist));
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);



                            }
                        }
                    }
                    else
                    {
                        //operation exist, search operation in distribution
                        Entities.Operation? operationEntityInDataBase = await _supervisorMobilityRepository.GetOperationForDistributionAsync(finalAssyChart.DistributionId, (int)item.OperationId);

                        if (operationEntityInDataBase != null)
                        {
                            //exist check if update any field
                            if (operationEntityInDataBase.Code != item.OperationCode || operationEntityInDataBase.Description != item.OperationDescription)
                            {
                                //update area 
                                ResumeActionsResultsToReturn.OperationUpdated++;
                                var OperationDataInDoc = _mapper.Map<OperationForUpdateDto>(new OperationForUpdateDto { Code = item.OperationCode, Description = item.OperationDescription, IsActive = (bool)item.OperationIsActive });

                                _mapper.Map(OperationDataInDoc, operationEntityInDataBase);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var operationtosave = _mapper.Map<OperationWithoutNavigationPropertiesDto>(operationEntityInDataBase);
                                operationtosave.Code = item.OperationCode;
                                operationtosave.Description = item.OperationCode;
                                operationtosave.OperationId = (int)item.OperationId;

                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(operationtosave);
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);

                            }
                            else
                            {
                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(_mapper.Map<OperationWithoutNavigationPropertiesDto>(operationEntityInDataBase));
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);

                            }
                            finalAssyChart.OperationId = operationEntityInDataBase.OperationId;

                        }
                        else
                        {
                            if (!await _supervisorMobilityRepository.OperationExistsByCodeAndDescriptionInDistributionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription))
                            {
                                ResumeActionsResultsToReturn.OperationCreated++;

                                //operation not exist, create now
                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = item.OperationCode, Description = item.OperationDescription, IsActive = (bool)item.OperationIsActive });

                                var finalOperation = _mapper.Map<Entities.Operation>(operationForCreate);

                                await _supervisorMobilityRepository.AddOperationForDistributionAsync(finalAssyChart.AreaId, finalAssyChart.DistributionId, finalOperation);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdOperationToSave = _mapper.Map<OperationWithoutNavigationPropertiesDto>(finalOperation);

                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(createdOperationToSave);
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);


                                finalAssyChart.OperationId = createdOperationToSave.OperationId;

                            }
                            else
                            {
                                //operation exist in database
                                var OperationBecauseExist = await _supervisorMobilityRepository.GetOperationForDistributionByCodeAndDescriptionAsync(finalAssyChart.DistributionId, item.OperationCode, item.OperationDescription);
                                finalAssyChart.OperationId = OperationBecauseExist.OperationId;

                                SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.Add(_mapper.Map<OperationWithoutNavigationPropertiesDto>(OperationBecauseExist));
                                indexoperacion = SaveAreas[indexarea].DistributionsInArea[indexdistribution].OperationsInDistribution.FindIndex(operationinlist => operationinlist.Code == item.OperationCode && operationinlist.Description == item.OperationDescription);



                            }
                        }
                    }//end if distribution



                    ////PRODUCTO
                    if (item.ProductId == -1)
                    {
                        //this is a new product
                        //chek if now exist or created to use and save
                        var indexproduct = SaveProducts.FindIndex(productinlist => productinlist.Code == item.ProductCode && productinlist.Description == item.ProductDescription);

                        if (indexproduct >= 0)
                        {
                            //area exist in save areas
                            finalAssyChart.ProductId = SaveProducts[indexproduct].ProductId;
                        }
                        else
                        {
                            //if not exist in data list of product saved, search in database whit code and description

                            if (!await _supervisorMobilityRepository.ProductExistByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription))
                            {
                                ResumeActionsResultsToReturn.ProductCreated++;

                                //operation not exist, create now
                                var productForCreate = _mapper.Map<ProductForCreationDto>(new ProductForCreationDto() { Code = item.ProductCode, Description = item.ProductDescription, IsActive = (bool)item.ProductIsActive });

                                var finalProduct = _mapper.Map<Product>(productForCreate);

                                _supervisorMobilityRepository.AddProduct(finalProduct);

                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var createdProductToSave = _mapper.Map<ProductDto>(finalProduct);

                                SaveProducts.Add(createdProductToSave);

                                finalAssyChart.ProductId = createdProductToSave.ProductId;

                            }
                            else
                            {
                                //product exist in database
                                var ProductBecauseExist = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
                                finalAssyChart.ProductId = ProductBecauseExist.ProductId;


                            }
                        }
                    }
                    else
                    {
                        //product exist, search in database
                        Product? productEntityInDataBase = await _supervisorMobilityRepository.GetProductAsync((int)item.ProductId);

                        if (productEntityInDataBase != null)
                        {
                            //exist check if update any field
                            if (productEntityInDataBase.Code != item.ProductCode || productEntityInDataBase.Description != item.ProductDescription)
                            {
                                //update area 
                                ResumeActionsResultsToReturn.ProductUpdated++;
                                var ProductDataInDoc = _mapper.Map<ProductForUpdateDto>(new ProductForUpdateDto { Code = item.ProductCode, Description = item.ProductDescription, IsActive = (bool)item.ProductIsActive });

                                _mapper.Map(ProductDataInDoc, productEntityInDataBase);
                                await _supervisorMobilityRepository.SaveChangesAsync();

                                var producttosave = _mapper.Map<ProductDto>(productEntityInDataBase);
                                producttosave.Code = item.ProductCode;
                                producttosave.Description = item.ProductDescription;
                                producttosave.ProductId = (int)item.ProductId;

                                SaveProducts.Add(producttosave);
                            }
                            else
                            {
                                SaveProducts.Add(_mapper.Map<ProductDto>(productEntityInDataBase));

                            }
                            finalAssyChart.ProductId = productEntityInDataBase.ProductId;

                        }
                        else
                        {
                            if (!await _supervisorMobilityRepository.ProductExistByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription))
                            {
                                ResumeActionsResultsToReturn.OperationCreated++;

                                //operation not exist, create now
                                var productForCreate = _mapper.Map<ProductForCreationDto>(new ProductForCreationDto() { Code = item.ProductCode, Description = item.ProductDescription, IsActive = (bool)item.ProductIsActive });
                                var finalProduct = _mapper.Map<Product>(productForCreate);
                                _supervisorMobilityRepository.AddProduct(finalProduct);
                                await _supervisorMobilityRepository.SaveChangesAsync();
                                var createdProductToSave = _mapper.Map<ProductDto>(finalProduct);
                                                                SaveProducts.Add(createdProductToSave);
                                                                finalAssyChart.ProductId = createdProductToSave.ProductId;
                                                            }
                            else                            {
                                //product exist in database
                                var ProductBecauseExist = await _supervisorMobilityRepository.GetProductByCodeAndDescriptionAsync(item.ProductCode, item.ProductDescription);
                                                                finalAssyChart.ProductId = ProductBecauseExist.ProductId;
                           }
                        }
                    }//end if distribution

                    Debug.WriteLine($"id {finalAssyChart.AssyChardId} plantid {finalAssyChart.PlantId}");
                    //Assychart
                    if (finalAssyChart.AssyChardId == -1)
                    {
                        finalAssyChart.CreationDate = DateTime.Now;
                        finalAssyChart.ModificationDate = DateTime.Now;
                        ResumeActionsResultsToReturn.AssyChartCreated++;
                        await _assyChartService.CreateAssyChartAsync(finalAssyChart);
                    }
                    else
                    {
                        //update assy chart
                        ResumeActionsResultsToReturn.AssyChartUpdated++;

                        var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync((int)item.AssyChardId);

                        if (assyChartEntity != null)
                        {
                            _mapper.Map(finalAssyChart, assyChartEntity);

                            await _supervisorMobilityRepository.SaveChangesAsync();
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

    }


}
