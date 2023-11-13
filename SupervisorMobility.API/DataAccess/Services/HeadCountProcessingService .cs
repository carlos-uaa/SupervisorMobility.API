using AutoMapper;
using ClosedXML;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class HeadCountProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private string _fileName;
        private int _userId;

        public HeadCountProcessingService(IServiceProvider serviceProvider, string fileName, int userId)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _fileName = fileName;
            _userId = userId;
        }

        public async Task StartAsync(string fileName, int userId)
        {
            _fileName = fileName;
            _userId = userId;

            await StartAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Tu lógica de procesamiento en segundo plano aquí
            await ProcessDocumentHeadCountAsync(_fileName, _userId, stoppingToken);
        }


        private async Task ProcessDocumentHeadCountAsync(string trustedFileNameForStorage, int UserIdUpload, CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var _supervisorMobilityRepository = serviceProvider.GetRequiredService<ISupervisorMobilityRepository>();
                var _email = serviceProvider.GetRequiredService<IEmailService>();

                await _supervisorMobilityRepository.RemoveAllHeadCountRegisters();

                User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);

                //Start Massive Upload 
                string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\headcount\\" + trustedFileNameForStorage;
                try
                {
                    using (var workBook = new XLWorkbook(filepath))
                    {
                        var pages = workBook.Worksheets.Count - 1;

                        //for (int p = 1; p <= pages; p++)
                        //{
                        IXLWorksheet ws = workBook.Worksheet(1);


                        bool firstRow = true;
                        int i = 1;
                        foreach (IXLRow row in ws.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            HeadCount _headCount = new HeadCount();

                            if (firstRow)
                            {
                                firstRow = false;
                            }
                            else
                            {
                                if (!row.IsEmpty())
                                {
                                    int maxRetries = 5; // Número máximo de intentos
                                    TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                    int retries = 0;

                                    while (retries < maxRetries)
                                    {
                                        try
                                        {


                                            try
                                            {
                                                // id subarea nombre subarea
                                                var valueFunctionDescription = ws.Cell(i, 5).GetString() != "" ? ws.Cell(i, 5).GetValue<string>().Trim() : "";


                                                bool contieneNumero = valueFunctionDescription.Any(char.IsDigit);

                                                //la celda esta dentro de los preocesso
                                                if (contieneNumero)
                                                {
                                                    //Tiene id de subarea,  extraemos numero
                                                    string numeroString = new string(valueFunctionDescription.Where(char.IsDigit).ToArray());

                                                    //convertimos
                                                    if (int.TryParse(numeroString, out int numero))
                                                    {
                                                        //guaramos id
                                                        _headCount.ID_subarea = numero;
                                                    }
                                                    else
                                                    {
                                                        //fallo el numero asignamos default
                                                        _headCount.ID_subarea = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    //No tiene id de subarea
                                                    _headCount.ID_subarea = 0;
                                                }

                                                try
                                                {
                                                    _headCount.nombre_subarea = valueFunctionDescription;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    _headCount.Fuction_Type = ws.Cell(i, 13).GetString() != "" ? ws.Cell(i, 13).GetValue<string>().Trim() : "";
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                //break;


                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                _headCount.RTO = ws.Cell(i, 9).GetString() != "" ? ws.Cell(i, 9).GetValue<string>() : "";

                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            //procedimiento
                                            try
                                            {
                                                _headCount.Codigo = ws.Cell(i, 1).GetString() != "" ? (int)ws.Cell(i, 1).Value : -1;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.CO = ws.Cell(i, 2).GetString() != "" ? ws.Cell(i, 2).GetValue<string>() : "";
                                                //                                  ToInsertIntoList.GOS = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                var valuesArea = ws.Cell(i, 3).GetString() != "" ? ws.Cell(i, 3).GetValue<string>() : "";
                                                var splitedArea = valuesArea.Split("-");

                                                try
                                                {
                                                    _headCount.ID_Area = int.Parse(splitedArea[0]);
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    _headCount.Nombre_Area = splitedArea[1];
                                                }
                                                catch (Exception ex)
                                                {

                                                }


                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {

                                                var valueDepartament = ws.Cell(i, 4).GetString() != "" ? ws.Cell(i, 4).GetValue<string>() : "";

                                                if (valueDepartament.Contains("_") && valueDepartament.Contains("-"))
                                                {
                                                    var CostDepartament = valueDepartament.Split("_");
                                                    var splitedDepartament = CostDepartament[0].Split("-");
                                                    try
                                                    {
                                                        _headCount.Cost_center = int.Parse(splitedDepartament[0]);
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.ID_Departamento = splitedDepartament[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = CostDepartament[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                                else if (!valueDepartament.Contains("_") && valueDepartament.Contains("-"))
                                                {
                                                    var firstSplit = valueDepartament.Split("-");
                                                    try
                                                    {
                                                        _headCount.Cost_center = int.Parse(firstSplit[0]);
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.ID_Departamento = firstSplit[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = firstSplit[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                                else if (valueDepartament.Contains("_") && !valueDepartament.Contains("-"))
                                                {
                                                    var firstSplit2 = valueDepartament.Split("_");

                                                    if (int.TryParse(firstSplit2[0], out int numero))
                                                    {
                                                        //guaramos id
                                                        _headCount.Cost_center = numero;
                                                    }
                                                    else
                                                    {
                                                        //fallo el numero asignamos default
                                                        _headCount.Cost_center = 0;
                                                    }

                                                    try
                                                    {
                                                        _headCount.ID_Departamento = firstSplit2[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = firstSplit2[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }

                                                }



                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                            try
                                            {
                                                _headCount.Nivel = ws.Cell(i, 6).GetString() != "" ? ws.Cell(i, 6).GetValue<string>() : "";

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Group = ws.Cell(i, 7).GetString() != "" ? ws.Cell(i, 7).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.BUDGET = ws.Cell(i, 8).GetString() != "" ? ws.Cell(i, 8).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                var valueHC = ws.Cell(i, 1).GetString() != "" ? ws.Cell(i, 1).Value.ToString() : "";
                                                try
                                                {
                                                    _headCount.HC = int.Parse(valueHC);
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Comentarios = ws.Cell(i, 11).GetString() != "" ? ws.Cell(i, 11).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.LABOR_TYPE = ws.Cell(i, 12).GetString() != "" ? ws.Cell(i, 12).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Fecha_de_alta = DateTime.Now;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.UserUploadId = UserIdUpload;
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                            try
                                            {
                                                _headCount.Usuario_de_alta = userEntity.Name;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            await _supervisorMobilityRepository.AddHeadCoutAsync(_headCount);


                                            retries = 0;

                                            Debug.WriteLine($"Intento {retries + 1} Linea Position [{i}]");

                                            // Si la operación tiene éxito, puedes salir del bucle
                                            break;
                                        }
                                        catch (Exception ex)
                                        {

                                            // Maneja la excepción aquí, si es necesario
                                            Debug.WriteLine($"Intento {retries + 1} Linea Position [{i}] falló: {ex.Message}");

                                            // Incrementa el número de intentos
                                            retries++;


                                            if (retries == 5)
                                            {
                                                //añade notificacion de error
                                                Notification NotyError = new Notification();
                                                NotyError.NotificationType = $"HeadCount Row Error {DateTime.Now}";
                                                NotyError.NotificationText = $"Error in data ROW [{i}], please check document and solve this issue";

                                                NotyError.MadeBy = "HeadCount System";
                                                NotyError.UserId = userEntity.UserId;
                                                NotyError.IsAccepted = true;
                                                NotyError.IsActive = true;
                                                NotyError.EntryDate = DateTime.Now;

                                                _supervisorMobilityRepository.AddNotificationAsync(NotyError); 
                                            }

                                            // Espera el intervalo de tiempo antes de volver a intentarlo
                                            await Task.Delay(retryInterval);
                                        }



                                    }//While

                                }//end is not empety row
                            }//end else first roe
                            i++;
                        }//end foreach

                        //}//for de paginas

                    }//end using woorkbook



                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en Using Woorkbook {ex.ToString()}");
                }//end trycatch to add excel to list

                int maxRetriesMail = 5; // Número máximo de intentos
                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retriesMail = 0;

                while (retriesMail < maxRetriesMail)
                {
                    try
                    {
                        //var emailMessage = _email.CreateEmailMessage(userEntity.Email, "Este es un mensaje de prueba enviado desde job observation");
                        //_email.Send(emailMessage);
                        
                        break;
                    }
                    catch (Exception ex)
                    {

                        // Maneja la excepción aquí, si es necesario
                        Debug.WriteLine($"Fallo crear Succes Notification: {ex.Message}");

                        // Incrementa el número de intentos
                        retriesMail++;
                       
                        // Espera el intervalo de tiempo antes de volver a intentarlo
                        await Task.Delay(retryIntervalMail);
                    }

                }

                // notificacion
                //añade notificacion de error

                int maxIntentos = 5; // Número máximo de intentos
                TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int intentos = 0;

                while (intentos < maxIntentos)
                {
                    try
                    {

                        Notification NotyFinish = new Notification();
                        NotyFinish.NotificationType = $"HeadCount Succes Procces {DateTime.Now}";
                        NotyFinish.NotificationText = $"Headcount document has been processed, you can now review its contents on the details page.";

                        NotyFinish.MadeBy = "HeadCount Process System ";
                        NotyFinish.UserId = userEntity.UserId;
                        NotyFinish.IsAccepted = true;
                        NotyFinish.IsActive = true;
                        NotyFinish.EntryDate = DateTime.Now;

                        _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                        await _supervisorMobilityRepository.SaveChangesAsync();
                        break;
                    }
                    catch (Exception ex)
                    {

                        // Maneja la excepción aquí, si es necesario
                        Debug.WriteLine($"Fallo crear Succes Notification: {ex.Message}");

                        // Incrementa el número de intentos
                        intentos++;
                        if (intentos == 5)
                        {
                            //añade notificacion de error
                            Notification NotyError = new Notification();
                            NotyError.NotificationType = $"HeadCount Finish: {DateTime.Now}";
                            NotyError.NotificationText = $"Finish procces document";

                            NotyError.MadeBy = "HeadCount System";
                            NotyError.UserId = userEntity.UserId;
                            NotyError.IsAccepted = true;
                            NotyError.IsActive = true;
                            NotyError.EntryDate = DateTime.Now;
                            _supervisorMobilityRepository.AddNotificationAsync(NotyError);
                        }


                        // Espera el intervalo de tiempo antes de volver a intentarlo
                        await Task.Delay(newintentTime);
                    }

                }
                await _supervisorMobilityRepository.SaveChangesAsync();


            }//end ussing scope

        }
    }
}
