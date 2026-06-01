using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API;
using System.Collections.Generic;

namespace Tests.HRITests.HRIDockTest
{
    public class HRIDocksServiceTests
    {

        // GetAllHRIDocksAsync --ready
        [Test]
        public async Task GetAllHRIDocksAsync_ReturnsActiveDocksOnly()
        {
            // arrange
            var mockRepo = new Mock<IHRIDocksRepository>();

            var docks = new List<HRIDock>
            {
                new HRIDock { Code = "D1", DockName = "Dock 1", IsActive = true },
                new HRIDock { Code = "D2", DockName = "Dock 2", IsActive = true }
            };

            mockRepo.Setup(r => r.GetAllHRIDocksAsync()).ReturnsAsync(new ServiceResponse<List<HRIDock>> { Data = docks, Success = true });

            var service = new HRIDocksService(mockRepo.Object);

            // act
            var result = await service.GetAllHRIDocksAsync();

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("D1", result.Data[0].Code);
            Assert.AreEqual("D2", result.Data[1].Code);
            Assert.IsTrue(result.Data.TrueForAll(d => d.IsActive));
        }


        // CreateHRIDockAsync --ready
        [Test]
        public async Task CreateHRIDockAsync_ReturnsValidationError_WhenCodeMissing()
        {
            // arrange
            var mockRepo = new Mock<IHRIDocksRepository>();
            var service = new HRIDocksService(mockRepo.Object);

            var newDock = new HRIDock { Code = "", DockName = "Dock 1" };

            // act
            var result = await service.CreateHRIDockAsync(newDock);

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));
            mockRepo.Verify(r => r.CreateHRIDockAsync(It.IsAny<HRIDock>()), Times.Never);
        }

        [Test]
        public async Task CreateHRIDockAsync_ReturnsValidationError_WhenDockNameMissing()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var service = new HRIDocksService(mockRepo.Object);

            var newDock = new HRIDock { Code = "D1", DockName = "" };

            var result = await service.CreateHRIDockAsync(newDock);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));
            mockRepo.Verify(r => r.CreateHRIDockAsync(It.IsAny<HRIDock>()), Times.Never);
        }

        [Test]
        public async Task CreateHRIDockAsync_CallsRepository_OnValidInput()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var newDock = new HRIDock { Code = "D1", DockName = "Dock 1" };

            var repoResponse = new ServiceResponse<HRIDock>
            {
                Data = newDock,
                Success = true
            };

            mockRepo.Setup(r => r.CreateHRIDockAsync(It.IsAny<HRIDock>())).ReturnsAsync(repoResponse);

            var service = new HRIDocksService(mockRepo.Object);

            var result = await service.CreateHRIDockAsync(newDock);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("D1", result.Data.Code);

            mockRepo.Verify(r => r.CreateHRIDockAsync(It.Is<HRIDock>(d => d.Code == "D1")), Times.Once);
        }


        // GetSingleHRIDockAsync --ready
        [Test]
        public async Task GetSingleHRIDockAsync_ReturnsDock_WhenExisting()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var dock = new HRIDock { Id = 1, Code = "D1", DockName = "Dock 1", IsActive = true };
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = dock, Success = true });

            var id = 1;
            var service = new HRIDocksService(mockRepo.Object);
            var result = await service.GetSingleHRIDockAsync(id);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(id, result.Data.Id);
            Assert.AreEqual("D1", result.Data.Code);

            mockRepo.Verify(r => r.GetSingleHRIDockAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task GetSingleHRIDockAsync_ReturnsError_WhenNotExisting()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = null, Success = false });

            var id = 99;
            var service = new HRIDocksService(mockRepo.Object);
            var result = await service.GetSingleHRIDockAsync(id);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
            mockRepo.Verify(r => r.GetSingleHRIDockAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task GetSingleHRIDockAsync_ReturnsError_WhenIdIsNullOrZero()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = null, Success = false });

            var id = 0;
            var service = new HRIDocksService(mockRepo.Object);
            var result = await service.GetSingleHRIDockAsync(id);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
            mockRepo.Verify(r => r.GetSingleHRIDockAsync(It.IsAny<int>()), Times.Once);
        }


        // UpdateHRIDockAsync --ready

        [Test]
        public async Task UpdateHRIDockAsync_ReturnsError_WhenIdInvalid()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var service = new HRIDocksService(mockRepo.Object);

            var update = new HRIDock { Id = 0, Code = "D1", DockName = "Dock 1" };

            var result = await service.UpdateHRIDockAsync(update);

            Assert.IsFalse(result.Success);
            mockRepo.Verify(r => r.UpdateHRIDockAsync(It.IsAny<HRIDock>()), Times.Never);
        }

        [Test]
        public async Task UpdateHRIDockAsync_CallsRepository_WhenExisting()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var existing = new HRIDock { Id = 1, Code = "D1", DockName = "Dock 1" };
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(1)).ReturnsAsync(new ServiceResponse<HRIDock> { Data = existing, Success = true });
            mockRepo.Setup(r => r.UpdateHRIDockAsync(It.IsAny<HRIDock>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = existing, Success = true });

            var service = new HRIDocksService(mockRepo.Object);

            var update = new HRIDock { Id = 1, Code = "D1-upd", DockName = "Dock 1 upd" };

            var result = await service.UpdateHRIDockAsync(update);

            Assert.IsTrue(result.Success);
            mockRepo.Verify(r => r.GetSingleHRIDockAsync(1), Times.Once);
            mockRepo.Verify(r => r.UpdateHRIDockAsync(It.Is<HRIDock>(d => d.Id == 1 && d.Code == "D1-upd")), Times.Once);
        }


        // DeleteHRIDockAsync --ready
        [Test]
        public async Task DeleteHRIDockAsync_ReturnsError_WhenNotExisting()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<HRIDock> { Data = null, Success = false });
            var service = new HRIDocksService(mockRepo.Object);

            var result = await service.DeleteHRIDockAsync(99);

            Assert.IsFalse(result.Success);
            mockRepo.Verify(r => r.DeleteHRIDockAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteHRIDockAsync_CallsRepository_WhenExisting()
        {
            var mockRepo = new Mock<IHRIDocksRepository>();
            var existing = new HRIDock { Id = 1, Code = "D1", DockName = "Dock 1" };
            mockRepo.Setup(r => r.GetSingleHRIDockAsync(1)).ReturnsAsync(new ServiceResponse<HRIDock> { Data = existing, Success = true });
            mockRepo.Setup(r => r.DeleteHRIDockAsync(1)).ReturnsAsync(new ServiceResponse<bool> { Data = true, Success = true });

            var service = new HRIDocksService(mockRepo.Object);

            var result = await service.DeleteHRIDockAsync(1);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data);
            mockRepo.Verify(r => r.GetSingleHRIDockAsync(1), Times.Once);
            mockRepo.Verify(r => r.DeleteHRIDockAsync(1), Times.Once);
        }

    }
}
