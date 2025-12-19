using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Testing;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Tests
{
    public class AnalysisTest : IDisposable
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

        // Tests for Create Analysis
        #region Get Single Analysis
        [Test, Order(1)]
        public async Task Test_CreateAnalysis_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var analysisId = 1;
            var getResponse = await _client.GetAsync($"/api/SOS/Analysis/{analysisId}");
            var analysis = await getResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(analysis);

            // Act: Post the copy in temporal DB
            var analysisCopy = new SOSAnalysisForCreateDto
            {
                SOSAnalysisId = 0,
                InternalControlNumber = analysis.InternalControlNumber,
                OperationName = analysis.OperationName,
                ProcessName = analysis.ProcessName,
                AnalysisLogbooks = analysis.AnalysisLogbooks,
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>(),
                CreatedDate = analysis.CreatedDate,
                IsActive = analysis.IsActive,
                SOSHubId = analysis.SOSHubId

            };

            //Assert
            var postResponse = await _customClient.PostAsJsonAsync($"/api/SOS/Analysis?SOSHubCollection_Id={1}", analysisCopy);
            Assert.IsNotNull(postResponse);
            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);
            postResponse.EnsureSuccessStatusCode();
            var createdAnalysis = await postResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(createdAnalysis);
            Assert.AreEqual(analysis.OperationName, createdAnalysis.OperationName);
            Assert.AreNotEqual(0, createdAnalysis.SOSAnalysisId);
        }
        #endregion

        // Tests for Get Single Analysis 
        #region Get Single Analysis
        [Test, Order(1)]
        public async Task Test_GetAnalysisById_ReturnsSuccess()
        {
            // Arrange
            var analysisId = 1;

            // Act
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync($"/api/SOS/Analysis/{analysisId}");
            stopwatch.Stop();

            // Assert: Status code
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Assert: MIME
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType!.ToString());

            // Assert: Body not empty
            Assert.IsTrue(response.Content.Headers.ContentLength > 0);

            // Assert: Deserialize
            var analysis = await response.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(analysis);

            // Assert: ID
            Assert.AreEqual(analysisId, analysis.SOSAnalysisId);

            // Assert: Required fields
            Assert.IsFalse(string.IsNullOrWhiteSpace(analysis.OperationName), "OperationName should not be empty");
            Assert.IsFalse(string.IsNullOrWhiteSpace(analysis.ProcessName), "ProcessName should not be empty");
            Assert.IsNotNull(analysis.CreatedDate);

            // Assert: Collections
            Assert.IsNotNull(analysis.Notes);
            Assert.IsNotNull(analysis.AnalysisLogbooks);
            Assert.IsNotNull(analysis.Times);

            // Assert: Relations
            Assert.IsTrue(analysis.SOSHubId > 0);

            // Assert: Performance
            Assert.Less(stopwatch.ElapsedMilliseconds, 3000, $"El endpoint tardó demasiado: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Test, Order(2)]
        public async Task Test_GetAnalysisById_ReturnsNotFound()
        {
            // Arrange
            var response = await _client.GetAsync("/api/SOS/Analysis/9999");
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        // Tests for Get All Analyses
        #region Get All Analyses
        [Test, Order(3)]
        public async Task Test_GetAllAnalyses_ReturnsSuccess()
        {
            // Act
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync("/api/SOS/Analysis/all");
            stopwatch.Stop();

            // ASSERT: Status Code
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "El endpoint /all no regresó 200 OK.");

            // ASSERT: MIME Type 
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString(), "El Content-Type no es JSON UTF-8.");

            // ASSERT: Response Body
            Assert.IsTrue(response.Content.Headers.ContentLength > 0, "El contenido regresó vacío.");

            // ASSERT: Deserialize
            var analyses = await response.Content.ReadFromJsonAsync<List<SOSAnalysisDto>>();
            Assert.IsNotNull(analyses, "No se pudo deserializar la lista de análisis.");
            Assert.IsNotEmpty(analyses, "La lista de análisis vino vacía, debería contener datos en BD.");

            // ASSERT: Validar datos de cada análisis
            foreach (var analysis in analyses)
            {
                Assert.IsTrue(analysis.SOSAnalysisId > 0, "Un análisis tiene un SOSAnalysisId inválido.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(analysis.OperationName), "Un análisis tiene OperationName vacío.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(analysis.ProcessName), "Un análisis tiene ProcessName vacío.");
                Assert.IsNotNull(analysis.CreatedDate, "Un análisis tiene CreatedDate nulo.");
                Assert.IsTrue(analysis.SOSHubId > 0, "Un análisis tiene SOSHubId inválido.");

                Assert.IsNotNull(analysis.Notes, "La colección Notes viene nula.");
                Assert.IsNotNull(analysis.Times, "La colección Times viene nula.");
                Assert.IsNotNull(analysis.AnalysisLogbooks, "La colección AnalysisLogbooks viene nula.");
            }

            // ASSERT: Performance
            Assert.Less(stopwatch.ElapsedMilliseconds, 2000, $"El endpoint tardó demasiado: {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion

        // Tests for Get Analysis by Distribution
        #region Get Analysis by Distribution
        [Test, Order(4)]
        public async Task Test_GetAnalysesByDistributionId_ReturnsSuccess()
        {
            // Arrange
            var distributionId = 1;

            // Act
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync($"/api/SOS/Analysis/byDistribution?Distribution_Id={distributionId}");
            stopwatch.Stop();

            // Assert: Status code
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Assert: MIME
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType!.ToString());

            // Assert: Body not empty
            Assert.IsTrue(response.Content.Headers.ContentLength > 0);

            // Assert: Deserialize
            var analyses = await response.Content.ReadFromJsonAsync<List<SOSAnalysisDto>>();
            Assert.IsNotNull(analyses);
            //Assert.IsNotEmpty(analyses);

            // Assert: Each analysis has the correct DistributionId
            foreach (var analysis in analyses)
            {
                Assert.IsTrue(analysis.SOSHubId == distributionId);
            }

            // Assert: Performance
            Assert.Less(stopwatch.ElapsedMilliseconds, 2000, $"El endpoint tardó demasiado: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Test, Order(5)]
        public async Task Test_GetAnalysesByDistributionId_ReturnsEmptyList()
        {
            // Arrange
            var distributionId = 9999; // Assuming this ID does not exist

            // Act
            var response = await _client.GetAsync($"/api/SOS/Analysis/byDistribution?Distribution_Id={distributionId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var analyses = await response.Content.ReadFromJsonAsync<List<SOSAnalysisDto>>();
            Assert.IsNotNull(analyses);
            Assert.IsEmpty(analyses);
        }
        #endregion

        // Tests for Update Analysis
        #region Update Analysis
        [Test, Order(6)]
        public async Task Test_UpdateAnalysis_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var analysisId = 1;
            var getResponse = await _client.GetAsync($"/api/SOS/Analysis/{analysisId}");
            var analysis = await getResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(analysis);

            // Post the copy in temporal DB
            var analysisCopy = new SOSAnalysisForCreateDto
            {
                SOSAnalysisId = 0,
                InternalControlNumber = analysis.InternalControlNumber,
                OperationName = analysis.OperationName,
                ProcessName = analysis.ProcessName,
                AnalysisLogbooks = analysis.AnalysisLogbooks,
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>(),
                CreatedDate = analysis.CreatedDate,
                IsActive = analysis.IsActive,
                SOSHubId = analysis.SOSHubId

            };

            var postResponse = await _customClient.PostAsJsonAsync($"/api/SOS/Analysis?SOSHubCollection_Id={1}", analysisCopy);
            postResponse.EnsureSuccessStatusCode();

            // Modify a field
            var originalOperationName = analysis.OperationName;
            analysis.OperationName = "Updated Operation Name";

            // Act
            var analysisDto = new SOSAnalysisForUpdateDto
            {
                SOSAnalysisId = analysis.SOSAnalysisId,
                InternalControlNumber = analysis.InternalControlNumber,
                OperationName = analysis.OperationName,
                ProcessName = analysis.ProcessName,
                AnalysisLogbooks = new List<SOSAnalysisLogbookForUpdateDto>(),
                Notes = new List<UpdateCommentaryDto>(),
                Times = new List<SOSTimeForUpdateDto>(),
                CreatedDate = analysis.CreatedDate,
                IsActive = analysis.IsActive,
                SOSHubId = analysis.SOSHubId
            };
            var putResponse = await _customClient.PutAsJsonAsync($"/api/SOS/Analysis/{analysisId}?userId={userId}", analysis);

            // Assert
            putResponse.EnsureSuccessStatusCode();

            // Verify the update
            var verifyResponse = await _customClient.GetAsync($"/api/SOS/Analysis/{analysisId}");
            var updatedAnalysis = await verifyResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(updatedAnalysis);
            Assert.AreEqual("Updated Operation Name", updatedAnalysis.OperationName);

            // Revert changes
            analysis.OperationName = originalOperationName;
            await _customClient.PutAsJsonAsync($"/api/SOS/Analysis/{analysisId}", analysis);
        }

        [Test, Order(7)]
        public async Task Test_UpdateAnalysis_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var nonExistentAnalysisId = 9999;
            var analysisDto = new SOSAnalysisForUpdateDto
            {
                SOSAnalysisId = nonExistentAnalysisId,
                InternalControlNumber = "ICN-9999",
                OperationName = "Non-existent Analysis",
                ProcessName = "Test Process",
                AnalysisLogbooks = new List<SOSAnalysisLogbookForUpdateDto>(),
                Notes = new List<UpdateCommentaryDto>(),
                Times = new List<SOSTimeForUpdateDto>(),
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                SOSHubId = 1
            };

            // Act
            var putResponse = await _customClient.PutAsJsonAsync($"/api/SOS/Analysis/{nonExistentAnalysisId}?userId={userId}", analysisDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, putResponse.StatusCode);
        }

        #endregion

        // Tests for Delete Analysis
        #region Delete Analysis
        [Test, Order(8)]
        public async Task Test_DeleteAnalysis_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var analysisId = 1;
            var getResponse = await _client.GetAsync($"/api/SOS/Analysis/{analysisId}");
            var analysis = await getResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(analysis);

            // Post the copy in temporal DB
            var analysisCopy = new SOSAnalysisForCreateDto
            {
                SOSAnalysisId = 0,
                InternalControlNumber = analysis.InternalControlNumber,
                OperationName = analysis.OperationName,
                ProcessName = analysis.ProcessName,
                AnalysisLogbooks = analysis.AnalysisLogbooks,
                Notes = new List<CreateCommentaryDto>(),
                Times = new List<SOSTimeForCreateDto>(),
                CreatedDate = analysis.CreatedDate,
                IsActive = analysis.IsActive,
                SOSHubId = analysis.SOSHubId

            };

            var postResponse = await _customClient.PostAsJsonAsync($"/api/SOS/Analysis?SOSHubCollection_Id={1}", analysisCopy);
            postResponse.EnsureSuccessStatusCode();
            var createdAnalysis = await postResponse.Content.ReadFromJsonAsync<SOSAnalysisDto>();
            Assert.IsNotNull(createdAnalysis);

            // Act
            var deleteResponse = await _customClient.DeleteAsync($"/api/SOS/Analysis/{createdAnalysis.SOSAnalysisId}?userId={userId}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();

            // Verify deletion
            var verifyResponse = await _customClient.GetAsync($"/api/SOS/Analysis/{createdAnalysis.SOSAnalysisId}");
            Assert.AreEqual(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        }

        [Test, Order(9)]
        public async Task Test_DeleteAnalysis_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var nonExistentAnalysisId = 9999;

            // Act
            var deleteResponse = await _customClient.DeleteAsync($"/api/SOS/Analysis/{nonExistentAnalysisId}?userId={userId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
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
