using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace HealthConditionForecast.Controllers
{
    public class UserSymptomSelectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserSymptomSelectionController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: UserSymptomSelectionController
        public async Task<IActionResult> Index()
        {
            var userSymptomSelections = await _context.UserSymptomSelections.Include(us => us.ArthritisSymptoms)
                .Include(us=>us.MigraineSymptoms).Include(us => us.SinusSymptoms).ToListAsync();

            return View(userSymptomSelections);  // pass the list to the view
        }

        // GET: UserSymptomSelectionController/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var selection = await _context.UserSymptomSelections
                .Include(s => s.ArthritisSymptoms)
                .Include(s => s.MigraineSymptoms)
                .Include(s => s.SinusSymptoms)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (selection == null)
                return NotFound();

            return View(selection);
        }

        public IActionResult SelectSymptoms(int userHealthConditionId)
        {
            UserHealthCondition userHealthCondition = _context.UserHealthConditions
                .FirstOrDefault(uhc => uhc.Id == userHealthConditionId);
            // Find the selected health condition to check its type (Name or some discriminator)
            var healthCondition = _context.HealthConditions
                .FirstOrDefault(h => h.Id == userHealthCondition.HealthConditionId);

            if (healthCondition == null)
                return NotFound();

            //ViewData["HealthConditionId"] = healthCondition.Id;
            ViewData["userHealthConditionId"] = userHealthConditionId;

            // Based on the health condition name, load only related symptoms
            if (healthCondition.Name.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.BeforeHeadache=new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s=>s.Type.ToString().Equals("BeforeHeadache")), "Id", "Name");
                ViewBag.MigraineWithAura = new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("MigraineWithAura")), "Id", "Name");
                ViewBag.DuringAttack = new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("DuringAttack")), "Id", "Name");
                /*ViewBag.Symptoms = new MultiSelectList(
                    _context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id),
                    "Id", "Name");*/
                ViewBag.ConditionType = "Migraine";
            }
            else if (healthCondition.Name.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Major= new MultiSelectList(_context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("Major")), "Id", "Name");
                ViewBag.Minor = new MultiSelectList(_context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("Minor")), "Id", "Name");
               /* ViewBag.Symptoms = new MultiSelectList(
                    _context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition.Id),
                    "Id", "Name");*/
                ViewBag.ConditionType = "Sinus";
            }
            else if (healthCondition.Name.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Symptoms = new MultiSelectList(
                    _context.ArthritisSymtoms.Where(s => s.HealthConditionId == healthCondition.Id),
                    "Id", "Name");
                ViewBag.ConditionType = "Arthritis";
            }
            else
            {
                ViewBag.Symptoms = new MultiSelectList(Enumerable.Empty<object>());
                ViewBag.ConditionType = "None";
            }

            return View();
        }

        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> SelectSymptoms(int healthConditionId, List<long> selectedSymptoms)
         {
             if (selectedSymptoms == null || selectedSymptoms.Count == 0)
             {
                 ModelState.AddModelError("", "Please select at least one symptom.");
             }

             if (ModelState.IsValid)
             {
                 var selection = new UserSymptomSelection
                 {
                     UserHealthConditionId = healthConditionId
                 };

                 // Based on the condition type, assign symptoms to correct property
                 var healthCondition = await _context.HealthConditions.FirstOrDefaultAsync(h => h.Id == healthConditionId);
                 if (healthCondition == null)
                     return NotFound();

                 if (healthCondition.Name.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
                 {
                     selection.MigraineSymptoms = await _context.MigraineSymptons
                         .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                     //selection.MigraineSymptoms = selectedSymptoms;
                 }
                 else if (healthCondition.Name.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
                 {
                     selection.SinusSymptoms = await _context.SinusSymptoms
                         .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                     //selection.MigraineSymptoms = selectedSymptoms;
                 }
                 else if (healthCondition.Name.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
                 {
                     selection.ArthritisSymptoms = await _context.ArthritisSymtoms
                         .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                    // selection.MigraineSymptoms = selectedSymptoms;
                 }

                 _context.UserSymptomSelections.Add(selection);
                 await _context.SaveChangesAsync();

                 return RedirectToAction("Index");
             }

             // Reload symptoms if validation failed
             return await Create(healthConditionId);
         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectSymptoms(int userHealthConditionId, List<long> selectedSymptoms)
        {
            if (selectedSymptoms == null || selectedSymptoms.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one symptom.");
            }

            var userHealthCondition = await _context.UserHealthConditions
                .FirstOrDefaultAsync(uhc => uhc.Id == userHealthConditionId);
            var healthCondition = _context.HealthConditions
                .FirstOrDefaultAsync(h => h.Id == userHealthCondition.HealthConditionId);

            if (userHealthCondition == null)
            {
                return NotFound("UserHealthCondition not found for the provided healthConditionId.");
            }

            if (ModelState.IsValid)
            {
                var selection = new UserSymptomSelection
                {
                    UserHealthConditionId = (int)userHealthCondition.Id
                     
                    
                };
                selection.MigraineSymptoms=new List<MigraineSympton>();
                selection.SinusSymptoms = new List<SinusSymptom>();
                selection.ArthritisSymptoms = new List<ArthritisSymtom>();

                string conditionName = healthCondition.Result.Name;

                // Populate only the relevant symptom list
                if (conditionName.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
                {
                    selection.MigraineSymptoms = await _context.MigraineSymptons
                        .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                    //add a condition for the MifgraineForecast, SinusForecst and ArthritisForcast if it is elligible for the forecast to happen
                    _context.UserSymptomSelections.Add(selection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ForecastMigraine","Forecast");

                }
                else if (conditionName.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
                {
                    selection.SinusSymptoms = await _context.SinusSymptoms
                        .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                    _context.UserSymptomSelections.Add(selection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ForecastSinus", "Forecast");
                }
                else if (conditionName.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
                {
                    selection.ArthritisSymptoms = await _context.ArthritisSymtoms
                        .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                    _context.UserSymptomSelections.Add(selection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ForecastArthritis", "Forecast");
                }

               
            }

            // Reload symptom list for redisplay if validation fails
            var healthCondition1 = await _context.HealthConditions.FindAsync(healthCondition.Id);
            ViewData["HealthConditionId"] = healthCondition.Id;
            ViewBag.ConditionType = healthCondition1?.Name;

            if (healthCondition != null)
            {
                if (healthCondition.Result.Name.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Symptoms = new MultiSelectList(
                        _context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition1.Id),
                        "Id", "Name", selectedSymptoms);
                }
                else if (healthCondition.Result.Name.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Symptoms = new MultiSelectList(
                        _context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition1.Id),
                        "Id", "Name", selectedSymptoms);
                }
                else if (healthCondition.Result.Name.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Symptoms = new MultiSelectList(
                        _context.ArthritisSymtoms.Where(s => s.HealthConditionId == healthCondition1.Id),
                        "Id", "Name", selectedSymptoms);
                }
            }

            return View();
        }


        // GET: UserSymptomSelectionController/Create
        public async Task<IActionResult> Create(int? userHealthConditionId)
        {
            if (userHealthConditionId == null)
                return NotFound();

            ViewData["UserHealthConditionId"] = userHealthConditionId;
            HealthCondition healthCondition = _context.HealthConditions.FirstOrDefault(u=>u.Id == userHealthConditionId);
            if(healthCondition.Name=="Migraine Headache")
            {
                ViewBag.MigraineSymptoms = new MultiSelectList(_context.MigraineSymptons, "Id", "Name");
            }
            else if (healthCondition.Name == "Arthritis")
            {
                ViewBag.ArthritisSymptoms = new MultiSelectList(_context.ArthritisSymtoms, "Id", "Name");
            }
            else if (healthCondition.Name == "Sinus Headache")
            {
                ViewBag.SinusSymptoms = new MultiSelectList(_context.SinusSymptoms, "Id", "Name");
            }

            return View();
        }

        // POST: UserSymptomSelectionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( UserSymptomSelection selection,
        List<long> selectedArthritisSymptoms,
        List<long> selectedMigraineSymptoms,
        List<long> selectedSinusSymptoms)
        {
            if (ModelState.IsValid)
            {
                selection.ArthritisSymptoms = await _context.ArthritisSymtoms
                    .Where(s => selectedArthritisSymptoms.Contains(s.Id)).ToListAsync();

                selection.MigraineSymptoms = await _context.MigraineSymptons
                    .Where(s => selectedMigraineSymptoms.Contains(s.Id)).ToListAsync();

                selection.SinusSymptoms = await _context.SinusSymptoms
                    .Where(s => selectedSinusSymptoms.Contains(s.Id)).ToListAsync();

                _context.Add(selection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Rebuild selection lists if model state is invalid
            ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
            ViewBag.ArthritisSymptoms = new MultiSelectList(_context.ArthritisSymtoms, "Id", "Name", selectedArthritisSymptoms);
            ViewBag.MigraineSymptoms = new MultiSelectList(_context.MigraineSymptons, "Id", "Name", selectedMigraineSymptoms);
            ViewBag.SinusSymptoms = new MultiSelectList(_context.SinusSymptoms, "Id", "Name", selectedSinusSymptoms);

            return View(selection);
        }

        // GET: UserSymptomSelectionController/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var selection = await _context.UserSymptomSelections
                .Include(s => s.ArthritisSymptoms)
                .Include(s => s.MigraineSymptoms)
                .Include(s => s.SinusSymptoms)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (selection == null)
                return NotFound();

            ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
            ViewBag.ArthritisSymptoms = new MultiSelectList(_context.ArthritisSymtoms, "Id", "Name", selection.ArthritisSymptoms.Select(s => s.Id));
            ViewBag.MigraineSymptoms = new MultiSelectList(_context.MigraineSymptons, "Id", "Name", selection.MigraineSymptoms.Select(s => s.Id));
            ViewBag.SinusSymptoms = new MultiSelectList(_context.SinusSymptoms, "Id", "Name", selection.SinusSymptoms.Select(s => s.Id));

            return View(selection);
        }

        // POST: UserSymptomSelectionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    long id,
    [Bind("Id,UserHealthConditionId")] UserSymptomSelection selection,
    List<long> selectedArthritisSymptoms,
    List<long> selectedMigraineSymptoms,
    List<long> selectedSinusSymptoms)
        {
            if (id != selection.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingSelection = await _context.UserSymptomSelections
                    .Include(s => s.ArthritisSymptoms)
                    .Include(s => s.MigraineSymptoms)
                    .Include(s => s.SinusSymptoms)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (existingSelection == null)
                    return NotFound();

                // Update navigation properties
                existingSelection.ArthritisSymptoms = await _context.ArthritisSymtoms
                    .Where(s => selectedArthritisSymptoms.Contains(s.Id)).ToListAsync();

                existingSelection.MigraineSymptoms = await _context.MigraineSymptons
                    .Where(s => selectedMigraineSymptoms.Contains(s.Id)).ToListAsync();

                existingSelection.SinusSymptoms = await _context.SinusSymptoms
                    .Where(s => selectedSinusSymptoms.Contains(s.Id)).ToListAsync();

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid
            ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
            ViewBag.ArthritisSymptoms = new MultiSelectList(_context.ArthritisSymtoms, "Id", "Name", selectedArthritisSymptoms);
            ViewBag.MigraineSymptoms = new MultiSelectList(_context.MigraineSymptons, "Id", "Name", selectedMigraineSymptoms);
            ViewBag.SinusSymptoms = new MultiSelectList(_context.SinusSymptoms, "Id", "Name", selectedSinusSymptoms);

            return View(selection);
        }

        // GET: UserSymptomSelectionController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var selection = await _context.UserSymptomSelections
                .FirstOrDefaultAsync(s => s.Id == id);

            if (selection == null)
                return NotFound();

            return View(selection);
        }

        // POST: UserSymptomSelectionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var selection = await _context.UserSymptomSelections
                .Include(s => s.ArthritisSymptoms)
                .Include(s => s.MigraineSymptoms)
                .Include(s => s.SinusSymptoms)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (selection != null)
            {
                // Optional: clear relations before deletion
                /*selection.ArthritisSymptoms.Clear();
                selection.MigraineSymptoms.Clear();
                selection.SinusSymptoms.Clear();*/

                _context.UserSymptomSelections.Remove(selection);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
