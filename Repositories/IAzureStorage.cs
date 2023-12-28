using Microsoft.AspNetCore.Http;
using Repositories.Models;

namespace Repositories
{
    public interface IAzureStorage
    {
        Task<APIResponse> UploadAsync(IFormFile file);
        Task<APIResponse> DownloadAsync(string blobFileName);
        Task<APIResponse> DeleteAsync(string blobFileName);
    }
}
