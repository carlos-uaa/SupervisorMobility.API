using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SupervisorMobility.API.DataAccess.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class ExportationController : ControllerBase
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IWebHostEnvironment _env;

        public ExportationController(ISOSAnalysis_ProcessRepository repository, IWebHostEnvironment env)
        {
            _AnalysisProcessRepository = repository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpGet("Excel/Analyses/{AnalysisId}")]
        public async Task<IActionResult> AnalysesExcelExport(int AnalysisId)
        {
            var analysis = await _AnalysisProcessRepository.GetSOSAnalysis(AnalysisId, true, true, true, true, true, true);

            string templateName = "DataAccess/Templates/Analysis Template.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                var sheet = package.Workbook.Worksheets["Analysis A"];

                #region information table

                sheet.Cells["D4"].Value = analysis.OperationName;
                sheet.Cells["D6"].Value = analysis.InternalControlNumber;
                sheet.Cells["G6"].Value = analysis.ProcessName;
                string SecurityEq = "";
                if (analysis.SOSHub?.SafetyEquipment != null && analysis.SOSHub.SafetyEquipment.Any())
                {
                    SecurityEq = string.Join(", ", analysis.SOSHub.SafetyEquipment.Select(se=>se.EquipmentName));
                }
                sheet.Cells["D7"].Value = SecurityEq;
                string Tools = "";
                if (analysis.SOSHub?.ToolsUsed != null && analysis.SOSHub.ToolsUsed.Any())
                {
                    Tools = string.Join(", ", analysis.SOSHub.ToolsUsed.Select(tu => tu.ToolName));
                }
                sheet.Cells["D8"].Value = Tools;
                sheet.Cells["D9"].Value = analysis.SOSHub.AppliedModel?.Description;
                sheet.Cells["D10"].Value = analysis.SOSHub.TrainingTime;

                #endregion

                // Save to file
                package.SaveAs(ms);
            }

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.IsNullOrEmpty(analysis.InternalControlNumber)? $"{analysis.InternalControlNumber} Analysis Report.xlsx" : "Analysis Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}
