using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HealthConditionForecast.Helpers
{
    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>(TEnum selectedValue) where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(e => new SelectListItem
            {
                Text = GetDisplayName(e),
                Value = e.ToString(),
                Selected = e.Equals(selectedValue)
            });
        }
        private static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
            if (member != null)
            {
                var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                    return displayAttribute.Name;
            }
            return enumValue.ToString();
        }
    }
}
