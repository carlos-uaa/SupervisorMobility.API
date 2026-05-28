using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.HRITests.HRIImagesTest
{
    internal class HRIImagesRepositoryTests
    {
        [Test]
        public async Task GetImagesByHRIIdAsync_ReturnsImagesForGivenHRIId()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                await context.HRImages.AddRangeAsync(
                    new HRImages { HriId = 1, ImageUrl = "url1", ImageType = "type1" },
                    new HRImages { HriId = 1, ImageUrl = "url2", ImageType = "type2" },
                    new HRImages { HriId = 2, ImageUrl = "url3", ImageType = "type3" }
                );
                await context.SaveChangesAsync();

                var repo = new HRImagesRepository(context);
                var result = await repo.GetImagesByHRIIdAsync(1);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.That(result.Data.Count, Is.EqualTo(2));
                Assert.IsTrue(result.Data.All(i => i.HriId == 1));
            }
        }

        [Test]
        public async Task GetHRImageByImageIdAsync_ReturnImageForGiverImageId()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                var image = new HRImages { HriId = 1, ImageUrl = "url1", ImageType = "type1" };
                await context.HRImages.AddAsync(image);
                await context.SaveChangesAsync();

                var repo = new HRImagesRepository(context);
                var result = await repo.GetHRImageByImageIdAsync(image.ImageId);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(image.ImageId, result.Data.ImageId);
            }
        }

        [Test]
        public async Task CreateHRImagesAsync_CreatesImagesSuccessfully()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRImagesRepository(context);

                var imagesToCreate = new List<CreateHRImageDto>
                {
                    new CreateHRImageDto { HriId = 1, ImageUrl = "url1", ImageType = "type1" },
                    new CreateHRImageDto { HriId = 1, ImageUrl = "url2", ImageType = "type2" }
                };

                var result = await repo.CreateHRImagesAsync(imagesToCreate);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsTrue(result.Data.All(i => i.HriId == 1));
            }
        }


        [Test]
        public async Task UpdateHRImageAsync_UpdatesExistingImage()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                var image = new List<HRImages> {
                    new HRImages { HriId = 1, ImageUrl = "url1", ImageType = "type1" },
                    new HRImages { HriId = 2, ImageUrl = "url2", ImageType = "type1" },
                    new HRImages { HriId = 3, ImageUrl = "url3", ImageType = "type1" },
                    new HRImages { HriId = 4, ImageUrl = "url4", ImageType = "type1" }
                };
                await context.HRImages.AddRangeAsync(image);
                await context.SaveChangesAsync();

                var repo = new HRImagesRepository(context);

                var updateDto = new List<UpdateHRImageDto>
                {
                    new UpdateHRImageDto
                    {
                        ImageId = 0,
                        ImageUrl = "updatedUrl5",
                        ImageType = "type5",
                        delete = false
                    },
                    new UpdateHRImageDto
                    {
                        ImageId = 2,
                        ImageUrl = "updatedUrl2",
                        ImageType = "updatedtype2",
                        delete = false
                    },
                    new UpdateHRImageDto
                    {
                        ImageId = 3,
                        ImageUrl = "updatedUrl",
                        ImageType = "updatedType",
                        delete = true
                    }
                };

                var result = await repo.UpdateHRImageAsync(updateDto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual("updatedUrl5", result.Data[0].ImageUrl);
                Assert.AreEqual("type5", result.Data[0].ImageType);
                Assert.IsNull(context.HRImages.FirstOrDefault(i => i.ImageId == 3));
            }
        }



        [Test]
        public async Task DeleteHRImageAsync_DeletesExistingImage()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                var image = new HRImages { HriId = 1, ImageUrl = "url1", ImageType = "type1" };
                await context.HRImages.AddAsync(image);
                await context.SaveChangesAsync();

                var repo = new HRImagesRepository(context);
                var result = await repo.DeleteHRImageAsync(image.ImageId);

                Assert.IsTrue(result.Success);
                Assert.IsFalse(context.HRImages.Any(i => i.ImageId == image.ImageId));
            }
        }
    }
}