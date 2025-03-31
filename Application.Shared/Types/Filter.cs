namespace Application.Shared.Types
{
    public class Filter
    {
        public FilterComparisionOperator? Comparator { get; set; } = null;
        public string? Field { get; set; } = null;
        public string? Value { get; set; } = null;

        public FilterLogicalOperator? Logical { get; set; } = null;
        public List<Filter>? Group { get; set; } = null;

        public enum FilterLogicalOperator
        {
            AND, OR
        }

        public enum FilterComparisionOperator
        {
            GREATER,
            GREATER_OR_EQUAL,
            LESSER,
            LESSER_OR_EQUAL,
            EQUAL,
            NOT_EQUAL,
            CONTAINS,
            NOT_CONTAINS,
            START_WITH,
            END_WITH,
        }
    }
}
