using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.QuestionTypeDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/QuestionTypes")]
    public class QuestionTypesController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public QuestionTypesController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetAllQuestionTypes()
        {
            var questionTypeEntities = await _supervisorMobilityRepository.GetQuestionTypesAsync();
            return Ok(_mapper.Map<IEnumerable<QuestionTypeDto>>(questionTypeEntities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetQuestionType(int id, bool includeChecklistQuestions = false)
        {
            var questionTypeEntitie = await _supervisorMobilityRepository.GetQuestionTypeAsync(id, includeChecklistQuestions);
            if (questionTypeEntitie == null)
            {
                return NotFound();
            }

            if (includeChecklistQuestions)
            {
                return Ok(_mapper.Map<QuestionTypeDto>(questionTypeEntitie));
            }

            return Ok(_mapper.Map<QuestionTypeDto>(questionTypeEntitie));
        }
    }
}
