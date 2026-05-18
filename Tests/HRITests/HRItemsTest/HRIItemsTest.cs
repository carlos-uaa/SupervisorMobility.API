using Moq;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HRItemsTest
{
    
    public class HRIItemsTest
    {
        [Test]
        public async Task CreateHRIItemAsync()
        {
            // arrange
            var mockService = new Mock<IHRIItemsService>();
        }
    }
}
