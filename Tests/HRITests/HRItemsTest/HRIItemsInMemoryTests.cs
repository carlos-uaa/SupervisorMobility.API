using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HRItemsTest
{
    public  class HRIItemsInMemoryTests
    {
        [Test]
        public async Task CreateHRIItemAsync()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var repository = new HRIItemsRepository(context);
            
            var newItem = new HRIItem
            {
                Name = "Test Item",
                ControlNumber = "1",
                IsActive = true
            };
            // act
            var result = await repository.CreateHRIItemAsync(newItem);
            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.That(result.Message, Is.EqualTo("HRIItem created successfully."));
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Name, Is.EqualTo(newItem.Name));
        }
    }
}
