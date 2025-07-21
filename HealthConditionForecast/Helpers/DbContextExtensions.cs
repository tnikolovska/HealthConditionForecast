using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Helpers
{
    public static class DbContextExtensions
    {
        public static void MergeCollections<T, TKey>(
            this DbContext context,
            ICollection<T> currentItems,
            IEnumerable<T> newItems,
            Func<T, TKey> keySelector) where T : class
        {
            // Remove items not in new list
            foreach (var rem in currentItems.Where(ci => !newItems.Any(ni => keySelector(ni).Equals(keySelector(ci)))).ToList())
                currentItems.Remove(rem);

            // Add new items not already present
            var existingKeys = currentItems.Select(keySelector).ToHashSet();
            foreach (var ni in newItems)
                if (!existingKeys.Contains(keySelector(ni)))
                    currentItems.Add(ni);
        }
    }
}
