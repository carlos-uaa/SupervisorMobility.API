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

        public LoginController(CustomHttpClientService customHttp)
        {
            //Prod
            _httpClient = customHttp.GetLoginHttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Dictionary<string, string> paremters)
        {
            var data = new
            {
                username = paremters["user"],
                password = paremters["pass"]
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
