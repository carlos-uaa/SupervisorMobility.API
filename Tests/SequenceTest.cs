using Microsoft.AspNetCore.Mvc.Testing;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class SequenceTest : IDisposable
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

        // Tests for get single Sequence
        #region Get Single Flow Tests
        [Test, Order(1)]
        public async Task GetSingleFlow_ReturnsSuccessStatusCode()
        {
            // Arrange
            var sequenceId = 1;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Sequence/{sequenceId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var sequence = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequence);
        }

        [Test, Order(2)]
        public async Task GetSingleFlow_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidSequenceId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Sequence/{invalidSequenceId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for get all Sequences
        #region Get All Sequences Tests
        [Test, Order(3)]
        public async Task GetAllSequences_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync($"/api/SOS/Sequence/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var sequences = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequences);
        }
        #endregion

        // Tests for get Sequence by Distribution
        #region Get Sequences by Distribution Tests
        [Test, Order(4)]
        public async Task GetSequencesByDistribution_ReturnsSuccessStatusCode()
        {
            // Arrange
            var distributionId = 1;

            // Act
            var response = await _client.GetAsync($"/api/SOS/Sequence/byDistribution?Distribution_Id={distributionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var sequences = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequences);
        }
        #endregion

        // Tests for create Sequence
        #region Create Sequence Tests
        [Test, Order(5)]
        public async Task CreateSequence_ValidData_ReturnsCreatedStatusCode()
        {
            // Arrange
            int sequenceId = 1;

            // Get an existant Sequence to use its data for creation
            var sequenceReponse = await _client.GetAsync($"/api/SOS/Sequence/{sequenceId}");
            sequenceReponse.EnsureSuccessStatusCode();
            var sequenceData = await sequenceReponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequenceData);
            var Sequence = System.Text.Json.JsonSerializer.Deserialize<SOSSequence>(sequenceData);

            // Create a Sequence DTO object with the data

            SOSSequenceForCreateDto newSequence = new SOSSequenceForCreateDto
            {
                InternalControlNumber = Sequence.InternalControlNumber,
                OperationName = Sequence.OperationName,
                ProcessName = Sequence.ProcessName,
                CreatedDate = Sequence.CreatedDate,
                IsActive = Sequence.IsActive,
                SOSHubId = Sequence.SOSHubId,
                SequenceLogbooks = new List<SOSSequenceLogbookForCreateDto>(),
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>()
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newSequence), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/SOS/Sequence?SOSHubCollection_Id=1", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var createdSequence = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(createdSequence);
        }
        #endregion

        // Tests for update Sequence
        #region Create Sequence Tests
        [Test, Order(5)]
        public async Task UpdateSequence_ValidData_ReturnsCreatedStatusCode()
        {
            // Arrange
            int sequenceId = 1;

            // Get an existant Sequence to use its data for creation
            var sequenceReponse = await _client.GetAsync($"/api/SOS/Sequence/{sequenceId}");
            sequenceReponse.EnsureSuccessStatusCode();
            var sequenceData = await sequenceReponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequenceData);
            var Sequence = System.Text.Json.JsonSerializer.Deserialize<SOSSequence>(sequenceData);

            // Create a Sequence DTO object with the data

            SOSSequenceForCreateDto newSequence = new SOSSequenceForCreateDto
            {
                InternalControlNumber = Sequence.InternalControlNumber,
                OperationName = Sequence.OperationName,
                ProcessName = Sequence.ProcessName,
                CreatedDate = Sequence.CreatedDate,
                IsActive = Sequence.IsActive,
                SOSHubId = Sequence.SOSHubId,
                SequenceLogbooks = new List<SOSSequenceLogbookForCreateDto>(),
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>()
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newSequence), Encoding.UTF8, "application/json");

            // Post the copy in temporal db
            var response = await _customClient.PostAsync("/api/SOS/Sequence?SOSHubCollection_Id=1", content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var createdSequence = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(createdSequence);

            // Create a update dto with modified fields
            SOSSequenceForUpdateDto updateSequence = new SOSSequenceForUpdateDto
            {
                SOSSequenceId = sequenceId,
                InternalControlNumber = "Updated Control Number",
                OperationName = Sequence.OperationName,
                ProcessName = Sequence.ProcessName,
                CreatedDate = Sequence.CreatedDate,
                IsActive = Sequence.IsActive,
                SOSHubId = Sequence.SOSHubId,
                SequenceLogbooks = new List<SOSSequenceLogbookForUpdateDto>(),
                Notes = new List<UpdateCommentaryDto>(),
                Times = new List<SOSTimeForUpdateDto>()
            };

            // Act
            var updateContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateSequence), Encoding.UTF8, "application/json");
            var updateResponse = await _customClient.PutAsync($"/api/SOS/Sequence/{sequenceId}", updateContent);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);
            var updatedSequenceData = await updateResponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(updatedSequenceData);
        }
        #endregion

        // Tests for delete Sequence
        #region Delete Sequence Tests
        [Test, Order(6)]
        public async Task DeleteSequence_ValidId_ReturnsNoContentStatusCode()
        {
            // Arrange
            int sequenceId = 1;

            // Get an existant Sequence to use its data for creation
            var sequenceReponse = await _client.GetAsync($"/api/SOS/Sequence/{sequenceId}");
            sequenceReponse.EnsureSuccessStatusCode();
            var sequenceData = await sequenceReponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(sequenceData);
            var Sequence = System.Text.Json.JsonSerializer.Deserialize<SOSSequence>(sequenceData);

            // Create a Sequence DTO object with the data

            SOSSequenceForCreateDto newSequence = new SOSSequenceForCreateDto
            {
                InternalControlNumber = Sequence.InternalControlNumber,
                OperationName = Sequence.OperationName,
                ProcessName = Sequence.ProcessName,
                CreatedDate = Sequence.CreatedDate,
                IsActive = Sequence.IsActive,
                SOSHubId = Sequence.SOSHubId,
                SequenceLogbooks = new List<SOSSequenceLogbookForCreateDto>(),
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>()
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newSequence), Encoding.UTF8, "application/json");

            // Post the copy in temporal db
            var response = await _customClient.PostAsync("/api/SOS/Sequence?SOSHubCollection_Id=1", content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var createdSequence = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(createdSequence);
            var createdSequenceObj = System.Text.Json.JsonSerializer.Deserialize<SOSSequence>(createdSequence);

            // Act
            var deleteResponse = await _customClient.DeleteAsync($"/api/SOS/Sequence/{sequenceId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Test, Order(7)]
        public async Task DeleteSequence_InvalidId_ReturnsNotFoundStatusCode()
        {
            // Arrange
            var invalidSequenceId = 9999;

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/SOS/Sequence/{invalidSequenceId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, deleteResponse.StatusCode);
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
