using Repositories.Models;
using UserObjects;

namespace Repositories
{
    public interface IRoleDistributionRepository
    {
        Task<RoleDistribution?> GetRoleByUserId(int userId);

        public Task<bool> AddRole(RoleDistribution roleDistribution);

        Task<List<RoleDistribution>> GetRoleDistributionsAdmin(int adminId);

        List<RoleDistributionResponse> GetRoleDistributionsFilter(FilteredResponse filterResponse, List<RoleDistributionResponse> rolesList);

        Task<bool> UpdateRole(RoleDistribution roleDistribution);
        Task<bool> UpdateRole(RoleDistribution roleDistribution, User currentUser, DateTime changedTime);

        Task UpdateRoleDistribution(RoleDistribution roleDistribution);
    }
}
