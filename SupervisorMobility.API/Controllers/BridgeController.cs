using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/BridgeCDMS")]
    public class BridgeController : ControllerBase
    {
        private readonly HttpClient _bridgeHttpClient;

        public BridgeController(CustomHttpClientService customHttp)
        {
            //Prod
            _bridgeHttpClient = customHttp.GetBridgeHttpClient();
        }






    }
}
