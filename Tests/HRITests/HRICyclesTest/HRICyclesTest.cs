using Microsoft.AspNetCore.Mvc;
using Moq;
using SupervisorMobility.API;
using SupervisorMobility.API.Controllers.HRIControllers;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;

namespace Tests.HRITests.HRICyclesTest
{
    public class HRICyclesTest
    {
        [Test]
        public async Task CreateHRICycleAsync()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();

            var newCycle = new CreateHRICyclesDto
            {
                Cycle = 1,
                HriId = 10,
                IsActive = true,
            };

            var responseCycle = new GetHRICyclesDto
            {
                CycleId = 1,
                Cycle = 1,
                HriId = 10,
                IsActive = true,
            };

            var expectedResponse = new ServiceResponse<GetHRICyclesDto>
            {
                Data = responseCycle,
                Success = true,
                Message = "HRICycle creado con éxito."
            };

            mockService.Setup(service => service.CreateHRICycle(It.IsAny<CreateHRICyclesDto>()))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.CreateHRICycle(newCycle);

            // assert
            Assert.IsNotNull(result);

            // 1. Convertimos el resultado al tipo de objeto HTTP esperado (OkObjectResult)
            var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.IsNotNull(okResult, "Se esperaba un OkObjectResult del controlador.");

            // 2. Extraemos el ServiceResponse del Value de ese OkObjectResult
            var actualResponse = okResult.Value as ServiceResponse<GetHRICyclesDto>;
            Assert.IsNotNull(actualResponse, "El contenido de OkObjectResult no es del tipo ServiceResponse esperado.");

            // 3. Ahora sí, comparamos las respuestas correspondientes
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));

            mockService.Verify(service => service.CreateHRICycle(It.IsAny<CreateHRICyclesDto>()), Times.Once);
        }

        // ==========================================
        // 1. GET ALL HRI CYCLES
        // ==========================================
        [Test]
        public async Task GetAllHRICycles_ShouldReturnOk_WhenDataExists()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            var expectedLines = new List<GetHRICyclesDto>
            {
                new GetHRICyclesDto { CycleId = 1, Cycle = 1, HriId = 10, IsActive = true },
                new GetHRICyclesDto { CycleId = 2, Cycle = 2, HriId = 10, IsActive = true }
            };
            var expectedResponse = new ServiceResponse<List<GetHRICyclesDto>>
            {
                Data = expectedLines,
                Success = true,
                Message = "Data retrieved successfully."
            };

            mockService.Setup(service => service.GetHRICycles())
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.GetAllHRICycles();

            // assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Se esperaba un OkObjectResult");

            var actualResponse = okResult.Value as ServiceResponse<List<GetHRICyclesDto>>;
            Assert.IsNotNull(actualResponse);
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));
            mockService.Verify(service => service.GetHRICycles(), Times.Once);
        }

        [Test]
        public async Task GetAllHRICycles_ShouldReturnNotFound_WhenDataIsNull()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            var expectedResponse = new ServiceResponse<List<GetHRICyclesDto>>
            {
                Data = null,
                Success = false,
                Message = "No cycles found."
            };

            mockService.Setup(service => service.GetHRICycles())
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.GetAllHRICycles();

            // assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Se esperaba un NotFoundObjectResult");

            var actualResponse = notFoundResult.Value as ServiceResponse<List<GetHRICyclesDto>>;
            Assert.IsNotNull(actualResponse);
            Assert.IsNull(actualResponse.Data);
        }

        // ==========================================
        // 2. GET HRI CYCLE BY ID
        // ==========================================
        [Test]
        public async Task GetHRICycleById_ShouldReturnOk_WhenIdExists()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            int cycleId = 1;
            var expectedResponse = new ServiceResponse<GetHRICyclesDto>
            {
                Data = new GetHRICyclesDto { CycleId = cycleId, Cycle = 1, HriId = 10, IsActive = true },
                Success = true
            };

            mockService.Setup(service => service.GetHRICycleById(cycleId))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.GetHRICycleById(cycleId);

            // assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualResponse = okResult.Value as ServiceResponse<GetHRICyclesDto>;
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));
            mockService.Verify(service => service.GetHRICycleById(cycleId), Times.Once);
        }

        // ==========================================
        // 3. CREATE HRI CYCLES BY HRI ID (LIST)
        // ==========================================
        [Test]
        public async Task CreateHRICyclesByHRIId_ShouldReturnOk()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            int hriId = 10;
            var dtoList = new List<CreateHRICyclesDto>
            {
                new CreateHRICyclesDto { Cycle = 1, HriId = hriId, IsActive = true }
            };
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Cycles created successfully."
            };

            mockService.Setup(service => service.CreateHRICyclesByHRIId(hriId, dtoList))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.CreateHRICyclesByHRIId(hriId, dtoList);

            // assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualResponse = okResult.Value as ServiceResponse<bool>;
            Assert.IsTrue(actualResponse.Data);
            mockService.Verify(service => service.CreateHRICyclesByHRIId(hriId, dtoList), Times.Once);
        }

        // ==========================================
        // 4. CREATE NEW DAILY REVISION
        // ==========================================
        [Test]
        public async Task CreateNewDailyRevision_ShouldReturnOk_WhenSuccessful()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            var newRevisionDto = new CreateDailyRevisionDto { /* Añade propiedades de tu DTO si es necesario */ };
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };

            mockService.Setup(service => service.CreateNewDailyRevision(newRevisionDto))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.CreateNewDailyRevision(newRevisionDto);

            // assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualResponse = okResult.Value as ServiceResponse<bool>;
            Assert.IsTrue(actualResponse.Data);
        }

        [Test]
        public async Task CreateNewDailyRevision_ShouldReturnBadRequest_WhenDataIsFalseOrNull()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            var newRevisionDto = new CreateDailyRevisionDto();
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = false, // Provoca el BadRequest según tu lógica
                Success = false,
                Message = "Error al crear la revisión"
            };

            mockService.Setup(service => service.CreateNewDailyRevision(newRevisionDto))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.CreateNewDailyRevision(newRevisionDto);

            // assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Se esperaba un BadRequestObjectResult");
        }

        // ==========================================
        // 5. DELETE HRI CYCLE
        // ==========================================
        [Test]
        public async Task DeleteHRICycle_ShouldReturnOk_WhenDeletedSuccessfully()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            int cycleId = 1;
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };

            mockService.Setup(service => service.DeleteHRICycle(cycleId))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.DeleteHRICycle(cycleId);

            // assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualResponse = okResult.Value as ServiceResponse<bool>;
            Assert.IsTrue(actualResponse.Data);
            mockService.Verify(service => service.DeleteHRICycle(cycleId), Times.Once);
        }

        [Test]
        public async Task DeleteHRICycle_ShouldReturnNotFound_WhenDeletionFails()
        {
            // arrange
            var mockService = new Mock<IHRICyclesService>();
            int cycleId = 99; // ID inexistente
            var expectedResponse = new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cycle not found."
            };

            mockService.Setup(service => service.DeleteHRICycle(cycleId))
                .ReturnsAsync(expectedResponse);

            var controller = new HRICyclesController(mockService.Object);

            // act
            var result = await controller.DeleteHRICycle(cycleId);

            // assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }
    }
}
