using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Repositories.Utils;
using SystemObjects;
using UserObjects;

namespace HSEApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleDistributionRepository _roleDistributionRepository;
        private readonly IAzureStorage _azureStorage;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;
        public UserController(IUserRepository userRepository, IRoleDistributionRepository roleDistributionRepository,
            IAzureStorage azureStorage, ISystemInformationRepository systemInformationRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleDistributionRepository = roleDistributionRepository;
            _azureStorage = azureStorage;
            _mapper = mapper;
            _azureStorage = azureStorage;
            _systemInformationRepository = systemInformationRepository;
            _response = new APIResponse();
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UserUpdateProfileRequest newUser)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }
            else
            {
                var userId = Common.GetUserId(HttpContext);
                var oldUser = await _userRepository.GetUserByUserId(userId);

                if (oldUser == null)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                if (!String.IsNullOrEmpty(newUser.PhoneNumber))
                {
                    if (!_userRepository.IsValidPhoneNumber(newUser.PhoneNumber!))
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid phone number.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Invalid phone number format!"
                        });
                    }
                }

                var userUpdate = new User
                {
                    FirstName = String.IsNullOrEmpty(newUser.FirstName) ? oldUser.FirstName : newUser.FirstName,
                    LastName = String.IsNullOrEmpty(newUser.LastName) ? oldUser.LastName : newUser.LastName,
                    GenderId = newUser.GenderId == null ? oldUser.GenderId : newUser.GenderId.Value,
                    Dob = newUser.Dob ?? null ,
                    PhoneNumber = String.IsNullOrEmpty(newUser.PhoneNumber) ? oldUser.PhoneNumber : newUser.PhoneNumber,
                    Address = String.IsNullOrEmpty(newUser.Address) ? null : newUser.Address,
                };

                userUpdate.UserId = userId;
                bool result = await _userRepository.UpdateProfile(userUpdate);

                if (!result)
                {
                    return Conflict(new APIResponse
                    {
                        Success = false,
                        Message = "Update profile failure."
                    });
                }
                else
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User updated their profile successfully.");

                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Update Profile successfully."
                    });
                }
            }
        }

        [HttpPost("change-image")]
        public async Task<IActionResult> ChangeImage([FromForm] FileUpload fileUpload)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }
            else
            {
                try
                {
                    //Get userId form token 
                    int userId = Common.GetUserId(HttpContext);
                    User? user = await _userRepository.GetUserByUserId(userId);

                    if (user == null)
                    {
                        return NotFound(new APIResponse { Success = false, Message = "This user is not available." });
                    }

                    if (fileUpload.files.Length == 0)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an empty file.");

                        return BadRequest(new APIResponse { Success = false, Message = "File not found." });
                    }
                    if (!Constants.IMAGE_EXTENSIONS.Contains(Path.GetExtension(fileUpload.files.FileName)))
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an invalid image file.");

                        return BadRequest(new APIResponse { Success = false, Message = "Please choose a file with extension .jpg or .png" });
                    }
                    if (fileUpload.files.Length > Constants.IMAGE_LIMIT_SIZE)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded a file larger than 2MB.");

                        return BadRequest(new APIResponse { Success = false, Message = "File larger than 2MB." });
                    }

                    // Store icon image to Azure storage
                    APIResponse resUpload = await _azureStorage.UploadAsync(fileUpload.files);
                    APIResponse resDelete = new APIResponse { Success = true };

                    // Check 1.3: Old icon service not null
                    //if (user.Image != null)
                    //{
                    //    // Delete old icon service
                    //    resDelete = await _azureStorage.DeleteAsync(user.Image);
                    //}
                    if (resUpload.Success && resDelete.Success)
                    {
                        bool result = await _userRepository.UpdateImage(userId, ((Blob)resUpload.Data).Name!);
                        if (!result)
                        {
                            return Conflict(new APIResponse { Success = false, Message = "Change image failure." });
                        }
                        _response = new APIResponse { Success = true, Message = "Change image successfully." };
                    }
                }
                catch (Exception)
                {
                    return Conflict(new APIResponse { Success = false, Message = "Change image failure(catch)." });
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User changed their image successfully.");

                return Ok(_response);
            }
        }

        [HttpGet("view-profile")]
        public async Task<IActionResult> ViewProfile()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }
            else
            {
                int userId = Common.GetUserId(HttpContext);
                var user = await _userRepository.GetUserByUserId(userId);

                if (user == null)
                {
                    return NotFound();
                }

                var userDTO = _mapper.Map<UserProfileResponse>(user);
                var role = await _userRepository.GetRoleByUserId(userId);

                if (role != null)
                {
                    userDTO.Role = role.Split(" ")[0];
                    if (String.IsNullOrWhiteSpace(userDTO.Role))
                    {
                        userDTO.Role = "No role";
                    }
                }
                else
                {
                    userDTO.Role = "Error";
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed their profile.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get user data successfully",
                    Data = userDTO
                });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest userRequest)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }
            else
            {
                var userId = Common.GetUserId(HttpContext);
                var user = await _userRepository.GetUserByUserId(userId);

                if (user == null)
                {
                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "Cannot find this user"
                    });
                }

                var checkOldPassword = await _userRepository.CheckCurrentPasswordByUserId(userId, userRequest.OldPassword);

                if (!checkOldPassword)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Old password is not correct"
                    });
                }

                if (userRequest.OldPassword.Equals(userRequest.NewPassword))
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "The new password cannot be same as the old password"
                    });
                }

                var rs = await _userRepository.ChangePassword(userId, userRequest.NewPassword);

                if (rs)
                {
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Change password successfully"
                    });
                }
                else
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Change password failure"
                    });
                }
            }
        }

        #region Old change pw
        //[HttpPost("check-current-password/{userId}")]
        //public async Task<IActionResult> CheckCurrentPassword([FromRoute] int userId, [FromForm] string currentPassword)
        //{
        //    check input current password
        //    if (!ModelState.IsValid || String.IsNullOrWhiteSpace(currentPassword))
        //    {
        //        return BadRequest(new APIResponse { Success = false, Message = "Please enter required field." });
        //    }
        //    if (!_userRepository.IsValidPassword(currentPassword))
        //    {
        //        return BadRequest(new APIResponse { Success = false, Message = "Password is wrong format." });
        //    }

        //    try
        //    {
        //        check current password is valid or not
        //       bool result = await _userRepository.CheckCurrentPasswordByUserId(userId, currentPassword);
        //        if (!result)
        //        {
        //            throw new Exception();
        //        }

        //        check current password of user successfully
        //        _response = new APIResponse { Success = true, Message = "Current password is valid." };
        //    }
        //    catch (Exception)
        //    {
        //        check current password of user failure
        //        _response = new APIResponse { Success = false, Message = "Current password is invalid." };
        //        return NotFound(_response);
        //    }
        //    return Ok(_response);
        //}

        //[HttpPatch("change-current-password/{userId}")]
        //public async Task<IActionResult> ChangeCurrentPassword([FromRoute] int userId, [FromBody] JsonPatchDocument patchUser)
        //{
        //    check input new password
        //    var operation = patchUser.Operations;
        //    var newPass = "";
        //    var type = "";
        //    if (!ModelState.IsValid || operation.Count == 0)
        //    {
        //        return BadRequest(new APIResponse { Success = false, Message = "Please enter required field." });
        //    }
        //    foreach (var item in operation)
        //    {
        //        type = item.op;
        //        if (item.value != null)
        //        {
        //            newPass = item.value.ToString();
        //        }
        //    }
        //    if (type.Equals("replace") && (String.IsNullOrWhiteSpace(newPass) || !_userRepository.IsValidPassword(newPass)))
        //    {
        //        return BadRequest(new APIResponse { Success = false, Message = "Password is wrong format." });
        //    }

        //    var same = await _userRepository.CheckCurrentPasswordByUserId(userId, newPass);
        //    if (same)
        //    {
        //        return BadRequest(new APIResponse { Success = false, Message = "New password must be different from current password." });
        //    }

        //    try
        //    {
        //        check to change current password successfully or not
        //        bool result = await _userRepository.ChangeCurrentPasswordByUserId(userId, patchUser);
        //        if (!result)
        //        {
        //            throw new Exception();
        //        }

        //        change current password of user successfully
        //        _response = new APIResponse { Success = true, Message = "Current password is changed successfully." };
        //    }
        //    catch (Exception)
        //    {
        //        change current password of user failure
        //        _response = new APIResponse { Success = false, Message = "Current password is changed failure." };
        //        return Conflict(_response);
        //    }
        //    return Ok(_response);
        //}

        #endregion

        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm] UserAddRequest userRequest)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);
            var genders = new List<int> { 1, 2, 3 };

            if (role.Contains("User7") || role.Contains("User15"))
            {
                if (!ModelState.IsValid || userRequest == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Updated data can not be null"
                    });
                }

                if (!genders.Contains(userRequest.GenderId))
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid genderId");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "GenderId is invalid"
                    });
                }

                if (userRequest.Email == null || userRequest.Password == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an empty email or password.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Email or Password is null"
                    });
                }

                if (await _userRepository.CheckExistedEmail(userRequest.Email))
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an email that is already registered.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Email already registered!"
                    });
                }

                if (!_userRepository.IsValidEmail(userRequest.Email))
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid email.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Invalid email format!"
                    });
                }

                if (!_userRepository.IsValidPassword(userRequest.Password))
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered a password with an invalid format.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Invalid password!"
                    });
                }

                if (userRequest.PhoneNumber != null && !_userRepository.IsValidPhoneNumber(userRequest.PhoneNumber)
                    || userRequest.PhoneNumber == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid phone number.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Invalid phone number format!"
                    });
                }

                // Check role valid
                if (userRequest.Role.MAdmin)
                {
                    if (userRequest.Role.MBOM || userRequest.Role.MQAO)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "User role is invalid"
                        });
                    }
                }
                else if (userRequest.Role.MQAO)
                {
                    if (userRequest.Role.MBOM || userRequest.Role.MAdmin)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "User role is invalid"
                        });
                    }
                }
                else if (userRequest.Role.MBOM)
                {
                    if (userRequest.Role.MAdmin || userRequest.Role.MQAO)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Divide authorize failure"
                        });
                    }
                }
                else if (!userRequest.Role.MAdmin && !userRequest.Role.MQAO && !userRequest.Role.MBOM)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Divide authorize failure"
                    });
                }

                // Check image valid
                var fileUpload = userRequest.Image;
                string fileName = "";

                if (fileUpload != null)
                {
                    // Check 1: File extension
                    if (!Constants.IMAGE_EXTENSIONS.Contains(Path.GetExtension(fileUpload.FileName)))
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an invalid image file.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Please choose a file with extension .jpg or .png"
                        });
                    }
                    // Check 2: File size
                    if (fileUpload.Length > Constants.IMAGE_LIMIT_SIZE)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded a file larger than 2MB.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "File larger than 2MB."
                        });
                    }
                    // Handle Image
                    APIResponse res = await _azureStorage.UploadAsync(fileUpload);

                    if (res.Success)
                    {
                        fileName = ((Blob)res.Data).Name!;
                    }
                    else
                    {
                        return BadRequest(res);
                    }
                }

                // Add user
                var newUser = new User
                {
                    Email = userRequest.Email,
                    Password = _userRepository.HashPassword(userRequest.Password),
                    FirstName = userRequest.FirstName,
                    LastName = userRequest.LastName,
                    GenderId = userRequest.GenderId,
                    Dob = userRequest.Dob,
                    PhoneNumber = userRequest.PhoneNumber,
                    Address = userRequest.Address,
                    // Null or Not Null
                    Image = String.IsNullOrEmpty(fileName) ? null : fileName,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    Status = userRequest.Status,
                };

                var newUserRole = new RoleDistribution
                {
                    MAdmin = userRequest.Role.MAdmin,
                    MQAO = userRequest.Role.MQAO,
                    MBOM = userRequest.Role.MBOM,
                    MSystem = userRequest.Role.MSystem,
                    MUser = userRequest.Role.MUser,
                    MService = userRequest.Role.MService,
                    MCriteria = userRequest.Role.MCriteria,
                };

                var addSuccess = await _userRepository.AddUser(newUser, newUserRole);

                if (addSuccess == false)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Add new user failure"
                    });
                }

                // send email with new user
                var emailInfo = Common.GetEmailInfo(userRequest.Email, userRequest.FirstName, "new account", "logging into", userRequest.Password);
                bool sentPassword = Common.SendMail(emailInfo);

                if (!sentPassword)
                {
                    return Problem("Send passwrod failure");
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "The user successfully added a new user.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Add new user successfully"
                });
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var userId = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("User1") || role.Contains("User7") || role.Contains("User15"))
            {
                var users = await _userRepository.GetUsers(userId);

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "The user viewed the list of users.");

                if (users.Count == 0)
                {
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "The list of users is empty"
                    });
                }

                // Handle map data
                var userDTOs = await MappingUser(users);
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get users successfully",
                    Data = userDTOs
                });
            }
            else
            {
                return Forbid();
            }
        }

        private async Task<List<UserResponse>> MappingUser(List<User> users)
        {
            var userDTOs = new List<UserResponse>();
            foreach (var user in users)
            {
                var userDTO = _mapper.Map<UserResponse>(user);
                var role = await _userRepository.GetRoleByUserId(user.UserId);
                if (role != null)
                {
                    userDTO.Role = role.Split(" ")[0];
                    if (String.IsNullOrWhiteSpace(userDTO.Role))
                    {
                        userDTO.Role = "No role";
                    }
                }
                else
                {
                    userDTO.Role = "Error";
                }
                userDTOs.Add(userDTO);
            }
            return userDTOs;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var currentUser = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("User1") || role.Contains("User7") || role.Contains("User15"))
            {
                var oldUser = await _userRepository.GetUserByUserId(userId);

                if (currentUser == userId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "An error occurs while getting this user.",
                    });
                }

                if (oldUser == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "User is not exist.",
                    });
                }

                var userDTO = await MappingUser(oldUser);

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed another user's information details.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get user details successfully.",
                    Data = userDTO
                });
            }
            else
            {
                return Forbid();
            }
        }

        private async Task<UserDetailsResponse> MappingUser(User user)
        {
            var userDTO = _mapper.Map<UserDetailsResponse>(user);
            var role = await _userRepository.GetRoleByUserId(user.UserId);
            if (role != null)
            {
                userDTO.Role = role.Split(" ")[0];
                if (String.IsNullOrWhiteSpace(userDTO.Role))
                {
                    userDTO.Role = "No role";
                }
            }
            else
            {
                userDTO.Role = "Error";
            }
            return userDTO;
        }

        [HttpPut("{userId}/change-status")]
        public async Task<IActionResult> ChangeUserStatus(int userId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var currentUser = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("User15"))
            {
                var user = await _userRepository.GetUserByUserId(userId);

                if (currentUser == userId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "An error occurs while getting this user.",
                    });
                }

                if (user == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "User is not exist.",
                    });
                }

                var changedTime = DateTime.Now;
                bool success = await _userRepository.ChangeUserStatus(userId, changedTime);

                if (success)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User changed the status of another user.");

                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Change user status successfully"
                    });
                }
                else
                {
                    return Conflict(new APIResponse
                    {
                        Success = false,
                        Message = "Change user staus failure"
                    });
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromForm] UserUpdateRequest userUpdate)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("User7") || role.Contains("User15"))
            {
                var fileUpload = userUpdate.Image;
                string fileName = "";
                var oldUser = await _userRepository.GetUserByUserId(userId);
                var oldUserRole = await _roleDistributionRepository.GetRoleByUserId(userId);

                if (oldUser == null) return NotFound();

                if (userId != userUpdate.UserId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Id not match"
                    });
                }

                if (fileUpload != null)
                {
                    // Check 1.1: Check file extension
                    if (!Constants.IMAGE_EXTENSIONS.Contains(Path.GetExtension(fileUpload.FileName)))
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an invalid image file.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Please choose a file with extension .jpg or .png"
                        });
                    }
                    // Check 1.2: Check file size
                    if (fileUpload.Length > Constants.IMAGE_LIMIT_SIZE)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded a file larger than 2MB.");
                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "File larger than 2MB."
                        });
                    }

                    // Store icon image to Azure storage
                    APIResponse res = await _azureStorage.UploadAsync(fileUpload);

                    if (res.Success)
                    {
                        fileName = ((Blob)res.Data).Name!;
                    }
                    else
                    {
                        return BadRequest(res);
                    }

                    // Check 1.3: Old image user not null
                    //if (oldUser.Image != null)
                    //{
                    //    // Delete old user image
                    //    res = await _azureStorage.DeleteAsync(oldUser.Image);
                    //}
                }

                if (userUpdate.Role != null)
                {
                    // Check role valid
                    if (userUpdate.Role.MAdmin)
                    {
                        if (userUpdate.Role.MBOM || userUpdate.Role.MQAO)
                        {
                            // Add log
                            await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                            return BadRequest(new APIResponse
                            {
                                Success = false,
                                Message = "Divide authorize failure"
                            });
                        }
                    }
                    else if (userUpdate.Role.MQAO)
                    {
                        if (userUpdate.Role.MBOM || userUpdate.Role.MAdmin)
                        {
                            // Add log
                            await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                            return BadRequest(new APIResponse
                            {
                                Success = false,
                                Message = "Divide authorize failure"
                            });
                        }
                    }
                    else if (userUpdate.Role.MBOM)
                    {
                        if (userUpdate.Role.MAdmin || userUpdate.Role.MQAO)
                        {
                            // Add log
                            await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                            return BadRequest(new APIResponse
                            {
                                Success = false,
                                Message = "Divide authorize failure"
                            });
                        }
                    }
                    else if (!userUpdate.Role.MAdmin && !userUpdate.Role.MQAO && !userUpdate.Role.MBOM)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User entered an invalid role.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Divide authorize failure"
                        });
                    }
                }

                // Update user
                var newUser = new User
                {
                    UserId = userId,
                    Password = userUpdate.Password is null ? oldUser.Password : _userRepository.HashPassword(userUpdate.Password),
                    FirstName = userUpdate.FirstName is null ? oldUser.FirstName : userUpdate.FirstName,
                    LastName = userUpdate.LastName is null ? oldUser.LastName : userUpdate.LastName,
                    GenderId = userUpdate.GenderId is null ? oldUser.GenderId : userUpdate.GenderId.Value,
                    Dob = userUpdate.Dob,
                    PhoneNumber = userUpdate.PhoneNumber is null ? oldUser.PhoneNumber : userUpdate.PhoneNumber,
                    Address = userUpdate.Address,
                    // New Image or No action
                    Image = String.IsNullOrEmpty(fileName) ? oldUser.Image : fileName,
                    CreatedOn = oldUser.CreatedOn,
                    ModifiedOn = DateTime.Now,
                    Status = userUpdate.Status is null ? oldUser.Status : userUpdate.Status.Value,
                };

                var newUserRole = new RoleDistribution
                {
                    MAdmin = userUpdate.Role == null ? oldUserRole!.MAdmin : userUpdate.Role.MAdmin,
                    MQAO = userUpdate.Role == null ? oldUserRole!.MQAO : userUpdate.Role.MQAO,
                    MBOM = userUpdate.Role == null ? oldUserRole!.MBOM : userUpdate.Role.MBOM,
                    MSystem = userUpdate.Role == null ? oldUserRole!.MSystem : userUpdate.Role.MSystem,
                    MUser = userUpdate.Role == null ? oldUserRole!.MUser : userUpdate.Role.MUser,
                    MService = userUpdate.Role == null ? oldUserRole!.MService : userUpdate.Role.MService,
                    MCriteria = userUpdate.Role == null ? oldUserRole!.MCriteria : userUpdate.Role.MCriteria,
                };

                var addSuccess = await _userRepository.UpdateUser(newUser, newUserRole);

                if (addSuccess == false)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Update user failure"
                    });
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "The user successfully updated another user's information.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Update user successful"
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}
