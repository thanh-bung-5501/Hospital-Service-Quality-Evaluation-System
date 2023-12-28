using DataAccess;
using Repositories.Models;
using Repositories.Utils;
using UserObjects;

namespace Repositories
{
    public class RoleDistributionRepository : IRoleDistributionRepository
    {
        private readonly RoleDistributionDAO _roleDistributionDAO = new RoleDistributionDAO();
        public async Task<bool> AddRole(RoleDistribution roleDistribution)
        {
            return await _roleDistributionDAO.AddRole(roleDistribution);
        }
        public async Task<RoleDistribution?> GetRoleByUserId(int userId)
        {
            return await _roleDistributionDAO.GetRoleByUserId(userId);
        }

        public async Task<List<RoleDistribution>> GetRoleDistributionsAdmin(int adminId)
        {
            return await _roleDistributionDAO.GetRoleDistributionsAdmin(adminId);
        }
        public List<RoleDistributionResponse> GetRoleDistributionsFilter(FilteredResponse filterResponse, List<RoleDistributionResponse> rolesList)
        {
            try
            {
                //serch
                if (!string.IsNullOrWhiteSpace(filterResponse.search))
                {
                    rolesList = rolesList.Where(x => x.Email != null && x.Email.Contains(filterResponse.search)).ToList();
                }
                //sort
                if (!string.IsNullOrWhiteSpace(filterResponse.sortedBy))
                {
                    switch (filterResponse.sortedBy)
                    {
                        case Constants.EMAIL_ASC:
                            rolesList = rolesList.OrderBy(x => x.Email).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.EMAIL_DESC:
                            rolesList = rolesList.OrderByDescending(x => x.Email).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MCRITERIA_ASC:
                            rolesList = rolesList.OrderBy(x => x.MCriteria).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MCRITERIA_DESC:
                            rolesList = rolesList.OrderByDescending(x => x.MCriteria).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MSERVICE_ASC:
                            rolesList = rolesList.OrderBy(x => x.MService).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MSERVICE_DESC:
                            rolesList = rolesList.OrderByDescending(x => x.MService).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MSYSTEM_ASC:
                            rolesList = rolesList.OrderBy(x => x.MSystem).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MSYSTEM_DESC:
                            rolesList = rolesList.OrderByDescending(x => x.MSystem).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MUSER_ASC:
                            rolesList = rolesList.OrderBy(x => x.MUser).ThenBy(x => x.UserId).ToList();
                            break;
                        case Constants.MUSER_DESC:
                            rolesList = rolesList.OrderByDescending(x => x.MUser).ThenBy(x => x.UserId).ToList();
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rolesList;
        }
        public async Task<bool> UpdateRole(RoleDistribution roleDistribution)
        {
            return await _roleDistributionDAO.UpdateRole(roleDistribution);
        }
        public async Task<bool> UpdateRole(RoleDistribution roleDistribution, User currentUser, DateTime changedTime)
        {
            return await _roleDistributionDAO.UpdateRole(roleDistribution, currentUser, changedTime);
        }

        public async Task UpdateRoleDistribution(RoleDistribution roleDistribution)
        {
            await _roleDistributionDAO.UpdateRoleDistribution(roleDistribution);
        }
    }
}
