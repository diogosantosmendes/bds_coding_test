using Application.Shared.Types;

namespace Application.Shared.Extensions
{
    public static class FilterExtensions
    {
        public static void CheckAllowedFields(this Filter? filter, string[] fields)
        {
            if (filter == null)
            {
                return;
            }

            if (filter.Field != null)
            {
                var capitalized = filter.Field.ToCapitalized();
                if (!fields.Any(f => f == capitalized))
                {
                    throw new ArgumentException($"Field {filter.Field} is not allowed");
                }
            }
            else if (filter.Group != null)
            {
                filter.Group.ForEach(f => f.CheckAllowedFields(fields));
            }
        }
    }
}
