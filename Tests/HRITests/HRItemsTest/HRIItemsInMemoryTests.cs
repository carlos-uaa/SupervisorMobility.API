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
        [Test]
        public async Task GetAllHRIItemsAsync()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var repository = new HRIItemsRepository(context);
            var item1 = new HRIItem { Name = "Item 1", ControlNumber = "1", IsActive = true };
            var item2 = new HRIItem { Name = "Item 2", ControlNumber = "2", IsActive = true };
            await repository.CreateHRIItemAsync(item1);
            await repository.CreateHRIItemAsync(item2);
            // act
            var result = await repository.GetAllHRIItemsAsync();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.That(result.Message, Is.EqualTo("HRIItems retrieved successfully."));
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Count, Is.EqualTo(2));

        }
        [Test]
        public async Task GetSingleHRIItemAsync()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var repository = new HRIItemsRepository(context);
            var item = new HRIItem { Name = "Item 1", ControlNumber = "1", IsActive = true };
            var createResult = await repository.CreateHRIItemAsync(item);
            Assert.IsTrue(createResult.Success);
            int itemId = createResult.Data!.Id;
            // act
            var result = await repository.GetSingleHRIItemAsync(itemId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.That(result.Message, Is.EqualTo("HRIItem retrieved successfully."));
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Id, Is.EqualTo(itemId));
        }
        [Test]
        public async Task UpdateHRIItemAsync()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var repository = new HRIItemsRepository(context);
            var item = new HRIItem { Name = "Item 1", ControlNumber = "1", IsActive = true };
            var createResult = await repository.CreateHRIItemAsync(item);
            Assert.IsTrue(createResult.Success);
            int itemId = createResult.Data!.Id;
            // act
            var updatedItem = new HRIItem { Id = itemId, Name = "Updated Item", ControlNumber = "1", IsActive = true };
            var updateResult = await repository.UpdateHRIItemAsync(updatedItem);
            Assert.IsNotNull(updateResult);
            Assert.IsTrue(updateResult.Success);
            Assert.That(updateResult.Message, Is.EqualTo("HRIItem updated successfully."));
            Assert.IsNotNull(updateResult.Data);
            Assert.That(updateResult.Data.Name, Is.EqualTo("Updated Item"));
        }
        [Test]
        public async Task DeleteHRIItemAsync()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var repository = new HRIItemsRepository(context);
            var item = new HRIItem { Name = "Item 1", ControlNumber = "1", IsActive = true };
            var createResult = await repository.CreateHRIItemAsync(item);
            Assert.IsTrue(createResult.Success);
            int itemId = createResult.Data!.Id;
            // act
            var deleteResult = await repository.DeleteHRIItemAsync(itemId);
            Assert.IsNotNull(deleteResult);
            Assert.IsTrue(deleteResult.Success);
            Assert.That(deleteResult.Message, Is.EqualTo("HRI Item deleted successfully."));
             
        }
        
    }
}
