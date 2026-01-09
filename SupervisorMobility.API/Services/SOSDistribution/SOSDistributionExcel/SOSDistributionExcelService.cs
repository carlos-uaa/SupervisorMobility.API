using OfficeOpenXml;
using System.Drawing;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel;
using OfficeOpenXml.Drawing;
using SupervisorMobility.API.DataAccess.Services.SOS_DistributionRepository;

namespace SupervisorMobility.API.Services.SOSDistribution.SOSDistributionExcel
{
    public class SOSDistributionExcelService : ISOSDistributionExcelService
    {
        private readonly ISOS_ProcessRepository _AnalysisProcessRepository;
        private readonly ISOS_DistributionRepository _DistributionRepository;
        public SOSDistributionExcelService(ISOS_ProcessRepository analysisProcessRepository, ISOS_DistributionRepository distributionRepository)
        {
            _AnalysisProcessRepository = analysisProcessRepository;
            _DistributionRepository = distributionRepository;
        }

        public async Task<string?> GetFileName(int sosDistributionId)
        {
            try
            {
                var response = await _DistributionRepository.GetDistributionName(sosDistributionId);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("INNER: " + ex.InnerException);
                return null;
            }
            return null;
        }
     
        public async Task<MemoryStream?> ExportSOSDistributionExcel(int sosDistributionId)
        {
            try
            {
                // Get the SOS Distribution
                var SosDistribution = await _DistributionRepository.GetSOSDistribution(sosDistributionId, true, true, true, true, includeTurns: true, includeTimes: true, includeCollections: true);
                if (SosDistribution == null)
                {
                    return null;
                }

                // Get the SOS Hub
                SOSHub Sos_Hub = await _AnalysisProcessRepository.GetSOSHub((int)SosDistribution.SOSHubId, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
                if (Sos_Hub == null)
                {
                    return null;
                }

                // Get the Distribution Excel Tenplate & Open it
                string templateName = "DataAccess/Templates/Distribution Template.xlsx";
                MemoryStream ms = new MemoryStream();

                using var templateStream = System.IO.File.OpenRead(templateName);

                using (var package = new ExcelPackage(templateStream))
                {
                    // Get the first worksheet in the workbook
                    var sheet = package.Workbook.Worksheets.First();

                    // Fill in the Header Information
                    sheet.Cells["B8"].Value = SosDistribution.ProcessName;
                    sheet.Cells["G8"].Value = SosDistribution.InternalControlNumber;

                    sheet.Cells["B10"].Value = SosDistribution.DistributionLogbooks?.First().Approver?.Name;
                    sheet.Cells["E10"].Value = Sos_Hub?.ReviewerEditors?.First()?.Name;
                    sheet.Cells["G10"].Value = Sos_Hub?.ApproverOwners?.First().Name;

                    sheet.Cells["B12"].Value = SosDistribution.CreatedAt?.ToString("dd-MMM-yyyy").Replace(".", "");
                    sheet.Cells["D12"].Value = SosDistribution.ApplicationMonth;
                    if (SosDistribution.SOSHubs?.Count() > 0)
                        sheet.Cells["G12"].Value = SosDistribution.SOSHubs.SelectMany(s => s.AppliedModels).Select(a => a.Code);
                    sheet.Cells["I12"].Value = SosDistribution.TackTime;
                    sheet.Cells["J12"].Value = Sos_Hub?.TrainingTime;
                    sheet.Cells["P12"].Value = Sos_Hub?.Plant?.Code;
                    sheet.Cells["U12"].Value = Sos_Hub?.Department?.Code;
                    sheet.Cells["X12"].Value = 1;
                    sheet.Cells["Z12"].Value = 1;

                    if (SosDistribution.Turns != null && SosDistribution.Turns.Any())
                    {
                        int i = 0;
                        int base_row = 8;
                        int max = Math.Min(3, SosDistribution.Turns.Count);

                        do
                        {
                            base_row += i;
                            var turn = SosDistribution.Turns.ElementAt(i);
                            sheet.Cells[$"K{base_row}"].Value = turn.TurnType;
                            sheet.Cells[$"M{base_row}"].Value = turn.Operator.Name;
                            sheet.Cells[$"V{base_row}"].Value = turn.Supervisor.Name;
                            i++;
                        } while (i < max);
                    }

                    // Aplication Models
                    string[] models = SosDistribution.AplicationModels.Split("§", StringSplitOptions.RemoveEmptyEntries);
                    string[] modelsCols = { "L15", "M15", "N15", "O15", "Q15" };
                    if (models.Any())
                    {   
                        for (int j = 0; j < Math.Min(models.Length, modelsCols.Length); j++)
                        {
                            if (!string.IsNullOrWhiteSpace(models[j]) && !models[j].Contains("§"))
                                sheet.Cells[$"{modelsCols[j]}"].Value = models[j];
                        }
                    }

                    // Starting distribution section
                    int start_row = 16;
                    int last_row = 49;
                    int current_row = start_row;
                    int count = 1;
                    int addedCount = 0;
                    string[] columns = { "L", "M", "N", "O", "Q" }; // Standard Time columns

                    foreach (var sequence in SosDistribution.SOSDistributionOperationSequence)
                    {
                        // Sequence Number & Sequence Step
                        sheet.Cells[$"B{current_row}"].Value = count;
                        sheet.Cells[$"C{current_row}"].Value = sequence.Section.Step;

                        // Standard Time per model
                        var times = SosDistribution.SOSDistributionOperationSequence.ToList()[count - 1].Times.Split("§");

                        if (times.Any())
                        {
                            for (int j = 0; j < Math.Min(times.Length, columns.Length); j++)
                            {
                                if (!string.IsNullOrWhiteSpace(times[j]) && !times[j].Contains(" "))
                                {
                                    string cellAddress = $"{columns[j]}{current_row}";
                                    sheet.Cells[cellAddress].Value = times[j];
                                }
                            }
                        }

                        // Critical Points & Reasons
                        foreach (var analysis in sequence.Section.Analyses)
                        {
                            if (analysis.CriticalPoints != null && analysis.CriticalPoints.Any())
                            {
                                foreach (var (cp, cpIndex) in analysis.CriticalPoints.Select((cp, cpIndex) => (cp, cpIndex)))
                                {
                                    string indexString = $"{cpIndex + 1}.- ";
                                    string critString = $"{cp}";

                                    // Escribe en la columna H, fila actual
                                    var cell = sheet.Cells[$"G{current_row}"];
                                    cell.Value = $"{indexString}{critString}";
                                    cell.Style.WrapText = true;

                                    // --- Cálculo de altura dinámica ---
                                    int charsPerLine = 35;
                                    int totalChars = cell.Value.ToString().Length;
                                    int estimatedLines = (int)Math.Ceiling((double)totalChars / charsPerLine);

                                    // Altura base por línea (puedes ajustar este valor según tu fuente/tamaño)
                                    double heightPerLine = 15.0;
                                    double finalHeight = estimatedLines * heightPerLine;

                                    var row = sheet.Row(current_row);
                                    row.CustomHeight = true;
                                    row.Height = finalHeight;

                                    current_row++; // Avanza a la siguiente fila
                                }
                            }
                        }

                        // Validate if we need to insert a new row
                        if (current_row >= last_row && count < SosDistribution.SOSDistributionOperationSequence.Count)
                        {
                            // Inserta new row
                            sheet.InsertRow(current_row + 1, 1);
                            addedCount++;

                            // Copy row style from row 16
                            if (sheet.Dimension != null)
                            {
                                var sourceRow = sheet.Cells[16, 1, 16, sheet.Dimension.End.Column];
                                var targetRow = sheet.Cells[current_row + 1, 1, current_row + 1, sheet.Dimension.End.Column];

                                sourceRow.Copy(targetRow);

                                // Clean content
                                for (int col = 1; col <= sheet.Dimension.End.Column; col++)
                                {
                                    sheet.Cells[current_row + 1, col].Value = null;
                                }
                            }
                        }

                        count++;
                        current_row++;
                    }

                    // Insert Images
                    const string basePath = "uploads/SOSDistribution/Ilustrations/";
                    const double maxInches = 4.26;
                    const float dpi = 96f;
                    int maxPixels = (int)(maxInches * dpi / 2);

                    var targetCells = new[] { "T16", "T25" };
                    int index = 0;

                    foreach (var illustration in SosDistribution.Illustrations.Take(2))
                    {
                        string encryptedPath = Path.Combine(basePath, illustration.StorageFileName);
                        string realExtension = Path.GetExtension(illustration.FileName)?.ToLowerInvariant();
                        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        if (!validExtensions.Contains(realExtension)) continue;
                        if (!File.Exists(encryptedPath)) continue;

                        string tempFile = Path.ChangeExtension(Path.GetTempFileName(), realExtension);
                        File.Copy(encryptedPath, tempFile, true);

                        var picture = sheet.Drawings.AddPicture($"img_{Guid.NewGuid()}", new FileInfo(tempFile));
                        picture.SetSize(maxPixels, maxPixels);
                        picture.EditAs = eEditAs.OneCell;

                        var cellAddress = targetCells[index];
                        var cell = sheet.Cells[cellAddress];
                        int anchorRow = cell.Start.Row - 1;

                        var mergeRange = cell.Merge ? cell.Address : cellAddress;
                        int startCol = sheet.Cells[mergeRange].Start.Column;
                        int endCol = sheet.Cells[mergeRange].End.Column;

                        // Calcular ancho total del rango en píxeles
                        double totalWidthPixels = 0;
                        for (int c = startCol; c <= endCol; c++)
                            totalWidthPixels += sheet.Column(c).Width * 7; // 1 unidad de ancho ≈ 7px

                        // Calcular desplazamiento horizontal desde startCol
                        double leftPadding = (totalWidthPixels - maxPixels) / 2;

                        // Convertir a desplazamiento en píxeles dentro de la columna
                        int offsetPixels = (int)leftPadding;

                        picture.SetPosition(anchorRow, 0, startCol, offsetPixels);
                        index++;
                    }

                    // Set Aditional Total Time & Cycle time
                    var aditionalTimes = SosDistribution.AdditionalTime.Split("§");
                    var currentItemColumn = last_row + 2 + addedCount;
                    if (aditionalTimes.Any())
                    {
                        for (int j = 0; j < Math.Min(aditionalTimes.Length, columns.Length); j++)
                        {
                            if (!string.IsNullOrWhiteSpace(aditionalTimes[j]) && !aditionalTimes[j].Contains(" "))
                            {
                                string cellAddress = $"{columns[j]}{currentItemColumn}";
                                sheet.Cells[cellAddress].Value = aditionalTimes[j];
                            }
                        }
                    }
                    var cyclesTimes = SosDistribution.CycleTime.Split("§");
                    if (cyclesTimes.Any())
                    {
                        for (int j = 0; j < Math.Min(cyclesTimes.Length, columns.Length); j++)
                        {
                            if (!string.IsNullOrWhiteSpace(cyclesTimes[j]) && !cyclesTimes[j].Contains(" "))
                            {
                                string cellAddress = $"{columns[j]}{currentItemColumn + 1}";
                                sheet.Cells[cellAddress].Value = cyclesTimes[j];
                            }
                        }
                    }

                    // Set Quantity table
                    int materialCount = 1;
                    string[] cols = { "T", "U", "V", "W", "X" };
                    int currentAlternativeRow = 36;

                    // Models
                    if (models.Any())
                    {
                        for (int j = 0; j < Math.Min(models.Length, cols.Length); j++)
                            if (!string.IsNullOrWhiteSpace(models[j]) && !models[j].Contains("§"))
                                sheet.Cells[$"{cols[j]}{currentAlternativeRow}"].Value = models[j];
                    }
                    //Quantities
                    var takeQuanitties = SosDistribution.SOSDistributionAdditionalTime.TakeQuantity.Split("§");
                    var leaveQuanitties = SosDistribution.SOSDistributionAdditionalTime.LeaveQuantity.Split("§");
                    var stepQuanitties = SosDistribution.SOSDistributionAdditionalTime.StepsQuantity.Split("§");
                    var takeTime = SosDistribution.SOSDistributionAdditionalTime.TakeTime.Split("§").First();
                    var leaveTime = SosDistribution.SOSDistributionAdditionalTime.LeaveTime.Split("§").First();
                    var stepTime = SosDistribution.SOSDistributionAdditionalTime.StepsTime.Split("§").First();

                    if (takeQuanitties.Any())
                    {
                        for (int j = 0; j < Math.Min(takeQuanitties.Length, cols.Length); j++)
                            if (!string.IsNullOrWhiteSpace(takeQuanitties[j]) && !takeQuanitties[j].Contains("§"))
                                sheet.Cells[$"{cols[j]}{currentAlternativeRow + 1}"].Value = takeQuanitties[j];
                    }
                    if (leaveQuanitties.Any())
                    {
                        for (int j = 0; j < Math.Min(leaveQuanitties.Length, cols.Length); j++)
                            if (!string.IsNullOrWhiteSpace(leaveQuanitties[j]) && !leaveQuanitties[j].Contains("§"))
                                sheet.Cells[$"{cols[j]}{currentAlternativeRow + 2}"].Value = leaveQuanitties[j];
                    }
                    if (stepQuanitties.Any())
                    {
                        for (int j = 0; j < Math.Min(stepQuanitties.Length, cols.Length); j++)
                            if (!string.IsNullOrWhiteSpace(stepQuanitties[j]) && !stepQuanitties[j].Contains("§"))
                                sheet.Cells[$"{cols[j]}{currentAlternativeRow + 3}"].Value = stepQuanitties[j];
                    }
                    // Total Quantities
                    for (int j = 0; j < cols.Length; j++)
                    {
                        double total = 0;

                        // Validate & sum take
                        if (takeQuanitties.Length > j && double.TryParse(takeQuanitties[j], out double takeVal))
                            total += takeVal;
                        // Validate & sum leave
                        if (leaveQuanitties.Length > j && double.TryParse(leaveQuanitties[j], out double leaveVal))
                            total += leaveVal;
                        // Validate & sum step
                        if (stepQuanitties.Length > j && double.TryParse(stepQuanitties[j], out double stepVal))
                            total += stepVal;
                        // Write the value if it has any value
                        if (total != 0)
                            sheet.Cells[$"{cols[j]}{currentAlternativeRow + 4}"].Value = total;
                    }
                    // TIME
                    sheet.Cells[$"Y{currentAlternativeRow + 1}"].Value = takeTime;
                    sheet.Cells[$"Y{currentAlternativeRow + 2}"].Value = leaveTime;
                    sheet.Cells[$"Y{currentAlternativeRow + 3}"].Value = stepTime;

                    // Set Material table
                    var equipment = Sos_Hub.SafetyEquipment.ToList();
                    int alternativeTablesStartRow = 43;
                    currentAlternativeRow = alternativeTablesStartRow;
                    if (equipment != null && equipment.Count > 0)
                        foreach (var item in equipment)
                        {
                            if (materialCount > 10) break;

                            sheet.Cells[$"U{currentAlternativeRow}"].Value = item.EquipmentName;

                            //if (currentAlternativeRow < alternativeTablesStartRow + 6 && materialCount < equipment.Count)
                            //{
                            //    // Insert new row
                            //    sheet.InsertRow(currentAlternativeRow + 1, 1);

                            //    // Copy row style from row 23 + count
                            //    if (sheet.Dimension != null)
                            //    {
                            //        var sourceRow = sheet.Cells[alternativeTablesStartRow, 1, alternativeTablesStartRow, sheet.Dimension.End.Column];
                            //        var targetRow = sheet.Cells[currentAlternativeRow + 1, 1, currentAlternativeRow + 1, sheet.Dimension.End.Column];
                            //        sourceRow.Copy(targetRow);
                            //        // Clean content
                            //        for (int col = 1; col <= sheet.Dimension.End.Column; col++)
                            //            sheet.Cells[currentAlternativeRow + 1, col].Value = null;
                            //    }
                            //}

                            materialCount++;
                            currentAlternativeRow++;
                        }

                    // Protect the sheet and save
                    sheet.Protection.IsProtected = true;
                    package.SaveAs(ms);
                }
                ms.Position = 0;

                return ms;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: "+ex.Message);
                Console.WriteLine("INNER: " + ex.InnerException);
                return null;
            }
        }
    }
}
