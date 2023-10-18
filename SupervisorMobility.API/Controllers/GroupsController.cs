using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public GroupsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
        {
            var groupEntities = await _supervisorMobilityRepository.GetGroupsAsync();
            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groupEntities));
        }

        [HttpGet("{groupId}", Name = "GetGroup")]
        public async Task<ActionResult> GetGroup(int groupId)
        {
            //Find Job Observation type
            var group = await _supervisorMobilityRepository
                .GetGroupAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GroupDto>(group));
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(
            GroupForCreationDto group)
        {
            //Mpa the pbject
            var finalGroup = _mapper.Map<Entities.Group>(group);
            _supervisorMobilityRepository.AddGroup(finalGroup);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createGroupToReturn =
                _mapper.Map<GroupDto>(finalGroup);

            return CreatedAtRoute("GetGroup",
                new
                {
                    groupId = createGroupToReturn.GroupId
                },
                createGroupToReturn);
        }


        [HttpPut("{groupId}")]
        public async Task<ActionResult> UpdateGroup(int groupId,
            GroupForUpdateDto group)
        {
            var groupEntity = await _supervisorMobilityRepository.GetGroupAsync(groupId);
            if (groupEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(group, groupEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpPatch("{groupId}")]
        public async Task<ActionResult> PartiallyUpdateGroup(
            int groupId,
            JsonPatchDocument<GroupForUpdateDto> patchDocumentGroup)
        {
            var groupEntity = await _supervisorMobilityRepository.GetGroupAsync(groupId);
            if (groupEntity == null)
            {
                return NotFound();
            }

            var groupToPatch = _mapper.Map<GroupForUpdateDto>(groupEntity);

            patchDocumentGroup.ApplyTo(groupToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentGroup))
            {
                return BadRequest();
            }

            _mapper.Map(groupToPatch, groupEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{groupId}")]
        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            var groupEntity = await _supervisorMobilityRepository.GetGroupAsync(groupId);
            if (groupEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteGroup(groupEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
