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
using SupervisorMobility.API.Models.KaizenDtos;
using SupervisorMobility.API.Models.KaizenTransactionDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Entities.CDMS;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Appearance/DataPanels")]
    [ApiController]
    public class DataPanelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        private readonly SupervisorMobilityContext _context;
        public DataPanelController(IStampingRepository stampingRepository, SupervisorMobilityContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreateDatePanelCategorie(DataPanelForCreateDto dataPanelForCreate)
        {
            DataPanel DPEntity = _mapper.Map<DataPanel>(dataPanelForCreate);
            DPEntity.ItemOrder = await _stampingRepository.DataPanelMaxItemOrderAsync();

            if (DPEntity.Specifications?.Count > 0)
            {
                foreach (var (item, index) in DPEntity.Specifications?.Select((item, index) => (item, index)))
                {
                    item.ItemOrder = index + 1;
                }
            }

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

        [HttpPut("{dataPanelId}")]
        public async Task<ActionResult<DataPanelDto>> UpdateDataPanel(int dataPanelId, DataPanelForUpdateDto _DataPanelForUpdate)
        {

            // Obtener el DataPanel existente junto con sus especificaciones
            DataPanel entityDataPanel = await _stampingRepository.getDataPanel(dataPanelId, includeSpecifications: true);

            if (entityDataPanel == null)
            {
                return NotFound();
            }

            // Filtrar nuevas especificaciones
            List<DataPanelSpecificationForUpdateDto> filteredList = _DataPanelForUpdate.Specifications
                .Where(t => t.DataPanelSpecificationId == null || t.DataPanelSpecificationId <= 0)
                .ToList();

            // Remover nuevas especificaciones de la lista principal para evitar duplicados
            if (filteredList.Any())
            {
                _DataPanelForUpdate.Specifications.ToList().RemoveAll(t => t.DataPanelSpecificationId == null || t.DataPanelSpecificationId <= 0);

                // Mapear nuevas especificaciones
                List<DataPanelSpecification> newSpecifications = _mapper.Map<List<DataPanelSpecification>>(filteredList);

                int sequence = await _stampingRepository.DataPanelSpecificationMaxItemOrderAsync(dataPanelId);

                foreach (var item in newSpecifications)
                {
                    item.DataPanelSpecificationId = null;
                    item.DataPanelId = dataPanelId;
                    item.ItemOrder = sequence++;
                }

                _context.DataPanelSpecifications.AddRange(newSpecifications);
                await _context.SaveChangesAsync();

                // Mapear y agregar nuevas especificaciones creadas al DTO de actualización
                List<DataPanelSpecificationForUpdateDto> newSpecificationsCreated = _mapper.Map<List<DataPanelSpecificationForUpdateDto>>(newSpecifications);
                _DataPanelForUpdate.Specifications.ToList().AddRange(newSpecificationsCreated);
            }

            // Actualizar las propiedades del DataPanel
            entityDataPanel.IsActive = _DataPanelForUpdate.IsActive;
            entityDataPanel.ItemOrder = _DataPanelForUpdate.ItemOrder;
            entityDataPanel.DataTitle = _DataPanelForUpdate.DataTitle;

            // Manejar especificaciones existentes
            foreach (var specDto in _DataPanelForUpdate.Specifications)
            {
                var existingSpec = entityDataPanel.Specifications
                    .FirstOrDefault(s => s.DataPanelSpecificationId == specDto.DataPanelSpecificationId);

                if (existingSpec != null)
                {
                    existingSpec.IsActive = specDto.IsActive;
                    existingSpec.ItemOrder = specDto.ItemOrder;
                    existingSpec.DataSpecification = specDto.DataSpecification;
                }
            }

            // Guardar los cambios en el DataPanel y sus especificaciones
            _context.DataPanels.Update(entityDataPanel);
            var result = await _context.SaveChangesAsync();

            //var result = await _stampingRepository.UpdateDataPanel(_DataPanelForUpdate, entityDataPanel);

            if (result > 0)
                return Ok(entityDataPanel);
            else
                return BadRequest();
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

            if (updateResult > 0)
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
