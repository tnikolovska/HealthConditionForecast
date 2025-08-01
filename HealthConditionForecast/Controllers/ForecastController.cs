using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace HealthConditionForecast.Controllers
{
    public class ForecastController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;


        public ForecastController(ApplicationDbContext context)
        { 
            _context = context;
            _httpClient = new HttpClient();
           
        }
        public List<Forecast> ParseJSON(string forecastSearchResults)
        {

            List<Forecast> forecastList = new List<Forecast>();
            if (forecastList != null)
            {
                forecastList.Clear();
            }

            if (!string.IsNullOrEmpty(forecastSearchResults))
            {
                forecastSearchResults = "{ \"DailyIndexValues\": " + forecastSearchResults + " }";
                var rootObject = JObject.Parse(forecastSearchResults);
                var results = rootObject["DailyIndexValues"] as JArray;
                int id = 0;
                foreach (var result in results)
                {
                    var forecast = new Forecast();


                    //forecast.Id = id;
                    forecast.IdForecast = result["ID"]?.ToObject<string>() ?? "0";
                    forecast.Name = result["Name"]?.ToObject<string>() ?? "";
                    var dateString = result["LocalDateTime"]?.ToString();
                    forecast.Date = DateTime.Parse(dateString);
                    forecast.Value = result["Value"]?.ToObject<decimal>() ?? 0;
                    forecast.Category = result["Category"]?.ToObject<string>() ?? "";
                    forecast.CategoryValue = result["CategoryValue"]?.ToObject<int>() ?? 0;
                    forecast.MobileLink = result["MobileLink"]?.ToObject<string>() ?? "";
                    forecast.Link = result["Link"]?.ToObject<string>() ?? "";
                    forecastList.Add(forecast);
                    System.Console.WriteLine(forecast.ToString());
                    id++;
                    //_context.Forecasts.Add(forecast);
                    //_context.SaveChanges();
                }
            }

            return forecastList;
        }
        [Authorize(Roles = "Admin,User")]

        /*public async Task<IActionResult> ForecastMigraine() 
        {
            if (User.Identity.IsAuthenticated)
            {
                if (TempData["CanShowForecast"] as string == "yes")
                {
                    List<Forecast> list = new List<Forecast>();
                    HttpResponseMessage response = await _httpClient.GetAsync("http://dataservice.accuweather.com/indices/v1/daily/5day/227397/27?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz");

                    if (response.IsSuccessStatusCode)
                    {
                        string forecastSearchResults = await response.Content.ReadAsStringAsync();
                        list = ParseJSON(forecastSearchResults);
                        if (list.Any())
                        {
                            _context.Forecasts.AddRange(list);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return View(list.ToList());
                }
                else return RedirectToAction("Create", "UserHealthCondition");
            }
            else
                return Redirect("/Identity/Account/Login");

        }*/
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> ForecastMigraine()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            if (TempData["CanShowForecast"] as string != "yes")
                return RedirectToAction("Create", "UserHealthCondition");

            List<Forecast> list = new List<Forecast>();
            string apiUrl = "http://dataservice.accuweather.com/indices/v1/daily/5day/227397/27?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz";

            var stopwatch = new Stopwatch();
            int retryCount = 0;
            const int maxRetries = 1;

            while (retryCount <= maxRetries)
            {
                stopwatch.Restart();

                try
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(5); // Fail if it takes longer than 5s

                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    stopwatch.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        string forecastSearchResults = await response.Content.ReadAsStringAsync();
                        list = ParseJSON(forecastSearchResults);

                        if (list.Any())
                        {
                            _context.Forecasts.AddRange(list);
                            await _context.SaveChangesAsync();
                        }

                        if (stopwatch.ElapsedMilliseconds > 2000)
                        {
                            ViewBag.Message = "⚠️ Forecast loaded slowly (" + stopwatch.ElapsedMilliseconds + "ms).";
                        }

                        return View(list);
                    }
                    else
                    {
                        ViewBag.Message = "⚠️ Forecast service returned an error.";
                    }
                }
                catch (TaskCanceledException)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ Forecast request timed out after 5 seconds.";
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ An unexpected error occurred: " + ex.Message;
                }

                retryCount++;
                if (retryCount > maxRetries)
                {
                    break;
                }

                // Optional: small delay before retrying
                await Task.Delay(500);
            }

            return View(list);
        }



        [Authorize(Roles = "Admin,User")]

        /*public async Task<IActionResult> ForecastSinus()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (TempData["CanShowForecast"] as string == "yes")
                {
                    List<Forecast> list = new List<Forecast>();
                    HttpResponseMessage response = await _httpClient.GetAsync("http://dataservice.accuweather.com/indices/v1/daily/5day/227397/30?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz");
                    if (response.IsSuccessStatusCode)
                    {
                        string forecastSearchResults = await response.Content.ReadAsStringAsync();
                        list = ParseJSON(forecastSearchResults);

                    }
                    return View(list.ToList());
                }
                else return RedirectToAction("Create", "UserHealthCondition");
            }
            else
                return Redirect("/Identity/Account/Login");
        }*/
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> ForecastSinus()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            if (TempData["CanShowForecast"] as string != "yes")
                return RedirectToAction("Create", "UserHealthCondition");

            List<Forecast> list = new List<Forecast>();
            string apiUrl = "http://dataservice.accuweather.com/indices/v1/daily/5day/227397/30?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz";

            var stopwatch = new Stopwatch();
            int retryCount = 0;
            const int maxRetries = 1;

            while (retryCount <= maxRetries)
            {
                stopwatch.Restart();

                try
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    stopwatch.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        string forecastSearchResults = await response.Content.ReadAsStringAsync();
                        list = ParseJSON(forecastSearchResults);

                        if (stopwatch.ElapsedMilliseconds > 2000)
                        {
                            ViewBag.Message = $"⚠️ Forecast loaded slowly ({stopwatch.ElapsedMilliseconds} ms).";
                        }

                        return View(list);
                    }
                    else
                    {
                        ViewBag.Message = "⚠️ Forecast service returned an error.";
                    }
                }
                catch (TaskCanceledException)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ Forecast request timed out after 5 seconds.";
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ An unexpected error occurred: " + ex.Message;
                }

                retryCount++;
                if (retryCount > maxRetries)
                    break;

                await Task.Delay(500); // Wait before retry
            }

            return View(list);
        }

        [Authorize(Roles = "Admin,User")]

        /* public async Task<IActionResult> ForecastArthritis()
         {
             if (User.Identity.IsAuthenticated)
             {
                 if (TempData["CanShowForecast"] as string == "yes")
                 {
                     List<Forecast> list = new List<Forecast>();
                     HttpResponseMessage response = await _httpClient.GetAsync("http://dataservice.accuweather.com/indices/v1/daily/5day/227397/21?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz");
                     if (response.IsSuccessStatusCode)
                     {
                         string forecastSearchResults = await response.Content.ReadAsStringAsync();
                         list = ParseJSON(forecastSearchResults);

                     }
                     return View(list.ToList());
                 }
                 else return RedirectToAction("Create", "UserHealthCondition");
             }
             else
                 return Redirect("/Identity/Account/Login");
         }*/
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> ForecastArthritis()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            if (TempData["CanShowForecast"] as string != "yes")
                return RedirectToAction("Create", "UserHealthCondition");

            List<Forecast> list = new List<Forecast>();
            string apiUrl = "http://dataservice.accuweather.com/indices/v1/daily/5day/227397/21?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz";

            var stopwatch = new Stopwatch();
            int retryCount = 0;
            const int maxRetries = 1;

            while (retryCount <= maxRetries)
            {
                stopwatch.Restart();

                try
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    stopwatch.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        string forecastSearchResults = await response.Content.ReadAsStringAsync();
                        list = ParseJSON(forecastSearchResults);

                        if (stopwatch.ElapsedMilliseconds > 2000)
                        {
                            ViewBag.Message = $"⚠️ Forecast loaded slowly ({stopwatch.ElapsedMilliseconds} ms).";
                        }

                        return View(list);
                    }
                    else
                    {
                        ViewBag.Message = "⚠️ Forecast service returned an error.";
                    }
                }
                catch (TaskCanceledException)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ Forecast request timed out after 5 seconds.";
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    ViewBag.Message = "⚠️ An unexpected error occurred: " + ex.Message;
                }

                retryCount++;
                if (retryCount > maxRetries)
                    break;

                await Task.Delay(500); // Optional delay before retry
            }

            return View(list);
        }

        [Authorize(Roles = "Admin")]
        // GET: ForecastController
        public async Task<IActionResult> Index()
        {
            var forecasts = await _context.Forecasts.ToListAsync();
            return View(forecasts);
            
        }

        // GET: ForecastController/Details/5
        /*public ActionResult Details(int id)
        {
            return View();
        }*/
        /*[Authorize(Roles = "Admin")]
        // GET: ForecastController/Create
        public ActionResult Create()
        {
            return View();
        }*/
        /*[Authorize(Roles = "Admin")]
        // POST: ForecastController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
        /*[Authorize(Roles = "Admin")]
        // GET: ForecastController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: ForecastController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
        [Authorize(Roles = "Admin")]
        // GET: ForecastController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forecast = await _context.Forecasts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }

            return View(forecast);
        }

        [Authorize(Roles = "Admin")]
        // POST: ForecastController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            var forecast = await _context.Forecasts.FindAsync(id);
            if (forecast != null)
            {
                _context.Forecasts.Remove(forecast);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
