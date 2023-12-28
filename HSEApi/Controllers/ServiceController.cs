using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Repositories.Utils;
using SystemObjects;


namespace HSEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAzureStorage _azureStorage;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IMapper _mapper;

        public ServiceController(IServiceRepository serviceRepository, IUserRepository userRepository, IAzureStorage azureStorage,
            ISystemInformationRepository systemInformationRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _azureStorage = azureStorage;
            _systemInformationRepository = systemInformationRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Service1") || role.Contains("Service7") || role.Contains("Service15"))
            {
                var services = await _serviceRepository.GetServices();
                var serviceDTOs = new List<ServiceResponse>();

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the list of services.");

                if (services.Count() == 0)
                {
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "The list of services is empty",
                    });
                }

                // Handle map data
                await MappingService(services, serviceDTOs);

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Services listed successfully",
                    Data = serviceDTOs
                });
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet("list-of-service-name")]
        public async Task<IActionResult> GetServicesForServiceComparison()
        {
            var services = await _serviceRepository.GetServices();
            var serviceDTOs = _mapper.Map<List<ServiceResponseForPatient>>(services);

            if (services.Count() == 0)
            {
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "The list of services is empty",
                });
            }

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Services listed successfully",
                Data = serviceDTOs
            });
        }

        private async Task MappingService(List<Service> services, List<ServiceResponse> serviceDTOs)
        {
            foreach (var item in services)
            {
                var serviceDTO = _mapper.Map<ServiceResponse>(item);
                var createdUser = await _userRepository.GetUserByUserId(item.CreatedBy!.Value);

                if (createdUser != null)
                {
                    serviceDTO.CreatedBy = createdUser.FirstName + " " + createdUser.LastName;
                }
                else
                {
                    serviceDTO.CreatedBy = "ADMIN";
                }

                if (item.ModifiedBy != null)
                {
                    var modifiedUser = await _userRepository.GetUserByUserId(item.ModifiedBy!.Value);
                    if (modifiedUser != null)
                    {
                        serviceDTO.ModifiedBy = modifiedUser.FirstName + " " + modifiedUser.LastName;
                    }
                }
                else
                {
                    serviceDTO.ModifiedBy = "ADMIN";
                }
                serviceDTOs.Add(serviceDTO);
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetServices([FromQuery] FilteredResponse filtered)
        //{
        //    var role = Common.GetUserRole(HttpContext);

        //    if (role.Contains("Service1") || role.Contains("Service7") || role.Contains("Service15"))
        //    {
        //        // set valid page and page size
        //        var validFilter = new FilteredResponse(filtered.search, filtered.sortedBy, filtered.page, filtered.pageSize);
        //        var services = await _serviceRepository.GetServices(validFilter);
        //        int totalItems = services.Count();
        //        int totalPages = (int)Math.Ceiling(totalItems / (double)validFilter.pageSize);

        //        if (totalItems == 0)
        //        {
        //            return Ok(new APIResponse
        //            {
        //                Success = true,
        //                Message = "Services are listed successfully",
        //                Data = new PagedResponse<ServiceResponse>
        //                {
        //                    TotalItems = totalItems,
        //                    Results = null,
        //                    TotalPages = totalPages,
        //                    CurrentPage = validFilter.page,
        //                    PageSize = Constants.PAGE_SIZE
        //                }
        //            });
        //        }

        //        // Paging
        //        if (validFilter.page <= totalPages)
        //        {
        //            services = services.Skip((validFilter.page - 1) * validFilter.pageSize)
        //                .Take(validFilter.pageSize).ToList();
        //        }
        //        else
        //        {
        //            return BadRequest(new APIResponse
        //            {
        //                Success = false,
        //                Message = "Current page larger than total pages"
        //            });
        //        }

        //        var serviceDTOs = _mapper.Map<List<ServiceResponse>>(services);

        //        return Ok(new APIResponse
        //        {
        //            Success = true,
        //            Message = "Services listed successfully",
        //            Data = new PagedResponse<ServiceResponse>
        //            {
        //                TotalItems = totalItems,
        //                Results = serviceDTOs,
        //                TotalPages = totalPages,
        //                CurrentPage = validFilter.page,
        //                PageSize = validFilter.pageSize
        //            }
        //        });
        //    }
        //    else
        //    {
        //        return Forbid();
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Service1") || role.Contains("Service7") || role.Contains("Service15"))
            {
                var service = await _serviceRepository.GetService(id);

                if (service == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                    return NotFound();
                }

                var serviceDTO = _mapper.Map<ServiceResponse>(service);

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the details of the service.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get service successfully",
                    Data = serviceDTO
                });
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetService2(int id)
        {
            var service = await _serviceRepository.GetService(id);

            if (service == null)
            {
                return NotFound();
            }

            var serviceDTO = _mapper.Map<ServiceResponse>(service);

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get service successfully",
                Data = serviceDTO
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromForm] ServiceCreateRequest serviceRequest)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Service7") || role.Contains("Service15"))
            {
                var fileUpload = serviceRequest.Icon;

                try
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
                        var newService = _mapper.Map<Service>(serviceRequest);

                        newService.CreatedBy = Common.GetUserId(HttpContext);
                        newService.ModifiedBy = Common.GetUserId(HttpContext);

                        bool result = await _serviceRepository.CreateService(newService, ((Blob)res.Data).Name!);

                        if (!result)
                        {
                            // Add fail
                            return Conflict(new APIResponse
                            {
                                Success = false,
                                Message = "New service is added failure"
                            });
                        }
                        else
                        {
                            // Add log
                            await _systemInformationRepository.AddSystemLog(HttpContext, "User added new service.");

                            // Add success
                            return Ok(new APIResponse
                            {
                                Success = true,
                                Message = "New service is added successfully"
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(res);
                    }
                }
                catch (Exception ex)
                {
                    return Conflict(ex.Message);
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] ServiceUpdateRequest serviceUpdate)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Service7") || role.Contains("Service15"))
            {
                try
                {
                    var fileUpload = serviceUpdate.Icon;
                    bool result = false;
                    var oldService = await _serviceRepository.GetService(id);

                    if (oldService == null)
                    {
                        return NotFound();
                    }

                    if (id != serviceUpdate.SerId)
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                        return BadRequest(new APIResponse
                        {
                            Success = false,
                            Message = "Id not match"
                        });
                    }

                    // Check 1: Update service icon
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

                        // Check 1.3: Old icon service not null
                        //if (oldService.Icon != null)
                        //{
                        //    // Delete old icon service
                        //    res = await _azureStorage.DeleteAsync(oldService.Icon);
                        //}

                        if (res.Success)
                        {
                            var newService = _mapper.Map<Service>(serviceUpdate);
                            newService.ModifiedBy = Common.GetUserId(HttpContext);
                            result = await _serviceRepository.UpdateService(newService, ((Blob)res.Data).Name!);
                        }
                        else
                        {
                            return BadRequest(res);
                        }
                    }
                    // Check 2: Non-update service icon
                    else
                    {
                        var newService = _mapper.Map<Service>(serviceUpdate);
                        newService.ModifiedBy = Common.GetUserId(HttpContext);
                        result = await _serviceRepository.UpdateService(newService, null);
                    }

                    if (!result)
                    {
                        // Update fail
                        return Conflict(new APIResponse
                        {
                            Success = false,
                            Message = "The service is updated failure"
                        });
                    }
                    else
                    {
                        // Add log
                        await _systemInformationRepository.AddSystemLog(HttpContext, "User updated service's information.");

                        // Update success
                        return Ok(new APIResponse
                        {
                            Success = true,
                            Message = "The service is updated successfully"
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Conflict(ex.Message);
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPut("{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var role = Common.GetUserRole(HttpContext);

            if (role.Contains("Service15"))
            {
                var service = await _serviceRepository.GetService(id);

                if (service == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return NotFound();
                }

                int userId = Common.GetUserId(HttpContext);
                var result = await _serviceRepository.ChangeStatus(id, userId);

                if (!result)
                {
                    // Change fail
                    return Conflict(new APIResponse
                    {
                        Success = false,
                        Message = "Update status failure"
                    });
                }
                else
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User changed service's status.");

                    // Delete success
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Change status successfully"
                    });
                }
            }
            else
            {
                return Forbid();
            }
        }
    }
}
