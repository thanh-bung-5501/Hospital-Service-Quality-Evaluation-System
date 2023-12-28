using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class SystemInformationUpdateRequet
    {
        public string? SysName { get; set; }
        public IFormFile? Icon { get; set; }
        public IFormFile? Logo { get; set; }
        public string? Zalo { get; set; }
        public string? Hotline { get; set; }
        public string? Address { get; set; }
    }
}
