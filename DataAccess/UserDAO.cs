using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserObjects;

namespace DataAccess
{
    public class UserDAO
    {
        private readonly UserDBContext _context = new UserDBContext();

        public async Task<User?> GetUserByUserId(int userId)
        {
            var user = new User();
            try
            {
                user = await _context.User.SingleOrDefaultAsync(o => o.UserId == userId)!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user!;
        }
        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            var user = new User();
            try
            {
                user = await _context.User.SingleOrDefaultAsync(o => o.RefreshToken.Equals(refreshToken));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public async Task<bool> UpdateProfile(User newUser)
        {
            bool result = false;
            try
            {
                var userOld = await GetUserByUserId(newUser.UserId);
                if (userOld == null)
                {
                    return false;
                }

                userOld.FirstName = newUser.FirstName;
                userOld.LastName = newUser.LastName;
                userOld.GenderId = newUser.GenderId;
                userOld.PhoneNumber = newUser.PhoneNumber;
                userOld.Dob = newUser.Dob;
                userOld.Address = newUser.Address;
                userOld.ModifiedOn = DateTime.Now;

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
        public async Task<bool> UpdateImage(int userId, string image)
        {
            bool result = false;
            try
            {
                User? user = await GetUserByUserId(userId);
                if (user == null)
                {
                    throw new Exception("This user is not available.");
                }
                user.Image = image;
                user.ModifiedOn = DateTime.Now;
                _context.Entry<User>(user).State = EntityState.Modified;
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
        public async Task<User?> GetUserByEmailAndPasswordHash(string email, string pwRequest)
        {
            var user = new User();
            try
            {
                user = await _context.User.SingleOrDefaultAsync(o => o.Email.Equals(email));
                if (user != null)
                {
                    if (await CheckCurrentPasswordByUserId(user.UserId, pwRequest))
                    {
                        return user;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        public async Task<bool> ChangePassword(int userId, string newPassword)
        {
            try
            {
                // check current user
                var user = await GetUserByUserId(userId) ?? throw new Exception("User not found");

                user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword, 13);
                user.ModifiedOn = DateTime.Now;

                int rs = await _context.SaveChangesAsync();

                if (rs == 0) return false;
                else return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckCurrentPasswordByUserId(int userId, string currentPassword)
        {
            bool result;
            try
            {
                // check current user
                var user = await GetUserByUserId(userId) ?? throw new Exception();

                // check password input vs current password in db by using bcrypt
                result = BCrypt.Net.BCrypt.EnhancedVerify(currentPassword, user.Password);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task<bool> ChangeCurrentPasswordByUserId(User oldUser, int workFactor)
        {
            bool result = true;
            try
            {
                // hash new password by using bcrypt then save in db
                oldUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(oldUser.Password, workFactor);
                oldUser.ModifiedOn = DateTime.Now;

                _context.Entry<User>(oldUser).State = EntityState.Modified;
                // or use this below to update password of user
                //context.User.Update(oldUser);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task SetRefreshTokenForUserLogin(int userId, string refreshToken, DateTime tokenExpire)
        {
            var user = new User();
            try
            {
                user = await _context.User.SingleOrDefaultAsync(o => o.UserId == userId);
                if (user == null)
                {
                    throw new Exception();
                }
                else
                {
                    user.RefreshToken = refreshToken;
                    user.TokenExpires = tokenExpire;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task RevokeRefreshTokenByUserId(int userId)
        {
            var user = new User();
            try
            {
                user = await _context.User.SingleOrDefaultAsync(o => o.UserId == userId);
                if (user == null)
                {
                    throw new Exception();
                }
                else
                {
                    user.RefreshToken = null;
                    user.TokenExpires = null;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            var user = new User();
            try
            {
                // find user info by email
                user = await _context.User.SingleOrDefaultAsync(o => o.Email.Equals(email) && o.Status == true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }
        public async Task<bool> CheckEmailSystemByInputEmail(string email)
        {
            bool result = true;
            try
            {
                // check email system of user
                var user = await GetUserByEmail(email) ?? throw new Exception();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task<bool> ResetPasswordSystemByInputEmail(string email, string resetPassword, int workFactor)
        {
            bool result = true;
            try
            {
                // check email system of user
                var oldUser = await GetUserByEmail(email) ?? throw new Exception();

                // update reset password by using bcrypt to hashing for old password of user
                oldUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(resetPassword, workFactor);
                oldUser.ModifiedOn = DateTime.Now;

                _context.Entry<User>(oldUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task<VerificationCode?> GetVerificationCodeByEmail(string email)
        {
            var code = new VerificationCode();
            try
            {
                // find verification code info by email
                code = await _context.VerificationCode.SingleOrDefaultAsync(o => o.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return code;
        }
        public async Task<bool> AddNewVerificationCode(VerificationCode verificationCode)
        {
            bool result = true;
            try
            {
                // add new code if forgot password first
                await _context.VerificationCode.AddAsync(verificationCode);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task<bool> UpdateVerificationCode(VerificationCode verificationCode)
        {
            bool result = true;
            try
            {
                // check code is exist before updating code
                var code = await GetVerificationCodeByEmail(verificationCode.Email) ?? throw new Exception();

                // update code if forgot password from second
                code.Code = verificationCode.Code;
                code.CodeExpires = verificationCode.CodeExpires;

                _context.Entry<VerificationCode>(code).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public async Task<bool> CheckExistedEmail(string email)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
                if (user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Occur an error with database when checking email: " + email);
                throw new Exception(ex.Message);
            }
            finally
            {
                Debug.WriteLine("End to check email: " + email);
            }
        }

        /**
        * Add new user
        **/
        public async Task<bool> AddUser(User newUser, RoleDistribution newUserRole)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool success = true;
                try
                {
                    await _context.User.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    newUserRole.UserId = newUser.UserId;
                    await _context.RoleDistribution.AddAsync(newUserRole);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    success = false;
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return success;
            }
        }

        public async Task<bool> UpdateUser(User newUser, RoleDistribution newUserRole)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool success = true;
                try
                {
                    var oldUser = await _context.User.SingleOrDefaultAsync(x => x.UserId == newUser.UserId)!;
                    var oldUserRole = await _context.RoleDistribution.SingleOrDefaultAsync(x => x.UserId == newUser.UserId);

                    if (oldUser == null)
                    {
                        throw new Exception("Not found User");
                    }

                    if (oldUserRole == null) throw new Exception("Not found user role");

                    oldUser.Password = newUser.Password;
                    oldUser.FirstName = newUser.FirstName;
                    oldUser.LastName = newUser.LastName;
                    oldUser.GenderId = newUser.GenderId;
                    oldUser.Dob = newUser.Dob;
                    oldUser.PhoneNumber = newUser.PhoneNumber;
                    oldUser.Address = newUser.Address;
                    oldUser.Image = newUser.Image;
                    oldUser.ModifiedOn = newUser.ModifiedOn;
                    oldUser.Status = newUser.Status;

                    await _context.SaveChangesAsync();

                    oldUserRole.MAdmin = newUserRole.MAdmin;
                    oldUserRole.MQAO = newUserRole.MQAO;
                    oldUserRole.MBOM = newUserRole.MBOM;

                    oldUserRole.MSystem = newUserRole.MSystem;
                    oldUserRole.MUser = newUserRole.MUser;
                    oldUserRole.MService = newUserRole.MService;
                    oldUserRole.MCriteria = newUserRole.MCriteria;

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    success = false;
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return success;
            }
        }

        public async Task<List<User>> GetUsers()
        {
            var users = new List<User>();
            try
            {
                users = await _context.User.OrderByDescending(x => x.UserId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }
        public async Task<List<User>> GetUsers(int userId)
        {
            var users = new List<User>();
            try
            {
                users = await _context.User.Where(u => u.UserId != userId).OrderByDescending(x => x.UserId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }



        /**
        * Disable (inactive) user
        **/
        public async Task<bool> ChangeUserStatus(int userId, DateTime changedTime)
        {
            try
            {
                var user = await GetUserByUserId(userId);

                if (user == null)
                {
                    return false;
                }

                user.ModifiedOn = changedTime;
                user.Status = !user.Status;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Update user
         **/
        public async Task<int> UpdateUser(User user)
        {
            try
            {
                _context.Entry<User>(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return user.UserId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string?> GetRoleByUserId(int userId)
        {
            try
            {
                var user = await GetUserByUserId(userId);
                string role = "";

                if (user == null)
                {
                    return null;
                }
                else
                {
                    var userRole = await _context.RoleDistribution.SingleOrDefaultAsync(x => x.UserId == userId);
                    if (userRole == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (userRole.MAdmin) role += "Admin";
                        if (userRole.MQAO) role += "QAO";
                        if (userRole.MBOM) role += "BOM";

                        if (userRole.MCriteria > 0) role += " Criteria" + userRole.MCriteria;
                        if (userRole.MService > 0) role += " Service" + userRole.MService;
                        if (userRole.MSystem > 0) role += " System" + userRole.MSystem;
                        if (userRole.MUser > 0) role += " User" + userRole.MUser;

                        return role;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
