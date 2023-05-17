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
using System.Drawing.Text;

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
        public async Task<ActionResult<IEnumerable<UsersWithNavigationDetails>>> GetUsers(bool collections = false)
        {
            var userEntity = await _supervisorMobilityRepository.GetAllUsersAsync();
            if (collections)
            {

                return Ok(_mapper.Map<IEnumerable<UsersWithNavigationDetails>>(userEntity));
            }
            else
            {

                return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationDetails>>(userEntity));
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersWithNavigationDetails>> GetUser(int userId, bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId, collections);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationDetails>(userEntity));

                }

                return NotFound();
            }
        }

        [HttpGet("ByObjectId")]
        public async Task<ActionResult<UsersWithNavigationDetails>> GetUserByObject(string ObjectId, bool collections = false)
        {

            var userEntity = await _assyChartService.FetchUserWhitObjectIdAsync(ObjectId);

            if (collections)
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationDetails>(userEntity));
                }
                return NotFound();
            }
        }


        [HttpGet("ByEmail")]
        public async Task<ActionResult<UsersWithNavigationDetails>> GetUserByEmail(string email, bool collections = false)
        {

            var userEntity = await _assyChartService.FetchUserByEmailAsync(email);

            if (collections)
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationDetails>(userEntity));
                }
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<ActionResult<UsersWithNavigationDetails>> CreateUser(UsersForCreation newUser)
        {
            List<Area> Areas = new List<Area>();
            List<User> Users = new List<User>();
            bool haveAreas = false;
            bool haveUsers = false;

            if (newUser.PlantId == 0)
            {
                newUser.PlantId = null;
            }
            else if (newUser.PlantId != null)
            {
                if (!await _supervisorMobilityRepository.PlantExistAsync((int)newUser.PlantId))
                {
                    return NotFound("No Planta");
                }
            }

            if (newUser.AreaId == 0)
            {
                newUser.AreaId = null;
            }
            else if (newUser.AreaId != null)
            {
                if (!await _supervisorMobilityRepository.AreaExistAsync((int)newUser.AreaId))
                {
                    return NotFound("No Area");
                }
            }

            if (newUser.GroupId == 0)
            {
                newUser.GroupId = null;
            }
            else if (newUser.GroupId != null)
            {
                if (!await _supervisorMobilityRepository.AreaExistAsync((int)newUser.GroupId))
                {
                    return NotFound("No Area");
                }
            }

            if (newUser.DistributionId == 0)
            {
                newUser.DistributionId = null;
            }
            else
            {
                if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)newUser.DistributionId))
                {
                    return NotFound("No Distribution");
                }
            }

            if (newUser.Payroll == 0)
            {
                newUser.Payroll = null;
            }

            if (newUser.SuperiorId == 0)
            {
                newUser.SuperiorId = null;
            }


            if (newUser.Subordinates != null)
            {
                haveUsers = true;
                foreach (var Sub in newUser.Subordinates)
                {
                    Users.Add(await _assyChartService.FetchUserAsync(Sub.UserId));
                }

                newUser.Subordinates = null;
            }

            if (newUser.Areas != null)
            {
                haveAreas = true;
                foreach (var AreainList in newUser.Areas)
                {
                    Areas.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)newUser.PlantId, AreainList.AreaId));
                }
                newUser.Areas = null;
            }

            var finalUser = await _assyChartService.CreateUserAsync(newUser);

            var UserToReturn = _mapper.Map<User>(finalUser);

            if (haveUsers)
            {
                foreach (var item in Users)
                {
                    _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, item);
                }
                   
            }

            if (haveAreas)
            {
                foreach (var item in Areas)
                {
                    _supervisorMobilityRepository.UserAddArea(UserToReturn, item);
                }
                    
            }

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(UserToReturn);

        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int userId, UsersForUpdateDto user)
        {
            List<Area> Areas = new List<Area>();
            List<User> Users = new List<User>();
            bool haveAreas = false;
            bool haveUsers = false;

            if (user.Areas != null)
            {
                haveAreas = true;
                foreach (var AreainList in user.Areas)
                {
                    Areas.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)user.PlantId, AreainList.AreaId));
                }
                user.Areas = null;
            }

          
            await _assyChartService.UpdateUserAsync(user, userId);

            var UserToReturn = _mapper.Map<User>(user);

            if (haveAreas)
            {
                foreach (var item in Areas)
                {
                    _supervisorMobilityRepository.UserAddArea(UserToReturn, item);
                }

            }

           

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
            //

            return Ok();
        }

        //******* Upload users    **********//

        [HttpPost("MasiveUpload")]
        public async Task<ActionResult<UploadUsersResult>> MassiveUpload(List<UsersWithPeopleAndWithoutNavigationDetails>  UsersToCreate)
        {
            
            UploadUsersResult ResultToReturn = new UploadUsersResult();
            foreach (var item in UsersToCreate)
            {
                List<Area> Areas = new List<Area>();
                List<User> Users = new List<User>();
                bool haveAreas = false;
                bool haveUsers = false;

                var UserToReturn = new User();

                if (item.PlantId == 0)
                {
                    item.PlantId = null;
                }
                else if (item.PlantId != null)
                {
                    if (!await _supervisorMobilityRepository.PlantExistAsync((int)item.PlantId))
                    {
                        return NotFound("No Planta");
                    }
                }

                if (item.AreaId == 0)
                {
                    item.AreaId = null;
                }
                else if (item.AreaId != null)
                {
                    if (!await _supervisorMobilityRepository.AreaExistAsync((int)item.AreaId))
                    {
                        return NotFound("No Area");
                    }
                }

                if (item.GroupId == 0)
                {
                    item.GroupId = null;
                }
                else if (item.GroupId != null)
                {
                    if (!await _supervisorMobilityRepository.AreaExistAsync((int)item.GroupId))
                    {
                        return NotFound("No Area");
                    }
                }

                if (item.DistributionId == 0)
                {
                    item.DistributionId = null;
                }
                else
                {
                    if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)item.DistributionId))
                    {
                        return NotFound("No Distribution");
                    }
                }

                if (item.Payroll == 0)
                {
                    item.Payroll = null;
                }

                if (item.SuperiorId == 0)
                {
                    item.SuperiorId = null;
                }


                if (item.Subordinates != null)
                {
                    haveUsers = true;
                    foreach (var Sub in item.Subordinates)
                    {
                        Users.Add(await _assyChartService.FetchUserAsync(Sub.UserId));
                    }

                    item.Subordinates = null;
                }

                if (item.Areas != null)
                {
                    haveAreas = true;
                    foreach (var AreainList in item.Areas)
                    {
                        Areas.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)item.PlantId, AreainList.AreaId));
                    }
                    item.Areas = null;
                }

                ///////////////////
                if (userItem.UserId == -1)
                {
                    //Usuario sin id
                    //Busqueda avanzada
                    var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync((int)userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);

                    if (entityUserPayAndExtras == null)
                    {
                        //new user
                        UsersForCreation newuser = new UsersForCreation()
                        {
                            Name = userItem.Name,
                            ObjectId = userItem.ObjectId,
                            Payroll = userItem.Payroll,
                            PlantId = (int)userItem.PlantId,
                            AreaId = (int)userItem.AreaId,
                            GroupId = (int)userItem.GroupId,
                            UserType = (int)userItem.UserType,
                            LastUpdated = userItem.LastUpdated,
                            DisabledDate = userItem.DisabledDate,
                            IsActive = userItem.IsActive
                        };

                        var finalUser = await _assyChartService.CreateUserAsync(newuser);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                    }
                    else
                    {
                        //User ya existe
                        ResultToReturn.UsersExist++;
                    }

                }
                else
                {
                    //User con id
                    var entityUser = await _assyChartService.FetchUserAsync((int)userItem.UserId);

                    if (entityUser == null)
                    {
                        //Si tiene un id erroneo, entra aqui
                        var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync((int)userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);
                        if (entityUserPayAndExtras == null)
                        {
                            //new user porque no existe
                            UsersForCreation newuser = new UsersForCreation()
                            {
                                Name = userItem.Name,
                                ObjectId = userItem.ObjectId,
                                Payroll = userItem.Payroll,
                                PlantId = (int)userItem.PlantId,
                                AreaId = (int)userItem.AreaId,
                                GroupId = (int)userItem.GroupId,
                                UserType = (int)userItem.UserType,
                                LastUpdated = userItem.LastUpdated,
                                DisabledDate = userItem.DisabledDate,
                                IsActive = userItem.IsActive
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
                        //Si el usuario existe entra aqui
                        ResultToReturn.UsersExist++;
                    }

                } 


                ////////
                if (item.UserId != 0)
                {
                    var usertoUpdate = _mapper.Map<UsersForUpdateDto>(item);
                    await _assyChartService.UpdateUserAsync(usertoUpdate, item.UserId);
                    UserToReturn = _mapper.Map<User>(item);
                    ResultToReturn.UsersUpdated++;
                }
                else
                {
                    var usertoCreate = _mapper.Map<UsersForCreation>(item);
                    var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                    UserToReturn = _mapper.Map<User>(finalUser);
                    ResultToReturn.UsersCreated++;
                }

                if (haveUsers)
                {
                    foreach (var elemntUser in Users)
                    {
                        _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, elemntUser);
                    }
                }

                if (haveAreas)
                {
                    foreach (var elemntArea in Areas)
                    {
                        _supervisorMobilityRepository.UserAddArea(UserToReturn, elemntArea);
                    }
                }

            }
            
                await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(ResultToReturn);

        }

        [HttpPost("MasiveUpload/Superior/{superiorId}")]
        public async Task<ActionResult<UploadUsersResult>> MassiveUsersToSuperior(List<UsersWithPeopleAndWithoutNavigationDetails> UsesToCreateInSuperior, int superiorId)
        {

        }


        [HttpPost("FileUpload/Data")]
        public async Task<ActionResult<UploadUsersResult>> ApplyUsersUpload(FileUploadGeneralDto FileToInsert)
        {
            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\users\\" + FileToInsert.StorageFileName;
            string originalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + System.IO.Path.GetExtension(file));

            
            List<UsersWithoutNavigationDetails> UsersListToSave = new List<UsersWithoutNavigationDetails>();

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

                        //Forcar a que la columna contenga estilo, si es vacia, sea detectada 
                        //userid
                        ws.Column("A").Style.Font.Underline = XLFontUnderlineValues.Single;
                        //nomian
                        ws.Column("B").Style.Font.Underline = XLFontUnderlineValues.Single;
                        //planta area grupo
                        ws.Column("D").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("E").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("F").Style.Font.Underline = XLFontUnderlineValues.Single;
                        //permissions 
                        ws.Column("G").Style.Font.Underline = XLFontUnderlineValues.Single;
                        //date
                        ws.Column("H").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("I").Style.Font.Underline = XLFontUnderlineValues.Single;
                        ws.Column("J").Style.Font.Underline = XLFontUnderlineValues.Single;
                        //is active
                        ws.Column("K").Style.Font.Underline = XLFontUnderlineValues.Single;

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
                                    var userToInsert = new UsersWithoutNavigationDetails();
                                    //UserId	ObjectId	Payroll	Name	Plant	Area	Grupo	Permission	CreateDate	UpdateDate	DisableDate	IsActive
                                    //1         2           3       4       5       6       7       8           9           10          11          12          
                                    //UserPorp
                                    userToInsert.UserId = ws.Cell(i, 1).Value.ToString() != "" ? int.Parse(ws.Cell(i, 1).Value.ToString()) : -1;
                                    //
                                    userToInsert.ObjectId = ws.Cell(i, 2).Value.ToString() != "" ? ws.Cell(i, 2).Value.ToString() : "";
                                    userToInsert.Payroll = ws.Cell(i, 3).Value.ToString() != "" ? int.Parse(ws.Cell(i, 3).Value.ToString()) : 0;
                                    userToInsert.Name = ws.Cell(i, 4).Value.ToString() != "" ? ws.Cell(i, 4).Value.ToString() : "";
                                    //Navigation Porpieties
                                    userToInsert.PlantId = ws.Cell(i, 5).Value.ToString() != "" ? int.Parse(ws.Cell(i, 5).Value.ToString()) : -1;
                                    userToInsert.AreaId = ws.Cell(i, 6).Value.ToString() != "" ? int.Parse(ws.Cell(i, 6).Value.ToString()) : -1;
                                    userToInsert.GroupId = ws.Cell(i, 7).Value.ToString() != "" ? int.Parse(ws.Cell(i, 7).Value.ToString()) : -1;
                                    //Permission
                                    userToInsert.UserType = ws.Cell(i, 8).Value.ToString() != "" ? int.Parse(ws.Cell(i, 8).Value.ToString()) : 0;


                                    //Date Controlls
                                    try
                                    {
                                        //create date
                                        userToInsert.CreatedDate = ws.Cell(i, 9).Value.ToString() != "" ? DateTime.Parse(ws.Cell(i, 9).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.CreatedDate = DateTime.Now;
                                    }
                                    try
                                    {
                                        //update
                                        userToInsert.LastUpdated = ws.Cell(i, 10).Value.ToString() != "" ? DateTime.Parse(ws.Cell(i, 10).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.LastUpdated = DateTime.Now;
                                    }

                                    try
                                    {
                                        //disabel
                                        userToInsert.DisabledDate = ws.Cell(i, 11).GetString() != "" ? DateTime.Parse(ws.Cell(i, 11).GetValue<string>()) : null;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.DisabledDate = null;
                                    }
                                    //Is acctive
                                    userToInsert.IsActive = ws.Cell(i, 12).GetString() != "" ? bool.Parse(ws.Cell(i, 12).Value.ToString()) : false;
                                    //UserId	ObjectId	Payroll	Name	Plant	Area	Grupo	Permission	CreateDate	UpdateDate	DisableDate	IsActive
                                    //1         2           3       4       5       6       7       8           9           10          11          12   


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
                    //Usuario sin id
                    //Busqueda avanzada
                    var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync((int)userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);

                    if (entityUserPayAndExtras == null)
                    {
                        //new user
                        UsersForCreation newuser = new UsersForCreation()
                        {
                            Name = userItem.Name,
                            ObjectId = userItem.ObjectId,
                            Payroll = userItem.Payroll,
                            PlantId = (int)userItem.PlantId,
                            AreaId = (int)userItem.AreaId,
                            GroupId = (int)userItem.GroupId,
                            UserType = (int)userItem.UserType,
                            LastUpdated = userItem.LastUpdated,
                            DisabledDate = userItem.DisabledDate,
                            IsActive = userItem.IsActive
                        };

                        var finalUser = await _assyChartService.CreateUserAsync(newuser);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                    }
                    else
                    {
                        //User ya existe
                        ResultToReturn.UsersExist++;
                    }

                }
                else
                {
                    //User con id
                    var entityUser = await _assyChartService.FetchUserAsync((int)userItem.UserId);

                    if (entityUser == null)
                    {
                        //Si tiene un id erroneo, entra aqui
                        var entityUserPayAndExtras = await _supervisorMobilityRepository.GetUserByPayrollAndMoreAsync((int)userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);
                        if (entityUserPayAndExtras == null)
                        {
                            //new user porque no existe
                            UsersForCreation newuser = new UsersForCreation()
                            {
                                Name = userItem.Name,
                                ObjectId = userItem.ObjectId,
                                Payroll = userItem.Payroll,
                                PlantId = (int)userItem.PlantId,
                                AreaId = (int)userItem.AreaId,
                                GroupId = (int)userItem.GroupId,
                                UserType = (int)userItem.UserType,
                                LastUpdated = userItem.LastUpdated,
                                DisabledDate = userItem.DisabledDate,
                                IsActive = userItem.IsActive
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
                        //Si el usuario existe entra aqui
                        ResultToReturn.UsersExist++;
                    }

                }

            }
            //restore extencion of file
            System.IO.File.Move(file, originalPath);

            return Ok(ResultToReturn);

        }

        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadAllUsersFormat")]
        public async Task<IActionResult> DownloadAllUsersFormat()
        {

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "UserId");
            ws.SetCellValue("B1", "ObjectId");
            ws.SetCellValue("C1", "Payroll");
            ws.SetCellValue("D1", "Name");
            ws.SetCellValue("E1", "Plant");
            ws.SetCellValue("F1", "Area");
            ws.SetCellValue("G1", "Group");
            ws.SetCellValue("H1", "Permission");
            ws.SetCellValue("I1", "User creation date");
            ws.SetCellValue("J1", "User update date");
            ws.SetCellValue("K1", "User disable date");
            ws.SetCellValue("L1", "Is Active");


            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllUsersFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadSSVFormat")]
        public async Task<IActionResult> DownloadSSVFormat()
        {

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "SSV Format");

            //ROW Data identificators
            ws.SetCellValue("A1", "UserId SSV");
            ws.SetCellValue("B1", "Name");
            ws.SetCellValue("C1", "Email");
            ws.SetCellValue("D1", "Plant");
            ws.SetCellValue("E1", "Group");
            ws.SetCellValue("F1", "AreasManage");

            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SSVFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadSupervisorFormat")]
        public async Task<IActionResult> DownloadSupervisorFormat()
        {

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "UserId Supervisor");
            ws.SetCellValue("B1", "Name");
            ws.SetCellValue("C1", "Email");
            ws.SetCellValue("D1", "SSV_Id Superior");
            ws.SetCellValue("E1", "Assign Area_ID");

            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SupervisorsFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadOperatorsFormat")]
        public async Task<IActionResult> DownloadOperatorsFormat()
        {

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "UserId Operator");
            ws.SetCellValue("B1", "Payroll");
            ws.SetCellValue("C1", "Name");
            ws.SetCellValue("D1", "Distribution Id");
            ws.SetCellValue("E1", "Supervisor_Id Superior");

            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OperatorsFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

    }
}