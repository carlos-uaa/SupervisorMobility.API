using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Models.ILU;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/ILU")]
    [ApiController]

    public class ILUController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public ILUController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }



        [HttpPost("Register")]
        public async Task<ActionResult> AddNewILU(ILURegisterForCreationDto ILUToRegister, int userID)
        {
            var finalILURegister = _mapper.Map<ILURegister>(ILUToRegister);

            ILURegister _newIluITraining = new();
            User MasterUser = await _supervisorMobilityRepository.GetUserAsync(userID);

            int Createresult = 0;

            if (finalILURegister.ILULevelId == 2 || finalILURegister.ILULevelId == 3)
            {

                ILURegister? LastLevelCheckTraining = await _supervisorMobilityRepository.GetLastILURegisterForUserAndDistribution((int)finalILURegister.OperatorId, (int)finalILURegister.DistributionId);
                //puede ser null

                if (LastLevelCheckTraining != null && LastLevelCheckTraining.ILULevelId.Value == 1)
                {
                    Createresult = await _supervisorMobilityRepository.AddILURegister(finalILURegister);

                    var AddILUToUser = await _supervisorMobilityRepository.AddILURegToUser(finalILURegister, MasterUser);

                    try
                    {
                        var temp = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync(finalILURegister.DistributionId.Value);
                        var temp2 = await _supervisorMobilityRepository.GetILULevel(finalILURegister.ILULevelId.Value);
                        HCIILU toAdd = new HCIILU
                        {
                            Description = temp.Description,
                            level = temp2.ILULevelCode.ToString(),
                            Register = finalILURegister,
                        };
                        await _supervisorMobilityRepository.AddHciIluReg(toAdd);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    _newIluITraining.DistributionId = (int)finalILURegister.DistributionId;
                    _newIluITraining.OperatorId = (int)finalILURegister.OperatorId;
                    _newIluITraining.AcquisitionDate = ILUToRegister.AcquisitionDate.Value.AddMinutes(1).AddSeconds(5);
                    _newIluITraining.ILULevelId = finalILURegister.ILULevelId == 2 ? 4 : 5;
                    _newIluITraining.isActive = true;

                    var CreateITrainee = await _supervisorMobilityRepository.AddILURegister(_newIluITraining);

                    var AddITrainee = await _supervisorMobilityRepository.AddILURegToUser(finalILURegister, MasterUser);

                    try
                    {
                        Distribution temp = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync(_newIluITraining.DistributionId.Value);
                        ILULevel temp2 = await _supervisorMobilityRepository.GetILULevel(_newIluITraining.ILULevelId.Value);
                        HCIILU toAdd = new HCIILU
                        {
                            Description = temp.Description,
                            level = temp2.ILULevelCode.ToString(),
                            Register = _newIluITraining,
                        };
                        await _supervisorMobilityRepository.AddHciIluReg(toAdd);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }


                    if (Createresult > 0 || CreateITrainee > 0)
                        return Ok(finalILURegister);
                    else
                        return NotFound();
                }

            }

            Createresult = await _supervisorMobilityRepository.AddILURegister(finalILURegister);

            var AddToUserResult = await _supervisorMobilityRepository.AddILURegToUser(finalILURegister, MasterUser);

            try
            {
                var temp = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync(finalILURegister.DistributionId.Value);
                var temp2 = await _supervisorMobilityRepository.GetILULevel(finalILURegister.ILULevelId.Value);
                HCIILU toAdd = new HCIILU
                {
                    Description = temp.Description,
                    level = temp2.ILULevelCode.ToString(),
                    Register = finalILURegister,
                };
                await _supervisorMobilityRepository.AddHciIluReg(toAdd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //buscar si existe hci del usuario, si no existe crearlo

            var hci = await _supervisorMobilityRepository.GetHCI((int)finalILURegister.OperatorId);

            if(hci == null)
            {
                HCI hciEntity = new();
                hciEntity.User = MasterUser;
                hciEntity.UserId = userID;
                hciEntity.ILUs = MasterUser.ILURegisers;


                var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);



                Console.WriteLine("Created HCi");
                Debug.WriteLine("Created HCi");

            }
                Console.WriteLine("exist HCi");
                Debug.WriteLine("Exist HCi");

            if (Createresult > 0)
                return Ok(finalILURegister);






            return NotFound();
        }


        [HttpGet("Register/{PATid}")]
        public async Task<ActionResult<ILURegisterDto>> getPatById(int ILURegisterId, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetILURegister(ILURegisterId);
                return Ok(_mapper.Map<ILURegisterDto>(PatsWhitCollections));

            }
            else
            {
                var Pat = await _supervisorMobilityRepository.GetPat(ILURegisterId);
                return Ok(_mapper.Map<ILURegisterWithoutNavigationDto>(Pat));
            }
        }


        [HttpGet("Levels")]
        public async Task<ActionResult<IEnumerable<ILULevelDto>>> GetAllLevelsILU(bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetAllILULevel();
                return Ok(_mapper.Map<IEnumerable<ILULevelDto>>(PatsWhitCollections));

            }
            else
            {
                var Pat = await _supervisorMobilityRepository.GetAllILULevel();
                return Ok(_mapper.Map<IEnumerable<ILULevelDto>>(Pat));
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPats(bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatWithCollections = await _supervisorMobilityRepository.GetAllPATs();
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatWithCollections));
        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository.GetAllPATs();
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));
        //    }
        //}

        //[HttpGet("SV/{idSV}")]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSV(
        //            int idSV, bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsOfSv(idSV);
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository
        //                        .GetAllPATsOfSv(idSV);
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

        //    }
        //}

        //[HttpGet("SSV/{idSSV}")]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSSV(
        //            int idSSV, bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsofSSV(idSSV);
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository
        //                        .GetAllPATsofSSV(idSSV);
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

        //    }
        //}
    }
}
