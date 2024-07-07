namespace MyConsoleApplication
{
    public class Filter
    {
        public string PropertyName { get; set; }
        public Op Operation { get; set; }
        public string Operators { get; set; } = string.Empty;
        public object Value { get; set; }
    }
    public enum Op
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }
}