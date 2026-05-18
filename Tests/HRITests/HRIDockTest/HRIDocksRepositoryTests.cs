using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using System.Linq;

namespace Tests.HRITests.HRIDockTest
{
    public class HRIDocksRepositoryTests
    {
        private DbContextOptions<SupervisorMobilityContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<SupervisorMobilityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }


        // GetAllHRIDocksAsync --Ready
        [Test]
        public async Task GetAllHRIDocksAsync_ReturnsActiveDocksOnly()
        {
            var options = CreateNewContextOptions();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                await context.HRIDocks.AddRangeAsync(
                    new HRIDock { Code = "D1", DockName = "Dock 1", IsActive = true },
                    new HRIDock { Code = "D2", DockName = "Dock 2", IsActive = false }
                );
                await context.SaveChangesAsync();

                var repo = new HRIDocksRepository(context);
                var result = await repo.GetAllHRIDocksAsync();

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(1, result.Data.Count);
                Assert.AreEqual("D1", result.Data.First().Code);
            }
        }


        // GetSingleHRIDockAsync --ready
        [Test]
        public async Task GetSingleHRIDockAsync_ReturnsDock_WhenExists()
        {
            var options = CreateNewContextOptions();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                var dock = new HRIDock { Code = "D1", DockName = "Dock 1", IsActive = true };
                await context.HRIDocks.AddAsync(dock);
                await context.SaveChangesAsync();

                var repo = new HRIDocksRepository(context);
                var result = await repo.GetSingleHRIDockAsync(dock.Id);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual("D1", result.Data.Code);
            }
        }


        // CreateHRIDockAsync --ready
        [Test]
        public async Task CreateHRIDockAsync_InsertsRecord()
        {
            var options = CreateNewContextOptions();

            await using (var context = new SupervisorMobilityContext(options))
            {
                var repo = new HRIDocksRepository(context);
                var dock = new HRIDock { Code = "D1", DockName = "Dock 1" };

                var result = await repo.CreateHRIDockAsync(dock);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual("D1", result.Data.Code);
                Assert.IsTrue(context.HRIDocks.Any(d => d.Code == "D1"));
            }
        }


        // UpdateHRIDockAsync --ready
        [Test]
        public async Task UpdateHRIDockAsync_UpdatesRecord()
        {
            var options = CreateNewContextOptions();

            await using (var context = new SupervisorMobilityContext(options))
            {
                // seed
                var dock = new HRIDock { Code = "D2", DockName = "Old" };
                await context.HRIDocks.AddAsync(dock);
                await context.SaveChangesAsync();

                var repo = new HRIDocksRepository(context);

                dock.DockName = "New";
                var result = await repo.UpdateHRIDockAsync(dock);

                Assert.IsTrue(result.Success);
                Assert.AreEqual("New", result.Data.DockName);
            }
        }


        // DeleteHRIDockAsync --ready
        [Test]
        public async Task DeleteHRIDockAsync_SetsIsActiveFalse()
        {
            var options = CreateNewContextOptions();

            await using (var context = new SupervisorMobilityContext(options))
            {
                var dock = new HRIDock { Code = "D3", DockName = "ToDelete", IsActive = true };
                await context.HRIDocks.AddAsync(dock);
                await context.SaveChangesAsync();

                var repo = new HRIDocksRepository(context);

                var result = await repo.DeleteHRIDockAsync(dock.Id);

                Assert.IsTrue(result.Success);
                var dbDock = context.HRIDocks.FirstOrDefault(d => d.Id == dock.Id);
                Assert.IsNotNull(dbDock);
                Assert.IsFalse(dbDock.IsActive);
            }
        }
    }
}
