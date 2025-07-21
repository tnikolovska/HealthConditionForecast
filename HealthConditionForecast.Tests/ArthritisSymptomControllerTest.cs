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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HealthConditionForecast.Tests
{
    public class ArthritisSymptomControllerTest
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;

            var context = new ApplicationDbContext(options);
            SeedHealthConditions(context);
            return context;
        }

        private void SeedHealthConditions(ApplicationDbContext context)
        {
            context.HealthConditions.Add(new HealthCondition
            {
                Id = 1,
                Name = "Arthritis",
                Description = "Test Arthritis"
            });
            context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewWithArthritisSymptoms()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.ArthritisSymtoms.Add(new ArthritisSymtom
            {
                Name = "Swelling",
                Description = "Joint swelling",
                HealthConditionId = 1
            });
            context.SaveChanges();

            var controller = new ArthritisSymptomController(context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ArthritisSymtom>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsSymptomAndRedirects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ArthritisSymptomController(context);
            var symptom = new ArthritisSymtom
            {
                Name = "Pain",
                Description = "Severe pain",
                HealthConditionId = 1
            };

            // Act
            var result = await controller.Create(symptom) as RedirectToActionResult;

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Single(context.ArthritisSymtoms);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewWithSymptom()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var symptom = new ArthritisSymtom
            {
                Name = "Stiffness",
                Description = "Morning stiffness",
                HealthConditionId = 1
            };
            context.ArthritisSymtoms.Add(symptom);
            context.SaveChanges();

            var controller = new ArthritisSymptomController(context);

            // Act
            var result = await controller.Edit(symptom.Id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ArthritisSymtom>(result.Model);
            Assert.Equal("Stiffness", model.Name);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSymptomAndRedirects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var symptom = new ArthritisSymtom
            {
                Name = "Redness",
                Description = "Joint redness",
                HealthConditionId = 1
            };
            context.ArthritisSymtoms.Add(symptom);
            context.SaveChanges();

            var controller = new ArthritisSymptomController(context);

            // Act
            var result = await controller.DeleteConfirmed(symptom.Id) as RedirectToActionResult;

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Empty(context.ArthritisSymtoms);
        }
    }
}
