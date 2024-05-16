using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SupervisorMobility.API.Entities.CDMS;
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
            //optenemos el enlace de descarga y la Key
            var FileInfoResponse = await _bridgeHttpClient.PostAsync("SMCcp/PostDownloadfileCcp", content);

            CDMS_DownloadFile DownloadLink = new();
            if (FileInfoResponse.IsSuccessStatusCode)
            {
                var result = await FileInfoResponse.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                DownloadLink = result;
            }
            else
            {
                return NotFound("Error en la Ruta o no se encontro el archivo en el bridge");
            }

            //Continua con la logica

            var fileKey = DownloadLink?.operation.NameDocKey;
            var fileURL = DownloadLink?.operation.URL;

            if (_env.IsDevelopment())
            {
                fileURL = fileURL.Replace("https://10.91.117.5:3000", "https://10.91.49.2:3000");
            }

            var fileNameWithOutIp = "";

            if (_env.IsDevelopment())
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.49.2:3000/CCP/", "");
            }
            else
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.117.5:3000/CCP/", "");
            }

            var filePathSave = Path.Combine(_env.ContentRootPath, "downloads\\CCP", fileNameWithOutIp);


            string directoryPath = Path.Combine(_env.ContentRootPath, "downloads\\CCP");

            // Verificar si el directorio existe; si no, crearlo
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath); // Crear el directorio si no existe
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear el directorio: {ex.Message}");
                }
            }

            try
            {
                //descargamos el archivo
                using (var fileDownloadResponse = await _bridgeHttpClient.GetAsync(fileURL))
                {
                    if (fileDownloadResponse.IsSuccessStatusCode)
                    {
                        // Guardar el archivo descargado en el sistema local
                        using (var fileStream = new FileStream(filePathSave, FileMode.Create))
                        {
                            await fileDownloadResponse.Content.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Archivo descargado exitosamente: {filePathSave}");
                    }
                    else
                    {
                        Console.WriteLine($"Error al descargar el archivo. StatusCode: {fileDownloadResponse.StatusCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            }


            // Retornamos el archivo local al cliente
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePathSave, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Leer los bytes del archivo local
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePathSave);

            // Crear un FileContentResult con los bytes del archivo y el tipo de contenido
            var resultWhitFile = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = fileKey // Nombre del archivo en la descarga
            };

            // Establecer la disposición del contenido como "attachment"
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + resultWhitFile.FileDownloadName);

            // Añadir header personalizado "KeyDocument"
            Response.Headers.Add("KeyDocument", fileKey);

            // Añadir header personalizado "PathDocument"
            Response.Headers.Add("PathDocument", filePathSave);

            // Retornar el FileContentResult
            return resultWhitFile;

        }

        [HttpPost("SMCcp/DeleteFileTempCcp")]
        public async Task<ActionResult> DeleteFileTempCcp(Dictionary<string, string> parameters)
        {
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

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                var uri = new Uri(_bridgeHttpClient.BaseAddress, "SMCcp/DeleteFileTempCcp");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _bridgeHttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    return Ok(result);
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"DELETE TEMP CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }


            return Ok();
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
            //optenemos el enlace de descarga y la Key
            var FileInfoResponse = await _bridgeHttpClient.PostAsync("SMHoe/PostArchivesDirectoryHOE", content);

            CDMS_DownloadFile DownloadLink = new();
            if (FileInfoResponse.IsSuccessStatusCode)
            {
                fileURL = documentUrlDownload;
                Console.WriteLine($"url a descargar: {documentUrlDownload}");

            if (_env.IsDevelopment())
            {
                fileURL = fileURL.Replace("https://10.91.117.5:3000", "https://10.91.49.2:3000");
            }

            var fileNameWithOutIp = "";

            if (_env.IsDevelopment())
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.49.2:3000/HOE/", "");
            }
            else
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.117.5:3000/HOE/", "");
            }

            var filePathSave = Path.Combine(_env.ContentRootPath, "downloads\\HOE", fileNameWithOutIp);


            string directoryPath = Path.Combine(_env.ContentRootPath, "downloads\\HOE");

            // Verificar si el directorio existe; si no, crearlo
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath); // Crear el directorio si no existe
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear el directorio: {ex.Message}");
                }
            }

            try
            {
                //descargamos el archivo
                using (var fileDownloadResponse = await _bridgeHttpClient.GetAsync(fileURL))
                {
                    if (fileDownloadResponse.IsSuccessStatusCode)
                    {
                        // Guardar el archivo descargado en el sistema local
                        using (var fileStream = new FileStream(filePathSave, FileMode.Create))
                        {
                            await fileDownloadResponse.Content.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Archivo descargado exitosamente: {filePathSave}");
                    }
                    else
                    {
                        Console.WriteLine($"Error al descargar el archivo. StatusCode: {fileDownloadResponse.StatusCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            }


            // Retornamos el archivo local al cliente
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePathSave, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Leer los bytes del archivo local
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePathSave);

            // Crear un FileContentResult con los bytes del archivo y el tipo de contenido
            var resultWhitFile = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = fileKey // Nombre del archivo en la descarga
            };

            // Establecer la disposición del contenido como "attachment"
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + resultWhitFile.FileDownloadName);

            // Añadir header personalizado "KeyDocument"
            Response.Headers.Add("KeyDocument", fileKey);

            // Añadir header personalizado "PathDocument"
            Response.Headers.Add("PathDocument", filePathSave);

            // Retornar el FileContentResult
            return resultWhitFile;

        }
        [HttpPost("SMHoe/DeleteFileTempHoe")]
        public async Task<ActionResult> DeleteFileTempHoe(Dictionary<string, string> parameters)
        {
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

            return Ok();
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
            //optenemos el enlace de descarga y la Key
            var FileInfoResponse = await _bridgeHttpClient.PostAsync("SMGos/PostDownloadfileGos", content);

            CDMS_DownloadFile DownloadLink = new();
            if (FileInfoResponse.IsSuccessStatusCode)
            {
                var result = await FileInfoResponse.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                DownloadLink = result;
            }
            else
            {
                return NotFound("Error en la Ruta o no se encontro el archivo en el bridge");
            }

            //Continua con la logica

            var fileKey = DownloadLink?.operation.NameDocKey;
            var fileURL = DownloadLink?.operation.URL;

            if (_env.IsDevelopment())
            {
                fileURL = fileURL.Replace("https://10.91.117.5:3000", "https://10.91.49.2:3000");
            }

            var fileNameWithOutIp = "";

            if (_env.IsDevelopment())
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.49.2:3000/GOS/", "");
            }
            else
            {
                fileNameWithOutIp = fileURL.Replace("https://10.91.117.5:3000/GOS/", "");
            }

            var filePathSave = Path.Combine(_env.ContentRootPath, "downloads\\GOS", fileNameWithOutIp);


            string directoryPath = Path.Combine(_env.ContentRootPath, "downloads\\GOS");

            // Verificar si el directorio existe; si no, crearlo
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath); // Crear el directorio si no existe
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear el directorio: {ex.Message}");
                }
            }

            try
            {
                //descargamos el archivo
                using (var fileDownloadResponse = await _bridgeHttpClient.GetAsync(fileURL))
                {
                    if (fileDownloadResponse.IsSuccessStatusCode)
                    {
                        // Guardar el archivo descargado en el sistema local
                        using (var fileStream = new FileStream(filePathSave, FileMode.Create))
                        {
                            await fileDownloadResponse.Content.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Archivo descargado exitosamente: {filePathSave}");
                    }
                    else
                    {
                        Console.WriteLine($"Error al descargar el archivo. StatusCode: {fileDownloadResponse.StatusCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            }


            // Retornamos el archivo local al cliente
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePathSave, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Leer los bytes del archivo local
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePathSave);

            // Crear un FileContentResult con los bytes del archivo y el tipo de contenido
            var resultWhitFile = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = fileKey // Nombre del archivo en la descarga
            };

            // Establecer la disposición del contenido como "attachment"
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + resultWhitFile.FileDownloadName);

            // Añadir header personalizado "KeyDocument"
            Response.Headers.Add("KeyDocument", fileKey);

            // Añadir header personalizado "PathDocument"
            Response.Headers.Add("PathDocument", filePathSave);

            // Retornar el FileContentResult
            return resultWhitFile;
        }


        [HttpPost("SMGos/DeleteFileTempGos")]
        public async Task<ActionResult> DeleteFileTempGos(Dictionary<string, string> parameters)
        {
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

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                var uri = new Uri(_bridgeHttpClient.BaseAddress, "SMGos/DeleteFileTempGos");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _bridgeHttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    return Ok(result);
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"DELETE TEMP GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

           

            return Ok();
        }

    }
}
