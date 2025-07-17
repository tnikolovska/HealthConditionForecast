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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
//using Moq;
//using Moq.Protected;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;


namespace HealthConditionForecast.Tests
{
    public class HealthConditionControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "HealthConditionDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_Returns_View_With_HealthConditions()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.HealthConditions.AddRange(
                new HealthCondition { Id = 1, Name = "Migraine", Description = "Headache" },
                new HealthCondition { Id = 2, Name = "Sinus", Description = "Nasal issue" });
            context.SaveChanges();

            var controller = new HealthConditionController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<HealthCondition>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_Returns_NotFound_For_Invalid_Id()
        {
            var context = GetInMemoryDbContext();
            var controller = new HealthConditionController(context);

            var result = await controller.Details(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Returns_View()
        {
            var context = GetInMemoryDbContext();
            var controller = new HealthConditionController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Removes_HealthCondition_And_Redirects()
        {
            var context = GetInMemoryDbContext();
            context.HealthConditions.Add(new HealthCondition { Id = 1, Name = "Migraine", Description = "Pain" });
            context.SaveChanges();

            var controller = new HealthConditionController(context);

            var result = await controller.Delete(1, null);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.HealthConditions);
        }
    }
}
