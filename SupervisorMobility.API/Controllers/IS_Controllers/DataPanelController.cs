using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.DataAccess.Entities.IS;
using Org.BouncyCastle.Pkcs;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Aparence/DataPanels")]
    [ApiController]
    public class DataPanelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public DataPanelController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreateDatePanelCategorie(DataPanelForCreateDto dataPanelForCreate)
        {
            DataPanel DPEntity = _mapper.Map<DataPanel>(dataPanelForCreate);

            var createdResult = await _stampingRepository.AddDataPanel(DPEntity);
            if (createdResult != null)
                return Ok(DPEntity);
            else
                return BadRequest(); ;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataPanelDto>>> GetDataPanelCategories(bool includeSpecifications = false)
        {

            var DataPanelEntities = await _stampingRepository.getAllDataPanels(includeSpecifications);
            if (DataPanelEntities == null)
            {
                return NotFound("DataPanelEntities categories not found!");
            }

            return Ok(_mapper.Map<IEnumerable<DataPanelDto>>(DataPanelEntities));
        }

        [HttpGet("{id}", Name = "GetDataPanelCategory")]
        public async Task<ActionResult<DataPanelDto>> GetDataPanelCategory(int id, bool includeSpecifications = false)
        {
            //Find Checklist category
            var DataPanelCategory = await _stampingRepository.getDataPanel(id, includeSpecifications);
            if (DataPanelCategory == null)
            {
                return NotFound("Data Panel not found!");
            }

            return Ok(_mapper.Map<DataPanelDto>(DataPanelCategory));
        }

        [HttpPut("sequence/{datapanel_Id}")]
        public async Task<ActionResult> UpdatedataPanelItemOrder(int datapanel_Id,
           DataPanelForUpdateSequenceDto dataPanel)
        {
            var dataPanelEntity = await _stampingRepository.getDataPanel(datapanel_Id);
            if (dataPanelEntity == null)
            {
                return NotFound("Data Panel category not found.");
            }

            if (dataPanel.ItemOrder == dataPanelEntity.ItemOrder)
            {
                return NoContent();
            }

            if (dataPanel.ItemOrder < 1
                || dataPanel.ItemOrder > await _stampingRepository.DataPanelMaxItemOrderAsync())
            {
                return BadRequest("ItemOrder must be greater than 1 and lower that the current max ItemOrder.");
            }

            var updateResult = await _stampingRepository.UpdateDataPanelsSequenceAsync(dataPanel, dataPanelEntity);
            
            if(updateResult > 0)
            {
                return Ok();
            }

            return NoContent();

        }

        [HttpDelete("{dataPanelId}")]
        public async Task<ActionResult> DeleteDataPanel(int dataPanelId)
        {
            DataPanel? entityDataPanel = await _stampingRepository.getDataPanel(dataPanelId);

            var result = await _stampingRepository.removeDataPanel(entityDataPanel);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }


    }
}
