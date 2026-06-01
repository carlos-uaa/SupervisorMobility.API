using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Bdd")]
        public ActionResult Hello()
        {

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("SupervisorMobilityDBConnectionString")))
                {
                    connection.Open();
                    return Ok("Connection successful to BDD!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message} \n CADENA DE CONEXIÓN: {_configuration.GetConnectionString("SupervisorMobilityDBConnectionString").ToString()}");
            }


        }

        [HttpGet("API")]
        public ActionResult ApiHello()
        {
            return Ok("Im Works, Hello World!");
        }



    }
}
