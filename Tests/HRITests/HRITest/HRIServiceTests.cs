using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Tests.HRITests.HRITest
{
    internal class HRIServiceTests
    {
        private Mock<IHRIRepository> _mockRepo;
        private HRIServices _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IHRIRepository>();
            _service = new HRIServices(_mockRepo.Object);
        }

        [Test]
        public async Task GetAllHRI_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetAllHRI()).ReturnsAsync(new ServiceResponse<List<GetHRIDto>> { Success = true, Data = new List<GetHRIDto>() });
            var res = await _service.GetAllHRI();
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetAllHRI(), Times.Once);
        }

        [Test]
        public async Task GetHRIById_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetHRIById(1)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _service.GetHRIById(1);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetHRIById(1), Times.Once);
        }

        [Test]
        public async Task CreateHRI_DelegatesToRepository()
        {
            var dto = new CreateHRIDto { ControlNumber = "C1" };
            _mockRepo.Setup(r => r.CreateHRI(dto)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _service.CreateHRI(dto);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.CreateHRI(dto), Times.Once);
        }

        [Test]
        public async Task CreateNewWeeeklyRevisions_DelegatesToRepository()
        {
            var list = new List<CreateWeeklyRevisionDto>();
            _mockRepo.Setup(r => r.CreateNewWeeeklyRevisions(list)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _service.CreateNewWeeeklyRevisions(list);
            Assert.IsTrue(res.Success && res.Data);
            _mockRepo.Verify(r => r.CreateNewWeeeklyRevisions(list), Times.Once);
        }

        [Test]
        public async Task UpdateHRI_DelegatesToRepository()
        {
            var dto = new UpdateHRIDto();
            _mockRepo.Setup(r => r.UpdateHRI(5, dto)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _service.UpdateHRI(5, dto);
            Assert.IsTrue(res.Success && res.Data);
            _mockRepo.Verify(r => r.UpdateHRI(5, dto), Times.Once);
        }

        [Test]
        public async Task GetAllHRITable_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetAllHRITable()).ReturnsAsync(new ServiceResponse<List<GetHRIToTableDto>> { Success = true, Data = new List<GetHRIToTableDto>() });
            var res = await _service.GetAllHRITable();
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetAllHRITable(), Times.Once);
        }

        [Test]
        public async Task DeleteHRI_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.DeleteHRI(2)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _service.DeleteHRI(2);
            Assert.IsTrue(res.Success && res.Data);
            _mockRepo.Verify(r => r.DeleteHRI(2), Times.Once);
        }

        [Test]
        public async Task GetHRIHistory_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetHRIHistory(3)).ReturnsAsync(new ServiceResponse<List<GetHRIHistoryActionDto>> { Success = true, Data = new List<GetHRIHistoryActionDto>() });
            var res = await _service.GetHRIHistory(3);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetHRIHistory(3), Times.Once);
        }

        [Test]
        public async Task CreateExcelHriFile_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.CreateExcelHriFile(4, 5, 2025)).ReturnsAsync(new ServiceResponse<byte[]> { Success = true, Data = new byte[] { 1, 2 } });
            var res = await _service.CreateExcelHriFile(4, 5, 2025);
            Assert.IsTrue(res.Success && res.Data.Length == 2);
            _mockRepo.Verify(r => r.CreateExcelHriFile(4, 5, 2025), Times.Once);
        }

        [Test]
        public async Task GetDailyByMonthAndYear_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetDailyByMonthAndYear(6, 7, 2024)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _service.GetDailyByMonthAndYear(6, 7, 2024);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetDailyByMonthAndYear(6, 7, 2024), Times.Once);
        }

        [Test]
        public async Task GetHriKPIs_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetHriKPIs()).ReturnsAsync(new ServiceResponse<HriKpis> { Success = true, Data = new HriKpis() });
            var res = await _service.GetHriKPIs();
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetHriKPIs(), Times.Once);
        }

        [Test]
        public async Task GetLinesChartData_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetLinesChartData(8)).ReturnsAsync(new ServiceResponse<LinesChartData> { Success = true, Data = new LinesChartData() });
            var res = await _service.GetLinesChartData(8);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetLinesChartData(8), Times.Once);
        }

        [Test]
        public async Task GetGeneralStatusChartData_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetGeneralStatusChartData(9)).ReturnsAsync(new ServiceResponse<GeneralStatusChartData> { Success = true, Data = new GeneralStatusChartData() });
            var res = await _service.GetGeneralStatusChartData(9);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetGeneralStatusChartData(9), Times.Once);
        }

        [Test]
        public async Task GetRecentRevisions_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetRecentRevisions(10, "f")).ReturnsAsync(new ServiceResponse<List<HriRecentRevisionsDto>> { Success = true, Data = new List<HriRecentRevisionsDto>() });
            var res = await _service.GetRecentRevisions(10, "f");
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetRecentRevisions(10, "f"), Times.Once);
        }
    }
}