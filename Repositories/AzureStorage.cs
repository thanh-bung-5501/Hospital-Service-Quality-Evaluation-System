using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Models;

namespace Repositories
{
    public class AzureStorage : IAzureStorage
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        public AzureStorage(IConfiguration configuration)
        {
            _storageConnectionString = configuration["AzureStorage:BlobConnectionString"];
            _storageContainerName = configuration["AzureStorage:BlobContainerName"];
        }
        public async Task<APIResponse> DeleteAsync(string blobFileName)
        {
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            BlobClient file = client.GetBlobClient(blobFileName);

            try
            {
                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }

            return new APIResponse
            {
                Success = true,
                Message = $"File: {blobFileName} is deleted successfully"
            };
        }

        public async Task<APIResponse> DownloadAsync(string blobFileName)
        {
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                BlobClient file = client.GetBlobClient(blobFileName);
                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    // Download the files details async
                    var content = await file.DownloadContentAsync();

                    return new APIResponse
                    {
                        Success = true,
                        Message = $"File: {blobFileName} is downloaded successfully",
                        Data = new Blob
                        {
                            Content = blobContent,
                            Name = blobFileName,
                            ContentType = content.Value.Details.ContentType
                        }
                    };
                }
                else
                {
                    return new APIResponse
                    {
                        Success = false,
                        Message = "File is not existed",
                    };
                }
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<APIResponse> UploadAsync(IFormFile blob)
        {
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var fileName = blob.Name + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(blob.FileName);
                BlobClient client = container.GetBlobClient(fileName);

                await using (Stream? file = blob.OpenReadStream())
                {
                    await client.UploadAsync(file, new BlobHttpHeaders { ContentType = blob.ContentType });
                }

                return new APIResponse
                {
                    Success = true,
                    Message = $"File {fileName} is uploaded successfully",
                    Data = new Blob
                    {
                        Name = client.Name,
                        Uri = client.Uri.AbsoluteUri,
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
