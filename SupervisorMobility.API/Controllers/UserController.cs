using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Models.ReturnResults;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersWithNavigationAndPeopleDetails>> GetUser(int userId, bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId, collections);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationAndPeopleDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationWithoutPeopleDetails>(userEntity));

                }

                return NotFound();
            }
        }

        [HttpGet("{SupervisorId}/Subordinates")]
        public async Task<ActionResult<IEnumerable<UsersWithNavigationAndPeopleDetails>>> GetSubordinates(int SupervisorId, bool collections = false)
        {
            var userEntity = await _supervisorMobilityRepository.GetAllSubordinatesAsync(SupervisorId);
            if (collections)
            {
                return Ok(_mapper.Map<IEnumerable<UsersWithNavigationAndPeopleDetails>>(userEntity));
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationWithoutPeopleDetails>>(userEntity));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersWithNavigationAndPeopleDetails>>> GetUsers(bool includeCollections = false, bool includeSubordinates = false)
        {
            var userEntity = await _supervisorMobilityRepository.GetAllUsersAsync(includeCollections, includeSubordinates);
            if (includeCollections)
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithNavigationAndPeopleDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutPeopleWithNavigation>>(userEntity));
            }
            else
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithPeopleWithoutNavigationDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationWithoutPeopleDetails>>(userEntity));
            }
        }


        [HttpGet("ByUserType")]
        public async Task<ActionResult<IEnumerable<UsersWithNavigationAndPeopleDetails>>> GetUserByType(int typeUser, bool includeCollections = false, bool includeSubordinates = false)
        {
            var userEntity = await _supervisorMobilityRepository.GetAllUserByTypeAsync(typeUser, includeCollections, includeSubordinates);
            if (includeCollections)
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithNavigationAndPeopleDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutPeopleWithNavigation>>(userEntity));
            }
            else
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithPeopleWithoutNavigationDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationWithoutPeopleDetails>>(userEntity));
            }
        }


        [HttpGet("ByUserTypeInPlantAndArea")]
        public async Task<ActionResult<IEnumerable<UsersWithNavigationAndPeopleDetails>>> ByUserTypeInPlantAndArea(int plantid, int areaid, int typeUser, bool includeCollections = false, bool includeSubordinates = false)
        {

            var userEntity = await _supervisorMobilityRepository.GetAllUserByTypeInPlantAreaAsync(plantid, areaid, typeUser, includeCollections, includeSubordinates);
            if (includeCollections)
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithNavigationAndPeopleDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutPeopleWithNavigation>>(userEntity));
            }
            else
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithPeopleWithoutNavigationDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationWithoutPeopleDetails>>(userEntity));
            }
        }

        [HttpGet("ByUserTypeInPlant")]
        public async Task<ActionResult<IEnumerable<UsersWithNavigationAndPeopleDetails>>> ByUserTypeInPlantAndArea(int plantid, int typeUser, bool includeCollections = false, bool includeSubordinates = false)
        {

            var userEntity = await _supervisorMobilityRepository.GetAllUserByTypeInPlantAsync(plantid, typeUser, includeCollections, includeSubordinates);
            if (includeCollections)
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithNavigationAndPeopleDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutPeopleWithNavigation>>(userEntity));
            }
            else
            {
                if (includeSubordinates)
                    return Ok(_mapper.Map<IEnumerable<UsersWithPeopleWithoutNavigationDetails>>(userEntity));
                else
                    return Ok(_mapper.Map<IEnumerable<UsersWithoutNavigationWithoutPeopleDetails>>(userEntity));
            }
        }

        [HttpGet("ByObjectId")]
        public async Task<ActionResult<UsersWithNavigationAndPeopleDetails>> GetUserByObject(string ObjectId, bool collections = false)
        {


            var userEntity = await _assyChartService.FetchUserWhitObjectIdAsync(ObjectId);

            if (collections)
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationAndPeopleDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationWithoutPeopleDetails>(userEntity));
                }
                return NotFound();
            }
        }


        [HttpGet("ByEmail")]
        public async Task<ActionResult<UsersWithNavigationAndPeopleDetails>> GetUserByEmail(string email, bool collections = false)
        {

            var userEntity = await _assyChartService.FetchUserByEmailAsync(email);

            if (collections)
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithNavigationAndPeopleDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWithoutNavigationWithoutPeopleDetails>(userEntity));
                }
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<ActionResult<UsersWithNavigationAndPeopleDetails>> CreateUser(UsersForCreation newUser)
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

            newUser.LastUpdated = newUser.CreatedDate;
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
        public async Task<ActionResult> UpdateUser(int userId, UsersForUpdateDto user, int areaTypeUpdate)
        {

            List<Area> AreasInUser = new List<Area>();
            List<User> UsersInUser = new List<User>();
            List<User> UsersWithoutChanges = new List<User>();
            bool haveAreas = false;
            bool haveUsers = false;


            if (user.PlantId == 0)
            {
                user.PlantId = null;
            }
            else if (user.PlantId != null)
            {
                if (!await _supervisorMobilityRepository.PlantExistAsync((int)user.PlantId))
                {
                    return NotFound("No Planta");
                }
            }

            if (user.AreaId == 0)
            {
                user.AreaId = null;
            }
            else if (user.AreaId != null)
            {
                if (!await _supervisorMobilityRepository.AreaExistAsync((int)user.AreaId))
                {
                    return NotFound("No Area");
                }
            }

            if (user.GroupId == 0)
            {
                user.GroupId = null;
            }
            else if (user.GroupId != null)
            {
                if (!await _supervisorMobilityRepository.GroupExistAsync((int)user.GroupId))
                {
                    return NotFound("No Group");
                }
            }

            if (user.DistributionId == 0)
            {
                user.DistributionId = null;
            }
            else if (user.DistributionId != null)
            {
                if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)user.DistributionId))
                {
                    return NotFound("No Distribution");
                }
            }

            if (user.Payroll == 0)
            {
                user.Payroll = null;
            }

            if (user.SuperiorId == 0)
            {
                user.SuperiorId = null;
            }

            if (user.Subordinates != null && user.Subordinates?.Count > 0)
            {
                haveUsers = true;
                foreach (var Sub in user.Subordinates)
                {
                    var userInDB = await _assyChartService.FetchUserAsync(Sub.UserId);

                    if (userInDB.AreaId != Sub.AreaId || userInDB.SuperiorId != Sub.SuperiorId)
                    {
                        _mapper.Map(Sub, userInDB);
                        UsersInUser.Add(userInDB);
                    }
                    else
                    {
                        UsersWithoutChanges.Add(userInDB);
                    }

                }

                user.Subordinates.Clear();
            }

            if (user.Areas != null && user.Areas?.Count > 0)
            {
                haveAreas = true;
                foreach (var AreainList in user.Areas)
                {
                    AreasInUser.Add(await _supervisorMobilityRepository.GetAreaOnlyIdAsync(AreainList.AreaId));
                }
                user.Areas.Clear();
            }

            var userToCompare = _mapper.Map<User>(user);
            var entityentity = await _supervisorMobilityRepository.GetUserAsync(userId, true);



            if (!entityentity.Equals(userToCompare))
            {

                if (userToCompare.SuperiorId != entityentity.SuperiorId)
                {
                    if (userToCompare.SuperiorId != null && entityentity.SuperiorId != null)
                    {
                        User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityentity.SuperiorId, true);
                        var usertoRemove = _mapper.Map<User>(entityentity);

                        _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                        User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)user.SuperiorId, true);
                        _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, usertoRemove);

                        user.SuperiorId = actualSuperior.UserId;
                        user.PlantId = actualSuperior.PlantId;
                        user.GroupId = actualSuperior.GroupId;

                        if (user.UserType == 4)
                            user.AreaId = actualSuperior.AreaId;
                    }
                }else if (userToCompare.SuperiorId is null && entityentity.SuperiorId != null)
                {
                    User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityentity.SuperiorId, true);
                    var usertoRemove = _mapper.Map<User>(entityentity);

                    _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);
                }
            }

            user.CreatedDate = (DateTime)entityentity.CreatedDate;
            user.LastUpdated = DateTime.Now;
            await _assyChartService.UpdateUserAsync(user, userId);

          
            User? UserToReturn = await _assyChartService.FetchUserAsync(userId);



            if (haveUsers)
            {
                await _supervisorMobilityRepository.UserRemoveAllSubordinated(UserToReturn);
                UserToReturn.Subordinates = null;

                foreach (var elementUserInList in UsersInUser)
                {
                    var elementAux = _mapper.Map<UsersForUpdateDto>(elementUserInList);
                    switch (UserToReturn.UserType)
                    {
                        case 2:
                            //itero sobre SV
                            elementAux.SuperiorId = UserToReturn.UserId;
                            if(areaTypeUpdate != 4)
                            {
                            elementAux.GroupId = UserToReturn.GroupId;
                            elementAux.PlantId = UserToReturn.PlantId;

                            }
                            await _assyChartService.UpdateUserAsync(elementAux, elementUserInList.UserId);

                            await _supervisorMobilityRepository.UserUpdateAllSubordinated(elementUserInList);

                            break;
                        case 3:
                            //itero sobre OP
                            elementAux.SuperiorId = UserToReturn.UserId;
                            if (areaTypeUpdate != 4)
                            { 
                                elementAux.GroupId = UserToReturn.GroupId;
                                elementAux.PlantId = UserToReturn.PlantId;
                                elementAux.AreaId = UserToReturn.AreaId;
                            }
                            await _assyChartService.UpdateUserAsync(elementAux, elementUserInList.UserId);

                            await _supervisorMobilityRepository.UserUpdateAllSubordinated(elementUserInList);
                            break;
                        case 5:
                            //itero sobre SsV
                            elementAux.SuperiorId = UserToReturn.UserId;
                            if (areaTypeUpdate != 4)
                            {
                                elementAux.PlantId = UserToReturn?.PlantId;
                            }
                            await _assyChartService.UpdateUserAsync(elementAux, elementUserInList.UserId);

                            await _supervisorMobilityRepository.UserUpdateAllSubordinated(elementUserInList);
                            break;
                    }

                    _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, elementUserInList);
                }

                foreach (var userRestore in UsersWithoutChanges)
                {
                    _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, userRestore);
                }
            }

            if (haveAreas)
            {
                await _supervisorMobilityRepository.UserRemoveAllAreas(UserToReturn);
                UserToReturn.Areas = null;
                foreach (var elemntArea in AreasInUser)
                {
                    _supervisorMobilityRepository.UserAddArea(UserToReturn, elemntArea);
                }
            }



            return Ok();
        }

        [HttpPut("ReassingToNewSuperior")]
        public async Task<ActionResult> RessignNewSuperior(List<UsersWithoutNavigationWithoutPeopleDetails> users, int reasignType)
        {
            


            foreach (var elemntUser in users)
            {
                var SuperiorNewData = await _assyChartService.FetchUserAsync((int)elemntUser.SuperiorId);
               User EntityUser = await _supervisorMobilityRepository.GetUserAsync(elemntUser.UserId);

                if (EntityUser.SuperiorId != elemntUser.SuperiorId)
                {
                    User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)EntityUser.SuperiorId, true);
                    await _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, EntityUser);
                }

                var elementAux = _mapper.Map<UsersForUpdateDto>(elemntUser);

                switch (SuperiorNewData.UserType)
                {
                    case 2:
                        //Si el jefe es un SSV, estoy manejando un SV
                        elementAux.SuperiorId = SuperiorNewData.UserId;

                        if(reasignType != 2)
                        {
                            elementAux.GroupId = SuperiorNewData.GroupId;
                            elementAux.PlantId = SuperiorNewData.PlantId;
                        }
                       

                        //Actualiza info de operadores
                        if(EntityUser.Subordinates?.Count > 0)
                        {
                            foreach (var SubInuser in EntityUser.Subordinates)
                            {
                                var SubSubEntity = await _supervisorMobilityRepository.GetUserAsync(SubInuser.UserId);
                                var SubSubUpdate = _mapper.Map<UsersForUpdateDto>(elemntUser);
                                SubSubUpdate.AreaId = elemntUser.AreaId;

                                await _assyChartService.UpdateUserAsync(SubSubUpdate, SubSubEntity.UserId);
                            }
                        }

                      
                        break;
                    case 3:
                        //Si el jefe es un SV, estoy manejando un operador
                        elementAux.SuperiorId = SuperiorNewData.UserId;
                        if (reasignType != 2)
                        {
                            elementAux.GroupId = SuperiorNewData.GroupId;
                            elementAux.PlantId = SuperiorNewData.PlantId;
                            elementAux.AreaId = SuperiorNewData.AreaId;
                        }
                        break;
                }

                await _assyChartService.UpdateUserAsync(elementAux, EntityUser.UserId);
                await _supervisorMobilityRepository.UserAddSubordinated(SuperiorNewData, EntityUser);

            }



            return Ok();
        }

        [HttpPut("Cleanuser")]
        public async Task<ActionResult> CleanUsersSupervisor(List<UsersWithoutNavigationWithoutPeopleDetails> Users)
        {
            foreach (UsersWithoutNavigationWithoutPeopleDetails UserInList in Users)
            {
                User usertoRemove = await _supervisorMobilityRepository.GetUserAsync(UserInList.UserId);

                User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)usertoRemove.SuperiorId, true);

                _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);
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


        //******* Upload users    **********//
        //[EnableCors("Cors")]

        [HttpPost("MasiveUpload")]
        public async Task<ActionResult<UploadUsersResult>> MassiveUpload(List<UsersWithNavigationAndPeopleDetails> UsersToCreate)
        {

            UploadUsersResult ResultToReturn = new UploadUsersResult();
            foreach (var ItemInUserList in UsersToCreate)
            {
                //Hci del usuario
                bool HCIStatus = false;
                int userId = 0;
                List<Area> AreasInUser = new List<Area>();
                List<User> UsersInUser = new List<User>();
                bool haveAreas = false;
                bool haveUsers = false;

                var UserToReturn = new User();

                if (ItemInUserList.PlantId == 0 || ItemInUserList.PlantId == -1)
                {
                    ItemInUserList.PlantId = null;
                }
                else if (ItemInUserList.PlantId != null)
                {
                    if (!await _supervisorMobilityRepository.PlantExistAsync((int)ItemInUserList.PlantId))
                    {
                        return NotFound("No Planta");
                    }
                }

                if (ItemInUserList.AreaId == 0 || ItemInUserList.AreaId == -1)
                {
                    ItemInUserList.AreaId = null;
                }
                else if (ItemInUserList.AreaId != null)
                {
                    if (!await _supervisorMobilityRepository.AreaExistAsync((int)ItemInUserList.AreaId))
                    {
                        return NotFound("No Area");
                    }
                }

                if (ItemInUserList.GroupId == 0 || ItemInUserList.GroupId == -1)
                {
                    ItemInUserList.GroupId = null;
                }
                else if (ItemInUserList.GroupId != null)
                {
                    if (!await _supervisorMobilityRepository.GroupExistAsync((int)ItemInUserList.GroupId))
                    {
                        return NotFound("No Group");
                    }
                }

                if (ItemInUserList.DistributionId == 0 || ItemInUserList.DistributionId == -1)
                {
                    ItemInUserList.DistributionId = null;
                }
                else if (ItemInUserList.DistributionId != null)
                {
                    if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)ItemInUserList.DistributionId))
                    {
                        return NotFound("No Distribution");
                    }
                }

                if (ItemInUserList.Payroll == 0 || ItemInUserList.Payroll == -1)
                {
                    ItemInUserList.Payroll = null;
                }

                if (ItemInUserList.SuperiorId == 0 || ItemInUserList.SuperiorId == -1)
                {
                    ItemInUserList.SuperiorId = null;
                }


                if (ItemInUserList.Subordinates != null)
                {
                    haveUsers = true;
                    foreach (var Sub in ItemInUserList.Subordinates)
                    {
                        UsersInUser.Add(await _assyChartService.FetchUserAsync(Sub.UserId));
                    }

                    ItemInUserList.Subordinates = null;
                }

                if (ItemInUserList.Areas != null)
                {
                    haveAreas = true;
                    foreach (var AreainList in ItemInUserList.Areas)
                    {
                        AreasInUser.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)ItemInUserList.PlantId, AreainList.AreaId));
                    }
                    ItemInUserList.Areas = null;
                }

                
                if (ItemInUserList.UserId == -1)
                {
                    bool existUser = false;
                    int typeUser = 0;
                    if (ItemInUserList.Payroll != null)
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)ItemInUserList.Payroll);
                        typeUser = existUser ? 1 : 0;
                    }

                    if (ItemInUserList.Email != "")
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByEmailAsync(ItemInUserList.Email);
                        typeUser = existUser ? 2 : 0;
                    }

                    if (existUser)
                    {
                        var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)ItemInUserList.Payroll) : await _supervisorMobilityRepository.GetUserByEmailAsync(ItemInUserList.Email);

                        if (entityentity == null)
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(ItemInUserList);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;

                            UserToReturn = _mapper.Map<User>(finalUser);

                        }
                        else
                        {
                            var InComingUserToCompare = _mapper.Map<User>(ItemInUserList);

                            if (!entityentity.Equals(InComingUserToCompare))
                            {

                                if (InComingUserToCompare.SuperiorId != entityentity.SuperiorId)
                                {
                                    if (InComingUserToCompare.SuperiorId != null && entityentity.SuperiorId != null)
                                    {
                                        User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityentity.SuperiorId, true);
                                        var usertoRemove = _mapper.Map<User>(entityentity);

                                        _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                                        User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)ItemInUserList.SuperiorId, true);
                                        _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, usertoRemove);

                                        ItemInUserList.SuperiorId = actualSuperior.UserId;
                                        ItemInUserList.PlantId = actualSuperior.PlantId;
                                        ItemInUserList.GroupId = actualSuperior.GroupId;

                                        if (ItemInUserList.UserType == 4)
                                            ItemInUserList.AreaId = actualSuperior.AreaId;

                                        InComingUserToCompare = _mapper.Map<User>(ItemInUserList);
                                    }
                                }
                                var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                userToUpdate.CreatedDate = (DateTime)entityentity.CreatedDate;
                                await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityentity.UserId);
                                UserToReturn = _mapper.Map<User>(userToUpdate);
                                ResultToReturn.UsersUpdated++;
                            }
                            else
                            {
                                UserToReturn = _mapper.Map<User>(entityentity);
                                ResultToReturn.UsersExist++;
                            }

                            HCIStatus = await _supervisorMobilityRepository.SearchExistHciForUserId(UserToReturn.UserId);
                            userId = UserToReturn.UserId;
                        }
                    }
                    else
                    {
                        var usertoCreate = _mapper.Map<UsersForCreation>(ItemInUserList);
                        var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                        UserToReturn = _mapper.Map<User>(finalUser);
                    }

                }
                else
                {
                    //User con id
                    var UserInDb = await _assyChartService.FetchUserAsync((int)ItemInUserList.UserId);

                    if (UserInDb == null)
                    {
                        //Si tiene un id erroneo, entra aqui para busqueda avanzada
                        bool existUser = false;
                        int typeUser = 0;
                        if (ItemInUserList.Payroll != null)
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)ItemInUserList.Payroll);
                            typeUser = existUser ? 1 : 0;
                        }

                        if (ItemInUserList.ObjectId != "")
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByObjectIdAsync(ItemInUserList.ObjectId);
                            typeUser = existUser ? 2 : 0;
                        }

                        if (existUser)
                        {
                            //el usuario existe 
                            var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)ItemInUserList.Payroll) : await _supervisorMobilityRepository.GetUserByObjectIdAsync(ItemInUserList.ObjectId);
                            //si fue un operador lo traigo mediante su payroll, si es es de otro tipo lo traigo mediante objectid

                            if (entityentity == null)
                            {
                                //usuario no existe, se crea
                                var usertoCreate = _mapper.Map<UsersForCreation>(ItemInUserList);
                                var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                                if (finalUser != null)
                                    ResultToReturn.UsersCreated++;
                                UserToReturn = _mapper.Map<User>(finalUser);
                            }
                            else
                            {
                                UserInDb = entityentity;
                                var InComingUserToCompare = _mapper.Map<User>(ItemInUserList);
                                if (!UserInDb.Equals(InComingUserToCompare))
                                {
                                    //El Usuario es diferente al de la base de datos
                                    if (InComingUserToCompare.SuperiorId != UserInDb.SuperiorId)
                                    {
                                        //Superior es diferente
                                        if (InComingUserToCompare.SuperiorId != null && UserInDb.SuperiorId == null)
                                        {
                                            //Verifico que los SuperiorId sean validos
                                            //Me aseguro de que eliminar al usuario como subordinado del anterior SUPERIRO y lo reasigno al nuevo superior
                                            User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)UserInDb.SuperiorId, true);

                                            _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, UserInDb);

                                            User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)ItemInUserList.SuperiorId, true);

                                            InComingUserToCompare.SuperiorId = actualSuperior.UserId;
                                            InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                            InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                            if (ItemInUserList.UserType == 4)
                                                InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                            _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, UserInDb);

                                            var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                            userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                            await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);
                                            UserToReturn = _mapper.Map<User>(userToUpdate);

                                            ResultToReturn.UsersUpdated++;
                                        }
                                        else if (InComingUserToCompare.SuperiorId != null && UserInDb.SuperiorId == null)
                                        {
                                            //Usuario sin Superior, asigno uno por primera ves
                                            User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)ItemInUserList.SuperiorId, true);

                                            InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                            InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                            if (ItemInUserList.UserType == 4)
                                                InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                            _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, UserInDb);


                                            var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                            userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                            await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);
                                            UserToReturn = _mapper.Map<User>(userToUpdate);

                                            ResultToReturn.UsersUpdated++;
                                        }
                                        else if (InComingUserToCompare.SuperiorId == null && UserInDb.SuperiorId != null)
                                        {
                                            //Elimina el supervisor y ya
                                            User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)UserInDb.SuperiorId, true);

                                            _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, UserInDb);
                                        }
                                    }
                                    else
                                    {
                                        if (InComingUserToCompare.SuperiorId == null)
                                        {
                                            ResultToReturn.UsersExist++; // No hay un SuperiorId, no se actualiza
                                            UserToReturn = _mapper.Map<User>(UserInDb);
                                        }
                                        else
                                        {
                                            User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)InComingUserToCompare.SuperiorId, true);
                                            if (InComingUserToCompare.PlantId != actualSuperior.PlantId ||
                                                InComingUserToCompare.AreaId != actualSuperior.AreaId ||
                                                InComingUserToCompare.GroupId != actualSuperior.GroupId)
                                            {
                                                InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                                InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                                if (InComingUserToCompare.UserType == 4)
                                                    InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                                var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                                userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                                await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);

                                                ResultToReturn.UsersUpdated++;
                                                UserToReturn = _mapper.Map<User>(userToUpdate);
                                            }
                                            else
                                            {
                                                ResultToReturn.UsersExist++;
                                                UserToReturn = _mapper.Map<User>(UserInDb);
                                            }
                                        }


                                    }

                                }
                                else
                                {
                                    //el usuario es el mismo en la base de datos, pero la comparacion no valida que siga siendo la misma informacion del superior y el usuario

                                    User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)UserInDb.SuperiorId, true);

                                    if (InComingUserToCompare.PlantId != actualSuperior.PlantId || InComingUserToCompare.AreaId != actualSuperior.AreaId || InComingUserToCompare.GroupId != actualSuperior.GroupId)
                                    {
                                        InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                        InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                        if (InComingUserToCompare.UserType == 4)
                                            InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                        var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                        userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                        await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);

                                        ResultToReturn.UsersUpdated++;
                                        UserToReturn = _mapper.Map<User>(userToUpdate);
                                    }
                                    else
                                    {
                                        ResultToReturn.UsersExist++;
                                        UserToReturn = _mapper.Map<User>(UserInDb);
                                    }
                                }//end else son iguales
                            }//end else usuario si existia
                            
                            HCIStatus = await _supervisorMobilityRepository.SearchExistHciForUserId(UserToReturn.UserId);
                            userId = UserToReturn.UserId;
                        }
                        else
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(ItemInUserList);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;
                            UserToReturn = _mapper.Map<User>(finalUser);
                        }
                    }
                    else
                    {
                        var InComingUserToCompare = _mapper.Map<User>(ItemInUserList);
                        if (!UserInDb.Equals(InComingUserToCompare))
                        {
                            //El Usuario es diferente al de la base de datos
                            if (InComingUserToCompare.SuperiorId != UserInDb.SuperiorId)
                            {
                                //Superior es diferente
                                if (InComingUserToCompare.SuperiorId != null && UserInDb.SuperiorId == null)
                                {
                                    //Verifico que los SuperiorId sean validos
                                    //Me aseguro de que eliminar al usuario como subordinado del anterior SUPERIRO y lo reasigno al nuevo superior
                                    if (UserInDb.SuperiorId != null)
                                    {
                                        User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)UserInDb.SuperiorId, true);

                                        // Remover subordinado solo si hay un superior existente
                                        _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, UserInDb);
                                    }

                                    User? actualSuperior = null;
                                    if (ItemInUserList.SuperiorId != null)
                                    {
                                        actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)ItemInUserList.SuperiorId, true);

                                        // Actualizar los datos solo si existe un superior
                                        InComingUserToCompare.SuperiorId = actualSuperior.UserId;
                                        InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                        InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                        if (ItemInUserList.UserType == 4)
                                            InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                        // Agregar subordinado al nuevo superior
                                        _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, UserInDb);
                                    }

                                    // Actualizar al usuario independientemente de si hay un superior o no
                                    var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                    userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;

                                    await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);
                                    UserToReturn = _mapper.Map<User>(userToUpdate);

                                    ResultToReturn.UsersUpdated++;

                                }
                                else if (InComingUserToCompare.SuperiorId != null && UserInDb.SuperiorId == null)
                                {
                                    //Usuario sin Superior, asigno uno por primera ves
                                    User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)ItemInUserList.SuperiorId, true);

                                    InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                    InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                    if (ItemInUserList.UserType == 4)
                                        InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                    _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, UserInDb);


                                    var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                    userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                    await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);
                                    UserToReturn = _mapper.Map<User>(userToUpdate);

                                    ResultToReturn.UsersUpdated++;
                                }
                                else if (InComingUserToCompare.SuperiorId == null && UserInDb.SuperiorId != null)
                                {
                                    //Elimina el supervisor y ya
                                    User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)UserInDb.SuperiorId, true);

                                    _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, UserInDb);
                                }
                            }
                            else
                            {
                                if (InComingUserToCompare.SuperiorId == null)
                                {
                                    ResultToReturn.UsersExist++; // No hay un SuperiorId, no se actualiza
                                    UserToReturn = _mapper.Map<User>(UserInDb);
                                }
                                else
                                {
                                    User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)InComingUserToCompare.SuperiorId, true);
                                    if (InComingUserToCompare.PlantId != actualSuperior.PlantId ||
                                        InComingUserToCompare.AreaId != actualSuperior.AreaId ||
                                        InComingUserToCompare.GroupId != actualSuperior.GroupId)
                                    {
                                        InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                        InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                        if (InComingUserToCompare.UserType == 4)
                                            InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                        var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                        userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                        await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);

                                        ResultToReturn.UsersUpdated++;
                                        UserToReturn = _mapper.Map<User>(userToUpdate);
                                    }
                                    else
                                    {
                                        ResultToReturn.UsersExist++;
                                        UserToReturn = _mapper.Map<User>(UserInDb);
                                    }
                                }
                            }

                        }
                        else
                        {
                            //el usuario es el mismo en la base de datos, pero la comparacion no valida que siga siendo la misma informacion del superior y el usuario

                            if (InComingUserToCompare.SuperiorId == null)
                            {
                                ResultToReturn.UsersExist++; // No hay un SuperiorId, no se actualiza
                                UserToReturn = _mapper.Map<User>(UserInDb);
                            }
                            else
                            {
                                User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)InComingUserToCompare.SuperiorId, true);
                                if (InComingUserToCompare.PlantId != actualSuperior.PlantId ||
                                    InComingUserToCompare.AreaId != actualSuperior.AreaId ||
                                    InComingUserToCompare.GroupId != actualSuperior.GroupId)
                                {
                                    InComingUserToCompare.PlantId = actualSuperior.PlantId;
                                    InComingUserToCompare.GroupId = actualSuperior.GroupId;

                                    if (InComingUserToCompare.UserType == 4)
                                        InComingUserToCompare.AreaId = actualSuperior.AreaId;

                                    var userToUpdate = _mapper.Map<UsersForUpdateDto>(InComingUserToCompare);
                                    userToUpdate.CreatedDate = (DateTime)UserInDb.CreatedDate;
                                    await _supervisorMobilityRepository.UpdateUser(userToUpdate, UserInDb.UserId);

                                    ResultToReturn.UsersUpdated++;
                                    UserToReturn = _mapper.Map<User>(userToUpdate);
                                }
                                else
                                {
                                    ResultToReturn.UsersExist++;
                                    UserToReturn = _mapper.Map<User>(UserInDb);
                                }
                            }

                        }//end else son iguales

                    }//end else existe en base de datos


                }//end else tiene id

                if (haveUsers)
                {
                    foreach (var elemntUser in UsersInUser)
                    {
                        _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, elemntUser);
                    }
                }

                if (haveAreas)
                {
                    await _supervisorMobilityRepository.RemoveAllAreasFromUser(UserToReturn);
                    foreach (var elemntArea in AreasInUser)
                    {
                        _supervisorMobilityRepository.UserAddArea(UserToReturn, elemntArea);
                    }
                }

                    if (!HCIStatus && userId != 0)
                    {
                        HCI hciEntity = new();

                        hciEntity.User = await _supervisorMobilityRepository.GetUserAsync(userId);

                        var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);
                    }

            }

            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(ResultToReturn);

        }

        //[EnableCors("Cors")]
        [HttpPost("MasiveUpload/Superior/{superiorId}")]
        public async Task<ActionResult<UploadUsersResult>> MassiveUsersToSuperior(List<UsersWithNavigationAndPeopleDetails> UsesToCreateInSuperior, int superiorId)
        {


            User MasterUser = await _supervisorMobilityRepository.GetUserAsync(superiorId, true);
            UploadUsersResult ResultToReturn = new UploadUsersResult();
            foreach (var item in UsesToCreateInSuperior)
            {
                //Hci del usuario
                bool HCIStatus = false;
                int userId = 0;

                if (item.SuperiorId != null)
                    if (item.SuperiorId != superiorId)
                    {
                        User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)item.SuperiorId, true);
                        var usertoRemove = _mapper.Map<User>(item);

                        _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                        item.SuperiorId = superiorId;
                        item.PlantId = MasterUser.PlantId;
                        item.GroupId = MasterUser.GroupId;

                        if (item.UserType == 4)
                            item.AreaId = MasterUser.AreaId;

                    }


                List<Area> AreasInUser = new List<Area>();
                List<User> UsersInUser = new List<User>();
                bool haveAreas = false;
                bool haveUsers = false;

                var UserToReturn = new User();

                if (item.PlantId == 0 || item.PlantId == -1)
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

                if (item.AreaId == 0 || item.AreaId == -1)
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

                if (item.GroupId == 0 || item.GroupId == -1)
                {
                    item.GroupId = null;
                }
                else if (item.GroupId != null)
                {
                    if (!await _supervisorMobilityRepository.GroupExistAsync((int)item.GroupId))
                    {
                        return NotFound("No Group");
                    }
                }

                if (item.DistributionId == 0 || item.DistributionId == -1)
                {
                    item.DistributionId = null;
                }
                else if (item.DistributionId != null)
                {
                    if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)item.DistributionId))
                    {
                        return NotFound("No Distribution");
                    }
                }

                if (item.Payroll == 0 || item.Payroll == -1)
                {
                    item.Payroll = null;
                }

                if (item.SuperiorId == 0 || item.SuperiorId == -1)
                {
                    item.SuperiorId = null;
                }



                if (item.Subordinates != null)
                {
                    haveUsers = true;
                    foreach (var Sub in item.Subordinates)
                    {
                        UsersInUser.Add(await _assyChartService.FetchUserAsync(Sub.UserId));
                    }

                    item.Subordinates = null;
                }

                if (item.Areas != null)
                {
                    haveAreas = true;
                    foreach (var AreainList in item.Areas)
                    {
                        AreasInUser.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)item.PlantId, AreainList.AreaId));
                    }
                    item.Areas = null;
                }

                ///////////////////
                if (item.UserId == -1)
                {
                    bool existUser = false;
                    int typeUser = 0;
                    if (item.Payroll != null)
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)item.Payroll);
                        typeUser = existUser ? 1 : 0;
                    }

                    if (item.Email != "")
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByEmailAsync(item.Email);
                        typeUser = existUser ? 2 : 0;
                    }

                    if (existUser)
                    {
                        var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)item.Payroll) : await _supervisorMobilityRepository.GetUserByEmailAsync(item.Email);

                        if (entityentity == null)
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(item);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;

                            UserToReturn = _mapper.Map<User>(finalUser);

                        }
                        else
                        {
                            var userToCompare = _mapper.Map<User>(item);

                            if (!entityentity.Equals(userToCompare))
                            {

                                var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);
                                userToUpdate.CreatedDate = (DateTime)entityentity.CreatedDate;

                                await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityentity.UserId);
                                UserToReturn = _mapper.Map<User>(userToUpdate);
                                ResultToReturn.UsersUpdated++;
                            }
                            else
                            {
                                UserToReturn = _mapper.Map<User>(entityentity);
                                ResultToReturn.UsersExist++;
                            }


                        }

                       
                    }
                    else
                    {
                        var usertoCreate = _mapper.Map<UsersForCreation>(item);
                        var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                        UserToReturn = _mapper.Map<User>(finalUser);


                    }
                    HCIStatus = await _supervisorMobilityRepository.SearchExistHciForUserId(UserToReturn.UserId);
                    userId = UserToReturn.UserId;
                }
                else
                {
                    //User con id
                    var entityUserwhitId = await _assyChartService.FetchUserAsync((int)item.UserId);

                    if (entityUserwhitId == null)
                    {
                        //Si tiene un id erroneo, entra aqui para busqueda avanzada
                        bool existUser = false;
                        int typeUser = 0;
                        if (item.Payroll != null)
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)item.Payroll);
                            typeUser = existUser ? 1 : 0;
                        }

                        if (item.Email != "")
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByEmailAsync(item.Email);
                            typeUser = existUser ? 2 : 0;
                        }

                        if (existUser)
                        {
                            var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)item.Payroll) : await _supervisorMobilityRepository.GetUserByEmailAsync(item.Email);

                            if (entityentity == null)
                            {
                                var usertoCreate = _mapper.Map<UsersForCreation>(item);
                                var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                                if (finalUser != null)
                                    ResultToReturn.UsersCreated++;
                                UserToReturn = _mapper.Map<User>(finalUser);

                            }
                            else
                            {
                                var userToCompare = _mapper.Map<User>(item);

                                if (!entityentity.Equals(userToCompare))
                                {

                                    var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);

                                    userToUpdate.CreatedDate = (DateTime)entityentity.CreatedDate;

                                    await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityentity.UserId);
                                    UserToReturn = _mapper.Map<User>(userToUpdate);
                                    ResultToReturn.UsersUpdated++;
                                }
                                else
                                {

                                    UserToReturn = _mapper.Map<User>(entityentity);
                                    ResultToReturn.UsersExist++;
                                }

                            }
                        }
                        else
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(item);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;
                            UserToReturn = _mapper.Map<User>(finalUser);

                        }
                    }
                    else
                    {
                        var userToCompare = _mapper.Map<User>(item);
                        if (!entityUserwhitId.Equals(userToCompare))
                        {

                            var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);
                            userToUpdate.CreatedDate = (DateTime)entityUserwhitId.CreatedDate;

                            await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityUserwhitId.UserId);
                            UserToReturn = _mapper.Map<User>(entityUserwhitId);

                            ResultToReturn.UsersUpdated++;
                        }
                        else
                        {
                            ResultToReturn.UsersExist++;
                            UserToReturn = _mapper.Map<User>(entityUserwhitId);
                        }

                    }
                    HCIStatus = await _supervisorMobilityRepository.SearchExistHciForUserId(UserToReturn.UserId);
                    userId = UserToReturn.UserId;
                }

                if (haveUsers)
                {
                    foreach (var elemntUser in UsersInUser)
                    {
                        _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, elemntUser);
                    }
                }

                if (haveAreas)
                {
                    foreach (var elemntArea in AreasInUser)
                    {
                        _supervisorMobilityRepository.UserAddArea(UserToReturn, elemntArea);
                    }
                }

                if (!HCIStatus && userId != 0)
                {
                    HCI hciEntity = new();

                    hciEntity.User = await _supervisorMobilityRepository.GetUserAsync(userId);

                    var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);
                }

                _supervisorMobilityRepository.UserAddSubordinated(MasterUser, UserToReturn);
            }

            return Ok(ResultToReturn);
        }

        string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null || cell.CellValue == null) return null;

            string value = cell.CellValue.Text;

            // Manejar celdas con SharedStringTable (para celdas con texto)
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
            }

            return value; // Para valores numéricos o fechas
        }

        //[EnableCors("Cors")]
        [HttpPost("FileUpload/Data")]
        public async Task<ActionResult<UploadUsersResult>> ApplyUsersUpload(FileUploadGeneralDto FileToInsert)
        {
            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\users\\" + FileToInsert.StorageFileName;
            string originalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + System.IO.Path.GetExtension(file));



            IEnumerable<User> _allUsers = new List<User>();

            IEnumerable<Plant> _plants = new List<Plant>();
            List<IEnumerable<Area>> _areas = new List<IEnumerable<Area>>();
            Dictionary<int, Dictionary<int, IEnumerable<Distribution>>> _distributions = new Dictionary<int, Dictionary<int, IEnumerable<Distribution>>>();
            IEnumerable<Entities.Group> _groups = new List<Entities.Group>();


            _plants = await _supervisorMobilityRepository.GetPlantsAsync();

            foreach (var plant in _plants)
            {
                var areas = await _supervisorMobilityRepository.GetAreasForPlantAsync(plant.PlantId);
                _areas.Add(areas);
                var areaDistributions = new Dictionary<int, IEnumerable<Distribution>>();
                foreach (var area in areas)
                {
                    var distributions = await _supervisorMobilityRepository.GetDistributionsForAreaAsync(area.AreaId);
                    areaDistributions.Add(area.AreaId, distributions);
                }
                _distributions.Add(plant.PlantId, areaDistributions);
            }


            _groups = await _supervisorMobilityRepository.GetGroupsAsync();

            _allUsers = await _supervisorMobilityRepository.GetAllUsersAsync();


            List<string[]> DataInFile = new List<string[]>();
            List<User> ListOfUsers = new List<User>();


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

                    List<string> RowsInFile = new List<string>();

                    using (SpreadsheetDocument document = SpreadsheetDocument.Open(file, false))
                    {
                        // Obtener la primera hoja de trabajo
                        WorkbookPart workbookPart = document.WorkbookPart;
                        Sheet sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().FirstOrDefault();
                        if (sheet != null)
                        {
                            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                            if (sheetData != null)
                            {
                                foreach (var row in sheetData.Elements<Row>())
                                {
                                   
                                    foreach (var cell in row.Elements<Cell>().Take(13)) // Leer hasta la columna 13
                                    {
                                        string cellValue = GetCellValue(workbookPart, cell);
                                        RowsInFile.Add(string.IsNullOrEmpty(cellValue) ? "§" : cellValue);
                                    }

                                    DataInFile.Add(RowsInFile.ToArray());
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                //remove titles 
                DataInFile.RemoveAt(0);

                try
                {
                    foreach (string[] row in DataInFile)
                    {

                        bool allEqual = row.All(item => item.Equals("§"));

                        if (allEqual)
                        {
                            // Todos los elementos son iguales a "§"
                            Console.WriteLine("Todos los elementos son iguales a '§'.");
                            break;
                        }

                        try
                        {
                            var ToInsertIntoList = new User();

                            try
                            {
                                ToInsertIntoList.UserType = row[5] != "§" ? int.Parse(row[5]) : -1;


                            }
                            catch (Exception ex)
                            {
                                Console.Write($"{ex.Message}");
                            }


                            switch (ToInsertIntoList.UserType)
                            {
                                case 1:
                                    try
                                    {
                                        ToInsertIntoList.UserId = row[0] != "§" ? int.Parse(row[0]) : -1;

                                        try
                                        {
                                            ToInsertIntoList.ObjectId = row[1] != "§" ? row[2] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.Name = row[3] != "§" ? row[3] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.Email = row[4] != "§" ? row[4] : row[403];

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                        break;
                                    }
                                    break;
                                case 2:
                                    try
                                    {
                                        ToInsertIntoList.UserId = row[0] != "§" ? int.Parse(row[0]) : -1;

                                        try
                                        {
                                            ToInsertIntoList.ObjectId = row[1] != "§" ? row[1] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                            break;
                                        }

                                        try
                                        {
                                            ToInsertIntoList.Name = row[3] != "§" ? row[3] : row[403];
                                        }
                                        catch (Exception ex)
                                        {


                                            break;
                                        }

                                        try
                                        {
                                            ToInsertIntoList.Email = row[4] != "§" ? row[4] : row[403];

                                        }
                                        catch (Exception ex)
                                        {


                                            break;
                                        }

                                        try
                                        {
                                            if (row[7].Contains(','))
                                            {
                                                string[]? SplitedSubordinates = row[7] != "§" ? row[7].Split(',') : null;

                                                if (SplitedSubordinates != null)
                                                {
                                                    if (ToInsertIntoList.Subordinates != null)
                                                    {
                                                        foreach (var item in SplitedSubordinates)
                                                        {
                                                            ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(item)));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Subordinates = new List<User>();
                                                        foreach (var item in SplitedSubordinates)
                                                        {
                                                            ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(item)));
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (row[7] != "§")
                                                {
                                                    if (ToInsertIntoList.Subordinates != null)
                                                    {

                                                        ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(row[7])));

                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Subordinates = new List<User>();
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(row[7])));
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                            break;
                                        }


                                        try
                                        {
                                            ToInsertIntoList.PlantId = row[8] != "§" ? int.Parse(row[8]) : int.Parse(row[403]);

                                            try
                                            {
                                                ToInsertIntoList.Plant = _plants.ToList().Find(p => p.PlantId == ToInsertIntoList.PlantId);
                                            }
                                            catch (Exception ex)
                                            {

                                                break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {

                                            break;
                                        }

                                        //subordinados

                                        try
                                        {
                                            if (row[9].Contains(','))
                                            {
                                                string[]? SplitedAreas = row[9] != "§" ? row[9].Split(',') : null;

                                                if (SplitedAreas != null)
                                                {
                                                    if (ToInsertIntoList.Areas != null)
                                                    {
                                                        foreach (var item in SplitedAreas)
                                                        {
                                                            ToInsertIntoList.Areas.Add(_areas[_plants.ToList().FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].ToList().Find(a => a.AreaId == int.Parse(item)));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Areas = new List<Area>();
                                                        foreach (var item in SplitedAreas)
                                                        {
                                                            ToInsertIntoList.Areas.Add(_areas[_plants.ToList().FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].ToList().Find(a => a.AreaId == int.Parse(item)));
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (row[9] != "§")
                                                {
                                                    if (ToInsertIntoList.Areas != null)
                                                    {

                                                        ToInsertIntoList.Areas.Add(_areas[_plants.ToList().FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].ToList().Find(a => a.AreaId == int.Parse(row[9])));

                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Areas = new List<Area>();
                                                        ToInsertIntoList.Areas.Add(_areas[_plants.ToList().FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].ToList().Find(a => a.AreaId == int.Parse(row[9])));

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {


                                            break;
                                        }

                                        try
                                        {
                                            ToInsertIntoList.GroupId = row[10] != "§" ? int.Parse(row[10]) : int.Parse(row[403]);

                                            try
                                            {
                                                ToInsertIntoList.Group = _groups.ToList().Find(p => p.GroupId == ToInsertIntoList.GroupId);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    break;
                                case 3:
                                    try
                                    {
                                        ToInsertIntoList.UserId = row[0] != "§" ? int.Parse(row[0]) : -1;

                                        try
                                        {
                                            ToInsertIntoList.ObjectId = row[1] != "§" ? row[1] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.Name = row[3] != "§" ? row[3] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.Email = row[4] != "§" ? row[4] : row[403];

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.SuperiorId = row[6] != "§" ? int.Parse(row[6]) : int.Parse(row[403]);

                                            try
                                            {
                                                ToInsertIntoList.Superior = _allUsers.ToList().Find(p => p.UserId == ToInsertIntoList.SuperiorId);

                                                try
                                                {
                                                    ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                                                    ToInsertIntoList.Plant = ToInsertIntoList.Superior?.Plant;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    ToInsertIntoList.Group = ToInsertIntoList.Superior?.Group;
                                                    ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            if (row[7].Contains(','))
                                            {
                                                string[]? SplitedSubordinates = row[7] != "§" ? row[7].Split(',') : null;

                                                if (SplitedSubordinates != null)
                                                {
                                                    if (ToInsertIntoList.Subordinates != null)
                                                    {
                                                        foreach (var item in SplitedSubordinates)
                                                        {
                                                            ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(item)));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Subordinates = new List<User>();
                                                        foreach (var item in SplitedSubordinates)
                                                        {
                                                            ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(item)));
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (row[7] != "§")
                                                {
                                                    if (ToInsertIntoList.Subordinates != null)
                                                    {

                                                        ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(row[7])));

                                                    }
                                                    else
                                                    {
                                                        ToInsertIntoList.Subordinates = new List<User>();
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.ToList().Find(u => u.UserId == int.Parse(row[7])));
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {


                                        }

                                        try
                                        {
                                            ToInsertIntoList.AreaId = row[9] != "§" ? int.Parse(row[9]) : int.Parse(row[403]);
                                            try
                                            {
                                                ToInsertIntoList.Area = ToInsertIntoList.Superior?.Areas.ToList().Find(a => a.AreaId == ToInsertIntoList.AreaId);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    break;
                                case 4:
                                    try
                                    {
                                        ToInsertIntoList.UserId = row[0] != "§" ? int.Parse(row[0]) : -1;

                                        try
                                        {
                                            ToInsertIntoList.Payroll = row[2] != "§" ? int.Parse(row[2]) : int.Parse(row[403]);
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.Name = row[3] != "§" ? row[3] : row[403];
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.SuperiorId = row[6] != "§" ? int.Parse(row[6]) : int.Parse(row[403]);
                                            try
                                            {
                                                ToInsertIntoList.Superior = _allUsers.ToList().Find(p => p.UserId == ToInsertIntoList.SuperiorId);

                                                try
                                                {
                                                    ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                                                    ToInsertIntoList.Plant = ToInsertIntoList.Superior?.Plant;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    ToInsertIntoList.AreaId = ToInsertIntoList.Superior?.AreaId;
                                                    ToInsertIntoList.Area = ToInsertIntoList.Superior?.Area;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    ToInsertIntoList.Group = ToInsertIntoList.Superior?.Group;
                                                    ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ToInsertIntoList.DistributionId = row[11] != "§" ? int.Parse(row[11]) : int.Parse(row[403]);

                                            try
                                            {
                                                ToInsertIntoList.Distribution = _distributions[(int)ToInsertIntoList.PlantId][(int)ToInsertIntoList.AreaId].ToList().Find(d => d.DistributionId == ToInsertIntoList.DistributionId);
                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Case 4 => Create All Users:{ex.Message}");

                                    }
                                    break;
                            }



                            ListOfUsers.Add(ToInsertIntoList);
                        }
                        catch (Exception ex)
                        {


                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

            UploadUsersResult ResultToReturn = new UploadUsersResult();
            foreach (var item in ListOfUsers)
            {
                List<Area> AreasInUser = new List<Area>();
                List<User> UsersInUser = new List<User>();
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
                        return NotFound("No Group");
                    }
                }

                if (item.DistributionId == 0)
                {
                    item.DistributionId = null;
                }
                else if (item.DistributionId != null)
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
                        UsersInUser.Add(await _assyChartService.FetchUserAsync(Sub.UserId));
                    }

                    item.Subordinates = null;
                }

                if (item.Areas != null)
                {
                    haveAreas = true;
                    foreach (var AreainList in item.Areas)
                    {
                        AreasInUser.Add(await _supervisorMobilityRepository.GetAreaForPlantAsync((int)item.PlantId, AreainList.AreaId));
                    }
                    item.Areas = null;
                }

                ///////////////////
                if (item.UserId == -1)
                {
                    bool existUser = false;
                    int typeUser = 0;
                    if (item.Payroll != null)
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)item.Payroll);
                        typeUser = existUser ? 1 : 0;
                    }

                    if (item.ObjectId != "")
                    {
                        existUser = await _supervisorMobilityRepository.UserExistByObjectIdAsync(item.ObjectId);
                        typeUser = existUser ? 2 : 0;
                    }

                    if (existUser)
                    {
                        var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)item.Payroll) : await _supervisorMobilityRepository.GetUserByEmailAsync(item.Email);

                        if (entityentity == null)
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(item);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;

                            UserToReturn = _mapper.Map<User>(finalUser);

                        }
                        else
                        {
                            var userToCompare = _mapper.Map<User>(item);

                            if (!entityentity.Equals(userToCompare))
                            {

                                if (userToCompare.SuperiorId != entityentity.SuperiorId)
                                {
                                    if (userToCompare.SuperiorId != null && entityentity.SuperiorId != null)
                                    {
                                        User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityentity.SuperiorId, true);
                                        var usertoRemove = _mapper.Map<User>(entityentity);

                                        _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                                        User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)item.SuperiorId, true);
                                        _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, usertoRemove);

                                        item.SuperiorId = actualSuperior.UserId;
                                        item.PlantId = actualSuperior.PlantId;
                                        item.GroupId = actualSuperior.GroupId;

                                        if (item.UserType == 4)
                                            item.AreaId = actualSuperior.AreaId;

                                        userToCompare = _mapper.Map<User>(item);
                                    }
                                }



                                var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);
                                userToUpdate.CreatedDate = (DateTime)entityentity.CreatedDate;
                                await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityentity.UserId);
                                UserToReturn = _mapper.Map<User>(userToUpdate);
                                ResultToReturn.UsersUpdated++;
                            }
                            else
                            {
                                UserToReturn = _mapper.Map<User>(entityentity);
                                ResultToReturn.UsersExist++;
                            }


                        }
                    }
                    else
                    {
                        var usertoCreate = _mapper.Map<UsersForCreation>(item);
                        var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                        if (finalUser != null)
                            ResultToReturn.UsersCreated++;
                        UserToReturn = _mapper.Map<User>(finalUser);


                    }

                }
                else
                {
                    //User con id
                    var entityUserwhitId = await _assyChartService.FetchUserAsync((int)item.UserId);

                    if (entityUserwhitId == null)
                    {
                        //Si tiene un id erroneo, entra aqui para busqueda avanzada
                        bool existUser = false;
                        int typeUser = 0;
                        if (item.Payroll != null)
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByPayrollAsync((int)item.Payroll);
                            typeUser = existUser ? 1 : 0;
                        }

                        if (item.Email != "")
                        {
                            existUser = await _supervisorMobilityRepository.UserExistByEmailAsync(item.Email);
                            typeUser = existUser ? 2 : 0;
                        }

                        if (existUser)
                        {
                            var entityentity = typeUser == 1 ? await _supervisorMobilityRepository.GetUserByPayrollAsync((int)item.Payroll) : await _supervisorMobilityRepository.GetUserByEmailAsync(item.Email);

                            if (entityentity == null)
                            {
                                var usertoCreate = _mapper.Map<UsersForCreation>(item);
                                var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                                if (finalUser != null)
                                    ResultToReturn.UsersCreated++;
                                UserToReturn = _mapper.Map<User>(finalUser);

                            }
                            else
                            {
                                var userToCompare = _mapper.Map<User>(item);

                                if (!entityentity.Equals(userToCompare))
                                {
                                    if (userToCompare.SuperiorId != entityentity.SuperiorId)
                                    {
                                        if (userToCompare.SuperiorId != null && entityentity.SuperiorId != null)
                                        {
                                            User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityentity.SuperiorId, true);
                                            var usertoRemove = _mapper.Map<User>(entityentity);

                                            _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                                            User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)item.SuperiorId, true);
                                            _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, usertoRemove);

                                            item.SuperiorId = actualSuperior.UserId;
                                            item.PlantId = actualSuperior.PlantId;
                                            item.GroupId = actualSuperior.GroupId;

                                            if (item.UserType == 4)
                                                item.AreaId = actualSuperior.AreaId;

                                            userToCompare = _mapper.Map<User>(item);
                                        }
                                    }

                                    var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);

                                    userToUpdate.CreatedDate = (DateTime)entityentity.CreatedDate;
                                    await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityentity.UserId);
                                    UserToReturn = _mapper.Map<User>(userToUpdate);
                                    ResultToReturn.UsersUpdated++;
                                }
                                else
                                {

                                    UserToReturn = _mapper.Map<User>(entityentity);
                                    ResultToReturn.UsersExist++;
                                }

                            }
                        }
                        else
                        {
                            var usertoCreate = _mapper.Map<UsersForCreation>(item);
                            var finalUser = await _assyChartService.CreateUserAsync(usertoCreate);
                            if (finalUser != null)
                                ResultToReturn.UsersCreated++;
                            UserToReturn = _mapper.Map<User>(finalUser);

                        }
                    }
                    else
                    {
                        var userToCompare = _mapper.Map<User>(item);
                        if (!entityUserwhitId.Equals(userToCompare))
                        {
                            if (userToCompare.SuperiorId != entityUserwhitId.SuperiorId)
                            {
                                if (userToCompare.SuperiorId != null && entityUserwhitId.SuperiorId != null)
                                {
                                    User exsuperior = await _supervisorMobilityRepository.GetUserAsync((int)entityUserwhitId.SuperiorId, true);
                                    var usertoRemove = _mapper.Map<User>(entityUserwhitId);

                                    _supervisorMobilityRepository.UserRemoveSubordinated(exsuperior, usertoRemove);

                                    User actualSuperior = await _supervisorMobilityRepository.GetUserAsync((int)item.SuperiorId, true);
                                    _supervisorMobilityRepository.UserAddSubordinated(actualSuperior, usertoRemove);

                                    item.SuperiorId = actualSuperior.UserId;
                                    item.PlantId = actualSuperior.PlantId;
                                    item.GroupId = actualSuperior.GroupId;

                                    if (item.UserType == 4)
                                        item.AreaId = actualSuperior.AreaId;

                                    userToCompare = _mapper.Map<User>(item);
                                }
                            }

                            var userToUpdate = _mapper.Map<UsersForUpdateDto>(userToCompare);
                            userToUpdate.CreatedDate = (DateTime)entityUserwhitId.CreatedDate;
                            await _supervisorMobilityRepository.UpdateUser(userToUpdate, entityUserwhitId.UserId);
                            UserToReturn = _mapper.Map<User>(userToUpdate);

                            ResultToReturn.UsersUpdated++;
                        }
                        else
                        {
                            ResultToReturn.UsersExist++;
                            UserToReturn = _mapper.Map<User>(entityUserwhitId);
                        }

                    }

                }

                if (haveUsers)
                {
                    foreach (var elemntUser in UsersInUser)
                    {
                        _supervisorMobilityRepository.UserAddSubordinated(UserToReturn, elemntUser);
                    }
                }

                if (haveAreas)
                {
                    foreach (var elemntArea in AreasInUser)
                    {
                        _supervisorMobilityRepository.UserAddArea(UserToReturn, elemntArea);
                    }
                }

            }

            await _supervisorMobilityRepository.SaveChangesAsync();




            //restore extencion of file
            System.IO.File.Move(file, originalPath);

            return Ok(ResultToReturn);

        }

        //******* Download Formaters    **********//

        [EnableCors("Cors")]
        [HttpGet("Bulk/DownloadAllUsersFormat")]
        public async Task<IActionResult> DownloadAllUsersFormat()
        {

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
                    new Cell() { CellValue = new CellValue("UserId"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("UserName@compasdcpcs.local"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Payroll"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Email"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("UserType"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("SuperiorId"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("SubordinadosId's"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Plant"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Area"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Group"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Distribution"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                // Fila de descripciones
                Row descriptionRow = new Row();
                descriptionRow.Append(
                    new Cell() { CellValue = new CellValue("This field is used for the User_id registered in the Mobility supervisor system,"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the UserName"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Payroll"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the staff name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is for the e-mail address, to which the notifications will be sent."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field belongs to the user's privilege level within the system."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the User_id Superior registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the User_id's Subordinates registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Plant_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Area_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Group_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Distribution_id registered in the Mobility supervisor system"), DataType = CellValues.String }
                );
                sheetData.Append(descriptionRow);

                // Fila de notas adicionales
                Row notesRow1 = new Row();
                notesRow1.Append(
                    new Cell() { CellValue = new CellValue("in case it already exists, the user information will be updated."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Is the email provided by the activedirectory under the name PrincipalName"), DataType = CellValues.String },
                    null,
                    null,
                    new Cell() { CellValue = new CellValue("Preferably the e-mail address that the person uses to receive the job information."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("1 - Admin: Full access to the entire system, 2 - SSV, 3 - SV, 4 - Operator"), DataType = CellValues.String },
                    null,
                    new Cell() { CellValue = new CellValue("In case only have one Subordinate, the User_ID of subordinated register in Mobility supervisor system"), DataType = CellValues.String },
                    null,
                    new Cell() { CellValue = new CellValue("In case you are registering an SSV, which has several areas, it is necessary to separate them by commas. eg (1,2,3) without parentheses"), DataType = CellValues.String }
                );
                sheetData.Append(notesRow1);

                Row notesRow2 = new Row();
                notesRow2.Append(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new Cell() { CellValue = new CellValue("Can be left blank"), DataType = CellValues.String }
                );
                sheetData.Append(notesRow2);

                workbookPart.Workbook.Save();
            }

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
                    Name = "SSV Format"
                };
                sheets.Append(sheet);

                // Obtener el objeto SheetData
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Encabezados
                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { CellValue = new CellValue("UserId SSV"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Email"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Plant"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Group"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("AreasManage"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("UserName@compasdcpcs.local"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                // Descripciones
                Row descriptionRow = new Row();
                descriptionRow.Append(
                    new Cell() { CellValue = new CellValue("This field is used for the User_id registered in the Mobility supervisor system,"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the staff name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is for the e-mail address, to which the notifications will be sent."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Plant_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Group_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Area_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the UserName"), DataType = CellValues.String }
                );
                sheetData.Append(descriptionRow);

                // Notas adicionales
                Row notesRow = new Row();
                notesRow.Append(
                    new Cell() { CellValue = new CellValue("in case it already exists, the user information will be updated."), DataType = CellValues.String },
                    null,
                    new Cell() { CellValue = new CellValue("Preferably the e-mail address that the person uses to receive the job information."), DataType = CellValues.String },
                    null,
                    null,
                    new Cell() { CellValue = new CellValue("In case of having several areas, it is necessary to separate them by commas. eg (1,2,3) without parentheses"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Is the email provided by the activedirectory under the name PrincipalName"), DataType = CellValues.String }
                );
                sheetData.Append(notesRow);

                workbookPart.Workbook.Save();
            }


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
                    new Cell() { CellValue = new CellValue("UserId Supervisor"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Email"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("SSV_Id Superior"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Assign Area_ID"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("UserName@compasdcpcs.local"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                // Descripciones
                Row descriptionRow = new Row();
                descriptionRow.Append(
                    new Cell() { CellValue = new CellValue("This field is used for the User_id registered in the Mobility supervisor system,"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the staff name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is for the e-mail address, to which the notifications will be sent."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the User_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Area_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the UserName"), DataType = CellValues.String }
                );
                sheetData.Append(descriptionRow);

                // Notas adicionales
                Row notesRow = new Row();
                notesRow.Append(
                    new Cell() { CellValue = new CellValue("in case it already exists, the user information will be updated."), DataType = CellValues.String },
                    null,
                    new Cell() { CellValue = new CellValue("Preferably the e-mail address that the person uses to receive the job information."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("the id of the person who will be his or her superior"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("the id of one of the areas administering the SSV who will be his or her superior"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Is the email provided by the activedirectory under the name PrincipalName"), DataType = CellValues.String }
                );
                sheetData.Append(notesRow);

                workbookPart.Workbook.Save();
            }

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
                    new Cell() { CellValue = new CellValue("UserId Operator"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Payroll"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Distribution Id"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Supervisor_Id Superior"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                // Descripciones
                Row descriptionRow = new Row();
                descriptionRow.Append(
                    new Cell() { CellValue = new CellValue("This field is used for the User_id registered in the Mobility supervisor system,"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Payroll"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the UserName"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the Distribution_id registered in the Mobility supervisor system"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("This field is used for the User_id Superior registered in the Mobility supervisor system"), DataType = CellValues.String }
                );
                sheetData.Append(descriptionRow);

                // Notas adicionales
                Row notesRow = new Row();
                notesRow.Append(
                    new Cell() { CellValue = new CellValue("in case it already exists, the user information will be updated."), DataType = CellValues.String },
                    null,
                    null,
                    new Cell() { CellValue = new CellValue("Can be left blank"), DataType = CellValues.String }
                );
                sheetData.Append(notesRow);

                workbookPart.Workbook.Save();
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OperatorsFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

    }
}