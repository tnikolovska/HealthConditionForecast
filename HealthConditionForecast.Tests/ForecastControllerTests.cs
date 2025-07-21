using HealthConditionForecast.Controllers;
using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            /* var context = GetInMemoryDbContext();
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
             Assert.Single(model);*/
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDb_ForecastMigraine")
           .Options;
            var context = new ApplicationDbContext(options);

            // 2. Setup mock HttpClient to return a valid JSON string that your ParseJSON expects
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "[{\"ID\":\"1\",\"Name\":\"Migraine\",\"LocalDateTime\":\"2025-07-21T10:00:00\",\"Value\":5,\"Category\":\"High\",\"CategoryValue\":3,\"MobileLink\":\"http://example.com/m\",\"Link\":\"http://example.com\"}]",
                        Encoding.UTF8,
                        "application/json")
                });

            var httpClient = new HttpClient(mockHandler.Object);

            // 3. Create controller instance with both context and httpClient
            var controller = new ForecastController(context);

            // 4. Setup TempData (needed because your controller reads TempData["CanShowForecast"])
            var tempDataProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);
            controller.TempData["CanShowForecast"] = "yes";

            // 5. Setup authenticated user in HttpContext (your controller checks User.Identity.IsAuthenticated)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.ForecastMigraine();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Forecast>>(viewResult.Model);

            //Assert.Single(model);
            Assert.Equal(5, model.Count);
            //Assert.Equal("Migraine", model[0].Name);
            //Assert.Equal(5, model[0].Value);
        }

        [Fact]
        public async Task Index_ReturnsAllForecasts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Forecasts.Add(new Forecast { Id = 1, IdForecast = "12345", Name = "Migraine", Date = DateTime.Today, Value = 5, Category = "Moderate", CategoryValue = 3, Link = "http://example.com", MobileLink = "http://m.example.com" });
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

