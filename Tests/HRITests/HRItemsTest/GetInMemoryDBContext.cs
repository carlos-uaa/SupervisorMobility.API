using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HRItemsTest
{
    public class GetInMemoryDBContext
    {
        public SupervisorMobilityContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
             
            
            return new SupervisorMobilityContext(options); ;
        }
    }
}
