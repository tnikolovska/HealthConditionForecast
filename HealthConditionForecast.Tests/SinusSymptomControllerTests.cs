using HealthConditionForecast.Controllers;
using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HealthConditionForecast.Tests
{
    public class SinusSymptomControllerTests
    {

        private ApplicationDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }
        [Fact]
        public async Task Index_ReturnsAllSinusSymptoms()
        {
            var context = GetInMemoryContext("IndexTest");
            context.HealthConditions.Add(new HealthCondition { Id = 1, Name = "Sinus Headache", Description = "..." });
            context.SinusSymptoms.AddRange(
                new SinusSymptom { Id = 1, Name = "S1", Description = "D1", HealthConditionId = 1, Type = SinusType.Major },
                new SinusSymptom { Id = 2, Name = "S2", Description = "D2", HealthConditionId = 1, Type = SinusType.Major }
            );
            await context.SaveChangesAsync();

            var ctrl = new SinusSymptomController(context);
            var result = Assert.IsType<ViewResult>(await ctrl.Index());
            var model = Assert.IsAssignableFrom<IEnumerable<SinusSymptom>>(result.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsSymptom()
        {
            var context = GetInMemoryContext("CreateTest");
            context.HealthConditions.Add(new HealthCondition { Id = 1, Name = "Sinus Headache", Description = "desc" });
            await context.SaveChangesAsync();

            var ctrl = new SinusSymptomController(context);
            var symptom = new SinusSymptom { Name = "New", Description = "desc", HealthConditionId = 1, Type = SinusType.Major };

            var result = await ctrl.Create(symptom);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, context.SinusSymptoms.Count());
        }

        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesSymptom()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.HealthConditions.Add(new HealthCondition { Id = 1L, Name = "Sinus Headache", Description = "desc" });
                seedContext.SinusSymptoms.Add(new SinusSymptom { Id = 1L, Name = "Sinus Symptom", Description = "desc", HealthConditionId = 1L, Type = SinusType.Minor });
                seedContext.SaveChanges();
            }

            // 2️⃣ Use a new context instance to run the controller
            using (var testContext = new ApplicationDbContext(options))
            {
                var controller = new SinusSymptomController(testContext);
                var updated = new SinusSymptom { Id = 1L, Name = "Updated Aura", Description = "Changed", HealthConditionId = 1L, Type = SinusType.Minor };

                var result = await controller.Edit(1L, updated);

                Assert.IsType<RedirectToActionResult>(result);

                var fromDb = testContext.SinusSymptoms.Find(1L);
                Assert.Equal("Updated Aura", fromDb.Name);
            }
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSymptom()
        {
            var context = GetInMemoryContext("DeleteTest");
            context.SinusSymptoms.Add(new SinusSymptom { Id = 1, Name = "ToDel", Description = "d", HealthConditionId = 1, Type = SinusType.Major });
            await context.SaveChangesAsync();

            var ctrl = new SinusSymptomController(context);
            var result = await ctrl.DeleteConfirmed(1);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.SinusSymptoms);
        }
    }
}
