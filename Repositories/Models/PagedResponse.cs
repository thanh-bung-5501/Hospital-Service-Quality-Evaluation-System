namespace Repositories.Models
{
    public class PagedResponse<T>
    {
        public int TotalItems { get; set; }
        public List<T>? Results { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
