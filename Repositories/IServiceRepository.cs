using Repositories.Models;
using SystemObjects;

namespace Repositories
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetServices(FilteredResponse? filtered);
        Task<List<Service>> GetServices();
        Task<List<Service>> GetServicesByStatus(bool status);
        Task<Service?> GetService(int id);
        Task<Service?> GetService(int id, bool status);
        Task<bool> CreateService(Service service, string iconPath);
        Task<bool> UpdateService(Service service, string? iconPath);
        Task<bool> ChangeStatus(int id, int modifiedBy);
        Task<bool> DeleteService(int serviceId);
    }
}