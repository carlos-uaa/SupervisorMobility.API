using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Net.WebRequestMethods;

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

        //CCP

        [HttpGet("SMCcp/GetDirectoryPathsCcp")]
        public async Task<ActionResult> GetDirectoryPathsCcp()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var response = await _bridgeHttpClient.GetAsync("SMCcp/GetDirectoryPathsCcp");

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMCcp/PostArchivesDirectoryCcp")]
        public async Task<ActionResult> PostArchivesDirectoryCcp(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMCcp/PostArchivesDirectoryCcp", content);

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMCcp/PostDownloadfileCcp")]
        public async Task<ActionResult> PostDownloadfileCcp(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMCcp/PostDownloadfileCcp", content);
            
            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMCcp/DeleteFileTempCcp")]
        public async Task<ActionResult> DeleteFileTempCcp(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMCcp/DeleteFileTempCcp", content);

            return Ok(response.Content.ReadAsStringAsync().Result);
        }


        //HOE
        [HttpGet("SMHoe/GetDirectoryPaths")]
        public async Task<ActionResult> GetDirectoryPaths()
        {
            var response = await _bridgeHttpClient.GetAsync("SMHoe/GetDirectoryPaths");

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMHoe/PostArchivesDirectoryHOE")]
        public async Task<ActionResult> PostArchivesDirectoryHOE(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMHoe/PostArchivesDirectoryHOE", content);

            var result = response.Content.ReadAsStringAsync().Result;

            return Ok(result);
        }

        //GOS
        [HttpGet("SMGos/GetDirectoryPathsGos")]
        public async Task<ActionResult> GetDirectoryPathsGos()
        {
            var response = await _bridgeHttpClient.GetAsync("SMGos/GetDirectoryPathsGos");

            var result = response.Content.ReadAsStringAsync().Result;

            return Ok(result);
        }

        [HttpPost("SMGos/PostArchivesDirectoryGos")]
        public async Task<ActionResult> PostArchivesDirectoryGos(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMGos/PostArchivesDirectoryGos", content);

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMGos/PostDownloadfileGos")]
        public async Task<ActionResult> PostDownloadfileGos(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMGos/PostDownloadfileGos", content);

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        [HttpPost("SMGos/DeleteFileTempGos")]
        public async Task<ActionResult> DeleteFileTempGos(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMGos/DeleteFileTempGos", content);

            return Ok(response.Content.ReadAsStringAsync().Result);
        }

    }
}
