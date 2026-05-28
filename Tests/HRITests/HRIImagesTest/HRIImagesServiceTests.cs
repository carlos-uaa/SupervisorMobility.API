using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using SupervisorMobility.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using SupervisorMobility.API.DataAccess.Services.HRIServices;

namespace Tests.HRITests.HRIImagesTest
{
    internal class HRIImagesServiceTests
    {
        private Mock<IHRImagesRepository> _mockRepo;
        private HRImagesService _service;
        private string _contentRoot;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IHRImagesRepository>();
            _contentRoot = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "HRIImagesTests", System.Guid.NewGuid().ToString());
            System.IO.Directory.CreateDirectory(_contentRoot);
            var mockEnv = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            mockEnv.Setup(e => e.ContentRootPath).Returns(_contentRoot);
            _service = new HRImagesService(_mockRepo.Object, mockEnv.Object);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (System.IO.Directory.Exists(_contentRoot)) System.IO.Directory.Delete(_contentRoot, true);
            }
            catch { }
        }

        [Test]
        public async Task GetImagesByHRIIdAsync_DelegatesToRepository()
        {
            _mockRepo.Setup(r => r.GetImagesByHRIIdAsync(1)).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });
            var res = await _service.GetImagesByHRIIdAsync(1);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.GetImagesByHRIIdAsync(1), Times.Once);
        }

        [Test]
        public async Task CreateHRImagesAsync_ReturnsError_WhenListEmpty()
        {
            var res = await _service.CreateHRImagesAsync(new List<CreateHRImageDto>());
            Assert.IsFalse(res.Success);
            Assert.IsTrue(res.Message.Contains("required"));
        }

        [Test]
        public async Task CreateHRImagesAsync_ReturnsError_WhenImageUrlMissing()
        {
            var list = new List<CreateHRImageDto> { new CreateHRImageDto { HriId = 1, ImageUrl = "", ImageType = "image/png" } };
            var res = await _service.CreateHRImagesAsync(list);
            Assert.IsFalse(res.Success);
            Assert.IsTrue(res.Message.Contains("ImageUrl"));
        }

        [Test]
        public async Task CreateHRImagesAsync_Success_MovesFileAndCallsRepo()
        {
            // create temp file
            var uploadsTemp = System.IO.Path.Combine(_contentRoot, "tempfiles");
            System.IO.Directory.CreateDirectory(uploadsTemp);
            var tempFile = System.IO.Path.Combine(uploadsTemp, "f.png");
            System.IO.File.WriteAllText(tempFile, "x");

            var dto = new CreateHRImageDto { HriId = 1, ImageUrl = System.IO.Path.Combine("tempfiles", "f.png"), ImageType = "image/png" };
            var list = new List<CreateHRImageDto> { dto };

            _mockRepo.Setup(r => r.CreateHRImagesAsync(It.IsAny<List<CreateHRImageDto>>())).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });

            var res = await _service.CreateHRImagesAsync(list);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.CreateHRImagesAsync(It.IsAny<List<CreateHRImageDto>>()), Times.Once);
        }

        [Test]
        public async Task UpdateHRImageAsync_ReturnsError_WhenListEmpty()
        {
            var res = await _service.UpdateHRImageAsync(new List<UpdateHRImageDto>());
            Assert.IsFalse(res.Success);
        }

        [Test]
        public async Task UpdateHRImageAsync_ReturnsValidationError_WhenMissingFields()
        {
            var list = new List<UpdateHRImageDto> { new UpdateHRImageDto { HriId = 1, ImageId = 0, ImageUrl = "", ImageType = "" } };
            var res = await _service.UpdateHRImageAsync(list);
            Assert.IsFalse(res.Success);
            Assert.IsTrue(res.Message.Contains("Validation error"));
        }

        [Test]
        public async Task UpdateHRImageAsync_Success_MovesAndDeletesAndCallsRepo()
        {
            // new temp image
            var tempDir = System.IO.Path.Combine(_contentRoot, "temp");
            System.IO.Directory.CreateDirectory(tempDir);
            var newTemp = System.IO.Path.Combine(tempDir, "n.png");
            System.IO.File.WriteAllText(newTemp, "x");

            // existing image to be deleted
            var uploadsDir = System.IO.Path.Combine(_contentRoot, "uploads", "HRIImages", "1");
            System.IO.Directory.CreateDirectory(uploadsDir);
            var existing = System.IO.Path.Combine(uploadsDir, "old.png");
            System.IO.File.WriteAllText(existing, "y");

            var updateList = new List<UpdateHRImageDto>
            {
                new UpdateHRImageDto { HriId = 1, ImageId = 0, ImageUrl = System.IO.Path.Combine("temp", "n.png"), ImageType = "image/png", delete = false },
                new UpdateHRImageDto { HriId = 1, ImageId = 5, ImageUrl = System.IO.Path.Combine("uploads", "HRIImages", "1", "old.png"), ImageType = "image/png", delete = true }
            };

            _mockRepo.Setup(r => r.GetHRImageByImageIdAsync(5)).ReturnsAsync(new ServiceResponse<HRImages> { Success = true, Data = new HRImages { ImageUrl = System.IO.Path.Combine("uploads", "HRIImages", "1", "old.png") } });
            _mockRepo.Setup(r => r.UpdateHRImageAsync(It.IsAny<List<UpdateHRImageDto>>())).ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });

            var res = await _service.UpdateHRImageAsync(updateList);
            Assert.IsTrue(res.Success);
            _mockRepo.Verify(r => r.UpdateHRImageAsync(It.IsAny<List<UpdateHRImageDto>>()), Times.Once);
        }

        [Test]
        public async Task DeleteHRImageAsync_ReturnsError_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetHRImageByImageIdAsync(9)).ReturnsAsync(new ServiceResponse<HRImages> { Success = false });
            var res = await _service.DeleteHRImageAsync(9);
            Assert.IsFalse(res.Success);
        }

        [Test]
        public async Task DeleteHRImageAsync_DelegatesToRepo_WhenFound()
        {
            _mockRepo.Setup(r => r.GetHRImageByImageIdAsync(3)).ReturnsAsync(new ServiceResponse<HRImages> { Success = true, Data = new HRImages { ImageUrl = System.IO.Path.Combine("uploads", "HRIImages", "3", "a.png") } });
            _mockRepo.Setup(r => r.DeleteHRImageAsync(3)).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            // create file referenced
            var filePath = System.IO.Path.Combine(_contentRoot, "uploads", "HRIImages", "3", "a.png");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);
            System.IO.File.WriteAllText(filePath, "z");

            var res = await _service.DeleteHRImageAsync(3);
            Assert.IsTrue(res.Success && res.Data);
            _mockRepo.Verify(r => r.DeleteHRImageAsync(3), Times.Once);
        }

        [Test]
        public async Task SaveImageInTempFolderAsync_ReturnsError_WhenNoFile()
        {
            var res = await _service.SaveImageInTempFolderAsync(null!);
            Assert.IsFalse(res.Success);
        }

        [Test]
        public async Task SaveImageInTempFolderAsync_SavesFile_WhenValidImage()
        {
            var ms = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(ms);
            writer.Write("x");
            writer.Flush();
            ms.Position = 0;

            var formFile = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
            formFile.Setup(f => f.Length).Returns(ms.Length);
            formFile.Setup(f => f.ContentType).Returns("image/png");
            formFile.Setup(f => f.FileName).Returns("t.png");
            formFile.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns<System.IO.Stream, System.Threading.CancellationToken>((s, c) => ms.CopyToAsync(s));

            var res = await _service.SaveImageInTempFolderAsync(formFile.Object);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.Data.Contains("uploads"));
            // file should exist
            var path = System.IO.Path.Combine(_contentRoot, res.Data.Replace('/', System.IO.Path.DirectorySeparatorChar));
            Assert.IsTrue(System.IO.File.Exists(path));
        }

        [Test]
        public void GetImageContent_ReturnsError_WhenInvalidPath()
        {
            var res = _service.GetImageContent("");
            Assert.IsFalse(res.Success);
        }

        [Test]
        public void GetImageContent_ReturnsError_WhenFileNotExist()
        {
            var res = _service.GetImageContent("uploads/HRIImages/1/no.png");
            Assert.IsFalse(res.Success);
        }

        [Test]
        public void GetImageContent_ReturnsSuccess_WhenFileExists()
        {
            var dir = System.IO.Path.Combine(_contentRoot, "uploads", "HRIImages", "1");
            System.IO.Directory.CreateDirectory(dir);
            var file = System.IO.Path.Combine(dir, "ok.png");
            System.IO.File.WriteAllText(file, "x");

            var res = _service.GetImageContent("uploads/HRIImages/1/ok.png");
            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Data);
        }
    }
}
