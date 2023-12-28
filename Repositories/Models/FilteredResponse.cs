using Repositories.Utils;

namespace Repositories.Models
{
    public class FilteredResponse
    {
        public FilteredResponse() { }
        public FilteredResponse(string? search, string? sortedBy, int page, int pageSize)
        {
            this.search = search;
            this.sortedBy = sortedBy;
            this.page = page < this.page ? this.page : page;
            this.pageSize = pageSize < this.pageSize ? this.pageSize : pageSize;
        }

        public string? search { get; set; } = string.Empty;
        public string? sortedBy { get; set; } = string.Empty;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = Constants.PAGE_SIZE;
    }
}
