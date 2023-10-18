using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _email;
        public EmailController(IEmailService emailService)
        {
            _email = emailService;
        }

        [HttpPost]
        public void sendemail(string email, string body)
        {
            var emailMessage = _email.CreateEmailMessage(email, body);
            _email.Send(emailMessage);
        }


    }
}
