using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Models.OperationDtos;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;

        public NotificationController(IAssyChartService assyChartService,
            IMapper mapper)
        {
            _assyChartService = assyChartService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
        {
            var Notifications = await _assyChartService.GetNotifications();
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(Notifications));
        }

      
        [HttpGet("{iduser}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotificationsFromUser(int iduser)
        {
            var Notifications = await _assyChartService.GetNotificationsFromUser(iduser);
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(Notifications));
        }

        [HttpDelete("delete/{notifyId}")]
        public async Task<ActionResult<NotificationDto>> DeleteNotification(int notifyId)
        {
            Notification NotificationEntity = await _assyChartService.FetchNotificationAsync(notifyId);

            var taskresult = await _assyChartService.RemoveNotificationAsync(NotificationEntity);

            if (taskresult)
            {
                return Ok(_mapper.Map<NotificationDto>(NotificationEntity));
            }

            return NotFound();

        }

        [HttpPut("read/{notifyId}")]
        public async Task<ActionResult<NotificationDto>> ReadNotification(int notifyId, NotificationForUpdateDto notifyToUpdate)
        {
            var NotificationEntity = await _assyChartService.FetchNotificationAsync(notifyId);

            var taskresult = await _assyChartService.UpdateNotificationAsync(notifyToUpdate, NotificationEntity);

            var notifytoretur = _mapper.Map<Notification>(notifyToUpdate);

            if (taskresult)
            {
                return Ok(_mapper.Map<NotificationDto>(notifytoretur));
            }

            return NotFound();
        }



        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(NotificationToCreateDto newnotify)
        {
            //var finalNotify = _mapper.Map<DataAccess.Entities.Notification>(newnotify);

            var notify = await _assyChartService.CreateNotificationAsync(newnotify);

            return Ok(notify);
        }
    }
}
