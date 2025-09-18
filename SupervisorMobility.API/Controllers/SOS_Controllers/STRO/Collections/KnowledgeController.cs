// +============================================================+\\
// +=================== KNOWLEDGE CONTROLLER ===================+\\
// +============================================================+\\

/// <summary>
/// Handles HTTP requests related to the "Knowledge" collection
/// within the STRO module of the SOS system. Provides endpoints to:
/// - Retrieve a specific knowledge record by ID
/// - Retrieve all knowledge records
/// - Create a new knowledge record
/// Interacts with IKnowledgeRepository and uses AutoMapper for DTO mapping.
/// Exceptions are handled inside each action and return appropriate HTTP responses.
/// </summary>

// - Core .NET imports
using Microsoft.AspNetCore.Mvc;

// - Third-party imports
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Collections.Knowledge.Dtos;
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Knowledges;


namespace SupervisorMobility.API.Controllers.SOS_Controllers.STRO.Collections
{
    [Route("api/SOS/STRO/Collections/Knowledge")]
    [ApiController]
    public class KnowledgeController : ControllerBase
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly IKnowledgeRepository _KnowledgeRepository;
        private readonly IMapper _Mapper;

        /// <summary>
        /// Constructor: Initializes dependencies for the controller
        /// </summary>
        /// <param name="knowledgeRepository">Repository for handling knowledge data operations</param>
        /// <param name="mapper">AutoMapper instance for DTO-to-entity mapping</param>
        public KnowledgeController(IKnowledgeRepository knowledgeRepository, IMapper mapper)
        {
            _KnowledgeRepository = knowledgeRepository;
            _Mapper = mapper;
        }

        // +============= ROUTES / ENDPOINTS ============+\\

        /// <summary>
        /// Retrieves a specific knowledge record by its ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the knowledge record (int)</param>
        /// <returns>Returns the knowledge object if found</returns>
        /// <response code="200">Knowledge record retrieved successfully</response>
        /// <response code="500">Internal server error occurred while fetching the record</response>
        /// <remarks>Throws an exception if the repository fails or database connection issues occur</remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKnowledge(int Id)
        {
            try
            {
                var resKnowlede = await _KnowledgeRepository.GetKnowledge(Id);
                return Ok(resKnowlede);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all knowledge records from the repository.
        /// </summary>
        /// <returns>Returns a list of all knowledge objects</returns>
        /// <response code="200">List of knowledge records retrieved successfully</response>
        /// <response code="500">Internal server error occurred while fetching records</response>
        /// <remarks>Throws an exception if the repository fails or database connection issues occur</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllKnowledge()
        {
            try
            {
                var resAllKnowledges = await _KnowledgeRepository.GetAllKnowledge();
                return Ok(resAllKnowledges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new knowledge record in the repository.
        /// </summary>
        /// <param name="createKnowledgeDto">DTO containing the information required to create a knowledge record</param>
        /// <returns>Returns the created knowledge object</returns>
        /// <response code="200">Knowledge record created successfully</response>
        /// <response code="500">Internal server error occurred while creating the record</response>
        /// <remarks>Throws an exception if mapping fails or repository creation fails</remarks>
        [HttpPost]
        public async Task<IActionResult> CreateKnowledge(CreateKnowledgeDto createKnowledgeDto)
        {
            try
            {
                var KnowledgeCreate = _Mapper.Map<Knowledge>(createKnowledgeDto);
                var resCreateKnowle = await _KnowledgeRepository.CreateKnowledge(KnowledgeCreate);
                return Ok(resCreateKnowle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
