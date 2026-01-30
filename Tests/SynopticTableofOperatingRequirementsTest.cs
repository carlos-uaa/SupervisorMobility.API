using Microsoft.AspNetCore.Mvc.Testing;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class SynopticTableofOperatingRequirementsTest : IDisposable
    {
        //StartUp
        #region StartUp
        private HttpClient _client;
        private WebApplicationFactory<Program> _appFactory;
        private HttpClient _customClient;
        private CustomWebApplicationFactory _customFactory;

        [SetUp]
        public void Setup()
        {
            _appFactory = new WebApplicationFactory<Program>();
            _client = _appFactory.CreateClient();
            _customFactory = new CustomWebApplicationFactory();
            _customClient = _customFactory.CreateClient();
        }
        #endregion

        // Tests for get single Synoptic Table of Operation Requirements
        #region Get Single Synoptic Table of Control Points Tests
        [Test, Order(1)]
        public async Task GetSingleSynopticTableofOperationRequirements_ReturnsSuccessStatusCode()
        {
            // Arrange
            var tableId = 1;

            // Act
            var response = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/{tableId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var sequence = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequence);
        }

        [Test, Order(2)]
        public async Task GetSingleSynopticTableofOperationRequirements_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidTableId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/{invalidTableId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for get all Synoptic Table of Operation Requirements
        #region Get All Synoptic Table of Control Points Tests
        [Test, Order(3)]
        public async Task GetAllSynopticTableofOperationRequirements_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var sequences = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequences);
        }
        #endregion

        // Tests for Generate Excel ST Operating Requirements
        #region Generate Excel ST Operating Requirements Tests
        //[Test, Order(4)]
        //public async Task GenerateExcelSTOperatingRequirements_ReturnsExcelFile()
        //{
        //    // Arrange
        //    int testId = 1;

        //    // Act
        //    var response = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/GenerateExcelSTOperatingRequirements/{testId}");

        //    // Assert
        //    response.EnsureSuccessStatusCode();

        //    // Verificar que el Content-Type sea el de un Excel
        //    Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                    response.Content.Headers.ContentType?.MediaType);

        //    // Verificar que el archivo tenga contenido
        //    var fileBytes = await response.Content.ReadAsByteArrayAsync();
        //    Assert.IsNotNull(fileBytes);
        //    Assert.IsTrue(fileBytes.Length > 0, "El archivo generado está vacío.");

        //    // Verificar que el nombre sugerido sea STOR.xlsx
        //    Assert.AreEqual("STOR.xlsx", response.Content.Headers.ContentDisposition?.FileName);
        //}
        #endregion

        // Tests for create Synoptic Table of Operation Requirements
        #region Create Synoptic Table of Operation Requirements Tests
        //[Test, Order(5)]
        //public async Task CreateSynopticTableofOperationRequirements_ReturnsSuccessStatusCode()
        //{
        //    // Arrange
        //    var tableId = 1;

        //    // Get an existant record
        //    var getResponse = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/{tableId}");
        //    getResponse.EnsureSuccessStatusCode();
        //    var getDataString = await getResponse.Content.ReadAsStringAsync();
        //    var getData = System.Text.Json.JsonSerializer.Deserialize<SOSSynopticRequirementsDto>(getDataString);

        //    // Create the create entity
        //    var createData = new SOSSynopticTableofOperatingRequirementsForCreateDto
        //    {
        //        InternalControlNumber = getData.InternalControlNumber,
        //        ProcessName = getData.ProcessName,
        //        CreatorId = getData.CreatorId,
        //        ReviewerId = getData.ReviewerId,
        //        ApproverId = getData.ApproverId,
        //        IsActive = getData.IsActive,
        //        SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequenceForCreateDto>(),
        //        SynopticRequirementsLogbooks = new List<SOSSynopticRequirementsLogbookForCreateDto>(),
        //        RequirementDifficulties = new List<SOSSynopticTableRequirementOperationDifficulty>(),
        //        Analyses = new List<SOSAnalysisDto>(),
        //        Sequences = new List<SOSSequenceDto>(),
        //        SOSHubs = new List<SOSHub>(),
        //        SOSHubId = getData.SOSHubId.Value
        //    };

        //    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(createData), Encoding.UTF8, "application/json");

        //    // Act
        //    var response = await _client.PostAsync($"/api/SOS/SynopticTableofOperatingRequirements", content);

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var responseData = await response.Content.ReadAsStringAsync();
        //    Assert.IsNotNull(responseData);
        //}
        #endregion

        // Tests for update Synoptic Table of Operation Requirements
        #region Update Synoptic Table of Operation Requirements Tests
        //[Test, Order(6)]
        //public async Task UpdateSynopticTableofOperationRequirements_ReturnsSuccessStatusCode()
        //{
        //    Arrange
        //   var tableId = 1;

        //    Get an existant record
        //    var getResponse = await _client.GetAsync($"/api/SOS/SynopticTableofOperatingRequirements/{tableId}");
        //    getResponse.EnsureSuccessStatusCode();
        //    var getDataString = await getResponse.Content.ReadAsStringAsync();
        //    var getData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(getDataString);

        //    Create the create entity
        //    var updateData = new Dictionary<string, object>(getData);

        //    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");

        //    Act
        //   var response = await _client.PutAsync($"/api/SOS/SynopticTableofOperatingRequirements/{tableId}", content);

        //    Assert
        //    response.EnsureSuccessStatusCode();
        //    var responseData = await response.Content.ReadAsStringAsync();
        //    Assert.IsNotNull(responseData);
        //}
        #endregion

        // Dispose method to clean up resources
        #region Dispose
        [OneTimeTearDown]
        public void Dispose()
        {
            _client.Dispose();
            _appFactory.Dispose();
        }
        #endregion
    }
}
