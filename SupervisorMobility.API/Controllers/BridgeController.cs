using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SupervisorMobility.API.Entities.CDMS.Downloads;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/BridgeCDMS")]
    public class BridgeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _bridgeHttpClient;

        public BridgeController(CustomHttpClientService customHttp, IWebHostEnvironment env)
        {
            //Prod
            _env = env;
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

        public class CDMS_DownloadFile
        {
            public bool success { get; set; }
            public Download_CDMS_Document operation { get; set; } = new Download_CDMS_Document();
            public string message { get; set; }
        }

        [HttpPost("SMGos/PostDownloadfileGos")]
        public async Task<IActionResult> PostDownloadfileGos(Dictionary<string, string> parameters)
        {
            try
            {
                var content = new FormUrlEncodedContent(parameters);

                var response = await _bridgeHttpClient.PostAsync("SMGos/PostDownloadfileGos", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GET LINK GOS, Status Code {response.StatusCode}");
                    return BadRequest($"Error retrieving file: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();

                if (result == null || string.IsNullOrEmpty(result.operation?.URL))
                {
                    return NotFound("File information not found in response.");
                }

                var fileURL = result.operation.URL;

                if (_env.IsDevelopment())
                {
                    // Replace development URL with production URL
                    fileURL = fileURL.Replace("https://10.91.117.5:3000", "https://10.91.49.2:3000");
                }

                using (var fileResponse = await _bridgeHttpClient.GetAsync(fileURL))
                {
                    if (!fileResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error downloading file. StatusCode: {fileResponse.StatusCode}");
                        return StatusCode((int)fileResponse.StatusCode, "Error downloading file.");
                    }

                    var fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();
                    var fileName = result.operation.NameDocKey ?? "downloaded_file"; // Default filename if not provided

                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(fileName, out var contentType))
                    {
                        contentType = "application/octet-stream"; // Default content type
                    }
                    // Save the downloaded file to a temporary directory
                    var tempDir = Path.Combine(_env.ContentRootPath, "downloads", "temp");
                    Directory.CreateDirectory(tempDir);
                    var filePath = Path.Combine(tempDir, fileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                    // Prepare response headers
                    Response.Headers.Add("KeyDocument", fileName);
                    Response.Headers.Add("PathDocument", filePath); // Include the file path in response headers

                    // Return file as download attachment
                    return File(fileBytes, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Download Gos File: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }



        [HttpPost("SMGos/DeleteFileTempGos")]
        public async Task<ActionResult> DeleteFileTempGos(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMGos/DeleteFileTempGos", content);

            if (parameters != null && parameters.TryGetValue("documentDelete", out string documentToDelete))
            {
                // Aquí puedes utilizar el valor de 'documentDelete'
                Console.WriteLine($"Documento a eliminar: {documentToDelete}");

                // Verificar si el archivo existe en la ruta proporcionada
                if (System.IO.File.Exists(documentToDelete))
                {
                    // Eliminar el archivo
                    System.IO.File.Delete(documentToDelete);
                    // Devolver una respuesta exitosa
                    Debug.WriteLine("Archivo eliminado correctamente.");
                }
                else
                {
                    Debug.WriteLine("El archivo no existe en la ubicación especificada.");
                }

                // Devolver una respuesta exitosa si la operación fue exitosa
                Debug.WriteLine("Documento eliminado correctamente.");
            }
            else
            {
                // Si no se encuentra el parámetro 'documentDelete', devolver un error
                Debug.WriteLine("Parámetro 'documentDelete' no encontrado o inválido.");
            }


            return Ok(response.Content.ReadAsStringAsync().Result);
        }

    }
}
