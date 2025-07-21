using System.Text.Json.Serialization;

namespace HealthConditionForecast.Models
{
    public class Symptom
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long HealthConditionId { get; set; }
        //public HealthCondition HealthCondition { get; set; }
            
    }





}

