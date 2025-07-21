namespace HealthConditionForecast.Models
{
    public class HealthCondition
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Symptom> Symptoms { get; set; }

        public HealthCondition()
        {
            Symptoms = new List<Symptom>();
        }
    }
}
