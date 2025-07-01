using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Controllers
{
    public class SymptomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SymptomController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: SymptomController
        public async Task<IActionResult> Index()
        {
            var symptoms = await _context.Symptoms.ToListAsync();

            return View(symptoms);
        }

        // GET: SymptomController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms.FirstOrDefaultAsync(s => s.Id == id);
            if (symptom == null)
                return NotFound();
            return View(symptom);
        }

        // GET: SymptomController/Create
        public ActionResult Create(long? healthConditionId)
        {
            if (healthConditionId == null)
                return BadRequest("HealthConditionId is required");
            ViewBag.HealthConditionId = healthConditionId;
            return View();
        }

        // POST: SymptomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Name,Description,HealthConditionId")] Symptom symptom)
        {
            
                if (ModelState.IsValid)
                {
                    _context.Symptoms.Add(symptom);
                    _context.SaveChangesAsync();
                    return RedirectToAction("Details", "HealthCondition", new { id = symptom.HealthConditionId });
                }
                //return RedirectToAction(nameof(Index));
                return View(symptom);
            
        }

        // GET: SymptomController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms.FirstOrDefaultAsync(s => s.Id == id);
            if (symptom == null)
                return NotFound();
            return View(symptom);
        }

        // POST: SymptomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Symptom symptom)
        {
            if (id != symptom.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var symptomToUpdate = _context.Symptoms.FirstOrDefault(s => s.Id == id);

                    if (symptomToUpdate == null)
                        return NotFound();

                    // Update basic fields
                    symptomToUpdate.Name = symptom.Name;
                    symptomToUpdate.Description = symptom.Description;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Symptoms.Any(e => e.Id == symptom.Id))
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


            return View(symptom);
        }

        // GET: SymptomController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var symptom = await _context.Symptoms
            .FirstOrDefaultAsync(s => s.Id == id);
            if (symptom == null)
                return NotFound();
            return View(symptom);
        }

        // POST: SymptomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
           var symptom = await _context.Symptoms
          .FirstOrDefaultAsync(s => s.Id == id);
            if (symptom == null)
                return NotFound();

            _context.Symptoms.Remove(symptom);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
