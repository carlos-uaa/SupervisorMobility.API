using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Tests.HRITests.HRILinesTest
{
    public class HRILineTest
    {
        [Test]
        public async Task CreateHRILineAsync()
        {
            // arrange
            var mockService = new Mock<IHRILinesService>();
            var newLine = new HRILines
            {
                LineName = "Test Line",
                Code = "1",
                IsActive = true
            };
            var expectedResponse = new ServiceResponse<HRILines>
            {
                Data = newLine,
                Success = true,
                Message = "HRILine created successfully."
            };

            mockService.Setup(service => service.CreateHRILineAsync(It.IsAny<HRILines>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRILinesController(mockService.Object);

            //act
            var result = await controller.CreateHRILineAsync(newLine);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));

            //verificar que el método del servicio se llamó exactamente una vez con el nuevo elemento
            mockService.Verify(service => service.CreateHRILineAsync(newLine), Times.Once);
        }

        [Test]
        public async Task GetAllHRILinesAsync()
        {
            // arrange
            var mockService = new Mock<IHRILinesService>();
            var expectedLines = new List<HRILines>
            {
                new HRILines { Id = 1, LineName = "Line 1", Code="1", IsActive = true },
                new HRILines { Id = 2, LineName = "Line 2", Code="2", IsActive = true }
            };
            var expectedResponse = new ServiceResponse<List<HRILines>>
            {
                Data = expectedLines,
                Success = true,
                Message = "HRILines retrieved successfully."
            };
            mockService.Setup(service => service.GetAllHRILinesAsync())
                .ReturnsAsync(expectedResponse);
            var controller = new HRILinesController(mockService.Object);
            // act
            var result = await controller.GetAllHRILinesAsync();
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            // verificar que el método del servicio se llamó exactamente una vez
            mockService.Verify(service => service.GetAllHRILinesAsync(), Times.Once);
        }
        [Test]
        public async Task GetSingleHRILineAsync()
        {
            // arrange
            var mockService = new Mock<IHRILinesService>();
            var expectedLine = new HRILines { Id = 1, LineName = "Line 1", Code = "1", IsActive = true };
            var expectedResponse = new ServiceResponse<HRILines>
            {
                Data = expectedLine,
                Success = true,
                Message = "HRILine retrieved successfully."
            };
            mockService.Setup(service => service.GetSingleHRILineAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRILinesController(mockService.Object);
            //act
            var result = await controller.GetSingleHRILineAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el ID correcto
            mockService.Verify(service => service.GetSingleHRILineAsync(1), Times.Once);
        }

        [Test]
        public async Task UpdateHRILineAsync()
        {
            // arrange
            var mockService = new Mock<IHRILinesService>();
            var updatedLine = new HRILines { Id = 1, LineName = "Updated Line", Code = "1", IsActive = true };
            var expectedResponse = new ServiceResponse<HRILines>
            {
                Data = updatedLine,
                Success = true,
                Message = "HRILine updated successfully."
            };
            mockService.Setup(service => service.UpdateHRILineAsync(It.IsAny<HRILines>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRILinesController(mockService.Object);
            //act
            var result = await controller.UpdateHRILineAsync(updatedLine);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el elemento actualizado
            mockService.Verify(service => service.UpdateHRILineAsync(updatedLine), Times.Once);
        }

        [Test]
        public async Task DeleteHRILineAsync()
        {
            // arrange
            var mockService = new Mock<IHRILinesService>();
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "HRILine deleted successfully."
            };
            mockService.Setup(service => service.DeleteHRILineAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);
            var controller = new HRILinesController(mockService.Object);
            //act
            var result = await controller.DeleteHRILineAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedResponse));
            //verificar que el método del servicio se llamó exactamente una vez con el ID correcto
            mockService.Verify(service => service.DeleteHRILineAsync(1), Times.Once);
        }
    }
}
