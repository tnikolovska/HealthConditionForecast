using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Controllers
{
    public class HealthConditionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthConditionController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: HealthConditionController
        /* public ActionResult Index()
         {
             var healthConditions = _context.HealthConditions.ToList();
             //return View(healthConditions);
             return View();
         }*/
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var healthConditions = await _context.HealthConditions
         
                .ToListAsync();

            return View(healthConditions);  // pass the list to the view
        }
        [Authorize(Roles = "Admin,User")]
        // GET: HealthConditionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
                return NotFound();

            var healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(h => h.Id == id);
            if (healthCondition == null)
                return NotFound();
            return View(healthCondition);
        }
        [Authorize(Roles = "Admin")]
        // GET: HealthConditionController/Create
        public ActionResult Create()
        {
            
                return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: HealthConditionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var healthCondition = new HealthCondition
                    {
                        Name = collection["Name"],
                        Description = collection["Description"]
                    };
                    _context.HealthConditions.Add(healthCondition);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                //return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        // GET: HealthConditionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
                return NotFound();

            var healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(h => h.Id == id);
            if (healthCondition == null)
                return NotFound();
            return View(healthCondition);
        }
        [Authorize(Roles = "Admin")]
        // POST: HealthConditionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("Id,Name,Description")] HealthCondition healthCondition)
        {
                if (id != healthCondition.Id)
                    return NotFound();
                if (ModelState.IsValid)
                {
                    try
                    {
                    var conditionToUpdate = _context.HealthConditions.FirstOrDefault(h => h.Id == id);

                        if (conditionToUpdate == null)
                            return NotFound();

                    // Update basic fields
                    conditionToUpdate.Name = healthCondition.Name;
                    conditionToUpdate.Description = healthCondition.Description;
                    await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.HealthConditions.Any(e => e.Id == healthCondition.Id))
                            return NotFound();
                        else
                            throw;
                    }


            }
            foreach (var kvp in ModelState)
            {
                foreach (var error in kvp.Value.Errors)
                {
                    Console.WriteLine($"Field: {kvp.Key} - Error: {error.ErrorMessage}");
                }
            }


            return View(healthCondition);
        }
        [Authorize(Roles = "Admin")]
        // GET: HealthConditionController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var healthCondition = await _context.HealthConditions
            .FirstOrDefaultAsync(h => h.Id == id);
            if (healthCondition == null)
                return NotFound();
            return View(healthCondition);
        }
        [Authorize(Roles = "Admin")]
        // POST: HealthConditionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            var healthCondition = await _context.HealthConditions
          .FirstOrDefaultAsync(h => h.Id == id);
            if (healthCondition == null)
                return NotFound();

            _context.HealthConditions.Remove(healthCondition);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
