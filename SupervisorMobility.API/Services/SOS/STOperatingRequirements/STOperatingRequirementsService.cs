// ====================== CORE IMPORTS ====================== //
using System.Globalization;

// ================= THIRD-PARTY LIBRARIES ================== //
using OfficeOpenXml;

// ================== DATA ACCESS IMPORTS =================== //
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;

// ====================== INTERFACES ======================= //
using SupervisorMobility.API.Interfaces.SOS;


namespace SupervisorMobility.API.Services.SOS
{
    /// <summary>
    /// Provides operations for managing Synoptic Table of Operating Requirements (STRO).
    /// Allows generation of Excel reports and access to logbook entries.
    /// </summary>
    public class STOperatingRequirementsService : ISTOperatingRequirementsService
    {
        private readonly ISOS_ProcessRepository _Sos_ProcessRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="STOperatingRequirementsService"/>.
        /// </summary>
        /// <param name="Sos_ProcessRepository">Repository used to access SOS process data.</param>
        public STOperatingRequirementsService(ISOS_ProcessRepository Sos_ProcessRepository)
        {
            _Sos_ProcessRepository = Sos_ProcessRepository;
        }


        /// <summary>
        /// Generates an Excel report for the Synoptic Table of Operating Requirements.
        /// </summary>
        /// <param name="id">ID of the Synoptic Table to generate (1 or 2).</param>
        /// <returns>A <see cref="byte"/> array representing the generated Excel file.</returns>
        /// <exception cref="Exception">
        /// Thrown if the STRO data is not found or if <c>SOSHubId</c> is null.
        /// </exception>
        public async Task<byte[]> GenerateExcelSTOperatingRequirements(int id)
        {
            SOSSynopticTableofOperatingRequirements SOSSynopticTableofOperatingRequirements = await _Sos_ProcessRepository.GetSOSSynopticTableofOperatingRequirements(id, true, true, true) ?? throw new Exception("Data not found");

            if (SOSSynopticTableofOperatingRequirements.SOSHubId == null) throw new Exception("SOSHubId is null");
            SOSHub sosHub = await _Sos_ProcessRepository.GetSOSHub((int)SOSSynopticTableofOperatingRequirements.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);


            string templateName = id == 1 ? "DataAccess/Templates/Template_CSRO.xlsx" : "DataAccess/Templates/Template_CSRO_1.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                if (id == 1)
                {
                    var sheet = package.Workbook.Worksheets.First();

                    // === Fill headers ===
                    sheet.Cells["A6"].Value = SOSSynopticTableofOperatingRequirements.ProcessName;
                    sheet.Cells["H6"].Value = sosHub?.Department?.Description;
                    sheet.Cells["K6"].Value = sosHub?.Plant?.Description;
                    sheet.Cells["O6"].Value = DateFormat(SOSSynopticTableofOperatingRequirements.CreatedAt);

                    // === Fill creators, reviewer, approver ===
                    sheet.Cells["A9"].Value = SOSSynopticTableofOperatingRequirements.Creator?.Name;
                    sheet.Cells["F9"].Value = SOSSynopticTableofOperatingRequirements.Reviewer?.Name;
                    sheet.Cells["L9"].Value = SOSSynopticTableofOperatingRequirements.Approver?.Name;

                    // === Fill logbooks ===
                    for (int index = 0; index <= 3; index++)
                    {
                        int row = 9 - index;
                        if (TryGetSynopticRequirementsLogbooksElementAtIndex(index, out var item, SOSSynopticTableofOperatingRequirements))
                        {
                            sheet.Cells[$"V{row}"].Value = item?.Approver?.Name;
                            sheet.Cells[$"Y{row}"].Value = SOSSynopticTableofOperatingRequirements?.SynopticRequirementsLogbooks?.Count;
                            sheet.Cells[$"AA{row}"].Value = item?.Changes;
                            sheet.Cells[$"AG{row}"].Value = DateFormat(item?.Date);
                            sheet.Cells[$"AH{row}"].Value = item?.NoRevision == 0 ? "N" : item?.NoRevision;
                        }
                    }

                    // === Fill body table ===
                    sheet.Cells["B16"].Value = $"{sosHub?.Folio} ({sosHub?.ProcessSheet})";

                }

                if (id == 2)
                {
                    var sheet = package.Workbook.Worksheets["CSRO 1.1"];
                    // === headers of CSRO === \
                    // name, departament, plant and date
                    sheet.Cells["B5"].Value = SOSSynopticTableofOperatingRequirements.ProcessName;
                    sheet.Cells["I5"].Value = sosHub?.Department?.Description;
                    sheet.Cells["N5"].Value = sosHub?.Plant?.Description;
                    sheet.Cells["R5"].Value = DateFormat(SOSSynopticTableofOperatingRequirements.CreatedAt);

                    // created, reviwer and approver
                    sheet.Cells["B8"].Value = SOSSynopticTableofOperatingRequirements.Creator?.Name;
                    sheet.Cells["H8"].Value = SOSSynopticTableofOperatingRequirements.Reviewer?.Name;
                    sheet.Cells["N8"].Value = SOSSynopticTableofOperatingRequirements.Approver?.Name;
                    sheet.Cells["T8"].Value = 1;
                    sheet.Cells["U8"].Value = 1;

                    // table of users approved
                    for (int index = 0; index <= 3; index++)
                    {
                        int row = 8 - index;
                        if (TryGetSynopticRequirementsLogbooksElementAtIndex(index, out var item, SOSSynopticTableofOperatingRequirements))
                        {
                            sheet.Cells[$"W{row}"].Value = item?.Approver?.Name;
                            sheet.Cells[$"Y{row}"].Value = SOSSynopticTableofOperatingRequirements?.SynopticRequirementsLogbooks?.Count;
                            sheet.Cells[$"Z{row}"].Value = item?.Changes;
                            sheet.Cells[$"AD{row}"].Value = DateFormat(item?.Date);
                            sheet.Cells[$"AF{row}"].Value = item?.NoRevision == 0 ? "N" : item?.NoRevision;
                        }
                    }

                    // body table
                    sheet.Cells["B13"].Value = 1;
                    sheet.Cells["C13"].Value = $"{sosHub?.Folio} ({sosHub?.ProcessSheet})";
                }


                package.SaveAs(ms);

            }

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> into a string in the format "dd/MM/yyyy hh:mm:ss tt".
        /// Returns an empty string if the date is null.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>Formatted date string in uppercase, or empty if null.</returns>
        private string DateFormat(DateTime? date)
        {
            if (!date.HasValue) return "";

            string language = CultureInfo.CurrentCulture.Name ?? "es-MX";
            CultureInfo cultureInfo = new CultureInfo(language);

            return date.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultureInfo).ToUpper();
        }


        /// <summary>
        /// Tries to retrieve a logbook from the Synoptic Table by an inverted index.
        /// </summary>
        /// <param name="index">Zero-based index from the end of the logbook collection.</param>
        /// <param name="item">The retrieved <see cref="SOSSynopticRequirementsLogbook"/> item, if found.</param>
        /// <param name="SOSSynopticTableofOperatingRequirements">The Synoptic Table containing the logbooks.</param>
        /// <returns>True if the item exists at the given index; otherwise false.</returns>
        public bool TryGetSynopticRequirementsLogbooksElementAtIndex(int index, out SOSSynopticRequirementsLogbook? item, SOSSynopticTableofOperatingRequirements SOSSynopticTableofOperatingRequirements)
        {
            item = null;

            ICollection<SOSSynopticRequirementsLogbook>? SSRlogBook = SOSSynopticTableofOperatingRequirements?.SynopticRequirementsLogbooks;
            if (SSRlogBook == null || SSRlogBook.Count == 0) return false;


            int invertedIndex = SSRlogBook.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < SSRlogBook.Count)
            {
                item = SSRlogBook.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }
    }
}
