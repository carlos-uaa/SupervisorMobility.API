using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupervisorMobility.API;

namespace Tests.HRITests.HRIImagesTest
{
    internal class HRImagesControllerTests
    {
        private Mock<IHRImagesService> _mockService;
        private HRImagesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IHRImagesService>();
            _controller = new HRImagesController(_mockService.Object);
        }

        [Test]
        public async Task CreateHRImagesAsync_DelegatesToService()
        {
            var list = new List<CreateHRImageDto> { new CreateHRImageDto { HriId = 1, ImageUrl = "tmp.png", ImageType = "image/png" } };
            _mockService.Setup(s => s.CreateHRImagesAsync(list)).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });
            var res = await _controller.CreateHRImagesAsync(list);
            Assert.IsTrue(res.Success);
            _mockService.Verify(s => s.CreateHRImagesAsync(list), Times.Once);
        }

        [Test]
        public async Task GetImagesByHRIIdAsync_DelegatesToService()
        {
            _mockService.Setup(s => s.GetImagesByHRIIdAsync(1)).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });
            var res = await _controller.GetImagesByHRIIdAsync(1);
            Assert.IsTrue(res.Success);
            _mockService.Verify(s => s.GetImagesByHRIIdAsync(1), Times.Once);
        }

        [Test]
        public async Task UpdateHRImageAsync_DelegatesToService()
        {
            var list = new List<UpdateHRImageDto> { new UpdateHRImageDto { HriId = 1, ImageId = 0, ImageUrl = "tmp.png", ImageType = "image/png" } };
            _mockService.Setup(s => s.UpdateHRImageAsync(list)).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });
            var res = await _controller.UpdateHRImageAsync(list);
            Assert.IsTrue(res.Success);
            _mockService.Verify(s => s.UpdateHRImageAsync(list), Times.Once);
        }

        [Test]
        public async Task DeleteHRImageAsync_DelegatesToService()
        {
            _mockService.Setup(s => s.DeleteHRImageAsync(5)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            var res = await _controller.DeleteHRImageAsync(5);
            Assert.IsTrue(res.Data);
            _mockService.Verify(s => s.DeleteHRImageAsync(5), Times.Once);
        }

        [Test]
        public async Task SaveImageInTempFolderAsync_DelegatesToService()
        {
            var mockFormFile = new Mock<IFormFile>();
            _mockService.Setup(s => s.SaveImageInTempFolderAsync(mockFormFile.Object)).ReturnsAsync(new ServiceResponse<string> { Success = true, Data = "uploads/temp/HRIImages/file.png" });
            var res = await _controller.SaveImageInTempFolderAsync(mockFormFile.Object);
            Assert.IsTrue(res.Success);
            _mockService.Verify(s => s.SaveImageInTempFolderAsync(mockFormFile.Object), Times.Once);
        }

        [Test]
        public void GetImage_ReturnsNotFound_WhenServiceReportsMissing()
        {
            _mockService.Setup(s => s.GetImageContent("p")).Returns(new ServiceResponse<HRImageContentDto> { Success = false, Message = "Image file does not exist." });
            var res = _controller.GetImage("p");
            Assert.IsInstanceOf<NotFoundResult>(res);
        }

        [Test]
        public void GetImage_ReturnsPhysicalFile_WhenServiceProvides()
        {
            var dto = new HRImageContentDto { FilePath = System.IO.Path.GetTempFileName(), ContentType = "image/png" };
            _mockService.Setup(s => s.GetImageContent("p")).Returns(new ServiceResponse<HRImageContentDto> { Success = true, Data = dto });
            var res = _controller.GetImage("p");
            Assert.IsInstanceOf<PhysicalFileResult>(res);
        }
    }
}
