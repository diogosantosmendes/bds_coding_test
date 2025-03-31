namespace Application.Shared.Types
{
    public class DataResult<T> : Result where T : class
    {
        public DataResult() : base() { }

        public DataResult(bool ok, T? data, string? message) : base(ok, message)
        {
            Data = data;
        }

        public T? Data { get; set; }

        public static DataResult<T> Fail(string message)
        {
            return new DataResult<T>(false, null, message);
        }
    }
}
