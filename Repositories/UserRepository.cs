using DataAccess;
using Microsoft.AspNetCore.JsonPatch;
using Repositories.Utils;
using System.Text.RegularExpressions;
using UserObjects;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO = new UserDAO();
        public async Task<User?> GetUserByUserId(int userId)
        {
            return await _userDAO.GetUserByUserId(userId);
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            return await _userDAO.GetUserByRefreshToken(refreshToken);
        }

        public async Task<bool> UpdateImage(int userId, string image)
        {
            return await _userDAO.UpdateImage(userId, image);
        }

        public async Task<bool> UpdateProfile(User newUser)
        {
            return await _userDAO.UpdateProfile(newUser);
        }

        public async Task<bool> ChangePassword(int userId, string newPassword)
        {
            return await _userDAO.ChangePassword(userId, newPassword);
        }

        public async Task<User?> GetUserByEmailAndPasswordHash(string email, string pwRequest)
        {
            return await _userDAO.GetUserByEmailAndPasswordHash(email, pwRequest);
        }

        public async Task<bool> CheckCurrentPasswordByUserId(int userId, string currentPassword)
        {
            return await _userDAO.CheckCurrentPasswordByUserId(userId, currentPassword);
        }

        public async Task<bool> ChangeCurrentPasswordByUserId(int userId, JsonPatchDocument patchUser)
        {
            var oldUser = new User();
            // this is level of iterations to executing hash by using bcrypt
            int workFactor = Constants.WORK_FACTOR;

            try
            {
                // check current user
                oldUser = await _userDAO.GetUserByUserId(userId) ?? throw new Exception();

                // apply one or more paths in patchUser to one or more attributes in user
                patchUser.ApplyTo(oldUser);
            }
            catch (Exception)
            {
                return false;
            }
            return await _userDAO.ChangeCurrentPasswordByUserId(oldUser, workFactor);
        }
        public async Task SetRefreshTokenForUserLogin(int userId, string refreshToken, DateTime tokenExpire)
        {
            await _userDAO.SetRefreshTokenForUserLogin(userId, refreshToken, tokenExpire);
        }

        public async Task RevokeRefreshTokenByUserId(int userId)
        {
            await _userDAO.RevokeRefreshTokenByUserId(userId);
        }
        public async Task<bool> CheckEmailSystemByInputEmail(string email)
        {
            return await _userDAO.CheckEmailSystemByInputEmail(email);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userDAO.GetUserByEmail(email);
        }
        public async Task<bool> ResetPasswordSystemByInputEmail(string email, string resetPassword)
        {
            // this is level of iterations to executing hash by using bcrypt
            int workFactor = Constants.WORK_FACTOR;

            return await _userDAO.ResetPasswordSystemByInputEmail(email, resetPassword, workFactor);
        }
        public async Task<VerificationCode?> GetVerificationCodeByEmail(string email)
        {
            return await _userDAO.GetVerificationCodeByEmail(email);
        }
        public async Task<bool> AddNewVerificationCode(VerificationCode verificationCode)
        {
            return await _userDAO.AddNewVerificationCode(verificationCode);
        }
        public async Task<bool> UpdateVerificationCode(VerificationCode verificationCode)
        {
            return await _userDAO.UpdateVerificationCode(verificationCode);
        }
        public async Task<bool> CheckExistedEmail(string email)
        {
            return await _userDAO.CheckExistedEmail(email);
        }

        #region Validation
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
        public bool IsValidEmail(string email)
        {
            string format = "[a-zA-Z0-9]+@[a-zA-Z]+\\.[a-zA-Z]";
            Regex regex = new Regex(format);
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
         * Contain at least 1 letter
         * Contain at least 1 number
         * Length: 8 - 20 characters
         **/
        public bool IsValidPassword(string password)
        {
            string format = "(?=^.{8,20}$)(?=.*\\d)(?![.\\n])(?=.*[a-z|A-Z]).*$";
            Regex regex = new Regex(format);
            Match match = regex.Match(password);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public async Task<bool> AddUser(User newUser, RoleDistribution newUserRole)
        {
            return await _userDAO.AddUser(newUser, newUserRole);
        }

        public async Task<bool> UpdateUser(User newUser, RoleDistribution newUserRole)
        {
            return await _userDAO.UpdateUser(newUser, newUserRole);
        }

        /**
        * Hashing password
        **/
        public string HashPassword(string initialPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(initialPassword, 13);
        }


        public async Task<List<User>> GetUsers()
        {
            var usersList = new List<User>();
            try
            {
                usersList = await _userDAO.GetUsers();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usersList;
        }
        public async Task<List<User>> GetUsers(int userId)
        {
            var usersList = new List<User>();
            try
            {
                usersList = await _userDAO.GetUsers(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usersList;
        }

        public async Task<bool> ChangeUserStatus(int userId, DateTime changedTime)
        {
            return await _userDAO.ChangeUserStatus(userId, changedTime);
        }

        public async Task<int> UpdateUser(User user)
        {
            return await _userDAO.UpdateUser(user);
        }

        public async Task<string?> GetRoleByUserId(int userId) => await _userDAO.GetRoleByUserId(userId);
    }
}
