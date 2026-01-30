using Microsoft.AspNetCore.Mvc.Testing;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class FlowTests : IDisposable
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

        // Tests for get single Flow
        #region Get Single Flow Tests
        [Test, Order(1)]
        public async Task GetSingleFlow_ReturnsSuccessStatusCode()
        {
            // Arrange
            var flowId = 1; 

            // Act
            var response = await _client.GetAsync($"/api/SOS/Flow/{flowId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var flow = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(flow);
        }

        [Test, Order(2)]
        public async Task GetSingleFlow_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidFlowId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Flow/{invalidFlowId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for get all Flows
        #region Get All Flows Tests
        [Test, Order(3)]
        public async Task GetAllFlows_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/SOS/Flow/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var flows = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(flows);
        }
        #endregion

        // Tests for create Flow
        #region Create Flow Tests
        //[Test, Order(4)]
        //public async Task CreateFlow_ValidData_ReturnsCreatedFlow()
        //{
        //    // Arrange
        //    var flowId = 1;

        //    // Get real flow data to use as template
        //    var getResponse = await _client.GetAsync($"/api/SOS/Flow/{flowId}");
        //    getResponse.EnsureSuccessStatusCode();
        //    var flowJson = await getResponse.Content.ReadAsStringAsync();
        //    var existingFlow = System.Text.Json.JsonSerializer.Deserialize<SOSFlow>(flowJson);
        //    Assert.IsNotNull(existingFlow);

        //    // Flow Create Dto with necessary properties
        //    var toCreateFlow = new SOSFlowForCreateDto
        //    {
        //        InternalControlNumber = existingFlow.InternalControlNumber,
        //        OperationName = existingFlow.OperationName,
        //        ProcessName = existingFlow.ProcessName,
        //        Flow = existingFlow.Flow,
        //        IsActive = true,
        //        ReviewerHSId = existingFlow.ReviewerHSId,
        //        SOSFlowId = 0,
        //        ApproverId = existingFlow.ApproverId,
        //        CreatedAt = DateTime.UtcNow,
        //        TargetTime = existingFlow.TargetTime,
        //        FlowLogbooks = new List<SOSFlowLogbookForCreateDto>()
        //    };

        //    // Serialize to JSON
        //    var jsonContent = System.Text.Json.JsonSerializer.Serialize(toCreateFlow);
        //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //    // Act
        //    var createdReponse = await _customClient.PostAsync("/api/SOS/Flow?SOSHubCollection_Id=1", content);

        //    // Assert
        //    createdReponse.EnsureSuccessStatusCode();
        //    var createdFlow = await createdReponse.Content.ReadAsStringAsync();
        //    Assert.IsNotNull(createdFlow);
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
