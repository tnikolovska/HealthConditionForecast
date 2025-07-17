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
    public class UserSymptomSelectionControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserSymptomSelectionController _controller;
        private const string TestUserId = "user-123";

        public UserSymptomSelectionControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // seed data
            _context.Users.Add(new ApplicationUser { Id = TestUserId, UserName="user-123" });
            _context.HealthConditions.Add(new HealthCondition { Id = 1, Name = "Migraine",Description="Migraine" });
            _context.UserHealthConditions.Add(new UserHealthCondition
            {
                Id = 10,
                UserId = TestUserId,
                HealthConditionId = 1
            });
            _context.MigraineSymptons.AddRange(
                new MigraineSympton { Id = 100, Name = "Aura",Description="Aura", HealthConditionId = 1, Type = MigraineType.MigraineWithAura },
                new MigraineSympton { Id = 101, Name = "Headache",Description="headache", HealthConditionId = 1, Type = MigraineType.DuringAttack }
            );
            _context.SaveChanges();

            _controller = new UserSymptomSelectionController(_context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, TestUserId),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Fact]
        public async Task Index_ReturnsViewWithSelections()
        {
            var selection = new UserSymptomSelection
            {
                Id = 200L,
                UserHealthConditionId = 10,
                MigraineSymptoms = new List<MigraineSympton> { _context.MigraineSymptons.Find(100L)! }
            };
            _context.UserSymptomSelections.Add(selection);
            _context.SaveChanges();

            var result = await _controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<UserSymptomSelection>>(result.Model);
            Assert.Single(model);
            Assert.Equal(200L, model.First().Id);
        }


        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithSelection()
        {
            var sel = new UserSymptomSelection
            {
                Id = 300L,
                UserHealthConditionId = 10,
                MigraineSymptoms = new[] { _context.MigraineSymptons.Find(101L)! }.ToList()
            };
            _context.UserSymptomSelections.Add(sel);
            _context.SaveChanges();

            var result = await _controller.Details(300L) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<UserSymptomSelection>(result.Model);
            Assert.Equal(300L, model.Id);
        }


        [Fact]
        public void SelectSymptoms_SetsCorrectViewBag_ForMigraine()
        {
            var result = _controller.SelectSymptoms(10) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal("Migraine", result.ViewData["ConditionType"]);
            Assert.NotNull(result.ViewData["BeforeHeadache"]);
            Assert.NotNull(result.ViewData["MigraineWithAura"]);
            Assert.NotNull(result.ViewData["DuringAttack"]);

        }

        [Fact]
        public async Task SelectSymptomsPost_ValidMigraine_RedirectsToForecast()
        {
            var ids = new List<long> { 100 };
            var result = await _controller.SelectSymptoms(10, ids) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ForecastMigraine", result.ActionName);
            Assert.Single(_context.UserSymptomSelections);
        }

        [Fact]
        public async Task SelectSymptomsPost_NoSelection_ShowsValidation()
        {
            var result = await _controller.SelectSymptoms(10, new List<long>()) as RedirectToActionResult;

            Assert.NotNull(result);
            //Assert.Equal("ValidationMessage", result.ActionName);
            //Assert.Equal(10, (int)result.RouteValues["userHealthConditionId"]);
            Assert.Equal("SelectSymptoms", result.ActionName);
        }





    }

}
