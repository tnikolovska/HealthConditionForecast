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
    public class SymptomControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SymptomTestDb")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureDeleted(); // Reset between tests
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task Index_ReturnsListOfSymptoms()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Symptoms.Add(new Symptom { Name = "TestSymptom", Description = "TestSymptom", HealthConditionId = 1 });
            await context.SaveChangesAsync();

            var controller = new SymptomController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Symptom>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdInvalid()
        {
            var controller = new SymptomController(GetInMemoryDbContext());
            var result = await controller.Details(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsSymptom()
        {
            var context = GetInMemoryDbContext();
            context.HealthConditions.Add(new HealthCondition
            {
                Id = 1,
                Name = "Migraine",
                Description = "Description of Migraine"  // <-- Add this!
            });
            await context.SaveChangesAsync();

            var controller = new SymptomController(context);
            var newSymptom = new Symptom { Name = "Pain", Description = "Sharp pain", HealthConditionId = 1 };

            var result = await controller.Create(newSymptom);

            Assert.Equal(1, context.Symptoms.Count());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ValidSymptom_UpdatesData()
        {
            var context = GetInMemoryDbContext();
            var symptom = new Symptom { Id = 1, Name = "Original", Description = "Old", HealthConditionId = 1 };
            context.Symptoms.Add(symptom);
            await context.SaveChangesAsync();

            var controller = new SymptomController(context);
            var updated = new Symptom { Id = 1, Name = "Updated", Description = "New" };

            var result = await controller.Edit(1, updated);

            Assert.Equal("Updated", context.Symptoms.First().Name);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSymptom()
        {
            var context = GetInMemoryDbContext();
            context.Symptoms.Add(new Symptom { Id = 1, Name = "DeleteMe", Description = "DeleteMe", HealthConditionId = 1 });
            await context.SaveChangesAsync();

            var controller = new SymptomController(context);
            var result = await controller.Delete(1, null);

            Assert.Empty(context.Symptoms);
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
