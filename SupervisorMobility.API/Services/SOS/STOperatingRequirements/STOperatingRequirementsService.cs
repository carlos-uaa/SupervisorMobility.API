// - Core .NET imports
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;

// - External imports
using OfficeOpenXml;
using SpreadsheetLight;
using OfficeOpenXml.Style;

// - Context imports
using SupervisorMobility.API.Interfaces.SOS;
using SupervisorMobility.API.DataAccess.Services;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Enums;
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Skills;
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Knowledges;


namespace SupervisorMobility.API.Services.SOS
{
    /// <summary>
    /// Provides operations for managing Synoptic Table of Operating Requirements (STRO).
    /// Allows generation of Excel reports and access to related data (hubs, distributions, knowledge, skills).
    /// </summary>
    public class STOperatingRequirementsService : ISTOperatingRequirementsService
    {
        //+====================== SERVICES ======================+\\
        private readonly ISOS_ProcessRepository _Sos_ProcessRepository;
        private readonly IKnowledgeRepository _KnowledgeRepository;
        private readonly ISkillRepository _SkillRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="STOperatingRequirementsService"/>.
        /// </summary>
        /// <param name="Sos_ProcessRepository">Repository used to access SOS process data.</param>
        /// <param name="knowledgeRepository">Repository used to access knowledge data.</param>
        /// <param name="skillRepository">Repository used to access skill data.</param>
        public STOperatingRequirementsService(ISOS_ProcessRepository Sos_ProcessRepository, IKnowledgeRepository knowledgeRepository, ISkillRepository skillRepository)
        {
            _Sos_ProcessRepository = Sos_ProcessRepository;
            _KnowledgeRepository = knowledgeRepository;
            _SkillRepository = skillRepository;
        }

        // =================================================== \\
        //&================ REPORT GENERATION ================&\\
        // =================================================== \\

        /// <summary>
        /// Generates an Excel report for the Synoptic Table of Operating Requirements (STRO).
        /// The report is dynamically built using a predefined template and populated with
        /// process information, distributions, sequences, analyses, knowledges, and skills.
        /// </summary>
        /// <param name="id">ID of the Synoptic Table to generate (typically 1 or 2).</param>
        /// <returns> A byte array representing the generated Excel file.</returns>
        /// <exception cref="Exception">Thrown if the STRO data is not found or if <c>SOSHubId</c> is null.</exception>
        public async Task<byte[]> GenerateExcelSTOperatingRequirements(int id)
        {
            //-============ DATA FETCHING =============-\\
            SOSSynopticTableofOperatingRequirements SOSSTRO = await _Sos_ProcessRepository.GetSOSSynopticTableofOperatingRequirements(id, true, true, true) ?? throw new Exception("Data not found");
            SOSSTRO.SOSHubs = await GetHubsWithDistribution(SOSSTRO);

            if (SOSSTRO.SOSHubId == null) throw new Exception("SOSHubId is null");
            SOSHub sosHub = await _Sos_ProcessRepository.GetSOSHub((int)SOSSTRO.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);

            //-============ TEMPLATE SETUP =============-\\
            string templateName = "DataAccess/Templates/Template_CSRO.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);
            using (var package = new ExcelPackage(templateStream))
            {
                var sheet = package.Workbook.Worksheets["CSRO 1.1"];
                // === Header Information === \\
                // name, departament, plant and date
                sheet.Cells["B5"].Value = SOSSTRO.ProcessName;
                sheet.Cells["I5"].Value = sosHub?.Department?.Description;
                sheet.Cells["N5"].Value = sosHub?.Plant?.Description;
                sheet.Cells["R5"].Value = DateFormat(SOSSTRO.CreatedAt);

                // created, reviwer and approver
                sheet.Cells["B8"].Value = SOSSTRO.Creator?.Name;
                sheet.Cells["H8"].Value = SOSSTRO.Reviewer?.Name;
                sheet.Cells["N8"].Value = SOSSTRO.Approver?.Name;
                sheet.Cells["T8"].Value = 1;
                sheet.Cells["U8"].Value = 1;

                // === Table of Approved Users === \\
                for (int index = 0; index <= 3; index++)
                {
                    int row = 8 - index;
                    if (TryGetSynopticRequirementsLogbooksElementAtIndex(index, out var item, SOSSTRO))
                    {
                        sheet.Cells[$"W{row}"].Value = item?.Approver?.Name;
                        sheet.Cells[$"Y{row}"].Value = SOSSTRO?.SynopticRequirementsLogbooks?.Count;
                        sheet.Cells[$"Z{row}"].Value = item?.Changes;
                        sheet.Cells[$"AD{row}"].Value = DateFormat(item?.Date);
                        sheet.Cells[$"AF{row}"].Value = item?.NoRevision == 0 ? "N" : item?.NoRevision;
                    }
                }

                int startRow = 13;

                //-============ DISTRIBUTIONS =============-\\
                foreach (var (distribution, indexDist) in GetDistributions(SOSSTRO!).Select((item, idx) => (item, idx)))
                {
                    List<SOSDistributionOperationSequence>? sections = BuildOperationSequences(distribution);

                    int rowCursor = startRow;

                    // === Sequences and Analyses === \\
                    foreach (var (rowCount, idx) in GenerateArraySeqAndAnalyses(distribution).Select((r, i) => (r, i)))
                    {
                        var sosNumCell = sheet.Cells[$"G{rowCursor}:G{rowCursor + rowCount - 1}"];
                        MergeAndStyleCell(sosNumCell, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center, true, 12, 90);

                        sosNumCell.Value = idx < (distribution?.Analyses?.Count ?? 0) ? distribution?.Analyses?.ElementAtOrDefault(idx)?.SOSHub?.Folio ?? string.Empty : distribution?.Sequences?.ElementAtOrDefault(idx - (distribution?.Analyses?.Count ?? 0))?.SOSHub?.Folio ?? string.Empty;
                        rowCursor += rowCount;

                    }

                    // === General Distribution Columns === \\
                    MergeAndStyleRange(sheet.Cells[$"C{startRow}:F{startRow + sections.Count - 1}"], distribution?.ProcessName, true, 14, ExcelHorizontalAlignment.Center);
                    MergeAndStyleRange(sheet.Cells[$"B{startRow}:B{startRow + sections.Count - 1}"], indexDist + 1, true, 14, ExcelHorizontalAlignment.Center);
                    MergeAndStyleRange(sheet.Cells[$"V{startRow}:X{startRow + sections.Count - 1}"], GetDifficultyLevel(distribution, SOSSTRO!), true, 20, ExcelHorizontalAlignment.Center);
                    MergeAndStyleRange(sheet.Cells[$"Y{startRow}:Z{startRow + sections.Count - 1}"], GetTrainingTime(distribution) + " Dias", true, 12, ExcelHorizontalAlignment.Center);

                    // === Knowledges === \\
                    var knowledges = GetKnowledges((int)distribution.SOSHubId!, SOSSTRO!);
                    var allKnowledge = await _KnowledgeRepository.GetAllKnowledge();
                    knowledges = knowledges.Select(k => { k.Knowledge = allKnowledge.FirstOrDefault(a => a.Id == k.KnowledgeId) ?? new Knowledge { Id = 0 }; return k; }).ToList();
                    MergeAndStyleRange(sheet.Cells[$"AA{startRow}:AC{startRow + sections.Count - 1}"], string.Join("\n", knowledges.Select(k => $"▪️ {k.Knowledge?.Name}")), false, wrapText: true, align: ExcelHorizontalAlignment.Left, vAlign: ExcelVerticalAlignment.Top);

                    // === Skills === \\
                    var skills = GetSkills((int)distribution.SOSHubId!, SOSSTRO!);
                    var allSkills = await _SkillRepository.GetAllSkill();
                    skills = skills.Select(s => { s.Skill = allSkills.FirstOrDefault(a => a.Id == s.SkillId) ?? new Skill { Id = 0 }; return s; }).ToList();
                    MergeAndStyleRange(sheet.Cells[$"AD{startRow}:AF{startRow + sections.Count - 1}"], string.Join("\n", skills.Select(s => $"▪️ {s.Skill?.Name}")), false, wrapText: true, align: ExcelHorizontalAlignment.Left, vAlign: ExcelVerticalAlignment.Top);

                    // === Operation Sections === \\
                    foreach (var (section, secIdx) in sections.Select((s, i) => (s, i)))
                    {
                        var stepSection = GetStepSection(distribution, section);

                        // Background row style
                        SetRowBackground(sheet, startRow, "H", "U", Color.White);

                        // Operation column
                        var operationRange = stepSection.IsMachineOperation ? sheet.Cells[$"K{startRow}:M{startRow}"] : sheet.Cells[$"H{startRow}:J{startRow}"];
                        MergeAndStyleCell(operationRange, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, wrapText: true, border: false);
                        operationRange.Value = stepSection.Step;

                        // Borders for operation
                        if (stepSection.IsMachineOperation)
                        {
                            MergeAndStyleCell(sheet.Cells[$"H{startRow}:J{startRow}"], ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, border: false);
                            SetLeftRightBorder(sheet.Cells[$"H{startRow}:J{startRow}"]);

                        }
                        else
                        {
                            MergeAndStyleCell(sheet.Cells[$"K{startRow}:M{startRow}"], ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, border: false);
                            SetLeftRightBorder(sheet.Cells[$"K{startRow}:M{startRow}"]);
                        }

                        // Conditions
                        var conditions = GetEstablishedCondition(stepSection.SectionId, SOSSTRO!);
                        MergeAndStyleCell(sheet.Cells[$"N{startRow}:Q{startRow}"], ExcelHorizontalAlignment.Right, ExcelVerticalAlignment.Center, wrapText: true, border: false);
                        sheet.Cells[$"N{startRow}:Q{startRow}"].Value = string.Join("\n", conditions.Select(c => $"🔹{c.Condition}"));
                        SetLeftRightBorder(sheet.Cells[$"N{startRow}:Q{startRow}"]);

                        // Critical Points / Quality
                        var criticalPoints = GetCriticalPoints(section);
                        MergeAndStyleCell(sheet.Cells[$"R{startRow}:U{startRow}"], ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center, wrapText: true, border: false);
                        sheet.Cells[$"R{startRow}:U{startRow}"].Value = string.Join("\n", criticalPoints.Select((c, i) => $"{i + 1}.- {c}"));
                        SetLeftRightBorder(sheet.Cells[$"R{startRow}:U{startRow}"]);

                        // Add bottom border to last section row
                        if (secIdx == sections.Count - 1)
                        {
                            SetBottomBorder(sheet.Cells[$"H{startRow}:J{startRow}"]);
                            SetBottomBorder(sheet.Cells[$"K{startRow}:M{startRow}"]);
                            SetBottomBorder(sheet.Cells[$"N{startRow}:Q{startRow}"]);
                            SetBottomBorder(sheet.Cells[$"R{startRow}:U{startRow}"]);
                        }

                        // Row height calculation
                        int heightRow = new[] { CalculateHeightRowOperation(stepSection), CalculateHeightRowEstablishedCondition(conditions), CalculateHeightRowQuality(criticalPoints) }.Max();


                        sheet.Row(startRow++).Height = heightRow;

                    }

                }


                package.SaveAs(ms);

            }

            ms.Position = 0;
            return ms.ToArray();
        }


        // =================================================== \\
        //&============== DATE & LOGBOOK HELPERS =============&\\
        // =================================================== \\

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


        // =================================================== \\
        //&============== DISTRIBUTIONS & HUBS ===============&\\
        // =================================================== \\

        /// <summary>
        /// Retrieves SOS distributions for the hubs in the given Synoptic Table.
        /// </summary>
        /// <param name="SOSSynopticRequeriments">The Synoptic Table containing SOS hubs.</param>
        /// <returns>List of <see cref="SOSDistribution"/> for hubs with valid distributions.</returns>
        private async Task<List<SOSDistribution>> GetDistributionsComplete(SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {

            IEnumerable<int> SOSHubsId = SOSSynopticRequeriments.SOSHubs!.Select(s => s.SOSHubId);
            var distributions = new List<SOSDistribution>();

            foreach (var HubId in SOSHubsId)
            {
                var distributionId = await _Sos_ProcessRepository.GetIdDistributionBySosHub(HubId);

                // NOTE: Skips hubs without a valid distribution (distributionId = 0)
                if (distributionId == 0) continue;

                var SOSDistribution = await _Sos_ProcessRepository.GetSOSDistribution(distributionId, includeSOS: true, includeCollections: true);
                distributions.Add(SOSDistribution);

            }

            return distributions;
        }

        /// <summary>
        /// Returns a list of SOS hubs that have at least one valid distribution attached.
        /// </summary>
        /// <param name="SOSSynopticRequeriments">The Synoptic Table containing SOS hubs.</param>
        /// <returns>List of <see cref="SOSHub"/> with attached distributions.</returns>
        private async Task<List<SOSHub>> GetHubsWithDistribution(SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            List<SOSDistribution> Distributions = await GetDistributionsComplete(SOSSynopticRequeriments);
            var SOSHubsSTRO = SOSSynopticRequeriments.SOSHubs ?? new List<SOSHub>();

            var selectedSosHubs = new List<SOSHub>();

            foreach (var SOSHub in SOSHubsSTRO)
            {
                var distribution = Distributions.FirstOrDefault(d => d.SOSHubId == SOSHub.SOSHubId);

                // NOTE: Only include hubs that have a valid distribution
                if (distribution == null) continue;

                // Attach the found distribution to the hub
                SOSHub.SOSDistribution = new List<SOSDistribution> { distribution };
                selectedSosHubs.Add(SOSHub);
            }

            return selectedSosHubs;

        }

        /// <summary>
        /// Retrieves all distributions from the SOS hubs in the given Synoptic Table.
        /// </summary>
        /// <param name="SOSSynopticRequeriments">The Synoptic Table containing SOS hubs.</param>
        /// <returns>List of <see cref="SOSDistribution"/> from all hubs.</returns>
        public List<SOSDistribution> GetDistributions(SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            // NOTE: Uses SelectMany to flatten distributions from multiple hubs
            return SOSSynopticRequeriments?.SOSHubs?.SelectMany(s => s.SOSDistribution ?? new List<SOSDistribution>()).ToList() ?? new List<SOSDistribution>();
        }

        // =================================================== \\
        //&=============== SEQUENCES & SECTIONS ==============&\\
        // =================================================== \\

        /// <summary>
        /// Builds the list of operation sequences for a given distribution, ensuring all expected sequences are present.
        /// </summary>
        /// <param name="distribution">The SOS distribution to process.</param>
        /// <returns>List of <see cref="SOSDistributionOperationSequence"/> including placeholders if sequences are missing.</returns>
        private List<SOSDistributionOperationSequence> BuildOperationSequences(SOSDistribution distribution)
        {
            // Order existing sequences by SequenceId, or use empty if none exist
            var sequences = (distribution.SOSDistributionOperationSequence ?? Enumerable.Empty<SOSDistributionOperationSequence>()).OrderBy(s => s.SequenceId).ToList();
            int expectedCount = (distribution.Analyses?.Count() ?? 0) + (distribution.Sequences?.Count() ?? 0);

            // NOTE: Fill missing sequences with placeholders to match expected count
            int missing = expectedCount - sequences.Count;
            if (missing > 0) sequences.AddRange(Enumerable.Range(0, missing).Select(_ => new SOSDistributionOperationSequence()));

            return sequences;
        }

        /// <summary>
        /// Gets the <see cref="Section"/> for a given operation sequence, or a default empty section if none exists.
        /// </summary>
        /// <param name="distribution">The SOS distribution containing analyses and sequences.</param>
        /// <param name="operationSequence">The operation sequence to match.</param>
        /// <returns>The matching <see cref="Section"/> or a new default <see cref="Section"/>.</returns>
        public Section GetStepSection(SOSDistribution distribution, SOSDistributionOperationSequence operationSequence)
        {
            // Combine sections from analyses and sequences
            List<Section> sections = distribution.Analyses!.SelectMany(a => a.SOSHub?.Sections ?? Enumerable.Empty<Section>()).Concat(distribution.Sequences!.SelectMany(s => s.SOSHub?.Sections ?? Enumerable.Empty<Section>())).ToList();

            // NOTE: Find the section matching the operation sequence
            Section? findStep = sections.FirstOrDefault(s => s.SectionId == operationSequence.SectionId);
            return findStep ?? new Section { Step = "", IsMachineOperation = false };
        }

        /// <summary>
        /// Generates an array of row spans corresponding to analyses and sequences in a distribution.
        /// </summary>
        /// <param name="distribution">The SOS distribution containing analyses, sequences, and operation sequences.</param>
        /// <returns>A list of integers representing the number of rows each analysis or sequence should occupy.</returns>
        private List<int> GenerateArraySeqAndAnalyses(SOSDistribution distribution)
        {
            int totalSeqAndAna = (distribution.Analyses?.Count() ?? 0) + (distribution.Sequences?.Count() ?? 0);

            var sequences = distribution.SOSDistributionOperationSequence?.ToList() ?? new List<SOSDistributionOperationSequence>();

            if (totalSeqAndAna == 0) return new List<int>();

            var rowSpans = new List<int>(new int[totalSeqAndAna]);

            if (sequences.Count >= totalSeqAndAna)
            {
                int baseValue = (int)Math.Round((double)sequences.Count / totalSeqAndAna);
                int acumulated = 0;

                for (int i = 0; i < rowSpans.Count; i++)
                {
                    int remaining = sequences.Count - acumulated;

                    // NOTE: Assign remaining count to last element, otherwise use baseValue
                    int value = (i == rowSpans.Count - 1) ? remaining : Math.Min(baseValue, remaining);

                    rowSpans[i] = value;
                    acumulated += value;

                    if (remaining <= 0) break;
                }
            }
            else
            {
                // NOTE: If fewer sequences than analyses, assign 1 row per element
                rowSpans = rowSpans.Select(x => 1).ToList();
            }

            return rowSpans;
        }


        // =================================================== \\
        //&=============== HEIGHT CALCULATIONS ===============&\\
        // =================================================== \\

        /// <summary>
        /// Calculates the row height required for an operation step based on its text length.
        /// </summary>
        /// <param name="section">The operation section containing the step text.</param>
        /// <returns>The calculated row height.</returns>
        private int CalculateHeightRowOperation(Section section)
        {
            const int charsPerBlock = 20;
            const int heightPerBlock = 15;
            const int extraPadding = 5;

            // NOTE: Height is proportional to number of text blocks plus extra padding
            int blocks = (int)Math.Ceiling(section.Step.Length / (double)charsPerBlock);
            return (blocks * heightPerBlock) + extraPadding;
        }


        /// <summary>
        /// Calculates the row height needed to display all established conditions.
        /// </summary>
        /// <param name="establishedConditions">List of established conditions.</param>
        /// <returns>The total calculated row height.</returns>
        private int CalculateHeightRowEstablishedCondition(List<EstablishedConditions> establishedConditions)
        {
            const int charsPerBlock = 20;
            const int heightPerBlock = 15;
            const int extraPadding = 5;

            // NOTE: Each condition contributes to height based on its text length
            int totalHeight = establishedConditions.Select(ec => (int)Math.Ceiling(ec.Condition.Length / (double)charsPerBlock) * heightPerBlock).Sum();

            return totalHeight + extraPadding;
        }


        /// <summary>
        /// Calculates the row height needed to display all quality points.
        /// </summary>
        /// <param name="Qualitys">List of quality points as strings.</param>
        /// <returns>The total calculated row height.</returns>
        private int CalculateHeightRowQuality(List<string> Qualitys)
        {
            const int charsPerBlock = 20;
            const int heightPerBlock = 15;
            const int extraPadding = 5;

            // NOTE: Each quality string contributes to height based on its length
            int totalHeight = Qualitys.Select(q => (int)Math.Ceiling(q.Length / (double)charsPerBlock) * heightPerBlock).Sum();
            return totalHeight + extraPadding;
        }



        // =================================================== \\
        //&=================== DATA FILTERS ==================&\\
        // =================================================== \\

        /// <summary>
        /// Retrieves all established conditions for a given section.
        /// </summary>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="SOSSynopticRequeriments">The synoptic table containing conditions.</param>
        /// <returns>List of <see cref="EstablishedConditions"/> for the section.</returns>
        private List<EstablishedConditions> GetEstablishedCondition(int sectionId, SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            return SOSSynopticRequeriments.EstablishedConditions?.Where(e => e.SectionId == sectionId).ToList() ?? new List<EstablishedConditions>();
        }

        /// <summary>
        /// Gets all critical points from the analyses of an operation sequence.
        /// </summary>
        /// <param name="operationSequence">The operation sequence to check.</param>
        /// <returns>Flattened list of representing critical points.</returns>
        public List<string> GetCriticalPoints(SOSDistributionOperationSequence operationSequence)
        {
            // NOTE: Flatten all critical points from analyses if they exist
            return operationSequence?.Section?.Analyses?.Where(a => a?.CriticalPoints != null).SelectMany(a => a.CriticalPoints).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Retrieves the difficulty level assigned to a distribution.
        /// </summary>
        /// <param name="distribution">The distribution to check.</param>
        /// <param name="SOSSynopticRequeriments">The synoptic table containing difficulty levels.</param>
        /// <returns>The <see cref="DifficultyLevel"/> assigned, or <see cref="DifficultyLevel.A"/> if none found.</returns>
        private DifficultyLevel GetDifficultyLevel(SOSDistribution distribution, SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            // NOTE: Default to DifficultyLevel.A if no specific difficulty is assigned
            var difficulty = SOSSynopticRequeriments.RequirementDifficulties?.FirstOrDefault(r => r.SOSHubId == distribution.SOSHubId);
            return difficulty?.DifficultyLevel ?? DifficultyLevel.A;
        }

        /// <summary>
        /// Retrieves the training time for a distribution's hub.
        /// </summary>
        /// <param name="distribution">The distribution to check.</param>
        /// <returns>Training time in days.</returns>
        public int GetTrainingTime(SOSDistribution distribution)
        {
            return distribution.SOSHubs!.FirstOrDefault(s => s.SOSHubId == distribution.SOSHubId)?.TrainingTime ?? 0;
        }

        /// <summary>
        /// Retrieves all knowledges for a given hub in a synoptic table.
        /// </summary>
        /// <param name="sosHubId">The ID of the hub.</param>
        /// <param name="SOSSynopticRequeriments">The synoptic table containing knowledge hubs.</param>
        /// <returns>List of <see cref="SOSSTROKnowledgeHub"/> for the hub.</returns>
        public List<SOSSTROKnowledgeHub> GetKnowledges(int sosHubId, SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            return SOSSynopticRequeriments.SOSSTROKnowledge!.Where(a => a.SOSHubId == sosHubId).ToList();
        }

        /// <summary>
        /// Retrieves all skills for a given hub in a synoptic table.
        /// </summary>
        /// <param name="sosHubId">The ID of the hub.</param>
        /// <param name="SOSSynopticRequeriments">The synoptic table containing skill hubs.</param>
        /// <returns>List of <see cref="SOSSTROSkillHub"/> for the hub.</returns>
        public List<SOSSTROSkillHub> GetSkills(int sosHubId, SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            return SOSSynopticRequeriments.SOSSTROSkill!.Where(a => a.SOSHubId == sosHubId).ToList() ?? new List<SOSSTROSkillHub>();
        }


        // =================================================== \\
        //&=================== EXCEL STYLES ==================&\\
        // =================================================== \\

        /// <summary>
        /// Merges an Excel range and applies basic styling including alignment, font, rotation, wrapping, and optional border.
        /// </summary>
        /// <param name="range">The Excel range to merge and style.</param>
        /// <param name="hAlign">Horizontal alignment.</param>
        /// <param name="vAlign">Vertical alignment.</param>
        /// <param name="bold">Whether to apply bold font.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="rotation">Text rotation in degrees.</param>
        /// <param name="wrapText">Whether to wrap text in the cell.</param>
        /// <param name="border">Whether to apply a thin border around the range.</param>
        private void MergeAndStyleCell(ExcelRange range, ExcelHorizontalAlignment hAlign, ExcelVerticalAlignment vAlign, bool bold = false, int fontSize = 11, int rotation = 0, bool wrapText = true, bool border = true)
        {
            range.Merge = true;
            range.Style.HorizontalAlignment = hAlign;
            range.Style.VerticalAlignment = vAlign;
            range.Style.Font.Bold = bold;
            range.Style.Font.Size = fontSize;
            range.Style.TextRotation = rotation;
            range.Style.WrapText = wrapText;
            if (border) range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        }

        /// <summary>
        /// Merges an Excel range, sets its value, and applies styling including alignment, font, wrapping, and border.
        /// </summary>
        /// <param name="range">The Excel range to merge and style.</param>
        /// <param name="value">Value to set in the range.</param>
        /// <param name="bold">Whether to apply bold font.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="align">Horizontal alignment.</param>
        /// <param name="vAlign">Vertical alignment.</param>
        /// <param name="wrapText">Whether to wrap text in the cell.</param>
        private void MergeAndStyleRange(ExcelRange range, object value, bool bold = false, int fontSize = 11, ExcelHorizontalAlignment align = ExcelHorizontalAlignment.Center, ExcelVerticalAlignment vAlign = ExcelVerticalAlignment.Center, bool wrapText = false)
        {
            range.Merge = true;
            range.Value = value;
            range.Style.HorizontalAlignment = align;
            range.Style.VerticalAlignment = vAlign;
            range.Style.Font.Bold = bold;
            range.Style.Font.Size = fontSize;
            range.Style.WrapText = wrapText;
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        }

        /// <summary>
        /// Sets the background color for a row in the Excel sheet between specified columns.
        /// </summary>
        /// <param name="sheet">The Excel worksheet.</param>
        /// <param name="row">Row number to style.</param>
        /// <param name="fromCol">Starting column (letter).</param>
        /// <param name="toCol">Ending column (letter).</param>
        /// <param name="color">Background color to apply.</param>
        private void SetRowBackground(ExcelWorksheet sheet, int row, string fromCol, string toCol, Color color)
        {
            sheet.Cells[$"{fromCol}{row}:{toCol}{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"{fromCol}{row}:{toCol}{row}"].Style.Fill.BackgroundColor.SetColor(color);
        }

        /// <summary>
        /// Applies a thin right border to the Excel range.
        /// </summary>
        /// <param name="range">The range to apply the right border.</param>
        private void SetRightBorder(ExcelRange range)
        {
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Color.SetColor(Color.Black);
        }

        /// <summary>
        /// Applies thin left and right borders to the Excel range.
        /// </summary>
        /// <param name="range">The range to style.</param>
        private void SetLeftRightBorder(ExcelRange range)
        {
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Color.SetColor(Color.Black);
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Color.SetColor(Color.Black);
        }

        /// <summary>
        /// Applies a thin bottom border to the Excel range.
        /// </summary>
        /// <param name="range">The range to style.</param>
        private void SetBottomBorder(ExcelRange range)
        {
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
    }
}
