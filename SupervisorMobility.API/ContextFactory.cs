using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;

namespace SupervisorMobility.API
{
    public class ContextFactory : IDbContextFactory<SupervisorMobilityContext>
    {
        private readonly DbContextOptions<SupervisorMobilityContext> _options;

        public ContextFactory(DbContextOptions<SupervisorMobilityContext> options)
        {
            _options = options;
        }

        public SupervisorMobilityContext CreateDbContext()
        {
            return new SupervisorMobilityContext(_options);
        }
    }

}
