using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.DataAccess.Entities.IS;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Appearance/DataPanels/Specification")]
    [ApiController]
    public class DataPanelSpecificationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public DataPanelSpecificationController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreateDatePanelCategorie(DataPanelSpecificationForCreateDto dataPanelSpectForCreate)
        {
            DataPanel DPEntity = await _stampingRepository.getDataPanel((int)dataPanelSpectForCreate.DataPanelId, true);

            dataPanelSpectForCreate.ItemOrder = await _stampingRepository.DataPanelSpecificationMaxItemOrderAsync((int)dataPanelSpectForCreate.DataPanelId);

            DataPanelSpecification DPSpecEntity = _mapper.Map<DataPanelSpecification>(dataPanelSpectForCreate);

            var createdResult = await _stampingRepository.AddDataPanelSpecification(DPEntity, DPSpecEntity);
            if (createdResult != null)
                return Ok(DPEntity);
            else
                return BadRequest(); ;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataPanelSpecificationDto>>> GetAllDataPanelSpecificationCategories(int DataPanel_id ,bool includeDataPanel = false)
        {

            var DataPanelSpecificationsEntities = await _stampingRepository.getAllDataPanelSpecificationFromDataPanel(DataPanel_id);
            if (DataPanelSpecificationsEntities == null)
            {
                return NotFound("DataPanelEntities categories not found!");
            }

            return Ok(_mapper.Map<IEnumerable<DataPanelSpecificationDto>>(DataPanelSpecificationsEntities));
        }

        [HttpGet("{id}", Name = "GetDataPanelSpecifications")]
        public async Task<ActionResult<DataPanelSpecificationDto>> GetDataPanelSpecificationCategory(int id, bool includeDataPanel = false)
        {
            //Find Checklist category
            var DataPanelSpecificationEntiti = await _stampingRepository.getDataPanelSpecification(id);
            if (DataPanelSpecificationEntiti == null)
            {
                return NotFound("Data Panel not found!");
            }

            return Ok(_mapper.Map<DataPanelSpecificationDto>(DataPanelSpecificationEntiti));
        }

        [HttpPut("sequence/{dataSpecification_Id}")]
        public async Task<ActionResult> UpdateDataSpecificationItemOrder(int dataSpecification_Id,
        DataPanelSpecificationForUpdateSequenceDto dataSpecification)
        {
            var dataPanelEntity = await _stampingRepository.getDataPanelSpecification(dataSpecification_Id);
            if (dataPanelEntity == null)
            {
                return NotFound("Data Panel Specification category not found.");
            }

            if (dataSpecification.ItemOrder == dataPanelEntity.ItemOrder)
            {
                return NoContent();
            }

            if (dataSpecification.ItemOrder < 1
                || dataSpecification.ItemOrder > await _stampingRepository.DataPanelSpecificationMaxItemOrderAsync((int)dataPanelEntity.DataPanelId))
            {
                return BadRequest("ItemOrder must be greater than 1 and lower that the current max ItemOrder.");
            }

            var updateResult = await _stampingRepository.UpdateDataPanelSpecificationSequenceAsync(dataSpecification, dataPanelEntity);

            if (updateResult > 0)
            {
                return Ok();
            }

            return NoContent();

        }

    }
}
