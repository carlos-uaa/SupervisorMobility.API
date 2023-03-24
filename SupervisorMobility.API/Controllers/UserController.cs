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
using SupervisorMobility.API.DataAccess.Entities;
using Microsoft.AspNetCore.Cors;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ReturnResults;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public UsersController(IWebHostEnvironment env, ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper, IAssyChartService assyChartService)
        {
            _env = env;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersWhitNavigationDetails>>> GetUsers(bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetAllUsersWhitPlantAreaAndGroupAsync();
                return Ok(_mapper.Map<IEnumerable<UsersWhitNavigationDetails>>(userEntity));
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetAllUsersAsync();
                return Ok(_mapper.Map<IEnumerable<UsersWhitoutNavigationDetails>>(userEntity));
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersWhitNavigationDetails>> GetUser(int userId, bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId, collections);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWhitNavigationDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWhitoutNavigationDetails>(userEntity));

                }

                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<UsersWhitNavigationDetails>> CreateUser(UsersForCreation newUser)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(newUser.PlantId))
            {
                return NotFound("No Planta");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(newUser.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.GroupExistAsync(newUser.GroupId))
            {
                return NotFound("No Group");
            }

            var finalUser = await _assyChartService.CreateUserAsync(newUser);

            return Ok(finalUser);

        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int userId, UsersForUpdateDto user)
        {
            var userEntity = await _assyChartService.FetchUserAsync(userId);
            if (userEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdateUserAsync(user, userEntity);

            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var userEntity = await _assyChartService.FetchUserAsync(userId);
            if (userEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveUserAsync(userEntity);

            return Ok();
        }


        [HttpPost("/massderegistration")]
        public async Task<ActionResult> Masderegistration()
        {


            return Ok();
        }

        //******* Upload users    **********//


        [HttpPost("FileUpload/Data")]
        public async Task<ActionResult<UploadUsersResult>> ApplyUsersUpload(FileUploadGeneralDto FileToInsert)
        {
            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\users\\" + FileToInsert.StorageFileName;
            string originalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + System.IO.Path.GetExtension(file));

            UploadUsersResult result = new UploadUsersResult();
            List<UsersWhitoutNavigationDetails> UsersListToSave = new List<UsersWhitoutNavigationDetails>();

            if (FileToInsert.ContentType == "text/csv")
            {
                //csv
            }
            else
            {
                // Obtiene la ruta del archivo con la extensión original

                if (FileToInsert.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    // Obtiene la nueva ruta del archivo con la nueva extensión
                    string newPath = System.IO.Path.ChangeExtension(originalPath, ".xlsx");
                    // Mueve el archivo a la nueva ruta
                    System.IO.File.Move(originalPath, newPath);
                    file = newPath;


                }
                else if (FileToInsert.ContentType == "application/vnd.ms-excel")
                {
                    // Obtiene la nueva ruta del archivo con la nueva extensión
                    string newPath = System.IO.Path.ChangeExtension(originalPath, ".xls");
                    // Mueve el archivo a la nueva ruta
                    System.IO.File.Move(originalPath, newPath);
                    file = newPath;

                }
                else
                {
                    return NotFound();
                }


                try
                {

                    using (var workBook = new XLWorkbook(file))
                    {
                        IXLWorksheet ws = workBook.Worksheet(1);

                        ws.Column("A").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("J").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("K").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("L").Style.Font.Underline = XLFontUnderlineValues.Single;

                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        int i = 2;
                        foreach (IXLRow row in ws.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                firstRow = false;
                            }
                            else
                            {
                                if (!row.IsEmpty())
                                {
                                    var userToInsert = new UsersWhitoutNavigationDetails();
                                    //1UserId	2Payroll	3Name	4Plant	5Area	6Grupo	7Admin	8Supervisor	9Operator	10Create	11Update	12Disable	13Active
                                    //UserPorp
                                    userToInsert.UserId = ws.Cell(i,1).Value.ToString() != "" ? int.Parse(ws.Cell(i, 1).Value.ToString()) : -1;
                                    userToInsert.Payroll = ws.Cell(i, 2).Value.ToString() != "" ? int.Parse(ws.Cell(i, 2).Value.ToString()) : 0;
                                    userToInsert.Name = ws.Cell(i, 3).Value.ToString() != "" ? ws.Cell(i, 3).Value.ToString() : "";
                                    //Navigation Porpieties
                                    userToInsert.PlantId = ws.Cell(i, 4).Value.ToString() != "" ? int.Parse(ws.Cell(i, 4).Value.ToString()) : -1;
                                    userToInsert.AreaId = ws.Cell(i, 5).Value.ToString() != "" ? int.Parse(ws.Cell(i, 5).Value.ToString()) : -1;
                                    userToInsert.GroupId = ws.Cell(i, 6).Value.ToString() != "" ? int.Parse(ws.Cell(i, 6).Value.ToString()) : -1;
                                    //Permission
                                    userToInsert.IsAdmin = ws.Cell(i, 7).Value.ToString() != "" ? bool.Parse(ws.Cell(i, 7).Value.ToString()) : false;
                                    userToInsert.IsSupervisor = ws.Cell(i, 8).Value.ToString() != "" ? bool.Parse(ws.Cell(i, 8).Value.ToString()) : false;
                                    userToInsert.IsOperator = ws.Cell(i, 9).Value.ToString() != "" ? bool.Parse(ws.Cell(i, 9).Value.ToString()) : false;
                                    //Date Controlls
                                    try
                                    {
                                        //create date
                                        userToInsert.CreatedDate = ws.Cell(i, 10).Value.ToString() != "" ? DateTime.Parse(ws.Cell(i, 10).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.CreatedDate = DateTime.Now;
                                    }
                                    try
                                    {
                                        //update
                                        userToInsert.LastUpdated = ws.Cell(i, 11).Value.ToString() != "" ? DateTime.Parse(ws.Cell(i, 11).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.LastUpdated = DateTime.Now;
                                    }

                                    try
                                    {
                                        //disabel
                                        userToInsert.DisabledDate = ws.Cell(i, 12).GetString() != "" ? DateTime.Parse(ws.Cell(i, 12).GetValue<string>()) : null;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.DisabledDate = null;
                                    }
                                    //Is acctive
                                    userToInsert.IsActive = ws.Cell(i, 13).GetString() != "" ? bool.Parse(ws.Cell(i, 13).Value.ToString()) : false;

                                    //1UserId	2Payroll	3Name	4Plant	5Area	6Grupo	7Admin	8Supervisor	9Operator	10Create	11Update	12Disable	13Active
                                    UsersListToSave.Add(userToInsert);

                                    i++;
                                }//end is not empety row
                            }//end else first roe

                        }//end foreach
                        
                    }//end using



                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }//end trycatch to add excel to list
            }

            UploadUsersResult ResultToReturn = new UploadUsersResult();

            foreach (var userItem in UsersListToSave)
            {
                if (userItem.UserId == -1)
                {

                    //Otros campos
                    var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync(userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);

                    if (entityUserPayAndExtras == null)
                    {

                        //new user
                        UsersForCreation newuser = new UsersForCreation()
                        {
                            Name = userItem.Name,
                            Payroll = userItem.Payroll,
                            PlantId = (int)userItem.PlantId,
                            AreaId = (int)userItem.AreaId,
                            GroupId = (int)userItem.GroupId,
                            IsActive = userItem.IsActive,
                            IsAdmin = userItem.IsAdmin,
                            IsSupervisor = userItem.IsSupervisor,
                            IsOperator = userItem.IsOperator,
                            CreatedDate = (DateTime)userItem.CreatedDate,
                            LastUpdated = userItem.LastUpdated,
                            DisabledDate = userItem.DisabledDate
                        };

                        var finalUser = await _assyChartService.CreateUserAsync(newuser);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                    }
                    else
                    {
                        ResultToReturn.UsersExist++;
                    }

                }
                else
                {

                    var entityUser = await _assyChartService.FetchUserAsync((int)userItem.UserId);

                    if (entityUser == null)
                    {

                        var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync(userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);
                        if (entityUserPayAndExtras == null)
                        {
                            //new user
                            UsersForCreation newuser = new UsersForCreation()
                            {
                                Name = userItem.Name,
                                Payroll = userItem.Payroll,
                                PlantId = (int)userItem.PlantId,
                                AreaId = (int)userItem.AreaId,
                                GroupId = (int)userItem.GroupId,
                                IsActive = userItem.IsActive,
                                IsAdmin = userItem.IsAdmin,
                                IsSupervisor = userItem.IsSupervisor,
                                IsOperator = userItem.IsOperator,
                                CreatedDate = (DateTime)userItem.CreatedDate,
                                LastUpdated = userItem.LastUpdated,
                                DisabledDate = userItem.DisabledDate
                            };

                            var finalUser = await _assyChartService.CreateUserAsync(newuser);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;
                        }
                        else
                        {
                            ResultToReturn.UsersExist++;
                        }
                    }
                    else
                    {
                        ResultToReturn.UsersExist++;
                    }

                }

            }
            //restore extencion of file
            System.IO.File.Move(file, originalPath);

            return Ok(ResultToReturn);

        }


    }
}