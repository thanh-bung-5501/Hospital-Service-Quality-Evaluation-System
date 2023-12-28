using DataAccess;
using Microsoft.AspNetCore.Http;
using Repositories.Utils;
using System.Text.RegularExpressions;
using SystemObjects;

namespace Repositories
{
    public class SystemInformationRepository : ISystemInformationRepository
    {
        private readonly SystemInformationDAO _systemInformationDAO = new SystemInformationDAO();
        public async Task<SystemInformation?> GetSystemInformationBySysId(int sysId)
        {
            return await _systemInformationDAO.GetSystemInformationBySysId(sysId);
        }
        public async Task<bool> UpdateSystemInformation(SystemInformation newSettings)
        {
            return await _systemInformationDAO.UpdateSystemInformation(newSettings);
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            string format = "^[0-9\\-\\+]{9,15}$";
            Regex regex = new Regex(format);
            Match match = regex.Match(phoneNumber);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<SystemLog>> GetSystemLog(DateTime dateFrom, DateTime dateTo)
        {
            return await _systemInformationDAO.GetSystemLog(dateFrom, dateTo);
        }

        public async Task<bool> AddSystemLog(HttpContext context, string logNote)
        {
            return await _systemInformationDAO.AddSystemLog(new SystemLog
            {
                UserId = Common.GetUserId(context),
                Note = logNote,
                CreatedOn = DateTime.Now
            });
        }

        public async Task<bool> AddSystemLog(int uId, string logNote)
        {
            return await _systemInformationDAO.AddSystemLog(new SystemLog
            {
                UserId = uId,
                Note = logNote,
                CreatedOn = DateTime.Now
            });
        }
    }
}
