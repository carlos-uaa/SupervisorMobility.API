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
        public async Task<ActionResult> PostDownloadfileGos(Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);

            var response = await _bridgeHttpClient.PostAsync("SMGos/PostDownloadfileGos", content);
            //Aqui esa logica
            CDMS_DownloadFile DownloadLink = new();
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                DownloadLink = result;
            }
            else
            {
                //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                Console.WriteLine($"GET LINK GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
            }


            var fileName = DownloadLink?.operation.NameDocKey;
            var fileURL = DownloadLink?.operation.URL;

            if (_env.IsDevelopment())
            {
                //https://10.91.117.5:3000/GOS/T10140-5NA_1_TAKE%20OFF-ISS%20CABLE%20BANDAHbV4ePCte.xls
                //https://10.91.49.2:3000/GOS/T10140-5NA_1_TAKE%20OFF-ISS%20CABLE%20BANDAHbV4ePCte.xls
                fileURL = fileURL.Replace("https://10.91.117.5:3000", "https://10.91.49.2:3000");
            }

            Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

            var fileWithOutIp = "";

            if (_env.IsDevelopment())
            {
                fileWithOutIp = fileURL.Replace("https://10.91.49.2:3000/GOS/", "");
            }
            else
            {
                fileWithOutIp = fileURL.Replace("https://10.91.117.5:3000/GOS/", "");
            }

            var filePath = Path.Combine(_env.ContentRootPath, "downloads\\GOS", fileWithOutIp);

            try
            {
                using (var fileResponse = await _bridgeHttpClient.GetAsync(fileURL))
                {
                    if (fileResponse.IsSuccessStatusCode)
                    {
                        // Guardar el archivo descargado en el sistema local
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileResponse.Content.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Archivo descargado exitosamente: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Error al descargar el archivo. StatusCode: {fileResponse.StatusCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            }

            // Construir la URL de descarga relativa
            var relativeFilePath = Path.Combine("downloads/GOS", fileWithOutIp).Replace("\\", "/");
           
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Leer los bytes del archivo descargado
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                                 // Crear una respuesta personalizada con los bytes del archivo y los encabezados
            var responseHeaders = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };

            responseHeaders.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            responseHeaders.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName // Nombre del archivo en la descarga
            };

          

            // Convertir la respuesta a IActionResult y devolverla
            var resultWhitFile = new FileContentResult(await responseHeaders.Content.ReadAsByteArrayAsync(), contentType)
            {
                FileDownloadName = fileName // Nombre del archivo en la descarga
            };

            return resultWhitFile;
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
