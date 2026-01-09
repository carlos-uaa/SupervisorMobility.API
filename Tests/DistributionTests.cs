using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class DistributionTests : IDisposable
    {
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

        // Tests for Get Distribution by ID
        #region Get Distribution By Id
        [Test, Order(1)]
        public async Task GetDistributionById_ReturnsOk()
        {
            // Arrange
            var distributionId = 1;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Distribution/{distributionId}");

            // Assert
            response.EnsureSuccessStatusCode(); 
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content); 
        }

        [Test, Order(2)]
        public async Task GetDistributionById_ReturnsNotFound()
        {
            // Arrange
            var distributionId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Distribution/{distributionId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for Get Distributions by SOSHub ID
        #region Get Distributions By SOSHub Id
        [Test, Order(3)]
        public async Task GetDistributionsBySOSHubId_ReturnsOk()
        {
            // Arrange
            var sosHubId = 2;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Distribution/bySosHub/{sosHubId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
        }

        [Test, Order(4)]
        public async Task GetDistributionsBySOSHubId_ReturnsNotFound()
        {
            // Arrange
            var sosHubId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Distribution/bySosHub/{sosHubId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for Get All Distributions
        #region Get All Distributions
        [Test, Order(5)]
        public async Task GetAllDistributions_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync($"/api/SOS/Distribution/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
        }
        #endregion

        // Tests for Create Distribution
        #region Create Distribution
        [Test, Order(6)]
        public async Task CreateDistribution_ReturnsCreated()
        {
            // Arrange
            int originalDistributionId = 1;
            int sosHubId = 2;

            var newDistribution = _client.GetAsync($"/api/SOS/Distribution/{originalDistributionId}").Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(newDistribution);

            // Build Distribution Dto
            SOSDistributionForCreateDto sOSDistributionForCreateDto = new SOSDistributionForCreateDto
            {
                SOSHubId = sosHubId,
                IsActive = true,
                InternalControlNumber = "NEW-CTRL-001",
                OperationName = "New Operation",
                ProcessName = "New Process",
                TackTime = "00:10:00",
                CreatedAt = DateTime.UtcNow,
                Turns = new List<TurnForCreateDto>(),
                SOSDistributionOperationSequence = new List<SOSDistributionOperationSequenceForCreateDto>(),
                DistributionLogbooks = new List<SOSDistributionLogbookForCreateDto>(),
                Illustrations = new List<FileUploadGeneralDto>(),
                Notes = new List<CreateCommentaryDto>(),
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(sOSDistributionForCreateDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _customClient.PostAsync($"/api/SOS/Distribution?SOSHubCollection_Id={sosHubId}", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseContent);
        }
        #endregion

        // Tests for Update Distribution
        #region Update Distribution
        //[Test, Order(7)]
        //public async Task UpdateDistribution_ReturnsOk()
        //{
        //    // Arrange
        //    var distributionId = 4;

        //    var getExistingDistributionResponse = await _client.GetAsync($"/api/SOS/Distribution/{distributionId}");
        //    var distribution = await getExistingDistributionResponse.Content.ReadFromJsonAsync<SOSDistributionDto>();
        //    Assert.IsNotNull(distribution);

        //    // Create a DTO from existing distribution
        //    SOSDistributionForCreateDto sOSDistributionForCreateDto = new SOSDistributionForCreateDto
        //    {
        //        InternalControlNumber = distribution.InternalControlNumber,
        //        OperationName = distribution.OperationName,
        //        ProcessName = distribution.ProcessName,
        //        TackTime = distribution.TackTime,
        //        SOSHubId = distribution.SOSHubId,
        //        IsActive = distribution.IsActive,
        //        Turns = new List<TurnForCreateDto>(),
        //        SOSDistributionOperationSequence = new List<SOSDistributionOperationSequenceForCreateDto>(),
        //        DistributionLogbooks = new List<SOSDistributionLogbookForCreateDto>(),
        //        Illustrations = distribution.Illustrations,
        //        Notes = new List<CreateCommentaryDto>(),
        //    };

        //    // Create Http Content
        //    var json = JsonConvert.SerializeObject(sOSDistributionForCreateDto);
        //    var createContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    // Save original distribution in temporal DB
        //    var response = await _customClient.PostAsync($"/api/SOS/Distribution?SOSHubCollection_Id={sOSDistributionForCreateDto.SOSHubId}", createContent);
        //    Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    Assert.IsNotNull(responseContent);

        //    // Modify a field
        //    sOSDistributionForCreateDto.OperationName = "Updated Operation Name";

        //    // Create content for PUT request
        //    SOSDistributionForUpdateDto sOSDistributionForUpdateDto = new SOSDistributionForUpdateDto
        //    {
        //        SOSDistributionId = distributionId,
        //        SOSHubId = sOSDistributionForCreateDto.SOSHubId,
        //        IsActive = sOSDistributionForCreateDto.IsActive,
        //        InternalControlNumber = sOSDistributionForCreateDto.InternalControlNumber,
        //        OperationName = sOSDistributionForCreateDto.OperationName,
        //        ProcessName = sOSDistributionForCreateDto.ProcessName,
        //        TackTime = sOSDistributionForCreateDto.TackTime,
        //        Turns = new List<TurnForUpdateDto>(),
        //        SOSDistributionOperationSequence = new List<SOSDistributionOperationSequenceForUpdateDto>(),
        //        DistributionLogbooks = new List<SOSDistributionLogbookForUpdateDto>(),
        //        Illustrations = sOSDistributionForCreateDto.Illustrations,
        //        Notes = new List<UpdateCommentaryDto>(),
        //        ControlNumber = "Updated Distribution Control Number",
        //        CreatedAt = DateTime.Today,
        //        ApplicationMonth = DateTime.Today,
        //        SOSDistributionAdditionalTimeId = 3,
        //        SOSDistributionAdditionalTime = new SOSDistributionAdditionalTimeForUpdateDto(),
        //        SOSHubs = new List<SOSHubForUpdateDto>(),
        //    };

        //    // Create Http Content
        //    var jsonUdate = JsonConvert.SerializeObject(sOSDistributionForUpdateDto);
        //    var updateContent = new StringContent(jsonUdate, Encoding.UTF8, "application/json");

        //    // Act
        //    var responseUpdated = await _customClient.PutAsync($"/api/SOS/Distribution/{distributionId}", updateContent);

        //    // Assert
        //    var responseUpdatedContent = await responseUpdated.Content.ReadAsStringAsync();
        //    Assert.AreEqual(System.Net.HttpStatusCode.OK, responseUpdated.StatusCode);
        //    Assert.IsNotNull(responseUpdatedContent);

        //    // Get the updated distribution to verify the change
        //    var getResponse = await _customClient.GetAsync($"/api/SOS/Distribution/{distributionId}");
        //    getResponse.EnsureSuccessStatusCode();
        //    var getContent = await getResponse.Content.ReadAsStringAsync();
        //    var updatedDistribution = System.Text.Json.JsonSerializer.Deserialize<SOSDistributionForCreateDto>(getContent);
        //    Assert.AreEqual("Updated Operation Name", updatedDistribution.OperationName);
        //}

        [Test, Order(8)]
        public async Task UpdateDistribution_ReturnsNotFound()
        {
            // Arrange
            var distributionId = 9999;

            SOSDistributionForCreateDto sOSDistributionForCreateDto = new SOSDistributionForCreateDto
            {
                OperationName = "Non-existent Operation",
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(sOSDistributionForCreateDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _customClient.PutAsync($"/api/SOS/Distribution/{distributionId}", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }
        #endregion

        // Tests for Delete Distribution
        #region Delete Distribution
        [Test, Order(9)]
        public async Task DeleteDistribution_ReturnsOk()
        {
            // Arrange
            var distributionId = 1;

            var getExistingDistributionResponse = await _client.GetAsync($"/api/SOS/Distribution/{distributionId}");
            var distribution = await getExistingDistributionResponse.Content.ReadFromJsonAsync<SOSDistributionDto>();
            Assert.IsNotNull(distribution);

            // Create a DTO from existing distribution
            SOSDistributionForCreateDto sOSDistributionForCreateDto = new SOSDistributionForCreateDto
            {
                InternalControlNumber = distribution.InternalControlNumber,
                OperationName = distribution.OperationName,
                ProcessName = distribution.ProcessName,
                TackTime = distribution.TackTime,
                SOSHubId = distribution.SOSHubId,
                IsActive = distribution.IsActive,
                Turns = new List<TurnForCreateDto>(),
                SOSDistributionOperationSequence = new List<SOSDistributionOperationSequenceForCreateDto>(),
                DistributionLogbooks = new List<SOSDistributionLogbookForCreateDto>(),
                Illustrations = distribution.Illustrations,
                Notes = new List<CreateCommentaryDto>(),
            };

            // Create Http Content
            var json = JsonConvert.SerializeObject(sOSDistributionForCreateDto);
            var createContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Save original distribution in temporal DB
            var response = await _customClient.PostAsync($"/api/SOS/Distribution?SOSHubCollection_Id={sOSDistributionForCreateDto.SOSHubId}", createContent);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseContent);

            // Act
            var deleteResponse = await _customClient.DeleteAsync($"/api/SOS/Distribution/{distributionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
        }

        [Test, Order(10)]
        public async Task DeleteDistribution_ReturnsNotFound()
        {
            // Arrange
            var distributionId = 9999;

            // Act
            var response = await _customClient.DeleteAsync($"/api/SOS/Distribution/{distributionId}");

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
