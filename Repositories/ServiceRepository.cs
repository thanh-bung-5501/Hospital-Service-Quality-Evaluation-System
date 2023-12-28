using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Repositories.Utils;
using SystemObjects;

namespace Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceDAO _serviceDAO = new ServiceDAO();
        public async Task<List<Service>> GetServices(FilteredResponse? filtered)
        {
            var services = _serviceDAO.GetServiceQueryAble();
            // Service active
            services = services.Where(x => x.Status == true);

            if (filtered == null)
            {
                return await services.ToListAsync();
            }
            else
            {
                try
                {
                    #region Filtering
                    if (!string.IsNullOrEmpty(filtered.search))
                    {
                        services = services.Where(x => x.SerName != null && x.SerName.Contains(filtered.search));
                    }
                    #endregion

                    #region Sorting
                    // Default sort by SerName
                    services = services.OrderBy(x => x.SerName);

                    if (!string.IsNullOrEmpty(filtered.sortedBy))
                    {
                        switch (filtered.sortedBy)
                        {
                            case Constants.SERVICE_NAME_DESC:
                                services = services.OrderByDescending(x => x.SerName);
                                break;
                            case Constants.CREATED_ON_ASC:
                                services = services.OrderBy(x => x.CreatedOn);
                                break;
                            case Constants.CREATED_ON_DESC:
                                services = services.OrderByDescending(x => x.CreatedOn);
                                break;
                                // another case here ...
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return await services.ToListAsync();
            }
        }
        public async Task<List<Service>> GetServices() => await _serviceDAO.GetServices();
        public async Task<List<Service>> GetServicesByStatus(bool status) => await _serviceDAO.GetServicesByStatus(status);
        public async Task<Service?> GetService(int id) => await _serviceDAO.GetService(id);
        public async Task<Service?> GetService(int id, bool status) => await _serviceDAO.GetService(id, status);
        public async Task<bool> CreateService(Service service, string iconPath) => await _serviceDAO.CreateService(service, iconPath);
        public async Task<bool> UpdateService(Service service, string? iconPath) => await _serviceDAO.UpdateService(service, iconPath);
        public async Task<bool> ChangeStatus(int id, int modifiedBy) => await _serviceDAO.ChagneStatus(id, modifiedBy);
        public async Task<bool> DeleteService(int serviceId) => await _serviceDAO.DeleteService(serviceId);
    }
}
