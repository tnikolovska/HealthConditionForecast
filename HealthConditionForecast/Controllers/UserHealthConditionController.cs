using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthConditionForecast.Controllers
{
    public class UserHealthConditionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserHealthConditionController(ApplicationDbContext context)
        {
            _context = context;
        }
        //[Authorize(Roles = "Admin")]
        // GET: UserHealthConditionController
        /* public async Task<IActionResult> AllUserHealthConditions()
         {

             var userHealthConditions = await _context.UserHealthConditions
             .ToListAsync();
             var usernames = await _context.Users
                 .Where(u => userHealthConditions.Select(uhc => uhc.UserId).Contains(u.Id))
                 .Select(u => u.UserName)
                 .ToListAsync();
             var healthConditionNames = await _context.HealthConditions
                 .Where(hc => userHealthConditions.Select(uhc => (long)uhc.HealthConditionId).Contains(hc.Id))
                 .Select(hc =>hc.Name)
                 .ToListAsync();
             UserHealthConditionList userHealthConditionList = new UserHealthConditionList
             {
                 usernames = usernames,
                 healthConditionNames = healthConditionNames,
             };

             return View(userHealthConditionList);
         }*/


        [Authorize(Roles = "Admin,User")]
        // GET: UserHealthConditionController
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            List<string> healthConditionNames=new List<string>();
            string username = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
            var userHealthConditions = await _context.UserHealthConditions.Where(u=>u.UserId.Equals(userId))
            .ToListAsync();
            var healthconditionNames = await _context.UserHealthConditions
                .Where(uhc => uhc.UserId == userId)
                .Join(_context.HealthConditions,
                      uhc => uhc.HealthConditionId,
                      hc => hc.Id,
                      (uhc, hc) => hc.Name)
                .ToListAsync();
            ViewData["UserName"] = username;

            return View(healthconditionNames);
        }

        // GET: UserHealthConditionController/Details/5
        // GET: UserHealthCondition/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var userHealthCondition = await _context.UserHealthConditions
                .FirstOrDefaultAsync(uhc => uhc.Id == id);

            if (userHealthCondition == null)
                return NotFound();

            return View(userHealthCondition);
        }

        // GET: UserHealthConditionController/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                //ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
                ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name");
                return View();
            }
            else return Redirect("/Identity/Account/Login");
        }
        [Authorize(Roles = "Admin,User")]
        // POST: UserHealthConditionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HealthConditionId")] UserHealthCondition userHealthCondition)
        {
            
                userHealthCondition.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ModelState.Remove("UserId");
                if (ModelState.IsValid)
                {
                    _context.UserHealthConditions.Add(userHealthCondition);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction("SelectSymptoms", "UserSymptomSelection", new { healthConditionId = userHealthCondition.HealthConditionId });
                    return RedirectToAction("SelectSymptoms", "UserSymptomSelection", new { userHealthConditionId = userHealthCondition.Id });
                }

                // ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userHealthCondition.UserId);
                ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", userHealthCondition.HealthConditionId);
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key} Error: {error.ErrorMessage}");
                    }
                }
                return View(userHealthCondition);
            
            
        }
        // GET: UserHealthConditionController/Edit/5
        /*[Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var userHealthCondition = await _context.UserHealthConditions.FindAsync(id);
            if (userHealthCondition == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userHealthCondition.UserId);
            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", userHealthCondition.HealthConditionId);
            return View(userHealthCondition);
        }

        // POST: UserHealthConditionController/Edit/5
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,HealthConditionId")] UserHealthCondition userHealthCondition)
        {
            if (id != userHealthCondition.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userHealthCondition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),, new { id = id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.UserHealthConditions.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userHealthCondition.UserId);
            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", userHealthCondition.HealthConditionId);
            return View(userHealthCondition);
        }*/
        [Authorize(Roles = "Admin")]
        // GET: UserHealthConditionController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var userHealthCondition = await _context.UserHealthConditions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userHealthCondition == null)
                return NotFound();

            return View(userHealthCondition);
        }

        [Authorize(Roles = "Admin")]
        // POST: UserHealthConditionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userHealthCondition = await _context.UserHealthConditions.FindAsync(id);
            if (userHealthCondition != null)
            {
                _context.UserHealthConditions.Remove(userHealthCondition);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
