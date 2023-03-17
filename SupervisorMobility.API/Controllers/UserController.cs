using AutoMapper;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUpload;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using System.Net;

using Microsoft.VisualBasic.FileIO;
using SpreadsheetLight;
using SupervisorMobility.API.Models.AreaDtos;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.ProductDtos;
using Azure;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DocumentFormat.OpenXml.Presentation;

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


        //******* Upload users    **********//


        [HttpPost("FileUpload/Data")]
        //public async Task<ActionResult<string>> ApplyUsersUpload(UploadResult FileInfo)
        public async Task<ActionResult<UploadUsersResult>> ApplyUsersUpload(FileUploadGeneralDto FileToInsert)
        {
            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\users\\" + FileToInsert.StorageFileName;

            UploadUsersResult result = new UploadUsersResult();
            List<User> UsersListToSave = new List<User>();

            if (FileToInsert.ContentType == "text/csv")
            {
                //csv
            }
            else
            {
                // Obtiene la ruta del archivo con la extensión original
                string originalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + System.IO.Path.GetExtension(file));

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
                        Debug.WriteLine($"Si abrio el excel");

                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        int i = 1;
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
                                    User userToInsert = new User();
                                    //1UserId	2Payroll	3Name	4Plant	5Area	6Grupo	7Admin	8Supervisor	9Operator	10Create	11Update	12Disable	13Active
                                    //UserPorp
                                    userToInsert.UserId = ws.Cell(i, 1).GetString() != "" ? ws.Cell(i, 1).GetValue<int>() : -1;
                                    userToInsert.Payroll = ws.Cell(i, 2).GetString() != "" ? ws.Cell(i, 2).GetValue<int>() : -1;
                                    userToInsert.Name = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                    //Navigation Porpieties
                                    userToInsert.PlantId = ws.Cell(i, 4).GetString() != "" ? ws.Cell(i, 4).GetValue<int>() : -1;
                                    userToInsert.AreaId = ws.Cell(i, 5).GetString() != "" ? ws.Cell(i, 5).GetValue<int>() : -1;
                                    userToInsert.GroupId = ws.Cell(i, 6).GetString() != "" ? ws.Cell(i, 6).GetValue<int>() : -1;
                                    //Permission
                                    userToInsert.IsAdmin = ws.Cell(i, 7).GetString() != "" ? ws.Cell(i, 7).GetValue<bool>() : false;
                                    userToInsert.IsSupervisor = ws.Cell(i, 8).GetString() != "" ? ws.Cell(i, 8).GetValue<bool>() : false;
                                    userToInsert.IsOperator = ws.Cell(i, 9).GetString() != "" ? ws.Cell(i, 9).GetValue<bool>() : false;
                                    //Date Controlls
                                    try
                                    {
                                        //create date
                                        userToInsert.CreatedDate = ws.Cell(i, 10).GetString() != "" ? DateTime.Parse(ws.Cell(i, 10).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.CreatedDate = DateTime.Now;
                                    }
                                    try
                                    {
                                        //update
                                        userToInsert.LastUpdated = ws.Cell(i, 11).GetString() != "" ? DateTime.Parse(ws.Cell(i, 11).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.LastUpdated = DateTime.Now;
                                    }

                                    try
                                    {
                                        //disabel
                                        userToInsert.DisabledDate = ws.Cell(i, 12).GetString() != "" ? DateTime.Parse(ws.Cell(i, 12).GetValue<string>()) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        userToInsert.DisabledDate = DateTime.Now;
                                    }
                                    //Is acctive
                                    userToInsert.IsActive = ws.Cell(i, 13).GetString() != "" ? ws.Cell(i, 13).GetValue<bool>() : false;

                                    //1UserId	2Payroll	3Name	4Plant	5Area	6Grupo	7Admin	8Supervisor	9Operator	10Create	11Update	12Disable	13Active
                                    i++;
                                    UsersListToSave.Add(userToInsert);
                                }
                            }

                        }//end foreach

                    }//end using

                    Debug.WriteLine($"");


                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }//end trycatch to add excel to list
            }

            foreach (User userItem in UsersListToSave)
            {
                //new validations 

                if (userItem.UserId == -1)
                {
                    //busqueda solo por nomina
                    if (userItem.Payroll.ToString() != "" && userItem.Name == "")
                    {
                        var userByPayroll = await _supervisorMobilityRepository.UserExistAsync(userItem.Payroll);


                    }
                    else if (userItem.Payroll.ToString() != "")
                    {
                        //busqueda avanzada planta area grupo
                        var advanceUser = await _supervisorMobilityRepository.UserExistAdvanceAsync(userItem.Name, userItem.Payroll, (int)userItem.PlantId, (int)userItem.AreaId, (int)userItem.GroupId);
                    }
                    else
                    {
                        //se crea
                    }




                }
                else
                {
                    //usuario existe

                    //get entity from db
                    var entityUser = await _assyChartService.FetchUserAsync(userItem.UserId);

                    if (entityUser != null)
                    {
                        //user exist

                    }
                    else
                    {
                        //user not exist

                        //busqueda avanzada

                    }

                }

            }

            result.UsersCreated = 3;
            return Ok(result);

        }


    }
}