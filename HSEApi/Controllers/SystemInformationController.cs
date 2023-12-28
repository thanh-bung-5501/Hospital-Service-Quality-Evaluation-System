using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Repositories.Utils;
using SystemObjects;

namespace HSEApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInformationController : ControllerBase
    {
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAzureStorage _azureStorage;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public SystemInformationController(ISystemInformationRepository systemInformationRepository, IUserRepository userRepository, IAzureStorage azureStorage, IMapper mapper)
        {
            _systemInformationRepository = systemInformationRepository;
            _userRepository = userRepository;
            _azureStorage = azureStorage;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetSystemInformation()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            int SysId = 1;
            try
            {
                //check right
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckViewRights(roleString, "System"))
                {
                    return Forbid();
                }

                var systemInformation = await _systemInformationRepository.GetSystemInformationBySysId(SysId);
                if (systemInformation == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound(new APIResponse { Success = false, Message = "This System Information is not available." });
                }
                _response = new APIResponse { Success = true, Message = "Get System Information successfully.", Data = systemInformation };
            }
            catch (Exception)
            {
                return Conflict(new APIResponse { Success = false, Message = "Get System Information failure(catch)." });
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed system's information.");

            return Ok(_response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSystemInformation([FromForm] SystemInformationUpdateRequet request)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                //check right
                var roleString = Common.GetUserRole(HttpContext);
                if (!Common.CheckCreateAndUpdateRights(roleString, "System"))
                {
                    return Forbid();
                }

                if (!_systemInformationRepository.IsValidPhoneNumber(request.Hotline)) //veryfy Hotline
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                    return BadRequest(new APIResponse { Success = false, Message = "Hotline is invalid phone number." });
                }
                if (!_systemInformationRepository.IsValidPhoneNumber(request.Zalo)) //veryfy Hotline
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                    return BadRequest(new APIResponse { Success = false, Message = "Zalo is invalid phone number." });
                }

                //check null and get old System Information
                var oldSettings = await _systemInformationRepository.GetSystemInformationBySysId(1);
                if (oldSettings == null)
                {
                    return NotFound();
                }

                var newSettings = new SystemInformation
                {
                    SysId = 1,
                    SysName = request.SysName,
                    Logo = oldSettings.Logo,
                    Icon = oldSettings.Icon,
                    Zalo = request.Zalo,
                    Hotline = request.Hotline,
                    Address = request.Address,
                };

                if (request.Logo != null) //logo not null
                {
                    if (!Constants.IMAGE_EXTENSIONS.Contains(Path.GetExtension(request.Logo.FileName))) //check file extension
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an invalid logo file.");

                        return BadRequest(new APIResponse { Success = false, Message = "Please choose a file with extension .jpg or .png" });
                    }
                    if (request.Logo.Length > Constants.IMAGE_LIMIT_SIZE) //check file capacity
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded a file larger than 2MB.");

                        return BadRequest(new APIResponse { Success = false, Message = "Logo file larger than 2MB." });
                    }

                    // Store icon image to Azure storage
                    APIResponse resLogoUpload = await _azureStorage.UploadAsync(request.Logo);
                    if (resLogoUpload.Success)
                    {
                        newSettings.Logo = ((Blob)resLogoUpload.Data).Name!;
                    }
                }

                if (request.Icon != null) //icon not null
                {
                    if (!Constants.ICON_IMAGE_EXTENSIONS.Contains(Path.GetExtension(request.Icon.FileName))) //check file extension
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded an invalid icon file.");

                        return BadRequest(new APIResponse { Success = false, Message = "Please choose a file with extension .png or .ico" });
                    }
                    if (request.Icon.Length > Constants.IMAGE_LIMIT_SIZE) //check file capacity
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User uploaded a file larger than 2MB.");

                        return BadRequest(new APIResponse { Success = false, Message = "Icon file larger than 2MB." });
                    }

                    // Store icon image to Azure storage
                    APIResponse resIconUpload = await _azureStorage.UploadAsync(request.Icon);
                    if (resIconUpload.Success)
                    {
                        newSettings.Icon = ((Blob)resIconUpload.Data).Name!;
                    }
                }

                bool result = await _systemInformationRepository.UpdateSystemInformation(newSettings);
                if (!result)
                {
                    return Conflict(new APIResponse { Success = false, Message = "Update System Information failure." });
                }
                _response = new APIResponse { Success = true, Message = "Update System Information successfully." };
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User updated system's information.");

            return Ok(_response);
        }

        [HttpGet("get-log")]
        public async Task<IActionResult> GetSystemLog(DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid."
                });
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Admin"))
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed system's log.");

                var data = await _systemInformationRepository.GetSystemLog(dateFrom, dateTo);
                var sysLogDTOs = new List<SystemLogResponse>();

                foreach (var item in data)
                {
                    var user = await _userRepository.GetUserByUserId(item.UserId);

                    sysLogDTOs.Add(new SystemLogResponse
                    {
                        LogId = item.LogId,
                        Email = user == null ? "Anonymous" : user.Email,
                        Note = item.Note,
                        CreatedOn = item.CreatedOn,
                    });
                }

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get system log successfully.",
                    Data = sysLogDTOs
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}
