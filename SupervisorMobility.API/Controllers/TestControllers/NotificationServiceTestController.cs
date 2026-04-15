using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.NotificationDtos;

namespace SupervisorMobility.API.Controllers.TestControllers
{
    [ApiController]
    [Route("api/test/notifications")]
    public class NotificationServiceTestController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationServiceTestController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            var notifications = await _notificationService.GetNotificationsAsync();
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(notifications));
        }

        [HttpGet("{notificationId:int}")]
        public async Task<ActionResult<NotificationDto>> GetById(int notificationId)
        {
            var notification = await _notificationService.FetchNotificationAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<NotificationDto>(notification));
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUser(int userId)
        {
            var notifications = await _notificationService.GetNotificationsFromUserAsync(userId);
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(notifications));
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Notification>> Create(
            [FromQuery] bool queueEmail = false,
            [FromQuery] bool queueWhatsApp = false,
            [FromQuery] bool queueTeams = false,
            [FromQuery] string? queueType = null)
        {
            var dto = new NotificationToCreateDto
            {
                MadeBy = "Test User",
                NotificationType = "Test Notification",
                NotificationText = "This is a test notification.",
                UserId = 1,
                IsAccepted = true,
                IsActive = true,
                EntryDate = DateTime.Now
            };
            SpecialOptionsNotification? options = null;
            if (queueEmail || queueWhatsApp || queueTeams || !string.IsNullOrWhiteSpace(queueType))
            {
                options = new SpecialOptionsNotification
                {
                    Email = queueEmail,
                    WhatsApp = queueWhatsApp,
                    MicrosoftTeams = queueTeams,
                    type = queueType
                };
            }

            var created = await _notificationService.CreateNotificationAsync(dto, options);
            var result = _mapper.Map<Notification>(created);

            Log.Information("Entró a Create. created={@Created}", result);

            return Ok(result);
            // return Ok(_mapper.Map<NotificationDto>(created));
            // var result = _mapper.Map<NotificationDto>(created);
            // return CreatedAtAction(nameof(GetById), new { notificationId = result.NotificationID }, result);
        }

        [HttpPut("{notificationId:int}")]
        public async Task<ActionResult<NotificationDto>> Update(int notificationId, [FromBody] NotificationForUpdateDto dto)
        {
            var current = await _notificationService.FetchNotificationAsync(notificationId);
            if (current == null)
            {
                return NotFound();
            }

            await _notificationService.UpdateNotificationAsync(dto, current);
            return Ok(_mapper.Map<NotificationDto>(current));
        }

        [HttpDelete("{notificationId:int}")]
        public async Task<ActionResult> Delete(int notificationId)
        {
            var current = await _notificationService.FetchNotificationAsync(notificationId);
            if (current == null)
            {
                return NotFound();
            }

            await _notificationService.RemoveNotificationAsync(current);
            return NoContent();
        }
    }
}
