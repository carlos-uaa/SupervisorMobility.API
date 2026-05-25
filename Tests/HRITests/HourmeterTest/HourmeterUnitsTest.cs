using Microsoft.AspNetCore.Mvc;
using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HourmeterTest
{
    public class HourmeterUnitsTest
    {
        [Test]
        public async Task GetAllHourmeterRevisions()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var expectedResponse = new ServiceResponse<List<GetHourmeterRevisionDto>>
            {
                Success = true,
                Data = new List<GetHourmeterRevisionDto>
                    {
                        new GetHourmeterRevisionDto {
                            Id = 1,
                            HriId = 1,
                            DailyRevisions = new List<GetDailyRevisionDto>
                                {
                                    new GetDailyRevisionDto
                                    {
                                        RevisionId = 1,
                                        RevisionCycleId = 1,
                                        CycleId = null,
                                        HourmeterRevisionId = null,
                                        Day = 1,
                                        Month = 1,
                                        UserId = 12,
                                        Responsible = new GetUserForHRIDailyRevsionDto
                                        {
                                            UserId = 12,
                                            Name = "Test User",
                                            Email = "testuser@example.com",
                                            UserType = 1,

                                        },
                                        UserType = "Test User Type",
                                        Status = "Test Status",
                                        IsActive = true
                                    }

                                } ,
                            IsActive = true
                        },
                        new GetHourmeterRevisionDto {
                            Id = 2,
                            HriId = 2,
                            DailyRevisions = new List<GetDailyRevisionDto>
                                {
                                    new GetDailyRevisionDto
                                    {
                                        RevisionId = 2,
                                        RevisionCycleId = 2,
                                        CycleId = null,
                                        HourmeterRevisionId = null,
                                        Day = 2,
                                        Month = 1,
                                        UserId = 13,
                                        Responsible = new GetUserForHRIDailyRevsionDto
                                        {
                                            UserId = 13,
                                            Name = "Test User",
                                            Email = "testuser@example.com",
                                            UserType = 1,

                                        },
                                        UserType = "Test User Type",
                                        Status = "Test Status",
                                        IsActive = true
                                    }

                                },
                            IsActive = true
                        }
                    },
                Message = "Hourmeter revisions retrieved successfully."
            };

            mockService.Setup(service => service.GetAllHourmeterRevisions())
                .ReturnsAsync(expectedResponse);

            var controller = new HRIHourmeterRevisionController(mockService.Object);

            var result = await controller.GetAllHourmeterRevisions();

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetHourmeterRevisionDto>>>());
            var actualResponse = (ServiceResponse<List<GetHourmeterRevisionDto>>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.Not.Null);
            Assert.That(actualResponse.Data.Count, Is.EqualTo(expectedResponse.Data.Count));
        }
        [Test]
        public async Task GetHourmeterRevisionByHRIId()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var response = new ServiceResponse<GetHourmeterRevisionDto>
            {
                Success = true,
                Data = new GetHourmeterRevisionDto
                {
                    Id = 1,
                    HriId = 1,
                    DailyRevisions = new List<GetDailyRevisionDto>
                                {
                                    new GetDailyRevisionDto
                                    {
                                        RevisionId = 1,
                                        RevisionCycleId = 1,
                                        CycleId = null,
                                        HourmeterRevisionId = null,
                                        Day = 1,
                                        Month = 1,
                                        UserId = 12,
                                        Responsible = new GetUserForHRIDailyRevsionDto
                                        {
                                            UserId = 12,
                                            Name = "Test User",
                                            Email = "testuser@example.com",
                                            UserType = 1,

                                        },
                                        UserType = "Test User Type",
                                        Status = "Test Status",
                                        IsActive = true
                                    }

                                },
                    IsActive = true
                },
                Message = "Hourmeter revision found."
            };

            mockService.Setup(service => service.GetHourmeterRevisionByHRIId(1))
                .ReturnsAsync(response);

            var controller = new HRIHourmeterRevisionController(mockService.Object);

            var result = await controller.GetHourmeterRevisionByHRIId(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHourmeterRevisionDto>>());
            var actualResponse = (ServiceResponse<GetHourmeterRevisionDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.Not.Null);

        }

        [Test]
        public async Task GetHourmeterRevisionById()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var response = new ServiceResponse<GetHourmeterRevisionDto>
            {
                Success = true,
                Data = new GetHourmeterRevisionDto
                {
                    Id = 1,
                    HriId = 1,
                    DailyRevisions = new List<GetDailyRevisionDto>
                                {
                                    new GetDailyRevisionDto
                                    {
                                        RevisionId = 1,
                                        RevisionCycleId = 1,
                                        CycleId = null,
                                        HourmeterRevisionId = null,
                                        Day = 1,
                                        Month = 1,
                                        UserId = 12,
                                        Responsible = new GetUserForHRIDailyRevsionDto
                                        {
                                            UserId = 12,
                                            Name = "Test User",
                                            Email = "testuser@example.com",
                                            UserType = 1,

                                        },
                                        UserType = "Test User Type",
                                        Status = "Test Status",
                                        IsActive = true
                                    }

                                },
                    IsActive = true
                },
                Message = "Hourmeter revision found."
            };

            mockService.Setup(service => service.GetHourmeterRevisionById(1))
                .ReturnsAsync(response);

            var controller = new HRIHourmeterRevisionController(mockService.Object);

            var result = await controller.GetHourmeterRevisionById(1);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHourmeterRevisionDto>>());
            var actualResponse = (ServiceResponse<GetHourmeterRevisionDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.Not.Null);
        }

        [Test]
        public async Task AddHourmeterRevision()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var response = new ServiceResponse<GetHourmeterRevisionDto>
            {
                Success = true,
                Data = new GetHourmeterRevisionDto
                {
                    Id = 1,
                    HriId = 1,
                    DailyRevisions = new List<GetDailyRevisionDto>
                                {
                                    new GetDailyRevisionDto
                                    {
                                        RevisionId = 1,
                                        RevisionCycleId = 1,
                                        CycleId = null,
                                        HourmeterRevisionId = null,
                                        Day = 1,
                                        Month = 1,
                                        UserId = 12,
                                        Responsible = new GetUserForHRIDailyRevsionDto
                                        {
                                            UserId = 12,
                                            Name = "Test User",
                                            Email = "testuser@example.com",
                                            UserType = 1,

                                        },
                                        UserType = "Test User Type",
                                        Status = "Test Status",
                                        IsActive = true
                                    }

                                },
                    IsActive = true
                },
                Message = "Hourmeter revision found."
            };
            var newHourmeter = new CreateHourMeterRevisionDto 
            {
                HriId = 1,
                IsActive = true
            };

            mockService.Setup(service => service.AddHourmeterRevision(newHourmeter))
                .ReturnsAsync(response);

            var controller = new HRIHourmeterRevisionController(mockService.Object);

            var result = await controller.AddHourmeterRevision(newHourmeter);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHourmeterRevisionDto>>());
            var actualResponse = (ServiceResponse<GetHourmeterRevisionDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.Not.Null);

        }

        [Test]
        public async Task AddDailyRevisionToHourmeterRevision()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var response = new ServiceResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Daily revision created successfully."
            };

            var newDaily = new CreateDailyRevisionDto
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
                CCPEmails = ""
            };

            
            mockService.Setup(service => service.CreateNewDailyRevision(newDaily))
                .ReturnsAsync(response);

            var controller = new HRIHourmeterRevisionController(mockService.Object);

            var result = await controller.AddDailyRevisionToHourmeterRevision(newDaily);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);
        }

        [Test]
        public async Task DeleteHourmeterRevision()
        {
            var mockService = new Mock<IHRIHourmeterRevisionService>();
            var response = new ServiceResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Hourmeter revision deleted successfully."
            };
            mockService.Setup(service => service.DeleteHourmeterRevision(1))
                .ReturnsAsync(response);
            var controller = new HRIHourmeterRevisionController(mockService.Object);
            var result = await controller.DeleteHourmeterRevision(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);
        }

    }
}
