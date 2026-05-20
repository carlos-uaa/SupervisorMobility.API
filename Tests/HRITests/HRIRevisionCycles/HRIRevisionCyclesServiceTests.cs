using Moq;
using NUnit.Framework;
using SupervisorMobility.API;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using System.Collections.Generic;

namespace Tests.HRITests.HRIRevisionCycles
{
    internal class HRIRevisionCyclesServiceTests
    {
        private Mock<IHRIRevisionCyclesRepository> _mockRepository;
        private HRIRevisionCyclesServices _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IHRIRevisionCyclesRepository>();
            _service = new HRIRevisionCyclesServices(_mockRepository.Object);
        }

        [Test]
        public async Task GetAllRevisionCycles_DelegatesToRepository()
        {
            _mockRepository.Setup(r => r.GetAllRevisionCycles())
                .ReturnsAsync(new ServiceResponse<List<GetRevisionCyclesDto>> { Success = true, Data = new List<GetRevisionCyclesDto>() });

            var result = await _service.GetAllRevisionCycles();

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(r => r.GetAllRevisionCycles(), Times.Once);
        }

        [Test]
        public async Task GetAllRevisionCyclesByRevisionItemId_DelegatesToRepository()
        {
            _mockRepository.Setup(r => r.GetAllRevisionCyclesByRevisionItemId(1))
                .ReturnsAsync(new ServiceResponse<List<GetRevisionCyclesDto>> { Success = true, Data = new List<GetRevisionCyclesDto>() });

            var result = await _service.GetAllRevisionCyclesByRevisionItemId(1);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(r => r.GetAllRevisionCyclesByRevisionItemId(1), Times.Once);
        }

        [Test]
        public async Task GetRevisionCycleById_DelegatesToRepository()
        {
            _mockRepository.Setup(r => r.GetRevisionCycleById(1))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto() });

            var result = await _service.GetRevisionCycleById(1);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(r => r.GetRevisionCycleById(1), Times.Once);
        }

        [Test]
        public async Task CreateRevisionCycle_DelegatesToRepository()
        {
            var dto = new CreateRevisionCyclesDto { Cycle = 1 };
            _mockRepository.Setup(r => r.CreateRevisionCycle(1, dto))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto() });

            var result = await _service.CreateRevisionCycle(1, dto);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(r => r.CreateRevisionCycle(1, dto), Times.Once);
        }

        [Test]
        public async Task CreateRevisionCyclesByRevisionItemId_DelegatesToRepository()
        {
            var list = new List<CreateRevisionCyclesDto> { new CreateRevisionCyclesDto { Cycle = 1 } };
            _mockRepository.Setup(r => r.CreateRevisionCyclesByRevisionItemId(1, list))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _service.CreateRevisionCyclesByRevisionItemId(1, list);

            Assert.IsTrue(result.Success && result.Data);
            _mockRepository.Verify(r => r.CreateRevisionCyclesByRevisionItemId(1, list), Times.Once);
        }

        [Test]
        public async Task CreateNewDailyRevision_DelegatesToRepository()
        {
            var dto = new CreateDailyRevisionDto();
            _mockRepository.Setup(r => r.CreateNewDailyRevision(dto))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _service.CreateNewDailyRevision(dto);

            Assert.IsTrue(result.Success && result.Data);
            _mockRepository.Verify(r => r.CreateNewDailyRevision(dto), Times.Once);
        }

        [Test]
        public async Task UpdateRevisionCycle_DelegatesToRepository()
        {
            var dto = new UpdateRevisionCycleDto { Cycle = 2 };
            _mockRepository.Setup(r => r.UpdateRevisionCycle(1, dto))
                .ReturnsAsync(new ServiceResponse<GetRevisionCyclesDto> { Success = true, Data = new GetRevisionCyclesDto { Cycle = 2 } });

            var result = await _service.UpdateRevisionCycle(1, dto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Cycle);
            _mockRepository.Verify(r => r.UpdateRevisionCycle(1, dto), Times.Once);
        }

        [Test]
        public async Task DeleteRevisionCycle_DelegatesToRepository()
        {
            _mockRepository.Setup(r => r.DeleteRevisionCycle(1))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            var result = await _service.DeleteRevisionCycle(1);

            Assert.IsTrue(result.Success && result.Data);
            _mockRepository.Verify(r => r.DeleteRevisionCycle(1), Times.Once);
        }
    }
}