using HealthConditionForecast.Controllers;
using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthConditionForecast.Tests
{
    
        public class UserHealthConditionControllerTests
        {
            private ApplicationDbContext _context;
            private UserHealthConditionController _controller;
            private const string TestUserId = "user-123";


        public void Dispose()
        {
            // Clear the change tracker after each test
            _context.ChangeTracker.Clear();
        }
    
        private ApplicationDbContext GetInMemoryContext(string dbName)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(opts);
        }

        public UserHealthConditionControllerTests()
            {

                var dbName = Guid.NewGuid().ToString();
                _context = GetInMemoryContext(dbName);

                _context.Users.Add(new ApplicationUser { Id = TestUserId, UserName = "testuser" });
                _context.HealthConditions.Add(new HealthCondition { Id = 1, Name = "Migraine", Description = "Migraine" });
                _context.SaveChanges();

                _controller = new UserHealthConditionController(_context);
                var user = new ClaimsPrincipal(new ClaimsIdentity(new[]{
                new Claim(ClaimTypes.NameIdentifier, TestUserId),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };
        }

            [Fact]
            public async Task Index_ReturnsViewWithUserConditions()
            {
            // Seed user condition
            _context.UserHealthConditions.Add(new UserHealthCondition
                {
                    Id = 1,
                    UserId = TestUserId,
                    HealthConditionId = 1
                });
                _context.SaveChanges();

                var result = await _controller.Index() as ViewResult;

                Assert.NotNull(result);
                var model = Assert.IsAssignableFrom<List<string>>(result.Model);
                Assert.Single(model);
                Assert.Contains("Migraine", model);
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);
                Assert.Collection(model,
                        item => Assert.Equal("Migraine", item)
                        );
             }   

            [Fact]
            public async Task Create_Post_Valid_AddsAndRedirects()
            {
                var model = new UserHealthCondition { HealthConditionId = 1 };

                var result = await _controller.Create(model) as RedirectToActionResult;

                Assert.NotNull(result);
                Assert.Equal("SelectSymptoms", result.ActionName);
                Assert.Single(_context.UserHealthConditions);
                Assert.Equal(TestUserId, _context.UserHealthConditions.First().UserId);
            }
        }
    }

