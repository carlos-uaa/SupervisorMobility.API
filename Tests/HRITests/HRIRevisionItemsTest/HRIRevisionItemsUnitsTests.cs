using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.Users;

namespace Tests.HRITests.HRIRevisionItemsTest
{
    public  class HRIRevisionItemsUnitsTests
    {
        [Test]
        public async Task GetAllHRIRevisionItems()
        {
                var mockService = new Mock<IHRIRevisionItemService>();
                var hriRevisionItems = new List<GetHRIRevisionItemDto>
                {
                    new GetHRIRevisionItemDto
                    {
                        ItemId = 1,
                        HriId = 1,
                        ItemNumber = 1,
                        RevisionPoint = "Test Revision Point",
                        RevisionMethodId = 1,
                        RevisionMethod = 
                        new GetRevisionMethodDto { 
                            Id = 1,
                            Code = "Test Revision Method",
                            Description = "Testing",
                            IsActive= true},
                        VeredictId = 1,
                        Veredict = new GetVeredictDto
                        {
                            Id = 1,
                            Code = "Test Veredict",
                            Description = "Testing",
                            IsActive= true
                        },
                        FrequencyId= 1,
                        Frequency = new GetFrequencyDto
                        {
                            Id = 1,
                            Code = "Test Frequency",
                            Description = "Testing",
                            IsActive= true
                        },
                        RevisionCycles = new List<GetRevisionCyclesDto>
                        {
                            new GetRevisionCyclesDto
                            {
                                RevisionCycleId = 1,
                                Cycle = 1,
                                HRIRevisionItemsId = 1,
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

                                }
                            }
                        },
                        IsActive = true
                    }
                };
                var serviceResponse = new ServiceResponse<List<GetHRIRevisionItemDto>>
                {
                    Data = hriRevisionItems,
                    Success = true,
                    Message = "HRI Revision Items retrieved successfully."
                };
                 mockService.Setup(service => service.GetAllHRIRevisionItems()).ReturnsAsync(serviceResponse);

                var controller = new HRIRevisionItemController(mockService.Object);
                var result = await controller.GetAllHRIRevisionItems();

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetHRIRevisionItemDto>>>());
            var actualResponse = (ServiceResponse<List<GetHRIRevisionItemDto>>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Count, Is.EqualTo(1));
        }
        [Test]
        public async Task GetHRIRevisionItemById()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var hriRevisionItem = new GetHRIRevisionItemDto
            {
                ItemId = 1,
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Test Revision Point",
                RevisionMethodId = 1,
                RevisionMethod =
                        new GetRevisionMethodDto
                        {
                            Id = 1,
                            Code = "Test Revision Method",
                            Description = "Testing",
                            IsActive = true
                        },
                VeredictId = 1,
                Veredict = new GetVeredictDto
                {
                    Id = 1,
                    Code = "Test Veredict",
                    Description = "Testing",
                    IsActive = true
                },
                FrequencyId = 1,
                Frequency = new GetFrequencyDto
                {
                    Id = 1,
                    Code = "Test Frequency",
                    Description = "Testing",
                    IsActive = true
                },
                RevisionCycles = new List<GetRevisionCyclesDto>
                        {
                            new GetRevisionCyclesDto
                            {
                                RevisionCycleId = 1,
                                Cycle = 1,
                                HRIRevisionItemsId = 1,
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

                                }
                            }
                        },
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetHRIRevisionItemDto>
            {
                Data = hriRevisionItem,
                Success = true,
                Message = "HRI Revision Item retrieved successfully."
            };

            mockService.Setup(service => service.GetHRIRevisionItemById(1)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetHRIRevisionItemById(1);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHRIRevisionItemDto>>());
            var actualResponse = (ServiceResponse<GetHRIRevisionItemDto>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.ItemId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetHRIRevisionItemsByHRIId()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var hriRevisionItems = new List<GetHRIRevisionItemDto>
                {
                    new GetHRIRevisionItemDto
                    {
                        ItemId = 1,
                        HriId = 1,
                        ItemNumber = 1,
                        RevisionPoint = "Test Revision Point",
                        RevisionMethodId = 1,
                        RevisionMethod =
                        new GetRevisionMethodDto {
                            Id = 1,
                            Code = "Test Revision Method",
                            Description = "Testing",
                            IsActive= true},
                        VeredictId = 1,
                        Veredict = new GetVeredictDto
                        {
                            Id = 1,
                            Code = "Test Veredict",
                            Description = "Testing",
                            IsActive= true
                        },
                        FrequencyId= 1,
                        Frequency = new GetFrequencyDto
                        {
                            Id = 1,
                            Code = "Test Frequency",
                            Description = "Testing",
                            IsActive= true
                        },
                        RevisionCycles = new List<GetRevisionCyclesDto>
                        {
                            new GetRevisionCyclesDto
                            {
                                RevisionCycleId = 1,
                                Cycle = 1,
                                HRIRevisionItemsId = 1,
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

                                }
                            }
                        },
                        IsActive = true
                    }
                };
            var serviceResponse = new ServiceResponse<List<GetHRIRevisionItemDto>>
            {
                Data = hriRevisionItems,
                Success = true,
                Message = "HRI Revision Items retrieved successfully for the specified HRI."
            };

            mockService.Setup(service => service.GetAllHRIRevisionItemsByHRIId(1)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetHRIRevisionItemsByHRIId(1);

            //Assert
            Assert.NotNull(result);
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetHRIRevisionItemDto>>>());
            var actualResponse = (ServiceResponse<List<GetHRIRevisionItemDto>>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Count, Is.EqualTo(1));


        }

        [Test]
        public async Task CreateHRIRevisionItem()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var newItem= new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Test Revision Point",
                RevisionMethodId = 1,
                VeredictId = 1,
                FrequencyId = 1,
                IsActive = true
            };
            var responseItem = new GetHRIRevisionItemDto
            {
                ItemId = 1,
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Test Revision Point",
                RevisionMethodId = 1,
                RevisionMethod =
                        new GetRevisionMethodDto
                        {
                            Id = 1,
                            Code = "Test Revision Method",
                            Description = "Testing",
                            IsActive = true
                        },
                VeredictId = 1,
                Veredict = new GetVeredictDto
                {
                    Id = 1,
                    Code = "Test Veredict",
                    Description = "Testing",
                    IsActive = true
                },
                FrequencyId = 1,
                Frequency = new GetFrequencyDto
                {
                    Id = 1,
                    Code = "Test Frequency",
                    Description = "Testing",
                    IsActive = true
                },
                RevisionCycles = new List<GetRevisionCyclesDto>
                        {
                            new GetRevisionCyclesDto
                            {
                                RevisionCycleId = 1,
                                Cycle = 1,
                                HRIRevisionItemsId = 1,
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

                                }
                            }
                        },
                IsActive = true
            };

            var serviceResponse = new ServiceResponse<GetHRIRevisionItemDto>
            {
                Data = responseItem,
                Success = true,
                Message = "HRI Revision Item created successfully."
            };

            mockService.Setup(service => service.CreateHRIRevisionItem(newItem)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.CreateHRIRevisionItem(newItem);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHRIRevisionItemDto>>());
            var actualResponse = (ServiceResponse<GetHRIRevisionItemDto>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.ItemId, Is.EqualTo(1));

        }

        [Test]
        public async Task CreateHRIREvisionItemsByHRIId()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var newItems = new List<CreateHRIRevisionItemDto>
            {
                new CreateHRIRevisionItemDto
                {
                    HriId = 1,
                    ItemNumber = 1,
                    RevisionPoint = "Test Revision Point",
                    RevisionMethodId = 1,
                    VeredictId = 1,
                    FrequencyId = 1,
                    IsActive = true
                }
            };

            var serviceResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "HRI Revision Items created successfully."
            };

            mockService.Setup(service => service.CreateHRIREvisionItemsByHRIId(1, newItems, 1)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.CreateHRIREvisionItemsByHRIId(1, newItems, 1);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);

        }

        [Test]
        public async Task UpdateHRIRevisionItem()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var updateItem = new UpdateHRIRevisionItemDto
            {
               
                ItemNumber = 1,
                RevisionPoint = "Updated Revision Point",
                RevisionMethodId = 1,
                VeredictId = 1,
                FrequencyId = 1
            };

            var responseItem = new GetHRIRevisionItemDto
            {
                ItemId = 1,
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Test Revision Point",
                RevisionMethodId = 1,
                RevisionMethod =
                        new GetRevisionMethodDto
                        {
                            Id = 1,
                            Code = "Test Revision Method",
                            Description = "Testing",
                            IsActive = true
                        },
                VeredictId = 1,
                Veredict = new GetVeredictDto
                {
                    Id = 1,
                    Code = "Test Veredict",
                    Description = "Testing",
                    IsActive = true
                },
                FrequencyId = 1,
                Frequency = new GetFrequencyDto
                {
                    Id = 1,
                    Code = "Test Frequency",
                    Description = "Testing",
                    IsActive = true
                },
                RevisionCycles = new List<GetRevisionCyclesDto>
                        {
                            new GetRevisionCyclesDto
                            {
                                RevisionCycleId = 1,
                                Cycle = 1,
                                HRIRevisionItemsId = 1,
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

                                }
                            }
                        },
                IsActive = true
            };

            var serviceResponse = new ServiceResponse<GetHRIRevisionItemDto>
            {
                Data = responseItem,
                Success = true,
                Message = "HRI Revision Item updated successfully."
            };

            mockService.Setup(service => service.UpdateHRIRevisionItem(1, updateItem)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.UpdateHRIRevisionItem(1, updateItem);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;

            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetHRIRevisionItemDto>>());
            var actualResponse = (ServiceResponse<GetHRIRevisionItemDto>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.ItemId, Is.EqualTo(1));

        }

        [Test]
        public async Task DeleteHRIRevisionItem()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var serviceResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "HRI Revision Item deleted successfully."
            };
            mockService.Setup(service => service.DeleteHRIRevisionItem(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.DeleteHRIRevisionItem(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);
        }

        [Test]
        public async Task GetAllFrecuencies()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var frequencies = new List<GetFrequencyDto>
            {
                new GetFrequencyDto
                {
                    Id = 1,
                    Code = "Test Frequency",
                    Description = "Testing",
                    IsActive= true
                }
            };
            var serviceResponse = new ServiceResponse<List<GetFrequencyDto>>
            {
                Data = frequencies,
                Success = true,
                Message = "Frequencies retrieved successfully."
            };
            mockService.Setup(service => service.GetAllFrequencies()).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetAllFrequencies();
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetFrequencyDto>>>());
            var actualResponse = (ServiceResponse<List<GetFrequencyDto>>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Count, Is.EqualTo(1));
        }
        [Test]
        public async Task GetFrequencyById()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var frequency = new GetFrequencyDto
            {
                Id = 1,
                Code = "Test Frequency",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetFrequencyDto>
            {
                Data = frequency,
                Success = true,
                Message = "Frequency retrieved successfully."
            };
            mockService.Setup(service => service.GetFrequencyById(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetFrequencyById(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetFrequencyDto>>());
            var actualResponse = (ServiceResponse<GetFrequencyDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }
        [Test]
        public async Task CreateFrequency()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var newFrequency = new CreateFrequencyDto
            {
                Code = "Test Frequency",
                Description = "Testing",
                IsActive = true
            };
            var responseFrequency = new GetFrequencyDto
            {
                Id = 1,
                Code = "Test Frequency",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetFrequencyDto>
            {
                Data = responseFrequency,
                Success = true,
                Message = "Frequency created successfully."
            };
            mockService.Setup(service => service.CreateFrequency(newFrequency)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.CreateFrequency(newFrequency);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetFrequencyDto>>());
            var actualResponse = (ServiceResponse<GetFrequencyDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateFrequency()
        {
                       var mockService = new Mock<IHRIRevisionItemService>();
            var updateFrequency = new UpdateFrequencyDto
            {
                Code = "Updated Frequency",
                Description = "Updated Testing",
               
            };
            var responseFrequency = new GetFrequencyDto
            {
                Id = 1,
                Code = "Updated Frequency",
                Description = "Updated Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetFrequencyDto>
            {
                Data = responseFrequency,
                Success = true,
                Message = "Frequency updated successfully."
            };
            mockService.Setup(service => service.UpdateFrequency(1, updateFrequency)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.UpdateFrequency(1, updateFrequency);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetFrequencyDto>>());
            var actualResponse = (ServiceResponse<GetFrequencyDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteFrequency()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var serviceResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Frequency deleted successfully."
            };
            mockService.Setup(service => service.DeleteFrequency(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.DeleteFrequency(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);
        }

        [Test]
        public async Task GetAllVeredicts()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var veredicts = new List<GetVeredictDto>
            {
                new GetVeredictDto
                {
                    Id = 1,
                    Code = "Test Veredict",
                    Description = "Testing",
                    IsActive= true
                }
            };
            var serviceResponse = new ServiceResponse<List<GetVeredictDto>>
            {
                Data = veredicts,
                Success = true,
                Message = "Veredicts retrieved successfully."
            };
            mockService.Setup(service => service.GetAllVeredicts()).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetAllVeredicts();
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetVeredictDto>>>());
            var actualResponse = (ServiceResponse<List<GetVeredictDto>>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetVeredictById()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var veredict = new GetVeredictDto
            {
                Id = 1,
                Code = "Test Veredict",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetVeredictDto>
            {
                Data = veredict,
                Success = true,
                Message = "Veredict retrieved successfully."
            };
            mockService.Setup(service => service.GetVeredictById(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetVeredictById(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetVeredictDto>>());
            var actualResponse = (ServiceResponse<GetVeredictDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }
         [Test]
         public async Task CreateVeredict()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var newVeredict = new CreateVeredictDto
            {
                Code = "Test Veredict",
                Description = "Testing",
                IsActive = true
            };
            var responseVeredict = new GetVeredictDto
            {
                Id = 1,
                Code = "Test Veredict",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetVeredictDto>
            {
                Data = responseVeredict,
                Success = true,
                Message = "Veredict created successfully."
            };
            mockService.Setup(service => service.CreateVeredict(newVeredict)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.CreateVeredict(newVeredict);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetVeredictDto>>());
            var actualResponse = (ServiceResponse<GetVeredictDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateVeredict()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var updateVeredict = new UpdateVeredictDto
            {
                Code = "Updated Veredict",
                Description = "Updated Testing",
            };
            var responseVeredict = new GetVeredictDto
            {
                Id = 1,
                Code = "Updated Veredict",
                Description = "Updated Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetVeredictDto>
            {
                Data = responseVeredict,
                Success = true,
                Message = "Veredict updated successfully."
            };
            mockService.Setup(service => service.UpdateVeredict(1, updateVeredict)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.UpdateVeredict(1, updateVeredict);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetVeredictDto>>());
            var actualResponse = (ServiceResponse<GetVeredictDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }
        [Test]
        public async Task DeleteVeredict()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var serviceResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Veredict deleted successfully."
            };
            mockService.Setup(service => service.DeleteVeredict(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.DeleteVeredict(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<bool>>());
            var actualResponse = (ServiceResponse<bool>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data, Is.True);
        }
        [Test]
        public async Task GetAllRevisionMethods()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var revisionMethods = new List<GetRevisionMethodDto>
            {
                new GetRevisionMethodDto
                {
                    Id = 1,
                    Code = "Test Revision Method",
                    Description = "Testing",
                    IsActive= true
                }
            };
            var serviceResponse = new ServiceResponse<List<GetRevisionMethodDto>>
            {
                Data = revisionMethods,
                Success = true,
                Message = "Revision Methods retrieved successfully."
            };
            mockService.Setup(service => service.GetAllRevisionMethods()).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetAllRevisionMethods();
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<List<GetRevisionMethodDto>>>());
            var actualResponse = (ServiceResponse<List<GetRevisionMethodDto>>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetRevisionMethodById()
        {
                       var mockService = new Mock<IHRIRevisionItemService>();
            var revisionMethod = new GetRevisionMethodDto
            {
                Id = 1,
                Code = "Test Revision Method",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetRevisionMethodDto>
            {
                Data = revisionMethod,
                Success = true,
                Message = "Revision Method retrieved successfully."
            };
            mockService.Setup(service => service.GetRevisionMethodById(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.GetRevisionMethodById(1);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetRevisionMethodDto>>());
            var actualResponse = (ServiceResponse<GetRevisionMethodDto>)okResult.Value;

            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));

        }

        [Test]
        public async Task CreateRevisionMethod() 
        { 
            var mockService = new Mock<IHRIRevisionItemService>();

            var newRevisionMethod = new CreateRevisionMethodDto
            {
                Code = "Test Revision Method",
                Description = "Testing",
                IsActive = true
            };
            var responseRevisionMethod = new GetRevisionMethodDto
            {
                Id = 1,
                Code = "Test Revision Method",
                Description = "Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetRevisionMethodDto>
            {
                Data = responseRevisionMethod,
                Success = true,
                Message = "Revision Method created successfully."
            };
            mockService.Setup(service => service.CreateRevisionMethod(newRevisionMethod)).ReturnsAsync(serviceResponse);

            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.CreateRevisionMethod(newRevisionMethod);

            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetRevisionMethodDto>>());
            var actualResponse = (ServiceResponse<GetRevisionMethodDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));

        }
        [Test]
        public async Task UpdateRevisionMethod()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var updateRevisionMethod = new UpdateRevisionMethodDto
            {
                Code = "Updated Revision Method",
                Description = "Updated Testing",
            };
            var responseRevisionMethod = new GetRevisionMethodDto
            {
                Id = 1,
                Code = "Updated Revision Method",
                Description = "Updated Testing",
                IsActive = true
            };
            var serviceResponse = new ServiceResponse<GetRevisionMethodDto>
            {
                Data = responseRevisionMethod,
                Success = true,
                Message = "Revision Method updated successfully."
            };
            mockService.Setup(service => service.UpdateRevisionMethod(1, updateRevisionMethod)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.UpdateRevisionMethod(1, updateRevisionMethod);
            //Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.TypeOf<ServiceResponse<GetRevisionMethodDto>>());
            var actualResponse = (ServiceResponse<GetRevisionMethodDto>)okResult.Value;
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.Data!.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteRevisionMethod()
        {
            var mockService = new Mock<IHRIRevisionItemService>();
            var serviceResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Revision Method deleted successfully."
            };
            mockService.Setup(service => service.DeleteRevisionMethod(1)).ReturnsAsync(serviceResponse);
            var controller = new HRIRevisionItemController(mockService.Object);
            var result = await controller.DeleteRevisionMethod(1);
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
