using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class SynopticTableOfControlPointsTest : IDisposable
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

        // Tests for get single Synoptic Table of Control Points
        #region Get Single Synoptic Table of Control Points Tests
        //[Test, Order(1)]
        //public async Task GetSingleSynopticTableofControlPoints_ReturnsSuccessStatusCode()
        //{
        //    // Arrange
        //    var tableId = 1;

        //    // Act
        //    var response = await _client.GetAsync($"/api/SOS/SynopticTableofControlPoints/{tableId}");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var sequence = await response.Content.ReadAsStringAsync();
        //    Assert.IsNotNull(sequence);
        //}

        [Test, Order(2)]
        public async Task GetSingleSynopticTableofControlPoints_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidTableId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/SOS/SynopticTableofControlPoints/{invalidTableId}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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
