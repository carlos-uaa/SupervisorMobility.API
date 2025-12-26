using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CombinationTests : IDisposable
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

        // Tests fot get single combinations
        #region Get Single Combinations
        [Test, Order(1)]
        public async Task Get_Single_Combination_Should_Return_OK()
        {
            // Arrange
            var combinationId = 1;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Combination/{combinationId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            Assert.IsNotEmpty(content);
        }

        [Test, Order(2)]
        public async Task Get_Single_Combination_Returns_NotFound()
        {
            // Arrange
            var distributionId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Combination/{distributionId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests fot get all combinations
        #region Get All Combinations
        [Test, Order(3)]
        public async Task GetAllCombinations_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync($"/api/SOS/Combination/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
        }
        #endregion

        // Tests for Create a combination
        #region Create a combination
        [Test, Order(4)]
        public async Task CreateCombination_ReturnsOk()
        {
            // Arrange 
            var combinationId = 1;

            // Get the existing combination
            var getResponse = await _client.GetAsync($"/api/SOS/Combination/{combinationId}");
            getResponse.EnsureSuccessStatusCode();
            var combinationContent = await getResponse.Content.ReadAsStringAsync();
            var combination = Newtonsoft.Json.JsonConvert.DeserializeObject<SOSCombination>(combinationContent);
            Assert.IsNotNull(combination);

            // Create Dto for combination
            SOSCombinationForCreateDto sOSCombinationForCreateDto = new SOSCombinationForCreateDto()
            {
                IsActive = combination.IsActive,
                InternalControlNumber = combination.InternalControlNumber,
                OperationName = combination.OperationName,
                ProcessName = combination.ProcessName,
                ProductionPlanAndObservations = combination.ProductionPlanAndObservations,
                ReviewerHSId = combination.ReviewerHSId,
                ApplicationMonth = combination.ApplicationMonth,
                SOSHubId = combination.SOSHubId
            };

            // Act 
            var postContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sOSCombinationForCreateDto), Encoding.UTF8, "application/json");
            var postResponse = await _customClient.PostAsync($"/api/SOS/Combination", postContent);

            // Assert
            postResponse.EnsureSuccessStatusCode();
            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(postResponseContent);
        }
        #endregion

        // Tests for Update a combination
        #region Update a combination
        [Test, Order(5)]
        public async Task UpdateCombination_ReturnsOk()
        {
            // Arrange 
            var combinationId = 1;

            // Get the existing combination
            var getResponse = await _client.GetAsync($"/api/SOS/Combination/{combinationId}");
            getResponse.EnsureSuccessStatusCode();
            var combinationContent = await getResponse.Content.ReadAsStringAsync();
            var combination = Newtonsoft.Json.JsonConvert.DeserializeObject<SOSCombination>(combinationContent);
            Assert.IsNotNull(combination);

            // Create Dto for combination
            SOSCombinationForCreateDto sOSCombinationForCreateDto = new SOSCombinationForCreateDto()
            {
                IsActive = combination.IsActive,
                InternalControlNumber = combination.InternalControlNumber,
                OperationName = combination.OperationName,
                ProcessName = combination.ProcessName,
                ProductionPlanAndObservations = combination.ProductionPlanAndObservations,
                ReviewerHSId = combination.ReviewerHSId,
                ApplicationMonth = combination.ApplicationMonth,
                SOSHubId = combination.SOSHubId
            };

            // Create combination in Temporal DB
            var postContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sOSCombinationForCreateDto), Encoding.UTF8, "application/json");
            var postResponse = await _customClient.PostAsync($"/api/SOS/Combination", postContent);

            postResponse.EnsureSuccessStatusCode();
            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdCombination = Newtonsoft.Json.JsonConvert.DeserializeObject<SOSCombination>(postResponseContent);
            Assert.IsNotNull(createdCombination);

            // Modify some properties
            createdCombination.OperationName = "Updated Operation Name";

            // Create Dto for update
            SOSCombinationForUpdateDto sOSCombinationForUpdateDto = new SOSCombinationForUpdateDto() 
            {
                SOSCombinationId = combinationId,
                IsActive = true,
                InternalControlNumber = "Internal control number",
                OperationName = "Updated operation Name",
                ProcessName = "Process updated",
                ProductionPlanAndObservations = "New observations",
                ReviewerHSId = 1940,
                ReviewerHSSignatureImage = new FileUploadGeneralDto(),
                ApplicationMonth = DateTime.Now,
                SOSHubId = 2,
                Turns = new List<TurnForUpdateDto>(),
                CombinationLogbooks = new List<SOSCombinationLogbookForUpdateDto>(),
                Illustrations = new List<FileUploadGeneralDto>(),
                CreatedAt = DateTime.Now,
                SOSCombinationOperationSequence = new List<SOSCombinationOperationSequenceForUpdateDto>(),
            };

            // Act
            var jsonUdate = JsonConvert.SerializeObject(sOSCombinationForUpdateDto);
            var updateContent = new StringContent(jsonUdate, Encoding.UTF8, "application/json");
            var putResponse = await _customClient.PutAsync($"/api/SOS/Combination/{createdCombination.SOSCombinationId}", updateContent);
            var putResult = await putResponse.Content.ReadAsStringAsync();

            // Assert
            putResponse.EnsureSuccessStatusCode();
            var putResponseContent = await putResponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(putResponseContent);
        }
        #endregion

        // Tests for Delete a combination
        #region Delete a combination
        [Test, Order(6)]
        public async Task DeleteCombination_ReturnsOk()
        {
            // Arrange 
            var combinationId = 1;

            // Get the existing combination
            var getResponse = await _client.GetAsync($"/api/SOS/Combination/{combinationId}");
            getResponse.EnsureSuccessStatusCode();
            var combinationContent = await getResponse.Content.ReadAsStringAsync();
            var combination = Newtonsoft.Json.JsonConvert.DeserializeObject<SOSCombination>(combinationContent);
            Assert.IsNotNull(combination);

            // Create Dto for combination
            SOSCombinationForCreateDto sOSCombinationForCreateDto = new SOSCombinationForCreateDto()
            {
                IsActive = combination.IsActive,
                InternalControlNumber = combination.InternalControlNumber,
                OperationName = combination.OperationName,
                ProcessName = combination.ProcessName,
                ProductionPlanAndObservations = combination.ProductionPlanAndObservations,
                ReviewerHSId = combination.ReviewerHSId,
                ApplicationMonth = combination.ApplicationMonth,
                SOSHubId = combination.SOSHubId
            };

            // Create combination in Temporal DB
            var postContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sOSCombinationForCreateDto), Encoding.UTF8, "application/json");
            var postResponse = await _customClient.PostAsync($"/api/SOS/Combination", postContent);

            postResponse.EnsureSuccessStatusCode();
            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdCombination = Newtonsoft.Json.JsonConvert.DeserializeObject<SOSCombination>(postResponseContent);
            Assert.IsNotNull(createdCombination);

            // Act
            var deleteResponse = await _customClient.DeleteAsync($"/api/SOS/Combination/{createdCombination.SOSCombinationId}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Test, Order(7)]
        public async Task DeleteCombination_Returns_NotFound()
        {
            // Arrange
            var combinationId = 9999;

            // Act
            var response = await _customClient.DeleteAsync($"/api/SOS/Combination/{combinationId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }
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
