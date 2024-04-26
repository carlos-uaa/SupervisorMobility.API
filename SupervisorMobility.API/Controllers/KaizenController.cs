using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.KaizenDtos;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using System.Linq.Expressions;
using SupervisorMobility.API.Models.KaizenTransactionDtos;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/kaizen")]
    public class KaizenController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly SupervisorMobilityContext _context;
        private readonly IWebHostEnvironment _env;
        public KaizenController(ISupervisorMobilityRepository supervisorMobilityRepository, SupervisorMobilityContext context, IWebHostEnvironment env,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<KaizenDto>> CreateKaizen(CreateKaizenDto KaizenForCreate)
        {
            Kaizen KaizenEntity = _mapper.Map<Kaizen>(KaizenForCreate);

            var entityKaizen = await _supervisorMobilityRepository.AddKaizen(KaizenEntity);
            if (entityKaizen != null)
                return Ok(KaizenEntity);
            else
                return BadRequest(); ;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KaizenWithAllDataDto>>> GetAllDataKaizen(bool includeNavigation = false, bool includePeople = false, bool includeEvidences= false, bool includeTransactions = false)
        {

            var entityKaizen = await _supervisorMobilityRepository.GetAllKaizens(includeNavigation, includePeople, includeEvidences, includeTransactions);
            if (entityKaizen != null)
                return Ok(entityKaizen);
            else
                return BadRequest(); 
        }

        [HttpGet("{kaizenId}")]
        public async Task<ActionResult<KaizenWithAllDataDto>> GetKaizen(int kaizenId, bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false)
        {

            var entityKaizen = await _supervisorMobilityRepository.GetKaizen(kaizenId, includeNavigation, includePeople, includeEvidences, includeTransactions);
            if (entityKaizen != null)
                return Ok(entityKaizen);
            else
                return BadRequest();
        }

        [HttpPut("{kaizenId}")]
        public async Task<ActionResult<KaizenWithAllDataDto>> UpdateKaizen(int kaizenId, UpdateKaizenDto KaizenForUpdate)
        {
            
            var entityKaizen = await _supervisorMobilityRepository.GetKaizen(kaizenId);

            List<UpdateKaizenTransactionDto> filteredList = KaizenForUpdate.Transactions.Where(t => t.KaizenTransactionId == null || t.KaizenTransactionId <= 0).ToList();

            if (filteredList.Any())
            {
                var transactionsList = KaizenForUpdate.Transactions.ToList();
                transactionsList.RemoveAll(t => t.KaizenTransactionId == null || t.KaizenTransactionId <= 0);

                // Asignar la lista actualizada de nuevo a la propiedad Transactions
                KaizenForUpdate.Transactions = transactionsList;

                List<KaizenTransaction> newTransactions = _mapper.Map<List<KaizenTransaction>>(filteredList);

                foreach (var item in newTransactions)
                {
                    item.KaizenTransactionId = null;
                }

                _context.KaizenTransactions.AddRange(newTransactions);
                _context.SaveChanges();

                List<UpdateKaizenTransactionDto> NewKaizenCreated = _mapper.Map<List<UpdateKaizenTransactionDto>>(newTransactions);

                foreach (UpdateKaizenTransactionDto item in NewKaizenCreated)
                {
                    KaizenForUpdate.Transactions.Add(item);
                }
            }

            var result = await _supervisorMobilityRepository.UpdateKaizen(KaizenForUpdate, entityKaizen);



            if (result > 0)
                return Ok(entityKaizen);
            else 
                return BadRequest();
        }

        [HttpDelete("{kaizenId}")]
        public async Task<ActionResult> DeleteKaizen(int kaizenId)
        {
            var entityKaizen = await _supervisorMobilityRepository.GetKaizen(kaizenId);

            var result = await _supervisorMobilityRepository.RemoveKaizen(entityKaizen);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPost("{kaizenId}/evidence/remove/{isPreviousEvidence}")]
        public async Task<ActionResult<int>> RemoveEvidence(int kaizenId, int isPreviousEvidence, [FromBody] int fileUploadId)
        {
            await _supervisorMobilityRepository.RemoveEvidenceForKaizenAsync(kaizenId, fileUploadId, isPreviousEvidence == 1);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
