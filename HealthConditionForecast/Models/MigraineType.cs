using System.ComponentModel.DataAnnotations;

namespace HealthConditionForecast.Models
{
    public enum MigraineType
    {
        [Display(Name = "Before Headache")]
        BeforeHeadache,

        [Display(Name = "Migraine With Aura")]
        MigraineWithAura,

        [Display(Name = "During Attack")]
        DuringAttack
    }
}
