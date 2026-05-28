using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API;

namespace Tests.HRITests.HRIDockTest
{
    internal class HRIDocksTest
    {

        // GetAllHRIDocksAsync
        [Test]
        public async Task GetAllHRIDocksAsync_ReturnsActiveDocksOnly()
        {
            // arrange
            var mockRepo = new Mock<IHRIDocksService>();

            var docks = new List<HRIDock>
            {
                new HRIDock { Code = "D1", DockName = "Dock 1", IsActive = true },
                new HRIDock { Code = "D2", DockName = "Dock 2", IsActive = true }
            };

            mockRepo.Setup(r => r.GetAllHRIDocksAsync()).ReturnsAsync(new ServiceResponse<List<HRIDock>> { Data = docks, Success = true });

            var service = new HRIDocksController(mockRepo.Object);

            // act
            var result = await service.GetAllHRIDocksAsync();

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("D1", result.Data[0].Code);
            Assert.AreEqual("D2", result.Data[1].Code);
            Assert.IsTrue(result.Data.TrueForAll(d => d.IsActive));

            mockRepo.Verify(r => r.GetAllHRIDocksAsync(), Times.Once);
        }


        // GetSingleHRIDockAsync
        [Test]
        public async Task GetSingleHRIDockAsync_ReturnsDock_WhenExists()
        {
            // arrange
            var mockRepo = new Mock<IHRIDocksService>();

            var dock = new HRIDock { Id = 1, Code = "D1", DockName = "Dock 1", IsActive = true };

            mockRepo.Setup(r => r.GetSingleHRIDockAsync(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = dock, Success = true });

            var service = new HRIDocksController(mockRepo.Object);

            // act
            var result = await service.GetSingleHRIDockAsync(1);

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Id);
            Assert.AreEqual("D1", result.Data.Code);

            mockRepo.Verify(r => r.GetSingleHRIDockAsync(It.IsAny<int>()), Times.Once);
        }


        // CreateHRIDockAsync
        [Test]
        public async Task CreateHRIDockAsync()
        {
            // arrange
            var mockService = new Mock<IHRIDocksService>();

            var newDock = new HRIDock
            {
                Id = 0,
                Code = "D1",
                DockName = "Dock 1",
                IsActive = true
            };

            var response = new ServiceResponse<HRIDock>
            {
                Data = newDock,
                Success = true
            };

            mockService
                .Setup(s => s.CreateHRIDockAsync(It.IsAny<HRIDock>()))
                .ReturnsAsync(response);

            var controller = new HRIDocksController(mockService.Object);

            // act
            var result = await controller.CreateHRIDockAsync(newDock);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success, "Expected Success to be true");
            Assert.IsNotNull(result.Data, "Expected Data not to be null");
            Assert.AreEqual("D1", result.Data.Code);
            Assert.AreEqual("Dock 1", result.Data.DockName);

            mockService.Verify(s => s.CreateHRIDockAsync(It.Is<HRIDock>(d => d.Code == "D1")), Times.Once);
        }


        // UpdateHRIDockAsync
        [Test]
        public async Task UpdateHRIDockAsync()
        {
            // arrange
            var mockService = new Mock<IHRIDocksService>();

            var updatedDock = new HRIDock
            {
                Id = 1,
                Code = "D1",
                DockName = "Updated Dock",
                IsActive = true
            };

            var response = new ServiceResponse<HRIDock>
            {
                Data = updatedDock,
                Success = true
            };

            mockService
                .Setup(s => s.UpdateHRIDockAsync(It.IsAny<HRIDock>()))
                .ReturnsAsync(response);

            var controller = new HRIDocksController(mockService.Object);

            // act
            var result = await controller.UpdateHRIDockAsync(updatedDock);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success, "Expected Success to be true");
            Assert.IsNotNull(result.Data, "Expected Data not to be null");
            Assert.AreEqual("D1", result.Data.Code);
            Assert.AreEqual("Updated Dock", result.Data.DockName);

            mockService.Verify(s => s.UpdateHRIDockAsync(It.Is<HRIDock>(d => d.Id == 1 && d.DockName == "Updated Dock")), Times.Once);
        }


        // DeleteHRIDockAsync
        [Test]
        public async Task DeleteHRIDockAsync()
        {
            // arrange
            var mockService = new Mock<IHRIDocksService>();

            var response = new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };

            mockService
                .Setup(s => s.DeleteHRIDockAsync(It.IsAny<int>()))
                .ReturnsAsync(response);

            var controller = new HRIDocksController(mockService.Object);

            // act
            var result = await controller.DeleteHRIDockAsync(1);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success, "Expected Success to be true");
            Assert.IsTrue(result.Data, "Expected Data to be true");

            mockService.Verify(s => s.DeleteHRIDockAsync(It.Is<int>(id => id == 1)), Times.Once);
        }
    }
}
