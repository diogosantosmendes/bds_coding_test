namespace Application.Shared.Types
{
    public class PageResult<T> : Result
    {
        public PageResult(): base() { }

        public PageResult(bool ok, string? message) : base(ok, message)
        {
            this.HasNext = false;
        }

        public PageResult(bool ok, List<T> data, Page page, string? message) : base(ok, message)
        {
            var hasNext = data.Count > page.Size;
            if (hasNext)
            {
                data.RemoveAt(data.Count - 1);
            }

            this.Page = page;
            this.HasNext = hasNext;
            this.Data = data;
        }

        public List<T>? Data { get; set; }

        public Page? Page { get; set; }

        public bool HasNext { get; set; }

        public static new PageResult<T> Fail(string message)
        {
            return new PageResult<T>(false, message);
        }
    }
}
