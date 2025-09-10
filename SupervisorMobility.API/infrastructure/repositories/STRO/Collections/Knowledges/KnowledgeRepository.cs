// +============================================================+\\
// +================== KNOWLEDGE REPOSITORY ===================+\\
// +============================================================+\\

/// <summary>
/// Implements data access operations for the "Knowledge" collection
/// using Entity Framework Core within the SOS.STRO module.
/// Provides methods to:
/// - Retrieve a single knowledge record by ID
/// - Retrieve all knowledge records
/// - Create a new knowledge record
/// Exceptions are thrown when records are not found or database operations fail.
/// </summary>


// - Core .NET imports
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

// - External imports
using AutoMapper;

// - Context imports
using SupervisorMobility.API.Context;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;


namespace SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Knowledges
{
    public class KnowledgeRepository : IKnowledgeRepository
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor: Initializes dependencies for the repository
        /// </summary>
        /// <param name="context">Database context for accessing Knowledge entities</param>
        /// <param name="mapper">AutoMapper instance for DTO-to-entity mapping</param>
        /// <exception cref="ArgumentNullException">Thrown if context is null</exception>
        public KnowledgeRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // +============ DATA ACCESS METHODS ============+\\

        /// <summary>
        /// Retrieves a specific knowledge record by its ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the knowledge record (int)</param>
        /// <returns>The knowledge entity corresponding to the given ID</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the knowledge record does not exist</exception>
        /// <remarks>Performs an asynchronous database query using EF Core</remarks>
        public async Task<Knowledge> GetKnowledge(int Id)
        {
            var findKnowledge = await _context.Knowledge.FirstOrDefaultAsync(k => k.Id == Id);
            if (findKnowledge == null) throw new KeyNotFoundException("Knowledge not found");

            return findKnowledge;
        }

        /// <summary>
        /// Retrieves all knowledge records from the database.
        /// </summary>
        /// <returns>A list of all knowledge entities</returns>
        /// <remarks>Performs an asynchronous database query using EF Core</remarks>
        public async Task<List<Knowledge>> GetAllKnowledge()
        {
            return await _context.Knowledge.ToListAsync();
        }

        /// <summary>
        /// Creates a new knowledge record in the database.
        /// </summary>
        /// <param name="createKnowledge">The knowledge entity to create</param>
        /// <returns>The created knowledge entity with assigned ID</returns>
        /// <remarks>
        /// Adds the entity to the DbContext and saves changes asynchronously.
        /// Throws exceptions if the database operation fails.
        /// </remarks>
        public async Task<Knowledge> CreateKnowledge(Knowledge createKnowledge)
        {
            await _context.Knowledge.AddAsync(createKnowledge);

            await _context.SaveChangesAsync();

            return createKnowledge;
        }


        //============  TODO: IMPLEMENT MISSING METHODS ============\\
        /// <summary>
        /// TODO: Implement additional repository methods as required by business logic:
        /// UpdateKnowledge(Knowledge updateKnowledge): Update an existing knowledge record
        /// DeleteKnowledge(int id): Delete a knowledge record by ID
        /// FindKnowledgeByCriteria(...): Query knowledge records based on specific filters or criteria
        ///
        /// These methods should handle exceptions, validate inputs, and ensure database consistency.
        /// Follow the same async patterns and EF Core practices as existing methods.
        /// </summary>

    }
}
