// +============================================================+\\
// +===================== SKILL CONTROLLER =====================+\\
// +============================================================+\\

/// <summary>
/// Handles HTTP requests related to the "Skill" collection
/// within the STRO module of the SOS system.
/// Provides endpoints to:
/// - Retrieve a specific skill record by ID
/// - Retrieve all skill records
/// - Create a new skill record
/// Interacts with ISkillRepository and uses AutoMapper for DTO mapping.
/// Exceptions are handled inside each action and return appropriate HTTP responses.
/// </summary>


// - Core .NET imports
using Microsoft.AspNetCore.Mvc;

// - Third-party imports
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Collections.Skill.Dtos;
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Skills;


namespace SupervisorMobility.API.Controllers.SOS_Controllers.STRO.Collections
{
    [Route("api/SOS/STRO/Collections/Skill")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly ISkillRepository _SkillRepository;
        private readonly IMapper _Mapper;

        /// <summary>
        /// Constructor: Initializes dependencies for the controller
        /// </summary>
        /// <param name="SkillRepository">Repository for handling skill data operations</param>
        /// <param name="mapper">AutoMapper instance for DTO-to-entity mapping</param>
        public SkillController(ISkillRepository SkillRepository, IMapper mapper)
        {
            _SkillRepository = SkillRepository;
            _Mapper = mapper;
        }

        // +============= ROUTES / ENDPOINTS ============+\\

        /// <summary>
        /// Retrieves a specific skill record by its ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the skill record (int)</param>
        /// <returns>Returns the skill object if found</returns>
        /// <response code="200">Skill record retrieved successfully</response>
        /// <response code="500">Internal server error occurred while fetching the record</response>
        /// <remarks>Throws an exception if the repository fails or database connection issues occur</remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkill(int Id)
        {
            try
            {
                var resKnowlede = await _SkillRepository.GetSkill(Id);
                return Ok(resKnowlede);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all skill records from the repository.
        /// </summary>
        /// <returns>Returns a list of all skill objects</returns>
        /// <response code="200">List of skill records retrieved successfully</response>
        /// <response code="500">Internal server error occurred while fetching records</response>
        /// <remarks>Throws an exception if the repository fails or database connection issues occur</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllSkill()
        {
            try
            {
                var resAllSkills = await _SkillRepository.GetAllSkill();
                return Ok(resAllSkills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new skill record in the repository.
        /// </summary>
        /// <param name="createSkillDto">DTO containing the information required to create a skill record</param>
        /// <returns>Returns the created skill object</returns>
        /// <response code="200">Skill record created successfully</response>
        /// <response code="500">Internal server error occurred while creating the record</response>
        /// <remarks>Throws an exception if mapping fails or repository creation fails</remarks>
        [HttpPost]
        public async Task<IActionResult> CreateSkill(CreateSkillDto createSkillDto)
        {
            try
            {
                var SkillCreate = _Mapper.Map<Skill>(createSkillDto);
                var resCreateKnowle = await _SkillRepository.CreateSkill(SkillCreate);
                return Ok(resCreateKnowle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
