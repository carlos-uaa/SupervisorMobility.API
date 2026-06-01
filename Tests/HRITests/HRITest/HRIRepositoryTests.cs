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


namespace Tests.HRITests.HRITest
{
    internal class HRIRepositoryTests
    {
        private DbContextOptions<SupervisorMobilityContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private CreateHRIDto dtoHri = new CreateHRIDto
        {
            HRILinesId = 1,
            Line = null,
            HRIItemId = 1,
            NameOfItem = null,
            ControlNumber = "GM-0004",
            HRIDockId = 1,
            Dock = null,
            Department = "Manufactura",
            Images = new List<CreateHRImageDto>
                {
                    new CreateHRImageDto { HriId = 0, ImageUrl = "uploads/temp/HRIImages/43ff4da5-8395-47ff-944c-d0be3e85a42b.png", ImageType = "image/png" },
                    new CreateHRImageDto { HriId = 0, ImageUrl = "uploads/temp/HRIImages/02d68585-1acb-4e92-8b5c-7512176a1c94.png", ImageType = "image/png" },
                    new CreateHRImageDto { HriId = 0, ImageUrl = "uploads/temp/HRIImages/cd2dd5ed-4b90-4ac1-baaa-7229d1c10fb9.png", ImageType = "image/png" }
                },
            ItemsRevised = new List<CreateHRIRevisionItemDto>
                {
                    new CreateHRIRevisionItemDto { HriId = 0, ItemNumber = 1, RevisionPoint = "Punto01", RevisionMethodId = 1, VeredictId = 1, FrequencyId = 1 },
                    new CreateHRIRevisionItemDto { HriId = 0, ItemNumber = 2, RevisionPoint = "Punto02", RevisionMethodId = 1, VeredictId = 1, FrequencyId = 1 },
                    new CreateHRIRevisionItemDto { HriId = 0, ItemNumber = 3, RevisionPoint = "Punto03", RevisionMethodId = 1, VeredictId = 1, FrequencyId = 2 }
                },
            WeeklyRevisions = null,
            HriCycles = new List<CreateHRICyclesDto>
                {
                    new CreateHRICyclesDto { HriId = 0, Cycle = 1, SupervisorUserId = 42, OperatorUserId = 457, IsActive = true }
                }
        };

        [Test]
        public async Task CreateHRI_InsertRecord()
        {
            // arrange
            var options = CreateNewContextOptions();
            var mockImgSer = new Mock<IHRImagesService>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockNotSer = new Mock<INotificationService>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();

            mockImgSer.Setup(r => r.CreateHRImagesAsync(It.IsAny<List<CreateHRImageDto>>())).ReturnsAsync(new ServiceResponse<List<HRImages>> { Data = new List<HRImages>(), Success = true });

            mockRevIteRep.Setup(r => r.CreateHRIREvisionItemsByHRIId(
                It.IsAny<int>(),
                It.IsAny<List<CreateHRIRevisionItemDto>>(),
                It.IsAny<int>())).ReturnsAsync(new ServiceResponse<bool> { Data = true, Success = true });

            mockCycRep.Setup(r => r.CreateHRICyclesByHRIId(
                    It.IsAny<int>(),
                    It.IsAny<List<CreateHRICyclesDto>>())).ReturnsAsync(new ServiceResponse<bool> { Data = true, Success = true });

            mockHouRevRep.Setup(r => r.AddHourmeterRevision(
                It.IsAny<CreateHourMeterRevisionDto>())).ReturnsAsync(new ServiceResponse<GetHourmeterRevisionDto> { Data = null, Success = true });

            mockNotSer.Setup(r => r.CreateNotificationAsync(
                    It.IsAny<NotificationToCreateDto>(),
                    It.IsAny<SpecialOptionsNotification>())).ReturnsAsync(new Notification());
                    
            mockMapper.Setup(m => m.Map<GetHRIDto>(It.IsAny<HRI>()))
                    .Returns((HRI s) => new GetHRIDto { ControlNumber = s.ControlNumber, HriId = s.HriId });

            await using (var context = new SupervisorMobilityContext(options))
            {
                

                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                // act
                var result = await repo.CreateHRI(dtoHri);

                // assert
                Assert.IsTrue(result.Success, "Expected the HRI creation to be successful.");
                Assert.IsNotNull(result.Data, "Expected the HRI data to be not null.");
                Assert.That(result.Data.ControlNumber, Is.EqualTo("GM-0004"), "Expected the control number to match.");
                Assert.IsTrue(context.HRIs.Any(d => d.HriId == result.Data.HriId), "Expected the HRI to be present in the context.");
                mockImgSer.Verify(r => r.CreateHRImagesAsync(It.IsAny<List<CreateHRImageDto>>()), Times.Once, "Expected the CreateHRImagesAsync method to be called once.");
                mockRevIteRep.Verify(r => r.CreateHRIREvisionItemsByHRIId(It.IsAny<int>(), It.IsAny<List<CreateHRIRevisionItemDto>>(), It.IsAny<int>()), Times.Once, "Expected the CreateHRIREvisionItemsByHRIId method to be called once.");
                mockCycRep.Verify(r => r.CreateHRICyclesByHRIId(It.IsAny<int>(), It.IsAny<List<CreateHRICyclesDto>>()), Times.Once, "Expected the CreateHRICyclesByHRIId method to be called once.");
                mockHouRevRep.Verify(r => r.AddHourmeterRevision(It.IsAny<CreateHourMeterRevisionDto>()), Times.Never, "Expected the AddHourmeterRevision method to never be called.");
                mockNotSer.Verify(r => r.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>()), Times.Once, "Expected the CreateNotificationAsync method to be called once.");
            }
        }
    

        [Test]
        public async Task CreateNewWeeeklyRevisions_CreatesWeeklyAndSendsNotification()
        {
            // arrange
            var options = CreateNewContextOptions();
            var mockNotSer = new Mock<INotificationService>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            // setup mapper to map CreateWeeklyRevisionDto -> WeeklyRevisions
            mockMapper.Setup(m => m.Map<WeeklyRevisions>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as CreateWeeklyRevisionDto;
                    return new WeeklyRevisions
                    {
                        HriId = dto!.HriId,
                        UserId = dto.UserId,
                        Month = dto.Month,
                        Week = dto.Week,
                        Year = dto.Year,
                        IsActive = dto.IsActive
                    };
                });

            // mapper for history mapping
            mockMapper.Setup(m => m.Map<HRIHistoryActions>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as HRIHistoryItemDto;
                    return new HRIHistoryActions
                    {
                        HRIid = dto!.HRIid,
                        Action = dto.Action,
                        ActionDate = dto.ActionDate,
                        ResponsibleUserId = dto.ResponsibleUserId,
                        ActionType = dto.ActionType
                    };
                });

            mockNotSer.Setup(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), It.IsAny<string?>()))
                .ReturnsAsync(new Notification());

            var weekly = new CreateWeeklyRevisionDto
            {
                HriId = 123,
                UserId = 42,
                Month = DateTime.Now.Month,
                Week = 2,
                Year = DateTime.Now.Year,
                IsActive = true,
                Notification = true,
                Title = "Weekly",
                Message = "Message",
                To = 42,
                IsUrgent = false,
                CCPEmails = "a@a.com"
            };

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                // act
                var result = await repo.CreateNewWeeeklyRevisions(new List<CreateWeeklyRevisionDto> { weekly });

                // assert
                Assert.IsTrue(result.Success);
                Assert.IsTrue(context.WeeklyRevisions.Any(w => w.HriId == 123 && w.Week == 2));
                Assert.IsTrue(context.HRIHistoryActions.Any(h => h.HRIid == 123));
                mockNotSer.Verify(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), It.IsAny<string?>()), Times.Once);
            }
        }

        [Test]
        public async Task DeleteHRI_SetsIsActiveFalseAndSendsNotification()
        {
            var options = CreateNewContextOptions();
            var mockNotSer = new Mock<INotificationService>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            mockNotSer.Setup(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), It.IsAny<string?>()))
                .ReturnsAsync(new Notification());

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed HRI
                var hri = new HRI { HriId = 100, ControlNumber = "DEL-100", IsActive = true, SupervisorUserId = 7 };
                await context.HRIs.AddAsync(hri);
                await context.SaveChangesAsync();

                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                var result = await repo.DeleteHRI(100);

                Assert.IsTrue(result.Success);
                var db = await context.HRIs.FirstOrDefaultAsync(h => h.HriId == 100);
                Assert.IsNotNull(db);
                Assert.IsFalse(db.IsActive);
                mockNotSer.Verify(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), null), Times.Once);
            }
        }

        [Test]
        public async Task SendHistoryAction_CreatesHistoryEntry()
        {
            var options = CreateNewContextOptions();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNotSer = new Mock<INotificationService>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            mockMapper.Setup(m => m.Map<HRIHistoryActions>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as HRIHistoryItemDto;
                    return new HRIHistoryActions
                    {
                        HRIid = dto!.HRIid,
                        Action = dto.Action,
                        ActionDate = dto.ActionDate,
                        ResponsibleUserId = dto.ResponsibleUserId,
                        ActionType = dto.ActionType
                    };
                });

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                var dto = new HRIHistoryItemDto
                {
                    HRIid = 200,
                    Action = "TEST",
                    ActionDate = DateTime.UtcNow,
                    ResponsibleUserId = 1,
                    ActionType = "CREATE"
                };

                var result = await repo.SendHistoryAction(dto);

                Assert.IsTrue(result.Success);
                Assert.IsTrue(context.HRIHistoryActions.Any(h => h.HRIid == 200 && h.Action == "TEST"));
            }
        }

        [Test]
        public async Task GetAllHRI_ReturnsMappedList()
        {
            var options = CreateNewContextOptions();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNotSer = new Mock<INotificationService>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            mockMapper.Setup(m => m.Map<GetHRIDto>(It.IsAny<HRI>()))
                .Returns((HRI s) => new GetHRIDto { HriId = s.HriId, ControlNumber = s.ControlNumber });

            await using (var context = new SupervisorMobilityContext(options))
            {
                await context.HRIs.AddAsync(new HRI { HriId = 1, ControlNumber = "A" });
                await context.HRIs.AddAsync(new HRI { HriId = 2, ControlNumber = "B" });
                await context.SaveChangesAsync();

                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                var result = await repo.GetAllHRI();

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(2, result.Data.Count);
            }
        }

        [Test]
        public async Task GetHRIById_ReturnsProperDto()
        {
            var options = CreateNewContextOptions();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNotSer = new Mock<INotificationService>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            mockMapper.Setup(m => m.Map<GetHRIDto>(It.IsAny<HRI>()))
                .Returns((HRI s) => new GetHRIDto { HriId = s.HriId, ControlNumber = s.ControlNumber });

            await using (var context = new SupervisorMobilityContext(options))
            {
                var hri = new HRI { HriId = 500, ControlNumber = "CTRL500" };
                await context.HRIs.AddAsync(hri);
                await context.SaveChangesAsync();

                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                var result = await repo.GetHRIById(500);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual("CTRL500", result.Data.ControlNumber);
            }
        }

        [Test]
        public async Task UpdateHRI_ProcessesCyclesItemsImagesAndNotifications()
        {
            var options = CreateNewContextOptions();

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNotSer = new Mock<INotificationService>();
            var mockRevIteRep = new Mock<IHRIRevisionItemRepository>();
            var mockCycRep = new Mock<IHRICyclesRepository>();
            var mockHouRevRep = new Mock<IHRIHourmeterRevisionRepository>();
            var mockRevCyclesRep = new Mock<IHRIRevisionCyclesRepository>();
            var mockImgSer = new Mock<IHRImagesService>();

            // mapper for history actions
            mockMapper.Setup(m => m.Map<HRIHistoryActions>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as HRIHistoryItemDto;
                    return new HRIHistoryActions
                    {
                        HRIid = dto!.HRIid,
                        Action = dto.Action,
                        ActionDate = dto.ActionDate,
                        ResponsibleUserId = dto.ResponsibleUserId,
                        ActionType = dto.ActionType
                    };
                });

            mockMapper.Setup(m => m.Map<CreateHRICyclesDto>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as UpdateFullHRICyclesDto;
                    return new CreateHRICyclesDto
                    {
                        HriId = dto.HriId,
                        Cycle = dto.Cycle,
                        SupervisorUserId = dto.SupervisorUserId,
                        OperatorUserId = dto.OperatorUserId,
                        IsActive = dto.IsActive ?? true
                    };
                });

            mockMapper.Setup(m => m.Map<CreateHRIRevisionItemDto>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var dto = src as UpdateRevisionItemDto;
                    return new CreateHRIRevisionItemDto
                    {
                        HriId = dto.HriId,
                        ItemNumber = dto.ItemNumber,
                        RevisionPoint = dto.RevisionPoint,
                        RevisionMethodId = dto.RevisionMethodId,
                        VeredictId = dto.VeredictId,
                        FrequencyId = dto.FrequencyId,
                        IsActive = dto.IsActive ?? true
                    };
                });

            // setup repository behaviors
            mockCycRep.Setup(r => r.DeleteHRICycle(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<bool> { Data = true, Success = true }); 
            mockRevCyclesRep.Setup(r => r.DeleteRevisionCycleByHriId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            mockCycRep.Setup(r => r.CreateHRICycle(It.IsAny<CreateHRICyclesDto>())).ReturnsAsync(new ServiceResponse<GetHRICyclesDto> { Success = true });
            mockRevCyclesRep.Setup(r => r.AddNewRevisionCycleToRevisionsItems(It.IsAny<int>(), It.IsAny<CreateRevisionCyclesDto>())).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });

            mockRevIteRep.Setup(r => r.DeleteHRIRevisionItem(It.IsAny<int>())).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            mockRevIteRep.Setup(r => r.ValidateItemForUpdate(It.IsAny<int>(), It.IsAny<UpdateHRIRevisionItemDto>())).ReturnsAsync(new ServiceResponse<bool> { Success = true, Data = true });
            mockRevIteRep.Setup(r => r.UpdateHRIRevisionItem(It.IsAny<int>(), It.IsAny<UpdateHRIRevisionItemDto>())).ReturnsAsync(new ServiceResponse<GetHRIRevisionItemDto> { Success = true });
            mockRevIteRep.Setup(r => r.CreateHRIRevisionItem(It.IsAny<CreateHRIRevisionItemDto>())).ReturnsAsync(new ServiceResponse<GetHRIRevisionItemDto> { Success = true });

            mockImgSer.Setup(i => i.UpdateHRImageAsync(It.IsAny<List<UpdateHRImageDto>>()))
                .ReturnsAsync(new ServiceResponse<List<HRImages>> { Success = true, Data = new List<HRImages>() });

            mockNotSer.Setup(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), It.IsAny<string?>()))
                .ReturnsAsync(new Notification());

            // seed context
            await using (var context = new SupervisorMobilityContext(options))
            {
                var hri = new HRI { HriId = 1, ControlNumber = "OLD", IsActive = true, SupervisorUserId = 5, SSVUserId = 6 };
                await context.HRIs.AddAsync(hri);
                await context.SaveChangesAsync();

                var repo = new HRIRepository(context,
                    mockMapper.Object,
                    mockRevIteRep.Object,
                    mockCycRep.Object,
                    mockHouRevRep.Object,
                    mockRevCyclesRep.Object,
                    mockImgSer.Object,
                    mockNotSer.Object);

                var updateDto = new UpdateHRIDto
                {
                    ControlNumber = "NEW",
                    SupervisorUserId = 99,
                    SSVUserId = 100,
                    HRICycles = new List<UpdateFullHRICyclesDto>
                    {
                        new UpdateFullHRICyclesDto { CycleId = 0, Cycle = 10, Deleted = false },
                        new UpdateFullHRICyclesDto { CycleId = 11, Cycle = 2, Deleted = true }
                    },
                    RevisionItems = new List<UpdateRevisionItemDto>
                    {
                        new UpdateRevisionItemDto { ItemId = 0, RevisionPoint = "NewItem" },
                        new UpdateRevisionItemDto { ItemId = 21, Deleted = true, RevisionPoint = "ToDelete" },
                        new UpdateRevisionItemDto { ItemId = 22, Deleted = false, RevisionPoint = "ToUpdate" }
                    },
                    Images = new List<UpdateHRImageDto>
                    {
                        new UpdateHRImageDto { ImageId = 1, ImageUrl = "u.png", ImageType = "image/png", delete = false }
                    }
                };

                // act
                var result = await repo.UpdateHRI(1, updateDto);

                // assert
                Assert.IsTrue(result.Success);
                var db = await context.HRIs.FirstOrDefaultAsync(h => h.HriId == 1);
                Assert.IsNotNull(db);
                Assert.AreEqual("NEW", db.ControlNumber);

                mockCycRep.Verify(r => r.DeleteHRICycle(11), Times.Once);
                mockRevCyclesRep.Verify(r => r.DeleteRevisionCycleByHriId(1, 2), Times.Once);
                mockCycRep.Verify(r => r.CreateHRICycle(It.IsAny<CreateHRICyclesDto>()), Times.Once);
                mockRevCyclesRep.Verify(r => r.AddNewRevisionCycleToRevisionsItems(1, It.IsAny<CreateRevisionCyclesDto>()), Times.Once);

                mockRevIteRep.Verify(r => r.DeleteHRIRevisionItem(21), Times.Once);
                mockRevIteRep.Verify(r => r.ValidateItemForUpdate(22, It.IsAny<UpdateHRIRevisionItemDto>()), Times.Once);
                mockRevIteRep.Verify(r => r.UpdateHRIRevisionItem(22, It.IsAny<UpdateHRIRevisionItemDto>()), Times.Once);
                mockRevIteRep.Verify(r => r.CreateHRIRevisionItem(It.IsAny<CreateHRIRevisionItemDto>()), Times.Once);

                mockImgSer.Verify(i => i.UpdateHRImageAsync(It.IsAny<List<UpdateHRImageDto>>()), Times.Once);

                mockNotSer.Verify(n => n.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>(), It.IsAny<string?>()), Times.Once);
            }
        }
    }
}
