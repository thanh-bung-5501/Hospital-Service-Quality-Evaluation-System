using Microsoft.AspNetCore.Http;
using SystemObjects;

namespace Repositories
{
    public interface ISystemInformationRepository
    {
        Task<SystemInformation?> GetSystemInformationBySysId(int sysId);
        Task<bool> UpdateSystemInformation(SystemInformation newSettings);
        bool IsValidPhoneNumber(string phoneNumber);
        Task<List<SystemLog>> GetSystemLog(DateTime dateFrom, DateTime dateTo);
        Task<bool> AddSystemLog(HttpContext context, string logNote);
        Task<bool> AddSystemLog(int uId, string logNote);
    }
}
