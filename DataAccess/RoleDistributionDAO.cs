using Microsoft.EntityFrameworkCore;
using UserObjects;

namespace DataAccess
{
    public class RoleDistributionDAO
    {
        private readonly UserDBContext _context = new UserDBContext();

        public async Task<bool> AddRole(RoleDistribution roleDistribution)
        {
            bool result = true;
            try
            {
                await _context.RoleDistribution.AddAsync(roleDistribution);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<RoleDistribution?> GetRoleByUserId(int userId)
        {
            var role = new RoleDistribution();
            try
            {
                role = await _context.RoleDistribution.SingleOrDefaultAsync(o => o.UserId == userId)!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return role;
        }

        public async Task<List<RoleDistribution>> GetRoleDistributionsAdmin(int adminId)
        {
            var roles = new List<RoleDistribution>();
            try
            {
                roles = await _context.RoleDistribution.Where(o => o.UserId != adminId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return roles;
        }

        public async Task<bool> UpdateRole(RoleDistribution roleDistribution)
        {
            bool result = false;
            try
            {
                var roleOld = await _context.RoleDistribution.SingleOrDefaultAsync(o => o.UserId == roleDistribution.UserId);
                if (roleOld == null)
                {
                    throw new Exception("This role distribution is not available.");
                }
                roleOld.MAdmin = roleDistribution.MAdmin;
                roleOld.MQAO = roleDistribution.MQAO;
                roleOld.MBOM = roleDistribution.MBOM;
                roleOld.MCriteria = roleDistribution.MCriteria;
                roleOld.MService = roleDistribution.MService;
                roleOld.MSystem = roleDistribution.MSystem;
                roleOld.MUser = roleDistribution.MUser;
                _context.Entry<RoleDistribution>(roleOld).State = EntityState.Modified;
                int res = await _context.SaveChangesAsync();
                if (res != 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<bool> UpdateRole(RoleDistribution roleDistribution, User currentUser, DateTime changedTime)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool result = false;
                try
                {
                    var roleOld = await _context.RoleDistribution.SingleOrDefaultAsync(o => o.UserId == roleDistribution.UserId);
                    if (roleOld == null)
                    {
                        throw new Exception("This role distribution is not available.");
                    }
                    roleOld.MAdmin = roleDistribution.MAdmin;
                    roleOld.MQAO = roleDistribution.MQAO;
                    roleOld.MBOM = roleDistribution.MBOM;
                    roleOld.MCriteria = roleDistribution.MCriteria;
                    roleOld.MService = roleDistribution.MService;
                    roleOld.MSystem = roleDistribution.MSystem;
                    roleOld.MUser = roleDistribution.MUser;
                    currentUser.ModifiedOn = changedTime;
                    _context.Entry<RoleDistribution>(roleOld).State = EntityState.Modified;
                    _context.Entry<User>(currentUser).State = EntityState.Modified;
                    int res = await _context.SaveChangesAsync();
                    transaction.Commit();
                    if (res != 0)
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return result;
            }
        }

        public async Task UpdateRoleDistribution(RoleDistribution roleDistribution)
        {
            try
            {
                _context.Entry<RoleDistribution>(roleDistribution).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
