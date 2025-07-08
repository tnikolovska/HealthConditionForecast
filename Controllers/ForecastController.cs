using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
                    _context.Forecasts.Add(forecast);
                    _context.SaveChanges();
                }
            }

            return forecastList;
        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ForecastMigraine() 
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Forecast> list = new List<Forecast>();
                HttpResponseMessage response = await _httpClient.GetAsync("http://dataservice.accuweather.com/indices/v1/daily/5day/227397/27?apikey=OeYyggfEf9RAfVh8RArY7paEcdyI8Kqz");
                if (response.IsSuccessStatusCode)
                {
                    string forecastSearchResults = await response.Content.ReadAsStringAsync();
                    list = ParseJSON(forecastSearchResults);
                }
                return View(list.ToList());
            }
            else
                return Redirect("/Identity/Account/Login");

        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ForecastSinus()
        {
            if (User.Identity.IsAuthenticated)
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
            else
                return Redirect("/Identity/Account/Login");
        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ForecastArthritis()
        {
            if (User.Identity.IsAuthenticated)
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
            else
                return Redirect("/Identity/Account/Login");
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
