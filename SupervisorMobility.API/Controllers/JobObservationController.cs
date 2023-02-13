using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.JobObservationConfigsDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservations")]
    [ApiController]
    public class JobObservationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public JobObservationController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ?? 
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllJobObservationsAsync()
        //{

        //    //var allJobObservations = await _supervisorMobilityRepository.GetAllJobObservationsAsync();

        //    return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        //}



    }
}
