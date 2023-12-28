using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Repositories.Models;
using Repositories.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SystemObjects;
using UserObjects;

namespace HSEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repoU;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private APIResponse _response;

        public AuthController(IConfiguration configuration, IUserRepository repoU, ISystemInformationRepository systemInformationRepository)
        {
            _configuration = configuration;
            _repoU = repoU;
            _systemInformationRepository = systemInformationRepository;
            _response = new APIResponse();
        }

        private string CreateAccessToken(User user, string userRole)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Id", user.UserId.ToString()),

                    // roles
                    new Claim(ClaimTypes.Role, userRole),
                }),
                Expires = DateTime.UtcNow.AddSeconds(int.Parse(_configuration["JWT:AccessTokenExpiration"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var user = await _repoU.GetUserByEmailAndPasswordHash(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new APIResponse
                {
                    Success = false,
                    Message = "User authentication failure."
                });
            }

            if (user.Status == false) return Forbid();

            var role = await _repoU.GetRoleByUserId(user.UserId);
            if (role == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(user.UserId, "User login failed.");

                return Unauthorized(new APIResponse
                {
                    Success = false,
                    Message = "User authorization failure."
                });
            }

            string accessToken = CreateAccessToken(user, role);
            var refreshToken = GenerateRefreshToken();
            SaveRefreshToken(user.UserId, refreshToken);

            // Add log
            await _systemInformationRepository.AddSystemLog(user.UserId, "User logged in.");

            return Ok(new APIResponse
            {
                Success = true,
                Message = "User authentication successfully.",
                Data = new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token
                }
            });
        }

        private void SaveRefreshToken(int userId, RefreshTokenResponse refreshToken)
        {
            _repoU.SetRefreshTokenForUserLogin(userId, refreshToken.Token, refreshToken.Expires);
        }

        private RefreshTokenResponse GenerateRefreshToken()
        {
            return new RefreshTokenResponse
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMonths(int.Parse(_configuration["JWT:RefreshTokenExpiration"])),
            };
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                // Self-issued tokens
                ValidateIssuer = false,
                ValidateAudience = false,

                // Sign the token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),

                ClockSkew = TimeSpan.Zero,
                // Not check lifetime at
                ValidateLifetime = false
            };

            try
            {
                // Check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);
                // Check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    // False
                    if (!result)
                    {
                        return Unauthorized(new APIResponse
                        {
                            Success = false,
                            Message = "The token model is invalid"
                        });
                    }
                }
                // Check 3: Check accessToken expire
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

                var expireDate = Common.UnixTimeStampToDateTime(utcExpireDate);

                if (expireDate > DateTime.Now)
                {
                    return Unauthorized(new APIResponse
                    {
                        Success = false,
                        Message = "The access token has not yet expired"
                    });
                }
                // Check 4: Check refreshToken exsit in DB
                var storedToken = await _repoU.GetUserByRefreshToken(model.RefreshToken);
                if (storedToken is null)
                {
                    return Unauthorized(new APIResponse
                    {
                        Success = false,
                        Message = "The refresh token has not existed"
                    });
                }
                // Check 5: Check refreshTokem expire
                if (storedToken.TokenExpires < DateTime.Now)
                {
                    return Unauthorized(new APIResponse
                    {
                        Success = false,
                        Message = "The refresh token has expired"
                    });
                }

                var role = await _repoU.GetRoleByUserId(storedToken.UserId);
                if (role == null)
                {
                    return Unauthorized(new APIResponse
                    {
                        Success = false,
                        Message = "User authorization failure."
                    });
                }

                // Create new token
                string accessToken = CreateAccessToken(storedToken, role);
                var refreshToken = GenerateRefreshToken();
                SaveRefreshToken(storedToken.UserId, refreshToken);

                // Add log
                await _systemInformationRepository.AddSystemLog(storedToken.UserId, "User refreshed their token.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "The token is refreshed successfully",
                    Data = new TokenModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken.Token
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new APIResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var userId = Common.GetUserId(HttpContext);

            await _repoU.RevokeRefreshTokenByUserId(userId);

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User signed out.");

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Logout successfully"
            });
        }

        [HttpPost("check-email-system/{email}")]
        public async Task<IActionResult> CheckEmailSystem([FromRoute] string email)
        {
            bool isEmailValid = false;
            bool sentResetCode = false;
            bool savedCode = false;
            string randomCode = "";
            var newCode = new VerificationCode();

            // check input email
            if (!ModelState.IsValid || email == null)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Please enter required field." });
            }
            if (!_repoU.IsValidEmail(email))
            {
                return BadRequest(new APIResponse { Success = false, Message = "Email is wrong format." });
            }

            try
            {
                // check email is valid in system or not
                isEmailValid = await _repoU.CheckEmailSystemByInputEmail(email);
                if (!isEmailValid)
                {
                    throw new Exception();
                }
                var user = await _repoU.GetUserByEmail(email);

                // generate OTP for reset code
                int iOTP = 6; // length of reset code
                randomCode = Common.GenerateOTPCode(iOTP); // get random numbers

                // insert code if forgot password first or update code in db
                int validTime = 60000; // 60000 miliseconds
                var code = await _repoU.GetVerificationCodeByEmail(email);
                newCode = new VerificationCode { Email = email, Code = randomCode, CodeExpires = DateTime.Now.AddMilliseconds(validTime) };
                if (code == null)
                {
                    savedCode = await _repoU.AddNewVerificationCode(newCode);
                }
                else
                {
                    savedCode = await _repoU.UpdateVerificationCode(newCode);
                }
                if (!savedCode)
                {
                    throw new Exception();
                }

                // send email with OTP code generated
                // set information of email
                var emailInfo = Common.GetEmailInfo(email, user!.FirstName, "reset code", "resetting password for", randomCode);
                sentResetCode = Common.SendMail(emailInfo); // send email
                if (!sentResetCode)
                {
                    throw new Exception();
                }

                // send reset code in email successfully
                _response = new APIResponse { Success = savedCode, Message = "Reset code is sent successfully." };
            }
            catch (Exception)
            {
                if (!isEmailValid)
                {
                    // email is invalid in system
                    _response = new APIResponse { Success = isEmailValid, Message = "Email is invalid." };
                    return NotFound(_response);
                }
                else if (!savedCode)
                {
                    // save reset code in db failure
                    _response = new APIResponse { Success = savedCode, Message = "Reset code is executed failure." };
                    return Conflict(_response);
                }
                else if (!sentResetCode)
                {
                    // send reset code in email failure
                    _response = new APIResponse { Success = sentResetCode, Message = "Reset code is sent failure." };
                    return Conflict(_response);
                }
            }
            return Ok(_response);
        }

        [HttpPost("verify-reset-code/{email}")]
        public async Task<IActionResult> VerifyResetCode([FromRoute] string email, [FromForm] string resetCode)
        {
            bool isCodeValid = false;
            bool sentResetPassword = false;
            bool updatedCode = false;
            string randomPassword = "";
            var defaultCode = new VerificationCode();

            // check input email
            if (!ModelState.IsValid || email == null || String.IsNullOrWhiteSpace(resetCode))
            {
                return BadRequest(new APIResponse { Success = false, Message = "Please enter required field." });
            }
            if (!_repoU.IsValidEmail(email))
            {
                return BadRequest(new APIResponse { Success = false, Message = "Email is wrong format." });
            }

            try
            {
                // check reset code is valid in system or not
                var code = await _repoU.GetVerificationCodeByEmail(email);
                if (code == null)
                {
                    throw new Exception();
                }
                else
                {
                    // wrong code
                    if (code.Code == null || !code.Code.Equals(resetCode))
                    {
                        throw new Exception();
                    }
                    // if resetCode not map code in VerificationCode or code expires is invalid
                    else if (code.CodeExpires != null && DateTime.Compare((DateTime)code.CodeExpires, DateTime.Now) < 0)
                    {
                        // set default to VerificationCode includes email input
                        defaultCode = new VerificationCode { Email = email, Code = null, CodeExpires = null };
                        await _repoU.UpdateVerificationCode(defaultCode);
                        throw new Exception();
                    }
                }
                isCodeValid = true;

                // generate reset password
                int passwordLength = 8; // length of reset password
                randomPassword = Common.GenerateResetPassword(passwordLength); // get reset password

                // update reset password in db
                updatedCode = await _repoU.ResetPasswordSystemByInputEmail(email, randomPassword);
                if (!updatedCode)
                {
                    throw new Exception();
                }

                // set code and code expires in db to default is null after update successfully
                defaultCode = new VerificationCode { Email = email, Code = null, CodeExpires = null };
                updatedCode = await _repoU.UpdateVerificationCode(defaultCode);
                if (!updatedCode)
                {
                    throw new Exception();
                }

                var user = await _repoU.GetUserByEmail(email);

                // send email with reset password
                var emailInfo = Common.GetEmailInfo(email, user!.FirstName, "reset password", "logging into", randomPassword);
                sentResetPassword = Common.SendMail(emailInfo);
                if (!sentResetPassword)
                {
                    throw new Exception();
                }

                // send reset password and set password in database successfully
                _response = new APIResponse { Success = updatedCode, Message = "Reset password is sent successfully." };
            }
            catch (Exception)
            {
                if (!isCodeValid)
                {
                    // codeSaved is invalid in system
                    _response = new APIResponse { Success = isCodeValid, Message = "Reset code is invalid." };
                    return NotFound(_response);
                }
                else if (!updatedCode)
                {
                    // update reset password or reset code in db failure
                    _response = new APIResponse { Success = updatedCode, Message = "Reset code is executed failure." };
                    return Conflict(_response);
                }
                else if (!sentResetPassword)
                {
                    // send reset password in email failure
                    _response = new APIResponse { Success = sentResetPassword, Message = "Reset password is sent failure." };
                    return Conflict(_response);
                }
            }
            return Ok(_response);
        }
    }
}
