using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class FileUpload
    {
        public IFormFile files { get; set; }
    }
}
