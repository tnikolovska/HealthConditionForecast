using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace HealthConditionForecast.Models
{
    public class Forecast
    {
        public long Id { get; set; }
        [JsonPropertyName("ID")]
        public string IdForecast { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; }
        [Precision(18, 2)]
        public decimal CategoryValue { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }





    }
}
