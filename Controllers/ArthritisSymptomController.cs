using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Controllers
{
    public class ArthritisSymptomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArthritisSymptomController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: ArthritisSymptomController
        public async Task<IActionResult> Index()
        {
            ViewData["HealthCondition"] = "Arthritis";
            var arthritisSymptoms = await _context.ArthritisSymtoms.ToListAsync();

            return View(arthritisSymptoms);
        }

        // GET: ArthritisSymptomController/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<ArthritisSymtom>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (symptom == null)
                return NotFound();
            ViewData["HealthCondition"] = "Arthritis";
            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // GET: ArthritisSymptomController/Create
        public IActionResult Create()
        {
            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: ArthritisSymptomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,HealthConditionId")] ArthritisSymtom symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition != null) {
                if (healthCondition.Name == "Arthritis")
                {
                    if (ModelState.IsValid)
                    {
                        _context.ArthritisSymtoms.Add(symptom);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");

                    }
                }

                ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
                return View(symptom);
            }
            else return RedirectToAction("Create", "ArthritisSymptom");
        }
        [Authorize(Roles = "Admin")]
        // GET: ArthritisSymptomController/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<ArthritisSymtom>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (symptom == null)
                return NotFound();

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // POST: ArthritisSymptomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,HealthConditionId")] ArthritisSymtom symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition.Name == "Arthritis")
            { 


                if (id != symptom.Id)
                        return NotFound();

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(symptom);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!_context.Symptoms.OfType<ArthritisSymtom>().Any(e => e.Id == id))
                                return NotFound();
                            else
                                throw;
                        }
                    }
            }

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);

            return View(symptom);
        }

        [Authorize(Roles = "Admin")]
        // GET: ArthritisSymptomController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<ArthritisSymtom>()
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewData["HealthCondition"] = "Arthritis";

            if (symptom == null)
                return NotFound();

            return View(symptom);
        }

        [Authorize(Roles = "Admin")]
        // POST: ArthritisSymptomController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var symptom = await _context.Symptoms
                .OfType<ArthritisSymtom>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (symptom != null)
            {
                _context.Symptoms.Remove(symptom);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
