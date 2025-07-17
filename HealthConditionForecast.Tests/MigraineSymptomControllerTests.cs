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


using HealthConditionForecast.Controllers;
using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace HealthConditionForecast.Tests
{
    public class MigraineSymptomControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            var migraineCondition = new HealthCondition { Id = 1, Name = "Migraine Headache", Description = "desc" };
            context.HealthConditions.Add(migraineCondition);

            context.MigraineSymptons.AddRange(
                new MigraineSympton { Id = 1, Name = "Aura", Description = "Visual disturbance", HealthConditionId = 1, Type = MigraineType.MigraineWithAura },
                new MigraineSympton { Id = 2, Name = "Nausea", Description = "Feeling sick", HealthConditionId = 1, Type = MigraineType.DuringAttack }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Index_ReturnsViewWithMigraineSymptoms()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MigraineSymptomController(context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            var model = Assert.IsAssignableFrom<IEnumerable<MigraineSympton>>(result.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ValidId_ReturnsSymptom()
        {
            var context = GetInMemoryContext();
            var controller = new MigraineSymptomController(context);

            var result = await controller.Details(1) as ViewResult;

            var symptom = Assert.IsType<MigraineSympton>(result.Model);
            Assert.Equal("Aura", symptom.Name);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = new MigraineSymptomController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsSymptom()
        {
            var context = GetInMemoryContext();
            var controller = new MigraineSymptomController(context);

            var symptom = new MigraineSympton
            {
                Name = "Throbbing Pain",
                Description = "Severe headache",
                HealthConditionId = 1,
                Type = MigraineType.DuringAttack
            };

            var result = await controller.Create(symptom);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(3, context.MigraineSymptons.Count());
        }

        /* [Fact]
         public async Task Edit_Post_ValidModel_UpdatesSymptom()
         {
             var context = GetInMemoryContext();
             var controller = new MigraineSymptomController(context);

             var updatedSymptom = new MigraineSympton
             {
                 Id = 1,
                 Name = "Updated Aura",
                 Description = "Changed",
                 HealthConditionId = 1,
                 Type = MigraineType.MigraineWithAura
             };

             var result = await controller.Edit(1, updatedSymptom);


             Assert.IsType<RedirectToActionResult>(result);
             Assert.Equal("Updated Aura", context.MigraineSymptons.Find(1).Name);
         }*/

        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesSymptom()
        {
            // 1️⃣ Seed data into a fresh in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.HealthConditions.Add(new HealthCondition { Id = 1L, Name = "Migraine Headache", Description = "desc" });
                seedContext.MigraineSymptons.Add(new MigraineSympton { Id = 1L, Name = "Aura", Description = "desc", HealthConditionId = 1L, Type = MigraineType.MigraineWithAura });
                seedContext.SaveChanges();
            }

            // 2️⃣ Use a new context instance to run the controller
            using (var testContext = new ApplicationDbContext(options))
            {
                var controller = new MigraineSymptomController(testContext);
                var updated = new MigraineSympton { Id = 1L, Name = "Updated Aura", Description = "Changed", HealthConditionId = 1L, Type = MigraineType.MigraineWithAura };

                var result = await controller.Edit(1L, updated);

                Assert.IsType<RedirectToActionResult>(result);

                var fromDb = testContext.MigraineSymptons.Find(1L);
                Assert.Equal("Updated Aura", fromDb.Name);
            }
        }



        [Fact]
        public async Task DeleteConfirmed_RemovesSymptom()
        {
            var context = GetInMemoryContext();
            var controller = new MigraineSymptomController(context);
            long id = 1L;
            var result = await controller.DeleteConfirmed(id);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(context.MigraineSymptons.Find(id));
        }
    }
}
