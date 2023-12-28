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
    public class RoleController : ControllerBase
    {
        private IUserRepository _userRepository;
        private IRoleDistributionRepository _roleDistributionRepository;
        private ISystemInformationRepository _systemInformationRepository;
        private IMapper _mapper;
        private APIResponse _response;

        public RoleController(IUserRepository userRepository, IRoleDistributionRepository roleDistributionRepository,
            ISystemInformationRepository systemInformationRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleDistributionRepository = roleDistributionRepository;
            _systemInformationRepository = systemInformationRepository;
            _mapper = mapper;
        }

        [HttpGet("role-details/{userId}")]
        public async Task<IActionResult> RoleDetails(int userId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                // check right
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckViewRights(roleString, "User"))
                {
                    return Forbid();
                }

                var role = await _roleDistributionRepository.GetRoleByUserId(userId);
                if (role == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse { Success = false, Message = "This user is not available." });
                }
                var user = await _userRepository.GetUserByUserId(userId);
                if (user == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse { Success = false, Message = "This user is not available." });
                }
                //mapper
                var roleReponse = _mapper.Map<RoleDistributionResponse>(role);
                roleReponse.Email = user.Email;
                _response = new APIResponse { Success = true, Message = "Get role details successfully.", Data = roleReponse };
            }
            catch (Exception)
            {
                return Conflict(new APIResponse { Success = false, Message = "Get role details failure." });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the role details.");

            return Ok(_response);
        }
        [HttpPost("update-role/{userId}")]
        public async Task<IActionResult> UpdateRole(int userId, RoleDistribution roleDistribution)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                if (userId != roleDistribution.UserId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return BadRequest(new APIResponse { Success = false, Message = "UserId does not match." });
                }

                //check right
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckCreateAndUpdateRights(roleString, "User"))
                {
                    return Forbid();
                }

                var result = await _roleDistributionRepository.UpdateRole(roleDistribution);
                if (!result)
                {
                    return Conflict(new APIResponse { Success = false, Message = "Update role failure." });
                }
                _response = new APIResponse { Success = true, Message = "Update role successfully." };
                // revolke user token
                await _userRepository.RevokeRefreshTokenByUserId(userId);
            }
            catch (Exception ex)
            {
                return Conflict(new APIResponse { Success = false, Message = ex.Message });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User updated another user's role.");

            return Ok(_response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                //Get adminId form token and check right
                int adminId = Common.GetUserId(HttpContext);
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckViewRights(roleString, "User"))
                {
                    return Forbid();
                }

                var roleList = new List<string>
                {
                    Constants.ADMIN, 
                    Constants.QAO, 
                    Constants.BOM
                };

                _response = new APIResponse
                {
                    Success = true,
                    Message = "Get roles successfully.",
                    Data = roleList
                };
            }
            catch (Exception)
            {
                return Conflict(new APIResponse { Success = false, Message = "Authorize fail." });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed list of role.");

            return Ok(_response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRole(int userId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                // check right
                var currentUser = Common.GetUserId(HttpContext);
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckViewRights(roleString, "User"))
                {
                    return Forbid();
                }

                if (currentUser == userId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "An error occurs while getting role of this user.",
                    });
                }

                var user = await _userRepository.GetUserByUserId(userId);
                if (user == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse { Success = false, Message = "This user is not exist." });
                }

                var role = await _roleDistributionRepository.GetRoleByUserId(userId);
                if (role == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse { Success = false, Message = "This user does not have role." });
                }

                //mapper
                var roleReponse = _mapper.Map<RoleDistributionResponse>(role);
                //roleReponse.Email = user.Email;
                _response = new APIResponse { Success = true, Message = "Get role details successfully.", Data = roleReponse };
            }
            catch (Exception)
            {
                return Conflict(new APIResponse { Success = false, Message = "Get role details failure." });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the details of user role.");

            return Ok(_response);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] RoleDistributionEditRequest request)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                //check right
                var id = Common.GetUserId(HttpContext);
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckCreateAndUpdateRights(roleString, "User"))
                {
                    return Forbid();
                }

                if (userId != request.UserId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(new APIResponse { Success = false, Message = "User id does not match." });
                }

                if (id == userId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "An error occurs while getting role of this user.",
                    });
                }

                if (request != null)
                {
                    // Check role valid
                    if (request.MAdmin)
                    {
                        if (request.MBOM || request.MQAO)
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
                    else if (request.MQAO)
                    {
                        if (request.MBOM)
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
                    else if (!request.MBOM)
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

                var changedTime = DateTime.Now;
                var currentUser = await _userRepository.GetUserByUserId(userId);
                if (currentUser == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return NotFound(new APIResponse { Success = false, Message = "User is not exist." });
                }

                //mapper
                var roleRequest = _mapper.Map<RoleDistribution>(request);
                var result = await _roleDistributionRepository.UpdateRole(roleRequest, currentUser, changedTime);

                var roleResponse = _mapper.Map<RoleDistributionResponse>(await _roleDistributionRepository.GetRoleByUserId(userId));
                _response = new APIResponse { Success = true, Message = "Update role successfully.", Data = roleResponse! };
            }
            catch (Exception)
            {
                return Conflict(new APIResponse { Success = false, Message = "Update role failure." });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User updated another user's role.");

            return Ok(_response);
        }
    }
}
