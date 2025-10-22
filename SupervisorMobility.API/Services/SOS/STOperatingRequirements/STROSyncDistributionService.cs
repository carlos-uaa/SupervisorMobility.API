// - External imports
using DocumentFormat.OpenXml.Wordprocessing;

// - Context imports
using SupervisorMobility.API.DataAccess.Services;

// - Entity's imports
using SupervisorMobility.API.DataAccess.Entities.SOS;

// - Repository's imports
using SupervisorMobility.API.infrastructure.repositories.STRO;

// - Interface's imports
using SupervisorMobility.API.Interfaces.SOS;


namespace SupervisorMobility.API.Services.SOS
{
    /// <summary>
    /// Service responsible for synchronizing STRO sequences with SOS Distributions.
    /// </summary>
    public class STROSyncDistributionService : ISTROSyncDistributionService
    {
        // +========================= DEPENDENCIES =========================+ \\
        private readonly ISOS_ProcessRepository _sosProcessRepository;
        private readonly ISTROSequencesRepository _stroSequencesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="STROSyncDistributionService"/> class.
        /// </summary>
        /// <param name="sosProcessRepository">Repository for SOS process data access.</param>
        /// <param name="stroSequencesRepository">Repository for STRO sequences data access.</param>
        public STROSyncDistributionService(ISOS_ProcessRepository sosProcessRepository, ISTROSequencesRepository stroSequencesRepository)
        {
            _sosProcessRepository = sosProcessRepository;
            _stroSequencesRepository = stroSequencesRepository;
        }

        // +======================== PUBLIC METHODS ========================+ \\

        /// <summary>
        /// Synchronizes STRO sequences with a given SOS Distribution.
        /// </summary>
        /// <param name="distributionId">The ID of the SOS Distribution to synchronize.</param>
        /// <returns>Returns true if synchronization completed successfully.</returns>
        public async Task<bool> SyncDistributionsWithSTROs(int distributionId)
        {
            // NOTE: Retrieve distribution and grouped STRO sequences
            var distribution = await GetDistribution(distributionId);
            var stroSequencesBySynoptic = await GetGroupedStroSequences(distribution.SOSHubId!.Value);

            // NOTE: Determine which sequences to delete, update, or add
            var sequencesToDelete = DetectSequencesToDelete(stroSequencesBySynoptic, distribution);
            var sequencesToUpdate = DetectSequencesToUpdate(stroSequencesBySynoptic, distribution);
            var sequencesToAdd = DetectSequencesToAdd(stroSequencesBySynoptic, distribution);

            // NOTE: Apply changes in repository
            foreach (var seq in sequencesToDelete) await _stroSequencesRepository.DeleteSTROSequences(seq.SOSSynopticRequirementsOperationSequenceId);
            foreach (var seq in sequencesToUpdate) await _stroSequencesRepository.UpdateSTROSequences(seq);
            foreach (var seq in sequencesToAdd) await _stroSequencesRepository.AddSTROSequences(seq);

            return true;
        }


        // +======= PRIVATE HELPERS FOR SYNC_DISTRIBUTION_WITH_STROS =======+ \\

        /// <summary>
        /// Retrieves a SOS Distribution by its ID.
        /// </summary>
        /// <param name="distributionId">The ID of the distribution.</param>
        /// <returns>The found <see cref="SOSDistribution"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if distribution or HubId is not found.</exception>P
        private async Task<SupervisorMobility.API.DataAccess.Entities.SOS.SOSDistribution> GetDistribution(int distributionId)
        {
            var distribution = await _sosProcessRepository.GetSOSDistribution(distributionId, false, false, false, true, false, false, false, true);
            if (distribution == null || distribution.SOSHubId == null) throw new InvalidOperationException("SOS Distribution not found or invalid HubId.");

            return distribution;
        }

        /// <summary>
        /// Retrieves grouped STRO sequences for a given SOS Hub.
        /// </summary>
        /// <param name="sosHubId">The SOS Hub ID.</param>
        /// <returns>An array of grouped STRO sequences.</returns>
        private async Task<IGrouping<int, SOSSynopticRequirementsOperationSequence>[]> GetGroupedStroSequences(int sosHubId)
        {
            var sequences = await _stroSequencesRepository.GetAllSTROSequencesByIdSosHubId(sosHubId);
            return sequences.GroupBy(s => s.SOSSynopticTableofOperatingRequirementsId).ToArray();
        }

        /// <summary>
        /// Detects STRO sequences that should be deleted because they are not present in the current SOS Distribution operations.
        /// </summary>
        /// <param name="stroGroups">Grouped STRO sequences by Synoptic Table of Operating Requirements ID.</param>
        /// <param name="distribution">The current SOS Distribution.</param>
        /// <returns>
        /// A list of <see cref="SOSSynopticRequirementsOperationSequence"/> that should be deleted.
        /// </returns>
        private static List<SOSSynopticRequirementsOperationSequence> DetectSequencesToDelete(IEnumerable<IGrouping<int, SOSSynopticRequirementsOperationSequence>> stroGroups, SupervisorMobility.API.DataAccess.Entities.SOS.SOSDistribution distribution)
        {
            // NOTE: Flatten all STRO groups and filter sequences not present in the distribution
            return stroGroups.SelectMany(group => group).Where(stroSeq => distribution.SOSDistributionOperationSequence?.All(d => d.SectionId != stroSeq.SectionId) ?? true).ToList();
        }

        /// <summary>
        /// Detects STRO sequences that need to be updated based on the current SOS Distribution.
        /// </summary>
        /// <param name="stroGroups">Grouped STRO sequences by Synoptic Table of Operating Requirements ID.</param>
        /// <param name="distribution">The current SOS Distribution.</param>
        /// <returns>
        /// A list of <see cref="SOSSynopticRequirementsOperationSequence"/> that should be updated.
        /// </returns>
        private static List<SOSSynopticRequirementsOperationSequence> DetectSequencesToUpdate(IEnumerable<IGrouping<int, SOSSynopticRequirementsOperationSequence>> stroGroups, SupervisorMobility.API.DataAccess.Entities.SOS.SOSDistribution distribution)
        {
            var toUpdate = new List<SOSSynopticRequirementsOperationSequence>();

            // NOTE: Flatten all groups and find matching distribution sequences
            foreach (var stroSeq in stroGroups.SelectMany(group => group))
            {
                var matching = distribution.SOSDistributionOperationSequence?.FirstOrDefault(d => d.SectionId == stroSeq.SectionId);

                // NOTE: Update sequence number if a match is found
                if (matching != null)
                {
                    stroSeq.Sequence = matching.SequenceId;
                    toUpdate.Add(stroSeq);
                }
            }

            return toUpdate;
        }

        /// <summary>
        /// Detects STRO sequences that should be added because they are missing
        /// from the existing grouped STRO sequences.
        /// </summary>
        /// <param name="stroGroups">Grouped STRO sequences by Synoptic Table of Operating Requirements ID.</param>
        /// <param name="distribution">The current SOS Distribution.</param>
        /// <returns>
        /// A list of new <see cref="SOSSynopticRequirementsOperationSequence"/> to be added.
        /// </returns>
        private static List<SOSSynopticRequirementsOperationSequence> DetectSequencesToAdd(IEnumerable<IGrouping<int, SOSSynopticRequirementsOperationSequence>> stroGroups, SupervisorMobility.API.DataAccess.Entities.SOS.SOSDistribution distribution)
        {
            var toAdd = new List<SOSSynopticRequirementsOperationSequence>();

            // NOTE: Loop through all distribution sequences
            foreach (var distSeq in distribution.SOSDistributionOperationSequence ?? Enumerable.Empty<SOSDistributionOperationSequence>())
            {
                foreach (var group in stroGroups)
                {
                    // NOTE: Add new sequence if it's missing in the group
                    if (group.All(s => s.SectionId != distSeq.SectionId))
                    {
                        toAdd.Add(new SOSSynopticRequirementsOperationSequence
                        {
                            Sequence = distSeq.SequenceId,
                            SectionId = distSeq.SectionId,
                            SosHubId = distribution.SOSHubId,
                            OperationPersonText = distSeq.Section?.Step,
                            OperationMachineText = distSeq.Section?.Step,
                            IsOperationPersonRequired = true,
                            IsOperationMachineRequired = false,
                            IsActive = true,
                            SOSSynopticTableofOperatingRequirementsId = group.Key
                        });
                    }
                }
            }

            return toAdd;
        }
    }
}
