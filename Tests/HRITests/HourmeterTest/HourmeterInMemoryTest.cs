using AutoMapper;
using Microsoft.Graph.Privacy;
using Moq;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text;
using Tests.HRITests.HRItemsTest;

namespace Tests.HRITests.HourmeterTest
{
    public  class HourmeterInMemoryTest
    {
        
        
        public HourmeterInMemoryTest( )
        {
            
        }
        [Test]
        public async Task GetAllHourmeterRevisions()
        {
                // arrange
                var context = new GetInMemoryDBContext().GetInMemoryDbContext();
                var mockMapper = new Mock<IMapper>();

                // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
                mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                    .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                    {
                        HriId = dto.HriId,
                        IsActive = dto.IsActive
                    });

                // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
                mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                    .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                    {
                        Id = hr.Id,
                        HriId = hr.HriId,
                        IsActive = hr.IsActive
                    });

                var _mapper = mockMapper.Object;
                var _notificationService = new Mock<INotificationService>().Object;
                var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);

                // act
                var item = new CreateHourMeterRevisionDto
                {
                    HriId = 1,
                    IsActive = true
                };
                var res = await repository.AddHourmeterRevision(item);

               var result = await repository.GetAllHourmeterRevisions();
               
                // assert
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(1));
        }
        [Test]
        public async Task GetHourmeterRevisionByHRIId()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
            mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                {
                    HriId = dto.HriId,
                    IsActive = dto.IsActive
                });
            // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
            mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                {
                    Id = hr.Id,
                    HriId = hr.HriId,
                    IsActive = hr.IsActive
                });
            var _mapper = mockMapper.Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);
            // act
            var item = new CreateHourMeterRevisionDto
            {
                HriId = 1,
                IsActive = true
            };
            await repository.AddHourmeterRevision(item);
            var result = await repository.GetHourmeterRevisionByHRIId(1);
            // assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.HriId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetHourmeterRevisionById()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
            mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                {
                    HriId = dto.HriId,
                    IsActive = dto.IsActive
                });
            // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
            mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                {
                    Id = hr.Id,
                    HriId = hr.HriId,
                    IsActive = hr.IsActive
                });
            var _mapper = mockMapper.Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);
            // act
            var item = new CreateHourMeterRevisionDto
            {
                HriId = 1,
                IsActive = true
            };
            var createResult = await repository.AddHourmeterRevision(item);
            int revisionId = createResult.Data!.Id;
            var result = await repository.GetHourmeterRevisionById(revisionId);
            // assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id, Is.EqualTo(revisionId));
        }

        [Test]
        public async Task AddHourmeterRevision()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
            mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                {
                    HriId = dto.HriId,
                    IsActive = dto.IsActive
                });
            // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
            mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                {
                    Id = hr.Id,
                    HriId = hr.HriId,
                    IsActive = hr.IsActive
                });
            var _mapper = mockMapper.Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);
            // act
            var item = new CreateHourMeterRevisionDto
            {
                HriId = 1,
                IsActive = true
            };
            var result = await repository.AddHourmeterRevision(item);
            // assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.HriId, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateNewDailyRevision()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            // Configurar el mapeo de CreateDailyRevisionDto a DailyRevision
            mockMapper.Setup(m => m.Map<DailyRevisions>(It.IsAny<CreateDailyRevisionDto>()))
                .Returns<CreateDailyRevisionDto>(dto => new DailyRevisions
                {
                    HourmeterRevisionId = dto.EntityRelationId,
                    Day = dto.Day,
                    Month = dto.Month,
                    Year = dto.Year,
                    UserId = dto.UserId,
                    UserType = dto.UserType,
                    Status = dto.Status,
                    IsActive = true
                });

            // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
            mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                {
                    HriId = dto.HriId,
                    IsActive = dto.IsActive
                });
            // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
            mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                {
                    Id = hr.Id,
                    HriId = hr.HriId,
                    IsActive = hr.IsActive
                });

            var _mapper = mockMapper.Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);
            // act
            var createDaily = new CreateDailyRevisionDto
            {
                EntityRelationId = 1,
                Day = 1,
                Month = 1,
                Year = 2024,
                UserId = 12,
                UserType = "Test User Type",
                Status = "Test Status",
                Notification = true,
                Title = "Test Title",
                Message = "Test Message",
                To = 13,
                IsUrgent = false,
                CCPEmails = "",
                
            };
            var item = new CreateHourMeterRevisionDto
            {
                HriId = 1,
                IsActive = true
            };
            var result1 = await repository.AddHourmeterRevision(item);
            var result = await repository.CreateNewDailyRevision(createDaily);
            // assert
            Assert.That(result.Data, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteHourmeterRevision()
        {
            // arrange
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            // Configurar el mapeo de CreateHourMeterRevisionDto a HourmeterRevision
            mockMapper.Setup(m => m.Map<HourmeterRevision>(It.IsAny<CreateHourMeterRevisionDto>()))
                .Returns<CreateHourMeterRevisionDto>(dto => new HourmeterRevision
                {
                    HriId = dto.HriId,
                    IsActive = dto.IsActive
                });
            // Configurar el mapeo de HourmeterRevision a GetHourmeterRevisionDto
            mockMapper.Setup(m => m.Map<GetHourmeterRevisionDto>(It.IsAny<HourmeterRevision>()))
                .Returns<HourmeterRevision>(hr => new GetHourmeterRevisionDto
                {
                    Id = hr.Id,
                    HriId = hr.HriId,
                    IsActive = hr.IsActive
                });
            var _mapper = mockMapper.Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var repository = new HRIHourmeterRevisionRepository(context, _mapper, _notificationService);
            // act
            var item = new CreateHourMeterRevisionDto
            {
                HriId = 1,
                IsActive = true
            };
            var createResult = await repository.AddHourmeterRevision(item);
            int revisionId = createResult.Data!.Id;
            var result = await repository.DeleteHourmeterRevision(revisionId);
            // assert
            Assert.That(result.Data, Is.EqualTo(true));
        }

    }
}
