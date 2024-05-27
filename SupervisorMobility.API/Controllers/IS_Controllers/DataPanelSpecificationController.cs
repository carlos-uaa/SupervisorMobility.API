using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Aparence/DataPanels/Specification")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataPanelSpecificationDto>>> GetAllDataPanelSpecificationCategories(int DataPanel_id ,bool includeDataPanel = false)
        {

            var DataPanelSpecificationsEntities = await _stampingRepository.getAllDataPanelSpecificationFromDataPanel(DataPanel_id, includeDataPanel);
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
            var DataPanelSpecificationEntiti = await _stampingRepository.getDataPanelSpecification(id, includeDataPanel);
            if (DataPanelSpecificationEntiti == null)
            {
                return NotFound("Data Panel not found!");
            }

            return Ok(_mapper.Map<DataPanelSpecificationDto>(DataPanelSpecificationEntiti));
        }

    }
}
