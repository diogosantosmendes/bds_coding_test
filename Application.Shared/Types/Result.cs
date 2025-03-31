namespace Application.Shared.Types
{
    public class Result
    {
        public Result()
        {
            Ok = true;
            ErrorCode = null;
        }

        public Result(bool ok, string? error)
        {
            Ok = ok;
            ErrorCode = error;
        }

        public bool Ok { get; set; }
        
        public string? ErrorCode { get; set; }

        public static Result Success()
        {
            return new Result(true,null);
        }

        public static DataResult<T> Success<T>(T data) where T : class
        {
            return new DataResult<T>(true, data, null);
        }

        public static PageResult<T> Success<T>(List<T> data, Page page) where T : class
        {
            return new PageResult<T>(true, data, page, null);
        }

        public static Result Fail(string error)
        {
            return new Result(false, error);
        }
    }
}
