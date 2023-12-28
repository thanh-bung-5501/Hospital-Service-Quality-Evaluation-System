using Microsoft.AspNetCore.JsonPatch;
using UserObjects;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserId(int userId);

        Task<User?> GetUserByRefreshToken(string refreshToken);

        Task<List<User>> GetUsers();
        Task<List<User>> GetUsers(int userId);

        Task<bool> UpdateProfile(User user);

        Task<bool> ChangePassword(int userId, string newPassword);

        Task<bool> UpdateImage(int userId, string image);

        Task<bool> CheckCurrentPasswordByUserId(int userId, string currentPassword);

        Task<bool> ChangeCurrentPasswordByUserId(int userId, JsonPatchDocument patchUser);

        Task<User?> GetUserByEmailAndPasswordHash(string email, string pwRequest);

        Task SetRefreshTokenForUserLogin(int userId, string refreshToken, DateTime tokenExpire);

        Task RevokeRefreshTokenByUserId(int userId);

        Task<bool> CheckEmailSystemByInputEmail(string email);

        Task<User?> GetUserByEmail(string email);

        Task<bool> ResetPasswordSystemByInputEmail(string email, string resetPassword);

        Task<VerificationCode?> GetVerificationCodeByEmail(string email);

        Task<bool> AddNewVerificationCode(VerificationCode verificationCode);

        Task<bool> AddUser(User newUser, RoleDistribution newUserRole);
        Task<bool> UpdateUser(User newUser, RoleDistribution newUserRole);

        Task<bool> UpdateVerificationCode(VerificationCode verificationCode);

        Task<int> UpdateUser(User user);

        Task<bool> CheckExistedEmail(string email);

        bool IsValidPhoneNumber(string phoneNumber);

        bool IsValidEmail(string email);

        bool IsValidPassword(string password);

        string HashPassword(string initialPassword);

        Task<bool> ChangeUserStatus(int userId, DateTime changedTime);

        Task<string?> GetRoleByUserId(int userId);
    }
}
