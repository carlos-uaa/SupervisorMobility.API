// - External imports
using AutoMapper;
using Microsoft.EntityFrameworkCore;

// - Context imports
using SupervisorMobility.API.Context;

// - Entity's imports
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;


namespace SupervisorMobility.API.infrastructure.repositories.STRO
{
    /// <summary>
    /// Repository for accessing and managing STRO sequences in the database.
    /// </summary>
    public class STROSequencesRepository : ISTROSequencesRepository
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="STROSequencesRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="mapper">The object mapper.</param>
        public STROSequencesRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves all STRO sequences associated with a given SOS Hub ID.
        /// </summary>
        /// <param name="IdSosHub">The SOS Hub ID.</param>
        /// <returns>A list of <see cref="SOSSynopticRequirementsOperationSequence"/>.</returns>
        public async Task<List<SOSSynopticRequirementsOperationSequence>> GetAllSTROSequencesByIdSosHubId(int IdSosHub)
        {
            // NOTE: Query all sequences for the specified SOS Hub
            return await _context.SOSSynopticRequirementsOperationSequences.Where(s => s.SosHubId == IdSosHub).ToListAsync();
        }

        /// <summary>
        /// Adds a new STRO sequence to the database.
        /// </summary>
        /// <param name="AddSTROSequence">The STRO sequence to add.</param>
        /// <returns>The added <see cref="SOSSynopticRequirementsOperationSequence"/>.</returns>
        public async Task<SOSSynopticRequirementsOperationSequence> AddSTROSequences(SOSSynopticRequirementsOperationSequence AddSTROSequence)
        {
            _context.SOSSynopticRequirementsOperationSequences.Add(AddSTROSequence);

            // NOTE: Persist changes to the database
            await _context.SaveChangesAsync();

            return AddSTROSequence;
        }

        /// <summary>
        /// Updates an existing STRO sequence in the database.
        /// </summary>
        /// <param name="UpdateSTROSequence">The STRO sequence to update.</param>
        /// <returns>The updated <see cref="SOSSynopticRequirementsOperationSequence"/>.</returns>
        public async Task<SOSSynopticRequirementsOperationSequence> UpdateSTROSequences(SOSSynopticRequirementsOperationSequence UpdateSTROSequence)
        {
            _context.SOSSynopticRequirementsOperationSequences.Update(UpdateSTROSequence);

            // NOTE: Persist changes to the database
            await _context.SaveChangesAsync();

            return UpdateSTROSequence;
        }

        /// <summary>
        /// Deletes a STRO sequence from the database by its ID.
        /// </summary>
        /// <param name="IdSTROSequence">The ID of the STRO sequence to delete.</param>
        public async Task DeleteSTROSequences(int IdSTROSequence)
        {
            // NOTE: Execute delete operation directly in database
            await _context.SOSSynopticRequirementsOperationSequences.Where(s => s.SOSSynopticRequirementsOperationSequenceId == IdSTROSequence).ExecuteDeleteAsync();
        }
    }
}