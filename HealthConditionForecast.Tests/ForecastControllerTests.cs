using HealthConditionForecast.Controllers;
using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HealthConditionForecast.Tests
{
    public class ForecastControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private HttpClient GetMockHttpClient(string responseContent)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseContent),
               });

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task ForecastMigraine_ReturnsViewWithForecasts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var forecastData = JsonConvert.SerializeObject(new[]
            {
            new {
                ID = "1",
                IdForecast = '1',
                Name = "Migraine",
                LocalDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Value = 5,
                Category = "High",
                CategoryValue = 3,
                MobileLink = "http://example.com/m",
                Link = "http://example.com"
            }
        });
            var controller = new ForecastController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.ControllerContext.HttpContext.User = TestClaimsPrincipalFactory.Create("User");

            // Inject mock HttpClient
            var field = typeof(ForecastController).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(controller, GetMockHttpClient(forecastData));

            // Act
            var result = await controller.ForecastMigraine();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Forecast>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_ReturnsAllForecasts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Forecasts.Add(new Forecast {Id=1,IdForecast="12345", Name = "Migraine", Date = DateTime.Today, Value=5, Category="Moderate",CategoryValue=3,Link= "http://example.com",MobileLink= "http://m.example.com" });
            await context.SaveChangesAsync();

            var controller = new ForecastController(context);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = TestClaimsPrincipalFactory.Create("Admin");

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Forecast>>(viewResult.Model);
            Assert.Single(model);
        }
    }
}
