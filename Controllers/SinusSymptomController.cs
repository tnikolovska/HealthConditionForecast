using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Controllers
{
    public class SinusSymptomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinusSymptomController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,User")]
        // GET: SinusSymptomController
        public async Task<IActionResult> Index()
        {
            ViewData["HealthCondition"] = "Sinus Headache";
            var sinusSymptoms = await _context.SinusSymptoms.ToListAsync();

            return View(sinusSymptoms);
        }
        // GET: SinusSymptomController/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<SinusSymptom>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (symptom == null)
                return NotFound();
            ViewData["HealthCondition"] = "Sinus Headache";
            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // GET: SinusSymptomController/Create
        public IActionResult Create()
        {
            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name");
            ViewData["SinusType"] = new SelectList(Enum.GetValues(typeof(SinusType)));
            return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: SinusSymptomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,HealthConditionId,Type")] SinusSymptom symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition.Name == "Sinus Headache")
            {


                if (ModelState.IsValid)
                {
                    _context.SinusSymptoms.Add(symptom);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
            }

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            ViewData["SinusType"] = new SelectList(Enum.GetValues(typeof(SinusType)));
            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // GET: SinusSymptomController/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<SinusSymptom>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (symptom == null)
                return NotFound();

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            ViewData["SinusType"] = new SelectList(Enum.GetValues(typeof(SinusType)));

            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,HealthConditionId,Type")] SinusSymptom symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition.Name == "Sinus Headache")
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
                        if (!_context.Symptoms.OfType<SinusSymptom>().Any(e => e.Id == id))
                            return NotFound();
                        else
                            throw;
                    }
            }
            }

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            ViewData["SinusType"] = new SelectList(Enum.GetValues(typeof(SinusType)));

            return View(symptom);
        }

        [Authorize(Roles = "Admin")]
        // GET: SinusSymptomController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<SinusSymptom>()
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewData["HealthCondition"] = "Sinus Headache";

            if (symptom == null)
                return NotFound();

            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // POST: SinusSymptomController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var symptom = await _context.Symptoms
                .OfType<SinusSymptom>()
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
