using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph.Models;
using Azure.Identity;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpGet]
        public ActionResult Hello()
        {
            return Ok("Hello");
        }

        [HttpGet("{objectId}")]
        public async Task<IActionResult> GetUser(string objectId)
        {
           
            string clientId = "7a184926-2f58-4f9c-872c-97d54d825912";
            string tenantId = "84539953-c856-42b8-a26c-a60e5362d3e4";
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret("TuSecretKey")
                .Build();

            var credential = new ChainedTokenCredential(new ManagedIdentityCredential(), new EnvironmentCredential());

            try
            {

                var graphServiceClient = new GraphServiceClient(credential, scopes);
               
                DirectoryObject? userRequest = await graphServiceClient.DirectoryObjects[objectId].GetAsync();

                if (userRequest != null)
                {
                    return Ok(new
                    {
                        Name = userRequest.AdditionalData["displayName"],
                        Email = userRequest.AdditionalData["mail"],
                        UserPrincipalName = userRequest.AdditionalData["userPrincipalName"]
                    });
                }
                return StatusCode(401, $"Error: Unknow");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


    }
}
