using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.ADUser;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LoginController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.91.116.212:4251/") };
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var data = new
            {
                username = username,
                password = password
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("", content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return BadRequest();
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

               
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500);
            }
        }
    }
}
