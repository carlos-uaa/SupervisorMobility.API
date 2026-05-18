using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HRItemsTest
{
    
    public class HRIItemsTest
    {
        [Test]
        public async Task CreateHRIItemAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
            var newItem = new HRIItem
            {
                
                Name = "Test Item",
                ControlNumber="1",
                IsActive = true
            };
            var expectedResponse = new ServiceResponse<HRIItem>
            {
                Data = newItem,
                Success = true,
                Message = "HRIItem created successfully."
            };

            mockService.Setup(service => service.CreateHRIItemAsync(It.IsAny<HRIItem>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRIItemController(mockService.Object);

            //act
            var result = await controller.CreateHRIItemAsync(newItem);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));

            //verificar que el método del servicio se llamó exactamente una vez con el nuevo elemento
            mockService.Verify(service => service.CreateHRIItemAsync(newItem), Times.Once);
        }

        [Test]
        public async Task GetAllHRIItemsAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
            var expectedItems = new List<HRIItem>
            {
                new HRIItem { Id = 1, Name = "Item 1", ControlNumber="1", IsActive = true },
                new HRIItem { Id = 2, Name = "Item 2", ControlNumber="2", IsActive = true }
            };
            var expectedResponse = new ServiceResponse<List<HRIItem>>
            {
                Data = expectedItems,
                Success = true,
                Message = "HRIItems retrieved successfully."
            };
            mockService.Setup(service => service.GetAllHRIItemsAsync())
                .ReturnsAsync(expectedResponse);
            var controller = new HRIItemController(mockService.Object);
            // act
            var result = await controller.GetAllHRIItemsAsync();
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            // verificar que el método del servicio se llamó exactamente una vez
            mockService.Verify(service => service.GetAllHRIItemsAsync(), Times.Once);
        }
        [Test]
        public async Task GetSingleHRIItemAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
            var expectedItem = new HRIItem { Id = 1, Name = "Item 1", ControlNumber="1", IsActive = true };
            var expectedResponse = new ServiceResponse<HRIItem>
            {
                Data = expectedItem,
                Success = true,
                Message = "HRIItem retrieved successfully."
            };
            mockService.Setup(service => service.GetSingleHRIItemAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRIItemController(mockService.Object);

            //act
            var result = await controller.GetSingleHRIItemAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el ID correcto
            mockService.Verify(service => service.GetSingleHRIItemAsync(1), Times.Once);
        }

        [Test]
        public async Task UpdateHRIItemAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
            var updatedItem = new HRIItem { Id = 1, Name = "Updated Item", ControlNumber = "1", IsActive = true };
            var expectedResponse = new ServiceResponse<HRIItem>
            {
                Data = updatedItem,
                Success = true,
                Message = "HRIItem updated successfully."
            };
            mockService.Setup(service => service.UpdateHRIItemAsync(It.IsAny<HRIItem>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRIItemController(mockService.Object);
            //act
            var result = await controller.UpdateHRIItemAsync(updatedItem);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el elemento actualizado
            mockService.Verify(service => service.UpdateHRIItemAsync(updatedItem), Times.Once);
        }

        [Test]
        public async Task DeleteHRIItemAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "HRIItem deleted successfully."
            };
            mockService.Setup(service => service.DeleteHRIItemAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRIItemController(mockService.Object);
            //act
            var result = await controller.DeleteHRIItemAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el ID correcto
            mockService.Verify(service => service.DeleteHRIItemAsync(1), Times.Once);
        }
    }
}
