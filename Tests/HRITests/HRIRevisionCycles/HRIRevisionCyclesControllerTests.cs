using Moq;
using NUnit.Framework;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using System.Collections.Generic;

namespace Tests.HRITests.HRIRevisionCycles
{
    internal class HRIRevisionCyclesControllerTests
    {
        private Mock<IHRIRevisionCyclesService> _mockService;
        private HRIRevisionCyclesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IHRIRevisionCyclesService>();
            _controller = new HRIRevisionCyclesController(_mockService.Object);
        }

        [Test]
        public async Task GetAllRevisionCycles_DelegatesToService()
        {
            _mockService.Setup(s => s.GetAllRevisionCycles())
                .ReturnsAsync(new ServiceResponse<List<GetRevisionCyclesDto>> { Success = true, Data = new List<GetRevisionCyclesDto>() });

            var result = await _controller.GetAllRevisionCycles();

            Assert.IsTrue(result.Success);
            _mockService.Verify(s => s.GetAllRevisionCycles(), Times.Once);
        }

        [Test]
        public async Task GetAllRevisionCyclesByRevisionItemId_DelegatesToService()
        {
            _mockService.Setup(s => s.GetAllRevisionCyclesByRevisionItemId(1))
                .ReturnsAsync(new ServiceResponse<List<GetRevisionCyclesDto>> { Success = true, Data = new List<GetRevisionCyclesDto>() });

            var result = await _controller.GetAllRevisionCyclesByRevisionItemId(1);

            Assert.IsTrue(result.Success);
            _mockService.Verify(s => s.GetAllRevisionCyclesByRevisionItemId(1), Times.Once);
        }

        [Test]
        public async Task GetRevisionCycleById_DelegatesToService()
        {
            _mockService.Setup(s => s.GetRevisionCycleById(1))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto() });

            var result = await _controller.GetRevisionCycleById(1);

            Assert.IsTrue(result.Success);
            _mockService.Verify(s => s.GetRevisionCycleById(1), Times.Once);
        }

        [Test]
        public async Task CreateRevisionCycle_DelegatesToService()
        {
            var dto = new CreateRevisionCyclesDto { Cycle = 1 };
            _mockService.Setup(s => s.CreateRevisionCycle(1, dto))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto() });

            var result = await _controller.CreateRevisionCycle(1, dto);

            Assert.IsTrue(result.Success);
            _mockService.Verify(s => s.CreateRevisionCycle(1, dto), Times.Once);
        }

        [Test]
        public async Task CreateRevisionCyclesByRevisionItemId_DelegatesToService()
        {
            var list = new List<CreateRevisionCyclesDto> { new CreateRevisionCyclesDto { Cycle = 1 } };
            _mockService.Setup(s => s.CreateRevisionCyclesByRevisionItemId(1, list))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _controller.CreateRevisionCyclesByRevisionItemId(1, list);

            Assert.IsTrue(result.Success && result.Data);
            _mockService.Verify(s => s.CreateRevisionCyclesByRevisionItemId(1, list), Times.Once);
        }

        [Test]
        public async Task CreateNewDailyRevision_DelegatesToService()
        {
            var dto = new CreateDailyRevisionDto();
            _mockService.Setup(s => s.CreateNewDailyRevision(dto))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _controller.CreateNewDailyRevision(dto);

            Assert.IsTrue(result.Success && result.Data);
            _mockService.Verify(s => s.CreateNewDailyRevision(dto), Times.Once);
        }

        [Test]
        public async Task UpdateRevisionCycle_DelegatesToService()
        {
            var dto = new UpdateRevisionCycleDto { Cycle = 2 };
            _mockService.Setup(s => s.UpdateRevisionCycle(1, dto))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto { Cycle = 2 } });

            var result = await _controller.UpdateRevisionCycle(1, dto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Cycle);
            _mockService.Verify(s => s.UpdateRevisionCycle(1, dto), Times.Once);
        }

        [Test]
        public async Task DeleteRevisionCycle_DelegatesToService()
        {
            _mockService.Setup(s => s.DeleteRevisionCycle(1))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _controller.DeleteRevisionCycle(1);

            Assert.IsTrue(result.Success && result.Data);
            _mockService.Verify(s => s.DeleteRevisionCycle(1), Times.Once);
        }
    }
}