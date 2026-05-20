using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SupervisorMobility.API;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;
using SupervisorMobility.API.Models.NotificationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;


namespace Tests.HRITests.HRITest
{
    internal class HRIControllerTests
    {
        private Mock<IHRIServices> _mockService;
        private SupervisorMobility.API.Controllers.HRIControllers.HRIController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IHRIServices>();
            _controller = new SupervisorMobility.API.Controllers.HRIControllers.HRIController(_mockService.Object);
        }

        [Test]
        public async Task GetAllHRI_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetAllHRI()).ReturnsAsync(new ServiceResponse<List<GetHRIDto>> { Success = true, Data = new List<GetHRIDto> { new GetHRIDto() } });
            var res = await _controller.GetAllHRI();
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetAllHRI(), Times.Once);
        }

        [Test]
        public async Task GetHRIById_ReturnsOk_WhenFound()
        {
            _mockService.Setup(s => s.GetHRIById(1)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _controller.GetHRIById(1);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetHRIById(1), Times.Once);
        }

        [Test]
        public async Task GetHRISoftInfoList_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetAllHRITable()).ReturnsAsync(new ServiceResponse<List<GetHRIToTableDto>> { Success = true, Data = new List<GetHRIToTableDto> { new GetHRIToTableDto() } });
            var res = await _controller.GetHRISoftInfoList();
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetAllHRITable(), Times.Once);
        }

        [Test]
        public async Task GetDailyByMonthAndYear_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetDailyByMonthAndYear(1, 5, 2025)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _controller.GetDailyByMonthAndYear(1, 5, 2025);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetDailyByMonthAndYear(1, 5, 2025), Times.Once);
        }

        [Test]
        public async Task GetHRIHistory_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetHRIHistory(2)).ReturnsAsync(new ServiceResponse<List<GetHRIHistoryActionDto>> { Success = true, Data = new List<GetHRIHistoryActionDto>() });
            var res = await _controller.GetHRIHistory(2);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetHRIHistory(2), Times.Once);
        }

        [Test]
        public async Task GetExcelHriFile_ReturnsFile_WhenSuccess()
        {
            _mockService.Setup(s => s.CreateExcelHriFile(3, 4, 2025)).ReturnsAsync(new ServiceResponse<byte[]> { Success = true, Data = new byte[] { 1, 2 } });
            var res = await _controller.GetExcelHriFile(3, 4, 2025);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.FileContentResult>(res);
            _mockService.Verify(s => s.CreateExcelHriFile(3, 4, 2025), Times.Once);
        }

        [Test]
        public async Task CreateHRI_ReturnsOk_WhenCreated()
        {
            var dto = new CreateHRIDto { ControlNumber = "C" };
            _mockService.Setup(s => s.CreateHRI(dto)).ReturnsAsync(new ServiceResponse<GetHRIDto> { Success = true, Data = new GetHRIDto() });
            var res = await _controller.CreateHRI(dto);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.CreateHRI(dto), Times.Once);
        }

        [Test]
        public async Task CreateNewWeeklyRevision_ReturnsOk_WhenSuccess()
        {
            var list = new List<CreateWeeklyRevisionDto>();
            _mockService.Setup(s => s.CreateNewWeeeklyRevisions(list)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _controller.CreateNewWeeklyRevision(list);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.CreateNewWeeeklyRevisions(list), Times.Once);
        }

        [Test]
        public async Task UpdateHRI_ReturnsOk_WhenSuccess()
        {
            var dto = new UpdateHRIDto();
            _mockService.Setup(s => s.UpdateHRI(1, dto)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _controller.UpdateHRI(1, dto);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.UpdateHRI(1, dto), Times.Once);
        }

        [Test]
        public async Task DeleteHRI_ReturnsOk_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteHRI(2)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _controller.DeleteHRI(2);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.DeleteHRI(2), Times.Once);
        }

        [Test]
        public async Task GetHriKPIs_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetHriKPIs()).ReturnsAsync(new ServiceResponse<HriKpis> { Success = true, Data = new HriKpis() });
            var res = await _controller.GetHriKPIs();
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetHriKPIs(), Times.Once);
        }

        [Test]
        public async Task GetLinesChartData_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetLinesChartData(5)).ReturnsAsync(new ServiceResponse<LinesChartData> { Success = true, Data = new LinesChartData() });
            var res = await _controller.GetLinesChartData(5);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetLinesChartData(5), Times.Once);
        }

        [Test]
        public async Task GetGeneralStatusChartData_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetGeneralStatusChartData(6)).ReturnsAsync(new ServiceResponse<GeneralStatusChartData> { Success = true, Data = new GeneralStatusChartData() });
            var res = await _controller.GetGeneralStatusChartData(6);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetGeneralStatusChartData(6), Times.Once);
        }

        [Test]
        public async Task GetRecentRevisions_ReturnsOk_WhenDataExists()
        {
            _mockService.Setup(s => s.GetRecentRevisions(7, "f")).ReturnsAsync(new ServiceResponse<List<HriRecentRevisionsDto>> { Success = true, Data = new List<HriRecentRevisionsDto>() });
            var res = await _controller.GetRecentRevisions(7, "f");
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(res.Result);
            _mockService.Verify(s => s.GetRecentRevisions(7, "f"), Times.Once);
        }
    }
}