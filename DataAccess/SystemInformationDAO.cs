using Microsoft.EntityFrameworkCore;
using SystemObjects;

namespace DataAccess
{
    public class SystemInformationDAO
    {
        private readonly SystemDBContext _context = new SystemDBContext();
        public async Task<SystemInformation?> GetSystemInformationBySysId(int sysId)
        {
            var systemInformation = new SystemInformation();
            try
            {
                systemInformation = await _context.SystemInformation.SingleOrDefaultAsync(o => o.SysId == sysId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return systemInformation;
        }
        public async Task<bool> UpdateSystemInformation(SystemInformation newSettings)
        {
            try
            {
                var oldSettings = await _context.SystemInformation.SingleOrDefaultAsync(o => o.SysId == newSettings.SysId);
                if (oldSettings == null)
                {
                    throw new Exception("Not found Setting in data base");
                }
                oldSettings.SysName = newSettings.SysName;
                oldSettings.Icon = newSettings.Icon;
                oldSettings.Logo = newSettings.Logo;
                oldSettings.Zalo = newSettings.Zalo;
                oldSettings.Hotline = newSettings.Hotline;
                oldSettings.Address = newSettings.Address;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<SystemLog>> GetSystemLog(DateTime dateFrom, DateTime dateTo)
            => await _context.SystemLog.Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1))
            .OrderByDescending(x => x.LogId).ToListAsync();

        public async Task<bool> AddSystemLog(SystemLog log)
        {
            bool result = true;
            try
            {
                await _context.SystemLog.AddAsync(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }
    }
}
