using HealthConditionForecast.Data;
using HealthConditionForecast.Helpers;
using HealthConditionForecast.Migrations;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace HealthConditionForecast.Controllers
{
    public class UserSymptomSelectionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserSymptomSelectionController> _logger;

        public UserSymptomSelectionController(ILogger<UserSymptomSelectionController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Authorize(Roles = "Admin,User")]
        // GET: UserSymptomSelectionController
        public async Task<IActionResult> Index()
        {
            var userId= User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userHealthConditionIds = await _context.UserHealthConditions
            .Where(u => u.UserId == userId)
            .Select(u => u.Id)
            .ToListAsync();
             var userSymptomSelections = await _context.UserSymptomSelections
            .Include(us => us.ArthritisSymptoms)
            .Include(us => us.MigraineSymptoms)
            .Include(us => us.SinusSymptoms)
            .Where(us => userHealthConditionIds.Contains(us.UserHealthConditionId))
            .ToListAsync();
            return View(userSymptomSelections);  // pass the list to the view
        }

        // GET: UserSymptomSelectionController/Details/5
        [Authorize(Roles = "Admin,User")]
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



        public IActionResult ValidationMessage(int userHealthConditionId)
        {
            var message = TempData["ValidationMessage"] as string;
            ViewBag.userHealthConditionId = userHealthConditionId;
            if (string.IsNullOrEmpty(message))
            {
                // No message, redirect back or somewhere
                return RedirectToAction("Index");
            }
            ViewBag.Message = message;
            return View();
        }



        [Authorize(Roles = "Admin,User")]
        public IActionResult SelectSymptoms(int userHealthConditionId)
        {
            var stopwatch = Stopwatch.StartNew();
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
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 1500)
            {
                _logger.LogWarning("SelectSymptoms (GET/POST) exceeded 1.5s: {0} ms", stopwatch.ElapsedMilliseconds);
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
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectSymptoms(int userHealthConditionId, List<long> selectedSymptoms)
        {
            var stopwatch = Stopwatch.StartNew();
            string validationMessage = "";

             if (selectedSymptoms == null || selectedSymptoms.Count == 0)
             {
                 ModelState.AddModelError("", "Please select at least one symptom.");
             }

             var userHealthCondition = await _context.UserHealthConditions
                 .FirstOrDefaultAsync(uhc => uhc.Id == userHealthConditionId);
            /*var healthCondition = _context.HealthConditions
                .FirstOrDefaultAsync(h => h.Id == userHealthCondition.HealthConditionId);*/

            var healthCondition = await _context.HealthConditions
                 .FirstOrDefaultAsync(h => h.Id == userHealthCondition.HealthConditionId);

            if (healthCondition == null) {
                return NotFound("HealthCondition not found for the provided userHealthCondition.");
            }
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
                 selection.MigraineSymptoms = new List<MigraineSympton>();
                 selection.SinusSymptoms = new List<SinusSymptom>();
                 selection.ArthritisSymptoms = new List<ArthritisSymtom>();

                 string conditionName = healthCondition.Name;

                 // Populate only the relevant symptom list
                 if (conditionName.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
                 {
                     selection.MigraineSymptoms = await _context.MigraineSymptons
                         .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                    if (selection.MigraineSymptoms == null || !selection.MigraineSymptoms.Any()) {

                        validationMessage = "⚠️ No matching migraine symptoms found in the database.";
                        TempData["ValidationMessage"] = validationMessage;
                        return RedirectToAction("ValidationMessage", new { userHealthConditionId });

                    }
                    if (selection.MigraineSymptoms == null)
                    {
                        throw new Exception("MigraineSymptoms is null!");
                    }
                    foreach (var m in selection.MigraineSymptoms)
                    {
                        if (m == null)
                        {
                            throw new Exception("MigraineSympton entry is null!");
                        }
                        if (m.Type == null)
                        {
                            throw new Exception("MigraineSympton Type is null!");
                        }
                    }
                    if ((selection.MigraineSymptoms.Count(m => m.Type == Models.MigraineType.BeforeHeadache)) > 0 ^ (selection.MigraineSymptoms.Count(m => m.Type == Models.MigraineType.MigraineWithAura)) > 0 ^ (selection.MigraineSymptoms.Count(m => m.Type == Models.MigraineType.DuringAttack)) > 0)
                     {
                         _context.UserSymptomSelections.Add(selection);
                         await _context.SaveChangesAsync();
                         TempData["CanShowForecast"] = "yes";
                         return RedirectToAction("ForecastMigraine", "Forecast");
                     }
                     else {
                         validationMessage = "⚠️ Please select at least one symptom from any of the migraine categories (Before Headache, Migraine With Aura, or During Attack) to proceed.";
                         TempData["ValidationMessage"] = validationMessage;
                         return RedirectToAction("ValidationMessage",new { userHealthConditionId = userHealthConditionId });
                     }

                 }
                 else if (conditionName.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
                 {

                     selection.SinusSymptoms = await _context.SinusSymptoms
                    .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                     if (((selection.SinusSymptoms.Count(s => s.Type == Models.SinusType.Major)) >= 1 && (selection.SinusSymptoms.Count(s => s.Type == Models.SinusType.Minor)) == 2) || (selection.SinusSymptoms.Count(s => s.Type == Models.SinusType.Major)) == 2)
                     {
                         _context.UserSymptomSelections.Add(selection);
                         await _context.SaveChangesAsync();
                         TempData["CanShowForecast"] = "yes";
                         return RedirectToAction("ForecastSinus", "Forecast");
                     }
                     else {
                         validationMessage = "⚠️ Please select at least 2 major symptoms or 1 major and 2 minor sinus symptoms.";
                         TempData["ValidationMessage"] = validationMessage;
                         return RedirectToAction("ValidationMessage", new { userHealthConditionId = userHealthConditionId });
                     }

                 }
                 else if (conditionName.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
                 {

                     selection.ArthritisSymptoms = await _context.ArthritisSymtoms
                     .Where(s => selectedSymptoms.Contains(s.Id)).ToListAsync();
                     if (selection.ArthritisSymptoms.Count() > 0)
                     {
                         _context.UserSymptomSelections.Add(selection);
                         await _context.SaveChangesAsync();
                         TempData["CanShowForecast"] = "yes";
                         return RedirectToAction("ForecastArthritis", "Forecast");
                     }
                     else {
                         validationMessage = "⚠️ Please select at least one arthritis symptom.";
                         TempData["ValidationMessage"] = validationMessage;
                        stopwatch.Stop();
                        if (stopwatch.ElapsedMilliseconds > 1500)
                        {
                            _logger.LogWarning("SelectSymptoms (GET/POST) exceeded 1.5s: {0} ms", stopwatch.ElapsedMilliseconds);
                        }
                        return RedirectToAction("ValidationMessage", new { userHealthConditionId = userHealthConditionId });
                     }

                 }


             }
             else { 
                 return RedirectToAction("SelectSymptoms", "UserSymptomSelection", new { userHealthConditionId = userHealthConditionId });
             }

             return RedirectToAction("SelectSymptoms", "UserSymptomSelection", new { userHealthConditionId = userHealthConditionId });

        }


        //new code

        /*void UpdateSymptoms<T>(
        ICollection<T> existing, List<T> news, Func<T, long> idSelector)
        {
            var toRemove = existing.Where(e => !news.Any(n => idSelector(n) == idSelector(e))).ToList();
            foreach (var rem in toRemove) existing.Remove(rem);

            var existingIds = existing.Select(idSelector).ToHashSet();
            foreach (var ni in news)
                if (!existingIds.Contains(idSelector(ni)))
                    existing.Add(ni);
        }*/


        /*[HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> SelectSymptoms(int userHealthConditionId, List<long> selectedSymptoms)
        {
            if (selectedSymptoms == null || !selectedSymptoms.Any())
            {
                TempData["ValidationMessage"] = "⚠️ Please select at least one symptom.";
                return RedirectToAction(nameof(SelectSymptoms), new { userHealthConditionId });
            }

            var uhc = await _context.UserHealthConditions.FindAsync((long)userHealthConditionId);
            if (uhc == null) return NotFound();

            var hc = await _context.HealthConditions.FindAsync((long)uhc.HealthConditionId);
            if (hc == null) return NotFound();

            var selection = await _context.UserSymptomSelections
                .Include(s => s.MigraineSymptoms)
                .Include(s => s.SinusSymptoms)
                .Include(s => s.ArthritisSymptoms)
                .FirstOrDefaultAsync(s => s.UserHealthConditionId == userHealthConditionId);

            if (selection == null)
            {
                selection = new UserSymptomSelection
                {
                    UserHealthConditionId = userHealthConditionId,
                    MigraineSymptoms = new List<MigraineSympton>(),
                    SinusSymptoms = new List<SinusSymptom>(),
                    ArthritisSymptoms = new List<ArthritisSymtom>()
                };
                _context.UserSymptomSelections.Add(selection);
            }

            TempData["CanShowForecast"] = "yes";

            if (hc.Name.Contains("Migraine", StringComparison.OrdinalIgnoreCase))
            {
                var newList = await _context.MigraineSymptons
                    .Where(s => selectedSymptoms.Contains(s.Id))
                    .ToListAsync();

                foreach (var symptom in newList)
                {
                    if (!selection.MigraineSymptoms.Any(s => s.Id == symptom.Id))
                        selection.MigraineSymptoms.Add(symptom);
                }

                if (!selection.MigraineSymptoms.Any())
                {
                    TempData["ValidationMessage"] = "⚠️ Please select at least one migraine symptom.";
                    return RedirectToAction("ValidationMessage", new { userHealthConditionId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ForecastMigraine", "Forecast");
            }
            else if (hc.Name.Contains("Sinus", StringComparison.OrdinalIgnoreCase))
            {
                var newList = await _context.SinusSymptoms
                    .Where(s => selectedSymptoms.Contains(s.Id))
                    .ToListAsync();

                foreach (var symptom in newList)
                {
                    if (!selection.SinusSymptoms.Any(s => s.Id == symptom.Id))
                        selection.SinusSymptoms.Add(symptom);
                }

                if (selection.SinusSymptoms.Count(s => s.Type.ToString() == "Major") < 1 ||
                    selection.SinusSymptoms.Count(s => s.Type.ToString() == "Minor") < 2)
                {
                    TempData["ValidationMessage"] = "⚠️ Please select at least 1 major and 2 minor sinus symptoms.";
                    return RedirectToAction("ValidationMessage", new { userHealthConditionId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ForecastSinus", "Forecast");
            }
            else if (hc.Name.Contains("Arthritis", StringComparison.OrdinalIgnoreCase))
            {
                var newList = await _context.ArthritisSymtoms
                    .Where(s => selectedSymptoms.Contains(s.Id))
                    .ToListAsync();

                foreach (var symptom in newList)
                {
                    if (!selection.ArthritisSymptoms.Any(s => s.Id == symptom.Id))
                        selection.ArthritisSymptoms.Add(symptom);
                }

                if (!selection.ArthritisSymptoms.Any())
                {
                    TempData["ValidationMessage"] = "⚠️ Please select at least one arthritis symptom.";
                    return RedirectToAction("ValidationMessage", new { userHealthConditionId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ForecastArthritis", "Forecast");
            }

            TempData["ValidationMessage"] = "⚠️ Invalid health condition.";
            return RedirectToAction("ValidationMessage", new { userHealthConditionId });
        }*/






        /* [Authorize(Roles = "Admin,User")]
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
         [Authorize(Roles = "Admin,User")]
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
         }*/
        /* [Authorize(Roles = "User")]
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
             UserHealthCondition userHealthCondition = _context.UserHealthConditions.FirstOrDefault(u => u.Id == id);
             var user = _context.Users.FirstOrDefault(u => u.Id == userHealthCondition.UserId);
             var healthCondition = _context.HealthConditions.FirstOrDefault(u => u.Id == userHealthCondition.HealthConditionId);
             if (user != null) {
                 ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
                 ViewData["userHealthConditionName"] = healthCondition.Name;
                 if (healthCondition.Name == "Arthritis"){
                     ViewBag.Symptoms = new MultiSelectList(
                     _context.ArthritisSymtoms.Where(s => s.HealthConditionId == healthCondition.Id),
                     "Id", "Name");

                 }
                 if (healthCondition.Name == "Migraine Headache")
                 {
                     ViewBag.BeforeHeadache = new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("BeforeHeadache")), "Id", "Name");
                     ViewBag.MigraineWithAura = new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("MigraineWithAura")), "Id", "Name");
                     ViewBag.DuringAttack = new MultiSelectList(_context.MigraineSymptons.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("DuringAttack")), "Id", "Name");
                 }
                 if (healthCondition.Name == "Sinus Headache") {
                     ViewBag.Major = new MultiSelectList(_context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("Major")), "Id", "Name");
                     ViewBag.Minor = new MultiSelectList(_context.SinusSymptoms.Where(s => s.HealthConditionId == healthCondition.Id).Where(s => s.Type.ToString().Equals("Minor")), "Id", "Name");
                 }

                 return View(selection);
             }
            else return NotFound();
         }
         [HttpPost]
         [ValidateAntiForgeryToken]
         [Authorize(Roles = "User")]
         public async Task<IActionResult> Edit(int id, List<long> selectedSymptoms, List<long> selectedBeforeHeadache, List<long> selectedMigraineWithAura, List<long> selectedDuringAttack, List<long> selectedMajor, List<long> selectedMinor)
         {
             var selection = await _context.UserSymptomSelections
                 .Include(s => s.ArthritisSymptoms)
                 .Include(s => s.MigraineSymptoms)
                 .Include(s => s.SinusSymptoms)
                 .FirstOrDefaultAsync(s => s.Id == id);

             if (selection == null)
                 return NotFound();

             var userHealthCondition = await _context.UserHealthConditions.FindAsync((long)selection.UserHealthConditionId);
             var healthCondition = await _context.HealthConditions.FindAsync((long)userHealthCondition.HealthConditionId);

             if (healthCondition == null)
                 return NotFound();

             try
             {
                 if (healthCondition.Name == "Arthritis")
                 {
                     selection.ArthritisSymptoms = await _context.ArthritisSymtoms
                         .Where(s => selectedSymptoms.Contains(s.Id))
                         .ToListAsync();
                 }
                 else if (healthCondition.Name == "Migraine Headache")
                 {
                     selection.MigraineSymptoms = await _context.MigraineSymptons
                         .Where(s => selectedBeforeHeadache.Contains(s.Id)
                                  || selectedMigraineWithAura.Contains(s.Id)
                                  || selectedDuringAttack.Contains(s.Id))
                         .ToListAsync();
                 }
                 else if (healthCondition.Name == "Sinus Headache")
                 {
                     selection.SinusSymptoms = await _context.SinusSymptoms
                         .Where(s => selectedMajor.Contains(s.Id)
                                  || selectedMinor.Contains(s.Id))
                         .ToListAsync();
                 }

                 _context.Update(selection);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Details),new { id=id}); // or Details or UserProfile
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("", "Error saving selection: " + ex.Message);
             }

             // Repopulate dropdowns if model state is invalid or save failed
             ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
             ViewData["userHealthConditionName"] = healthCondition.Name;

             if (healthCondition.Name == "Arthritis")
             {
                 ViewBag.Symptoms = new MultiSelectList(
                     _context.ArthritisSymtoms.Where(s => s.HealthConditionId == healthCondition.Id),
                     "Id", "Name", selectedSymptoms);
             }
             else if (healthCondition.Name == "Migraine Headache")
             {
                 ViewBag.BeforeHeadache = new MultiSelectList(
                     _context.MigraineSymptons.Where(s => s.Type.ToString() == "BeforeHeadache"),
                     "Id", "Name", selectedBeforeHeadache);

                 ViewBag.MigraineWithAura = new MultiSelectList(
                     _context.MigraineSymptons.Where(s => s.Type.ToString() == "MigraineWithAura"),
                     "Id", "Name", selectedMigraineWithAura);

                 ViewBag.DuringAttack = new MultiSelectList(
                     _context.MigraineSymptons.Where(s => s.Type.ToString() == "DuringAttack"),
                     "Id", "Name", selectedDuringAttack);
             }
             else if (healthCondition.Name == "Sinus Headache")
             {
                 ViewBag.Major = new MultiSelectList(
                     _context.SinusSymptoms.Where(s => s.Type.ToString() == "Major"),
                     "Id", "Name", selectedMajor);

                 ViewBag.Minor = new MultiSelectList(
                     _context.SinusSymptoms.Where(s => s.Type.ToString() == "Minor"),
                     "Id", "Name", selectedMinor);
             }

             return View(selection);
         }

         /* [Authorize(Roles = "User")]
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
                  //return RedirectToAction(nameof(Details),new { id = id});
                  if (selectedArthritisSymptoms.Count > 0) {
                      return RedirectToAction("ForecastArthritis", "Forecast");
                  }
                  else if (selectedMigraineSymptoms.Count > 0)
                  {
                      return RedirectToAction("ForecastMigraine", "Forecast");
                  }
                  else if (selectedSinusSymptoms.Count > 0)
                  {
                      return RedirectToAction("ForecastSinus", "Forecast");
                  }
              }

             // If (modeState is invalid {
              ViewData["UserHealthConditionId"] = selection.UserHealthConditionId;
              ViewBag.ArthritisSymptoms = new MultiSelectList(_context.ArthritisSymtoms, "Id", "Name", selectedArthritisSymptoms);
              ViewBag.MigraineSymptoms = new MultiSelectList(_context.MigraineSymptons, "Id", "Name", selectedMigraineSymptoms);
              ViewBag.SinusSymptoms = new MultiSelectList(_context.SinusSymptoms, "Id", "Name", selectedSinusSymptoms); */

        /* return View(selection);
     }*/
        // }
        //[Authorize(Roles = "Admin")]
        // GET: UserSymptomSelectionController/Delete/5
        /*public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var selection = await _context.UserSymptomSelections.Include(s => s.ArthritisSymptoms).Include(s => s.MigraineSymptoms).Include(s => s.SinusSymptoms)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (selection == null)
                return NotFound();

            return View(selection);
        }*/
        /*[Authorize(Roles = "Admin")]
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

        /* _context.UserSymptomSelections.Remove(selection);
         await _context.SaveChangesAsync();
     }

     return RedirectToAction(nameof(Index));
 }*/

    }
}
