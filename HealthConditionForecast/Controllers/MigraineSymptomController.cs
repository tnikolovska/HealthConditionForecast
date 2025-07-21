using HealthConditionForecast.Data;
using HealthConditionForecast.Helpers;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static HealthConditionForecast.Models.MigraineSympton;

namespace HealthConditionForecast.Controllers
{
    public class MigraineSymptomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MigraineSymptomController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: MigraineSymptomController
        public async Task<IActionResult> Index()
        {
            ViewData["HealthCondition"] = "Migraine Headache";
            var migraineSymptoms = await _context.MigraineSymptons.ToListAsync();

            return View(migraineSymptoms);
        }

        // GET: MigraineSymptomController/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<MigraineSympton>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (symptom == null)
                return NotFound();
            ViewData["HealthCondition"] = "Migrane Headache";
            return View(symptom);
        }

        [Authorize(Roles = "Admin")]
        // GET: MigraineSymptomController/Create
        public IActionResult Create()
        {
            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name");
            //ViewData["HealthConditionId"] = healthConditionId;
            ViewData["MigraineType"] = EnumExtensions.ToSelectList<MigraineType>(default);
            //ViewData["MigraineType"] = new SelectList(Enum.GetValues(typeof(MigraineType)));
            return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: MigraineSymptomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,HealthConditionId,Type")] MigraineSympton symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition != null)
            {
                if (healthCondition.Name == "Migraine Headache")
                {

                    if (ModelState.IsValid)
                    {
                        _context.MigraineSymptons.Add(symptom);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");

                    }

                    ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
                    //ViewData["HealthConditionId"] = healthConditionId;
                    ViewData["MigraineType"] = new SelectList(Enum.GetValues(typeof(MigraineType)), symptom.Type);
                    return View(symptom);
                }
                return View(symptom);

            }
            else return RedirectToAction("Create", "MigraineSymptom");

        }
        [Authorize(Roles = "Admin")]
        // GET: MigraineSymptomController/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<MigraineSympton>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (symptom == null)
                return NotFound();

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            //ViewData["MigraineType"] = new SelectList(Enum.GetValues(typeof(MigraineType)), symptom.Type);
            ViewData["MigraineType"] = EnumExtensions.ToSelectList<MigraineType>(default);
            return View(symptom);
        }

        [Authorize(Roles = "Admin")]
        // POST: MigraineSymptomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,HealthConditionId,Type")] MigraineSympton symptom)
        {
            HealthCondition healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(hc => hc.Id == symptom.HealthConditionId);
            if (healthCondition.Name == "Migraine Headache")
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
                        if (!_context.Symptoms.OfType<MigraineSympton>().Any(e => e.Id == id))
                            return NotFound();
                        else
                            throw;
                    }
                }
            }

            ViewData["HealthConditionId"] = new SelectList(_context.HealthConditions, "Id", "Name", symptom.HealthConditionId);
            //ViewData["MigraineType"] = new SelectList(Enum.GetValues(typeof(MigraineType)), symptom.Type);
            ViewData["MigraineType"] = EnumExtensions.ToSelectList<MigraineType>(default);
            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // GET: MigraineSymptomController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var symptom = await _context.Symptoms
                .OfType<MigraineSympton>()
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewData["HealthCondition"] = "Migraine Headache";

            if (symptom == null)
                return NotFound();

            return View(symptom);
        }
        [Authorize(Roles = "Admin")]
        // POST: MigraineSymptomController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var symptom = await _context.Symptoms
                .OfType<MigraineSympton>()
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
