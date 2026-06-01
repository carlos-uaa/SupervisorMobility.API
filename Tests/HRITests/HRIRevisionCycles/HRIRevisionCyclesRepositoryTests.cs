using Microsoft.EntityFrameworkCore;
using Moq;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.NotificationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Tests.HRITests.HRIRevisionCycles
{
    internal class HRIRevisionCyclesRepositoryTests
    {

        [Test]
        public async Task GetAllRevisionCycles_ReturnAllRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 2, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 3, IsActive = true }
                );
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetAllRevisionCycles();

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(3, result.Data.Count);
            }
        }

        [Test]
        public async Task GetAllRevisionCycles_NotFoundRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetAllRevisionCycles();

                Assert.IsFalse(result.Success);
                Assert.IsNull(result.Data);
            }
        }

        [Test]
        public async Task GetAllRevisionCyclesByRevisionItemId_ReturnRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map<GetRevisionCyclesDto>(It.IsAny<RevisionCycles>()))
                    .Returns((RevisionCycles rc) => new GetRevisionCyclesDto
                    {
                        RevisionCycleId = rc.RevisionCycleId,
                        Cycle = rc.Cycle,
                        IsActive = rc.IsActive,
                        HRIRevisionItemsId = rc.HRIRevisionItemsId,
                        DailyRevisions = rc.DailyRevisions?.Select(dr => new GetDailyRevisionDto
                        {
                            RevisionId = dr.RevisionId,
                            RevisionCycleId = dr.RevisionCycleId,
                            CycleId = dr.CycleId,
                            HourmeterRevisionId = dr.HourmeterRevisionId,
                            Day = dr.Day,
                            Month = dr.Month,
                            UserId = dr.UserId,
                            Responsible = null,
                            UserType = dr.UserType,
                            Status = dr.Status,
                            IsActive = dr.IsActive
                        }).ToList() ?? new List<GetDailyRevisionDto>()
                    });

            await using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 2, IsActive = true }
                );
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetAllRevisionCyclesByRevisionItemId(1);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsTrue(result.Data.All(rc => rc.HRIRevisionItemsId == 1));
            }
        }

        [Test]
        public async Task GetAllRevisionCyclesByRevisionItemId_NotFoundRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 2, IsActive = true }
                );
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetAllRevisionCyclesByRevisionItemId(3);

                Assert.IsFalse(result.Success);
                Assert.IsNull(result.Data);
            }
        }

        [Test]
        public async Task GetRevisionCycleById_ReturnRevisionCycle()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map<GetRevisionCyclesDto>(It.IsAny<RevisionCycles>()))
                    .Returns((RevisionCycles rc) => new GetRevisionCyclesDto
                    {
                        RevisionCycleId = rc.RevisionCycleId,
                        Cycle = rc.Cycle,
                        IsActive = rc.IsActive,
                        HRIRevisionItemsId = rc.HRIRevisionItemsId,
                        DailyRevisions = rc.DailyRevisions?.Select(dr => new GetDailyRevisionDto
                        {
                            RevisionId = dr.RevisionId,
                            RevisionCycleId = dr.RevisionCycleId,
                            CycleId = dr.CycleId,
                            HourmeterRevisionId = dr.HourmeterRevisionId,
                            Day = dr.Day,
                            Month = dr.Month,
                            UserId = dr.UserId,
                            Responsible = null,
                            UserType = dr.UserType,
                            Status = dr.Status,
                            IsActive = dr.IsActive
                        }).ToList() ?? new List<GetDailyRevisionDto>()
                    });

            await using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 2, IsActive = true }
                );
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetRevisionCycleById(1);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(result.Data.RevisionCycleId == 1);
            }
        }

        [Test]
        public async Task GetRevisionCycleById_NotFoundRevisionCycle()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 2, IsActive = true }
                );
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.GetRevisionCycleById(99);

                Assert.IsFalse(result.Success);
                Assert.IsNull(result.Data);
            }
        }

        [Test]
        public async Task CreateRevisionCycle_CreatesRevisionCycle()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map<RevisionCycles>(It.IsAny<CreateRevisionCyclesDto>()))
                    .Returns((CreateRevisionCyclesDto dto) => new RevisionCycles
                    {
                        RevisionCycleId = 1,
                        Cycle = dto.Cycle,
                        HRIRevisionItemsId = 0,
                        HRIRevisionItems = null,
                        IsActive = true,
                        DailyRevisions = null
                    });

            mockMapper.Setup(m => m.Map<GetRevisionCyclesDto>(It.IsAny<RevisionCycles>()))
                    .Returns((RevisionCycles rc) => new GetRevisionCyclesDto
                    {
                        RevisionCycleId = rc.RevisionCycleId,
                        Cycle = rc.Cycle,
                        IsActive = rc.IsActive,
                        HRIRevisionItemsId = rc.HRIRevisionItemsId,
                        DailyRevisions = rc.DailyRevisions?.Select(dr => new GetDailyRevisionDto
                        {
                            RevisionId = dr.RevisionId,
                            RevisionCycleId = dr.RevisionCycleId,
                            CycleId = dr.CycleId,
                            HourmeterRevisionId = dr.HourmeterRevisionId,
                            Day = dr.Day,
                            Month = dr.Month,
                            UserId = dr.UserId,
                            Responsible = null,
                            UserType = dr.UserType,
                            Status = dr.Status,
                            IsActive = dr.IsActive
                        }).ToList() ?? new List<GetDailyRevisionDto>()
                    });

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var createDto = new CreateRevisionCyclesDto { Cycle = 1 };
                var result = await repo.CreateRevisionCycle(1, createDto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(context.RevisionCycles.Any(rc => rc.Cycle == 1 && rc.HRIRevisionItemsId == 1 && rc.RevisionCycleId == 1));
            }
        }

        [Test]
        public async Task CreateRevisionCyclesByRevisionItemId_CreateRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();

            mockMapper.Setup(m => m.Map<RevisionCycles>(It.IsAny<CreateRevisionCyclesDto>()))
                .Returns((CreateRevisionCyclesDto dto) => new RevisionCycles
                {
                    Cycle = dto.Cycle,
                    HRIRevisionItemsId = 0,
                    HRIRevisionItems = null,
                    IsActive = true,
                    DailyRevisions = null
                });

            var list = new List<CreateRevisionCyclesDto>
            {
                new CreateRevisionCyclesDto { Cycle = 1 },
                new CreateRevisionCyclesDto { Cycle = 2 },
                new CreateRevisionCyclesDto { Cycle = 3 }
            };

            using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, null);
                var result = await repo.CreateRevisionCyclesByRevisionItemId(1, list);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(result.Data);
                Assert.IsTrue(context.RevisionCycles.Count(rc => rc.HRIRevisionItemsId == 1) == 3);
            }
        }

        [Test]
        public async Task CreateNewDailyRevision_CreatesDailyRevision()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockNot.Setup(r => r.CreateNotificationAsync(
                    It.IsAny<NotificationToCreateDto>(),
                    It.IsAny<SpecialOptionsNotification>())).ReturnsAsync(new Notification());

            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddAsync(new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true });
                await context.HRIRevisionItems.AddAsync(new HRIRevisionItems
                {
                    ItemId = 1,
                    HriId = 1,
                    HRI = new HRI { HriId = 1 },
                    ItemNumber = 1,
                    RevisionPoint = "Point1",
                    RevisionMethodId = 1,
                    RevisionMethod = new RevisionMethod { Id = 1, Code = "Code1", Description = "Description1", IsActive = true }
                });
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var createDto = new CreateDailyRevisionDto
                {
                    EntityRelationId = 1,
                    Day = 1,
                    Month = 1,
                    Year = 2026,
                    UserId = 1,
                    UserType = "type1",
                    Status = "NG",
                    Notification = true
                };
                var result = await repo.CreateNewDailyRevision(createDto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(context.DailyRevisions.Any(dr => dr.RevisionCycleId == 1 && dr.UserId == 1 && dr.RevisionId == 1));
                mockNot.Verify(r => r.CreateNotificationAsync(It.IsAny<NotificationToCreateDto>(), It.IsAny<SpecialOptionsNotification>()), Times.Once, "Expected the CreateNotificationAsync method to be called once.");
            }
        }

        [Test]
        public async Task UpdateRevisionCycle_UpdateRevision()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map(It.IsAny<UpdateRevisionCycleDto>(), It.IsAny<RevisionCycles>()))
                .Returns((UpdateRevisionCycleDto dto, RevisionCycles revisionCycle) =>
                {
                    revisionCycle.Cycle = dto.Cycle;
                    return revisionCycle;
                });

            mockMapper.Setup(m => m.Map<GetRevisionCyclesDto>(It.IsAny<RevisionCycles>()))
                .Returns((RevisionCycles rc) => new GetRevisionCyclesDto
                {
                    RevisionCycleId = rc.RevisionCycleId,
                    Cycle = rc.Cycle,
                    IsActive = rc.IsActive,
                    HRIRevisionItemsId = rc.HRIRevisionItemsId,
                    DailyRevisions = rc.DailyRevisions?.Select(dr => new GetDailyRevisionDto
                    {
                        RevisionId = dr.RevisionId,
                        RevisionCycleId = dr.RevisionCycleId,
                        CycleId = dr.CycleId,
                        HourmeterRevisionId = dr.HourmeterRevisionId,
                        Day = dr.Day,
                        Month = dr.Month,
                        UserId = dr.UserId,
                        Responsible = null,
                        UserType = dr.UserType,
                        Status = dr.Status,
                        IsActive = dr.IsActive
                    }).ToList() ?? new List<GetDailyRevisionDto>()
                });


            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddAsync(new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true });
                await context.SaveChangesAsync();



                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var updateDto = new UpdateRevisionCycleDto { Cycle = 2 };
                var result = await repo.UpdateRevisionCycle(1, updateDto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(context.RevisionCycles.Any(rc => rc.RevisionCycleId == 1 && rc.Cycle == 2));
            }
        }

        [Test]
        public async Task DeleteRevisionCycle_SetFalseInIsActiveRevisionCycle()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.RevisionCycles.AddAsync(new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true });
                context.DailyRevisions.AddRange([
                    new DailyRevisions{ RevisionId = 1, RevisionCycleId = 1, CycleId = 1, Day = 1, Month = 1, Year = 2026, UserId = 1, Status = "NG", IsActive = true },
                    new DailyRevisions{ RevisionId = 2, RevisionCycleId = 1, CycleId = 1, Day = 1, Month = 1, Year = 2026, UserId = 1, Status = "OK", IsActive = true }
                ]);

                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.DeleteRevisionCycle(1);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(context.RevisionCycles.FirstOrDefault(rc => rc.RevisionCycleId == 1));
                Assert.IsFalse(context.RevisionCycles.FirstOrDefault(rc => rc.RevisionCycleId == 1).IsActive);
            }
        }

        [Test]
        public async Task DeleteRevisionCycleByHriId_SetFalseInIsActiveRevisionCycles()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.HRIRevisionItems.AddAsync(new HRIRevisionItems
                {
                    ItemId = 1,
                    HriId = 1,
                    HRI = new HRI { HriId = 1 },
                    ItemNumber = 1,
                    RevisionPoint = "Point1",
                    RevisionMethodId = 1,
                    RevisionMethod = new RevisionMethod { Id = 1, Code = "Code1", Description = "Description1", IsActive = true }
                });
                await context.RevisionCycles.AddRangeAsync(
                    new RevisionCycles { RevisionCycleId = 1, Cycle = 1, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 2, Cycle = 2, HRIRevisionItemsId = 1, IsActive = true },
                    new RevisionCycles { RevisionCycleId = 3, Cycle = 3, HRIRevisionItemsId = 2, IsActive = true }
                );
                context.DailyRevisions.AddRange([
                    new DailyRevisions{ RevisionId = 1, RevisionCycleId = 1, CycleId = 1, Day = 1, Month = 1, Year = 2026, UserId = 1, Status = "NG", IsActive = true },
                    new DailyRevisions{ RevisionId = 2, RevisionCycleId = 1, CycleId = 1, Day = 1, Month = 1, Year = 2026, UserId = 1, Status = "OK", IsActive = true }
                ]);


                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var result = await repo.DeleteRevisionCycleByHriId(1, 1);

                Assert.IsTrue(result.Success);
                var revisiondelete = await context.RevisionCycles.Where(rc => rc.HRIRevisionItemsId == 1 && rc.Cycle == 1).ToListAsync();
                var revisionactive = await context.RevisionCycles.Where(rc => rc.HRIRevisionItemsId == 2).ToListAsync();
                Assert.IsTrue(revisiondelete.All(rc => !rc.IsActive ?? false));
                Assert.IsTrue(revisionactive.All(rc => rc.IsActive ?? false));
            }
        }

        [Test]
        public async Task AddNewRevisionCycleToRevisionsItems_AddRevisionCycle()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map<RevisionCycles>(It.IsAny<CreateRevisionCyclesDto>()))
                .Returns((CreateRevisionCyclesDto dto) => new RevisionCycles
                {
                    RevisionCycleId = 1,
                    Cycle = dto.Cycle,
                    HRIRevisionItemsId = 0,
                    HRIRevisionItems = null,
                    IsActive = true,
                    DailyRevisions = null
                });

            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data
                await context.HRIRevisionItems.AddAsync(new HRIRevisionItems
                {
                    ItemId = 1,
                    HriId = 1,
                    HRI = new HRI { HriId = 1 },
                    ItemNumber = 1,
                    RevisionPoint = "Point1",
                    RevisionMethodId = 1,
                    RevisionMethod = new RevisionMethod { Id = 1, Code = "Code1", Description = "Description1", IsActive = true }
                });
                await context.SaveChangesAsync();

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);
                var createDto = new CreateRevisionCyclesDto { Cycle = 1 };
                var result = await repo.AddNewRevisionCycleToRevisionsItems(1, createDto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(context.RevisionCycles.Any(rc => rc.Cycle == 1 && rc.HRIRevisionItemsId == 1 && rc.RevisionCycleId == 1));
            }
        }

        [Test]
        public async Task SendHistoryAction_SendHistory()
        {
            var options = new DbContextOptionsBuilder<SupervisorMobilityContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockNot = new Mock<INotificationService>();

            mockMapper.Setup(m => m.Map<HRIHistoryActions>(It.IsAny<HRIHistoryItemDto>()))
                .Returns((HRIHistoryItemDto dto) => new HRIHistoryActions
                    {
                        HRIid = dto.HRIid,
                        ResponsibleUserId = dto.ResponsibleUserId,
                        Action = dto.Action,
                        ActionType = dto.ActionType,
                        ActionDate = dto.ActionDate
                    });

            using (var context = new SupervisorMobilityContext(options))
            {
                // Seed data

                var repo = new HRIRevisionCyclesRepository(context, mockMapper.Object, mockNot.Object);

                var dto = new HRIHistoryItemDto {
                    HRIid = 1,
                    ResponsibleUserId = 1,                    
                    Action = "CREATE",
                    ActionType = "CREATE",
                    ActionDate = DateTime.Now
                };

                var result = await repo.SendHistoryAction(dto);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.IsTrue(result.Data);
            }
        }
    }
}