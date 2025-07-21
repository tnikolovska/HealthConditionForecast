using HealthConditionForecast.Migrations;

namespace HealthConditionForecast.Models
{
    public class UserSymptomSelection
    {
        public long Id { get; set; }
        public int UserHealthConditionId { get; set; }
        //public UserHealthCondition UserHealthCondition { get; set; }
        public ICollection<MigraineSympton> MigraineSymptoms { get; set; } = new List<MigraineSympton>();
        public ICollection<SinusSymptom> SinusSymptoms { get; set; } = new List<SinusSymptom>();
        public ICollection<ArthritisSymtom> ArthritisSymptoms { get; set; } = new List<ArthritisSymtom>();

       /* public UserSymptomSelection()
        {
            ArthritisSymptoms = new List<ArthritisSymtom>();
            MigraineSymptoms = new List<MigraineSympton>();
            SinusSymptoms = new List<SinusSymptom>();
        }*/

    }
}
