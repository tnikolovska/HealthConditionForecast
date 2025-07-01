namespace HealthConditionForecast.Models
{
    public class UserHealthCondition
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int HealthConditionId { get; set; }
        public HealthCondition HealthCondition { get; set; }



    }
}
