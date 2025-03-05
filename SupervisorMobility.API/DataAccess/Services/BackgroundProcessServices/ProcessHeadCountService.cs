using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.DataAccess.Services.BackgroundProcessServices
{
    public class ProcessHeadCountService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProcessHeadCountService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessDocumentHeadCountAsync(string trustedFileNameForStorage, int UserIdUpload, CancellationToken stoppingToken)
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
                string messageError = "";
                try
                {
                    // Abrir archivo Excel
                    using (SpreadsheetDocument document = SpreadsheetDocument.Open(filepath, false))
                    {
                        WorkbookPart workbookPart = document.WorkbookPart;
                        Sheet sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().FirstOrDefault();

                        if (sheet != null)
                        {
                            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                            bool firstRow = true;
                            int i = 1;

                            foreach (Row row in sheetData.Elements<Row>())
                            {
                                HeadCount _headCount = new HeadCount();

                                if (firstRow)
                                {
                                    firstRow = false; // Ignorar la primera fila (cabecera)
                                                      // Columna 1 A: Código
                                                      // Columna 2 B: CO
                                                      // Columna 3 C: ID y nombre de área
                                                      // Columna 4 D: Departamento
                                                      // Columna 5 E: Extraer y procesar valor
                                                      // Columna 6 F: Nivel
                                                      // Columna 7 G: Grupo
                                                      // Columna 8 H: Presupuesto
                                                      // Columna 9 I: RTO
                                                      // Columna 11 J: Comentarios
                                                      // Columna 12 L: Tipo de labor
                                                      // Columna 13 M: Line Operative 
                                                      // Columna 14 N: DSTR
                                                      // Columna 15 O: Fuction_Type
                                }
                                else
                                {
                                    if (row.Elements<Cell>().Any())
                                    {
                                        int retries = 0;
                                        const int maxRetries = 5;
                                        TimeSpan retryInterval = TimeSpan.FromSeconds(5);

                                        while (retries < maxRetries)
                                        {
                                            try
                                            {
                                                var cells = row.Elements<Cell>().ToList();

                                                // Columna 5: Extraer y procesar valor
                                                string valueFunctionDescription = GetCellValue(workbookPart, cells.ElementAtOrDefault(4))?.Trim() ?? "";

                                                if (valueFunctionDescription.Any(char.IsDigit))
                                                {
                                                    string numberString = new string(valueFunctionDescription.Where(char.IsDigit).ToArray());
                                                    _headCount.ID_subarea = int.TryParse(numberString, out int numero) ? numero : 0;
                                                }
                                                else
                                                {
                                                    _headCount.ID_subarea = 0;
                                                }

                                                _headCount.nombre_subarea = valueFunctionDescription;

                                                // Columna 15: Fuction_Type
                                                _headCount.Fuction_Type = GetCellValue(workbookPart, cells.ElementAtOrDefault(14))?.Trim() ?? "";

                                                // Columna 9: RTO
                                                _headCount.RTO = GetCellValue(workbookPart, cells.ElementAtOrDefault(8)) ?? "";

                                                // Columna 1: Código
                                                _headCount.Codigo = int.TryParse(GetCellValue(workbookPart, cells.ElementAtOrDefault(0)), out int code) ? code : -1;

                                                // Columna 2: CO
                                                _headCount.CO = GetCellValue(workbookPart, cells.ElementAtOrDefault(1)) ?? "";

                                                // Columna 3: ID y nombre de área
                                                string valuesArea = GetCellValue(workbookPart, cells.ElementAtOrDefault(2)) ?? "";
                                                var splitedArea = valuesArea.Split("-");
                                                _headCount.ID_Area = int.TryParse(splitedArea.ElementAtOrDefault(0), out int areaId) ? areaId : 0;
                                                _headCount.Nombre_Area = splitedArea.ElementAtOrDefault(1) ?? "";

                                                // Columna 4: Departamento
                                                string valueDepartament = GetCellValue(workbookPart, cells.ElementAtOrDefault(3)) ?? "";
                                                ProcessDepartment(valueDepartament, _headCount);

                                                // Columna 6: Nivel
                                                _headCount.Nivel = GetCellValue(workbookPart, cells.ElementAtOrDefault(5)) ?? "";

                                                // Columna 7: Grupo
                                                _headCount.Group = GetCellValue(workbookPart, cells.ElementAtOrDefault(6)) ?? "";

                                                // Columna 8: Presupuesto
                                                _headCount.BUDGET = GetCellValue(workbookPart, cells.ElementAtOrDefault(7)) ?? "";

                                                // Columna 11: Comentarios
                                                _headCount.Comentarios = GetCellValue(workbookPart, cells.ElementAtOrDefault(10)) ?? "";

                                                // Columna 12: Tipo de labor
                                                _headCount.LABOR_TYPE = GetCellValue(workbookPart, cells.ElementAtOrDefault(11)) ?? "";

                                                // Datos adicionales
                                                _headCount.Fecha_de_alta = DateTime.Now;
                                                _headCount.UserUploadId = UserIdUpload;
                                                _headCount.Usuario_de_alta = userEntity.Name;

                                                await _supervisorMobilityRepository.AddHeadCoutAsync(_headCount);

                                                Debug.WriteLine($"Intento {retries + 1} Línea [{i}] completado");
                                                break; // Operación exitosa
                                            }
                                            catch (Exception ex)
                                            {
                                                retries++;
                                                Debug.WriteLine($"Intento {retries} en Línea [{i}] falló: {ex.Message}");

                                                if (retries == maxRetries)
                                                {
                                                    messageError += $"Error en la fila [{i}]. Verifica el documento.\n";
                                                }

                                                await Task.Delay(retryInterval);
                                            }
                                        }
                                    }
                                }

                                i++;
                            }
                        }
                    }
                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en Using Woorkbook {ex.ToString()}");
                }//end trycatch to add excel to list

                int maxRetriesMail = 2; // Número máximo de intentos
                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retriesMail = 0;

                //while (retriesMail < maxRetriesMail)
                //{
                try
                {


                    if (!string.IsNullOrEmpty(messageError))
                    {
                        var emailMessageError = _email.CreateEmailMessage(userEntity.Email, "Headcount processed", $"Headcount document has been processed, you can now review its contents on the details page. \n LIST ERRORS:  \n" + messageError);
                        _email.Send(emailMessageError);
                    }
                    else
                    {
                        var emailMessage = _email.CreateEmailMessage(userEntity.Email, "Headcount processed", $"Headcount document has been processed, you can now review its contents on the details page.");
                        _email.Send(emailMessage);
                    }
                    //break;
                }
                catch (Exception ex)
                {

                    // Maneja la excepción aquí, si es necesario
                    Debug.WriteLine($"Fallo send Succes e-mail: {ex.Message}");

                    // Incrementa el número de intentos
                    retriesMail++;

                    // Espera el intervalo de tiempo antes de volver a intentarlo
                    await Task.Delay(retryIntervalMail);

                    Notification NotyError = new Notification();
                    NotyError.NotificationType = $"HeadCount Error Succes e-mail {DateTime.Now}";
                    NotyError.NotificationText = messageError;

                    NotyError.MadeBy = "HeadCount System";
                    NotyError.UserId = userEntity.UserId;
                    NotyError.IsAccepted = true;
                    NotyError.IsActive = true;
                    NotyError.EntryDate = DateTime.Now;

                    _supervisorMobilityRepository.AddNotificationAsync(NotyError);
                }

                //}

                // notificacion
                //añade notificacion de error

                int maxIntentos = 3; // Número máximo de intentos
                TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int intentos = 0;

                while (intentos < maxIntentos)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(messageError))
                        {


                            Notification NotyFinish = new Notification();
                            NotyFinish.NotificationType = $"HeadCount Procces - Succes With Errors  {DateTime.Now}";
                            NotyFinish.NotificationText = $"Headcount document has been processed, you can now review its contents on the details page. \n LIST ERRORS:  \n" + messageError;

                            NotyFinish.MadeBy = "HeadCount Process System ";
                            NotyFinish.UserId = userEntity.UserId;
                            NotyFinish.IsAccepted = true;
                            NotyFinish.IsActive = true;
                            NotyFinish.EntryDate = DateTime.Now;

                            _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                        }
                        else
                        {
                            Notification NotyFinish = new Notification();
                            NotyFinish.NotificationType = $"HeadCount Procces - Succes  {DateTime.Now}";
                            NotyFinish.NotificationText = $"Headcount document has been processed, you can now review its contents on the details page.";

                            NotyFinish.MadeBy = "HeadCount Process System ";
                            NotyFinish.UserId = userEntity.UserId;
                            NotyFinish.IsAccepted = true;
                            NotyFinish.IsActive = true;
                            NotyFinish.EntryDate = DateTime.Now;

                            _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                        }



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

        // Método para obtener valor de una celda
        string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null || cell.CellValue == null) return null;

            string value = cell.CellValue.Text;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
            }

            return value;
        }

        // Método para procesar departamento
        void ProcessDepartment(string valueDepartament, HeadCount _headCount)
        {
            if (valueDepartament.Contains("_") && valueDepartament.Contains("-"))
            {
                var parts = valueDepartament.Split("_");
                var splitedDepartament = parts[0].Split("-");
                _headCount.Cost_center = int.TryParse(splitedDepartament.ElementAtOrDefault(0), out int costCenter) ? costCenter : 0;
                _headCount.ID_Departamento = splitedDepartament.ElementAtOrDefault(1);
                _headCount.Nombre_Departamento = parts.ElementAtOrDefault(1);
            }
            else if (!valueDepartament.Contains("_") && valueDepartament.Contains("-"))
            {
                var splitedDepartament = valueDepartament.Split("-");
                _headCount.Cost_center = int.TryParse(splitedDepartament.ElementAtOrDefault(0), out int costCenter) ? costCenter : 0;
                _headCount.ID_Departamento = splitedDepartament.ElementAtOrDefault(1);
                _headCount.Nombre_Departamento = splitedDepartament.ElementAtOrDefault(1);
            }
            else if (valueDepartament.Contains("_"))
            {
                var parts = valueDepartament.Split("_");
                _headCount.Cost_center = int.TryParse(parts.ElementAtOrDefault(0), out int costCenter) ? costCenter : 0;
                _headCount.ID_Departamento = parts.ElementAtOrDefault(1);
                _headCount.Nombre_Departamento = parts.ElementAtOrDefault(1);
            }
        }


    }//end ProcessHeadCountService

}//end Namespace
