// +============================================================+\\
// +===================== SKILL REPOSITORY ====================+\\
// +============================================================+\\

/// <summary>
/// Implements data access operations for the "Skill" collection
/// using Entity Framework Core within the SOS.STRO module.
/// Provides methods to:
/// - Retrieve a single skill record by ID
/// - Retrieve all skill records
/// - Create a new skill record
/// Exceptions are thrown when records are not found or database operations fail.
/// </summary>


// - Core .NET imports
using Microsoft.EntityFrameworkCore;

// - External imports
using AutoMapper;

// - Context imports
using SupervisorMobility.API.Context;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;


namespace SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Skills
{
    public class SkillRepository : ISkillRepository
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor: Initializes dependencies for the repository
        /// </summary>
        /// <param name="context">Database context for accessing Skill entities</param>
        /// <param name="mapper">AutoMapper instance for DTO-to-entity mapping</param>
        /// <exception cref="ArgumentNullException">Thrown if context is null</exception>
        public SkillRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves a specific skill record by its ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the skill record (int)</param>
        /// <returns>The skill entity corresponding to the given ID</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the skill record does not exist</exception>
        /// <remarks>Performs an asynchronous database query using EF Core</remarks>
        public async Task<Skill> GetSkill(int Id)
        {
            var findSkill = await _context.Skill.FirstOrDefaultAsync(k => k.Id == Id);
            if (findSkill == null) throw new KeyNotFoundException("Skill not found");

            return findSkill;
        }

        /// <summary>
        /// Retrieves all skill records from the database.
        /// </summary>
        /// <returns>A list of all skill entities</returns>
        /// <remarks>Performs an asynchronous database query using EF Core</remarks>
        public async Task<List<Skill>> GetAllSkill()
        {
            return await _context.Skill.ToListAsync();
        }

        /// <summary>
        /// Creates a new skill record in the database.
        /// </summary>
        /// <param name="createSkill">The skill entity to create</param>
        /// <returns>The created skill entity with assigned ID</returns>
        /// <remarks>
        /// Adds the entity to the DbContext and saves changes asynchronously.
        /// Throws exceptions if the database operation fails.
        /// </remarks>
        public async Task<Skill> CreateSkill(Skill createSkill)
        {
            await _context.Skill.AddAsync(createSkill);

            await _context.SaveChangesAsync();

            return createSkill;
        }

        // ============ TODO: IMPLEMENT MISSING METHODS ============\\
        /// <summary>
        /// TODO: Implement additional repository methods as required by business logic:
        /// UpdateSkill(Skill updateSkill): Update an existing skill record
        /// DeleteSkill(int id): Delete a skill record by ID
        /// FindSkillByCriteria(...): Query skill records based on specific filters or criteria
        ///
        /// These methods should handle exceptions, validate inputs, and ensure database consistency.
        /// Follow the same async patterns and EF Core practices as existing methods.
        /// </summary>
    }
}
