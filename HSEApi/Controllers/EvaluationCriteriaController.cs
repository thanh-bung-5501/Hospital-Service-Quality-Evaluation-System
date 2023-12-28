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
    public class EvaluationCriteriaController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEvaluationCriteriaRepository _evaCriRepository;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IMapper _mapper;
        public EvaluationCriteriaController(IServiceRepository serviceRepository, IUserRepository userRepository,
            IEvaluationCriteriaRepository evaCriRepository, IMapper mapper, ISystemInformationRepository systemInformationRepository)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _evaCriRepository = evaCriRepository;
            _mapper = mapper;
            _systemInformationRepository = systemInformationRepository;
        }

        [HttpGet]
        //public async Task<IActionResult> GetAllEvaluationCriteria([FromQuery] FilteredResponse filteredResponse, [FromQuery] EvaluationCriteriaRequest request)
        //public async Task<IActionResult> GetAllEvaluationCriteria([FromQuery] EvaluationCriteriaRequest request)
        public async Task<IActionResult> GetAllEvaluationCriteria()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            // set valid page and page size
            //var validFilter = new FilteredResponse(filterResponse.search, filterResponse.sortedBy, filterResponse.page, filterResponse.pageSize);
            //var pagedResponse = new PagedResponse<EvaluationCriteriaResponse>();
            var dataMap = new List<EvaluationCriteriaResponse>();

            // get current user role
            var role = Common.GetUserRole(HttpContext);

            // current user role is qao or admin
            if (role != null && (role.Contains("Criteria1") || role.Contains("Criteria7") || role.Contains("Criteria15")))
            {
                // get all evaluation criteria
                //var data = await _evaCriRepository.GetAllEvaluationCriteria(role, request, validFilter);
                var data = await _evaCriRepository.GetAllEvaluationCriteria();

                // get all evaluation criteria mapping
                dataMap = _mapper.Map<List<EvaluationCriteriaResponse>>(data);
                // total data
                var totalRecords = dataMap.Count;
                // total page
                //var totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)validFilter.pageSize));

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the list of evaluation criteria.");

                // list empty
                if (totalRecords == 0)
                {
                    return Ok(new APIResponse
                    {
                        Success = true,
                        //Message = "All evaluation criteria is retrieved successfully.",
                        //Data = new PagedResponse<EvaluationCriteriaResponse>
                        //{
                        //    TotalItems = totalRecords,
                        //    Results = null,
                        //    TotalPages = totalPages,
                        //    CurrentPage = validFilter.page,
                        //    PageSize = validFilter.pageSize
                        //}
                        Message = "The list of evaluation criteria is empty."
                    });
                }

                // error message
                //if (validFilter.page > totalPages)
                //{
                //    return BadRequest(new APIResponse { Success = false, Message = "Current page must be smaller than total pages." });
                //}

                foreach (var item in dataMap)
                {
                    // each evaluation criteria mapping with user and service
                    var createdUser = await _userRepository.GetUserByUserId(item.CreatedBy);
                    var modifiedUser = await _userRepository.GetUserByUserId(item.ModifiedBy);
                    var service = await _serviceRepository.GetService(item.SerId);
                    if (createdUser != null || modifiedUser != null)
                    {
                        MappingCriteria(item, createdUser, modifiedUser, service);
                    }
                }

                // pagination by page and page size
                //var pagedData = dataMap.Skip((validFilter.page - 1) * validFilter.pageSize).Take(validFilter.pageSize).ToList();

                // get paged response data
                //pagedResponse = new PagedResponse<EvaluationCriteriaResponse>
                //{
                //    TotalItems = totalRecords,
                //    Results = pagedData,
                //    TotalPages = totalPages,
                //    CurrentPage = validFilter.page,
                //    PageSize = validFilter.pageSize
                //};
            }
            else
            {
                return Forbid();
            }

            // success message
            return Ok(new APIResponse { Success = true, Message = "All evaluation criteria retrieved successfully.", Data = dataMap });
        }

        [HttpGet("{criId}")]
        public async Task<IActionResult> GetEvaluationCriteria([FromRoute] int criId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var evaCriMap = new EvaluationCriteriaResponse();
            // get current user role
            var role = Common.GetUserRole(HttpContext);

            // current user role
            if (role != null && (role.Contains("Criteria1") || role.Contains("Criteria7") || role.Contains("Criteria15")))
            {
                // get evaluation criteria by id
                var evaCri = await _evaCriRepository.GetEvaluationCriteria(criId);

                // error message
                if (evaCri == null)
                {
                    return NotFound(new APIResponse { Success = false, Message = "Evaluation criteria is not exist." });
                }

                // get evaluation criteria mapping
                evaCriMap = _mapper.Map<EvaluationCriteriaResponse>(evaCri);

                // evaluation criteria mapping with user and service
                var createdUser = await _userRepository.GetUserByUserId(evaCriMap.CreatedBy);
                var modifiedUser = await _userRepository.GetUserByUserId(evaCriMap.ModifiedBy);
                var service = await _serviceRepository.GetService(evaCriMap.SerId);
                if (createdUser != null || modifiedUser != null)
                {
                    MappingCriteria(evaCriMap, createdUser, modifiedUser, service);
                }
            }
            else
            {
                return Forbid();
            }


            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the evaluation criteria details.");

            // succes message
            return Ok(new APIResponse { Success = true, Message = "Evaluation criteria is retrieved successfully.", Data = evaCriMap });
        }
        private void MappingCriteria(EvaluationCriteriaResponse item, User? createdUser, User? modifiedUser, Service? service)
        {
            // check created and modified user
            item.Users = new List<UserResponse>();

            // created user is different from modified user
            if (createdUser != null && modifiedUser != null && item.CreatedBy != item.ModifiedBy)
            {
                item.Users.Add(_mapper.Map<UserResponse>(createdUser));
                item.Users.Add(_mapper.Map<UserResponse>(modifiedUser));
            }
            // modified user is null
            else if (createdUser != null)
            {
                item.Users.Add(_mapper.Map<UserResponse>(createdUser));
            }
            // created user is null
            else if (modifiedUser != null)
            {
                item.Users.Add(_mapper.Map<UserResponse>(modifiedUser));
            }
            item.Service = _mapper.Map<ServiceResponse>(service);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvaluationCriteria([FromBody] EvaluationCriteriaAddRequest request)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var existEvaCri = new EvaluationCriteria();
            // get current user
            var userId = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            // current user role is qao or admin
            if (role != null && (role.Contains("Criteria7") || role.Contains("Criteria15")))
            {
                // check requested body
                // set error message
                APIResponse response = new APIResponse { Success = false, Message = "Evaluation criteria request is wrong format." };
                if (!ModelState.IsValid)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // check service
                response.Message = "Service request is wrong format.";
                if (request.SerId <= 0)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // check exist description
                existEvaCri = await _evaCriRepository.GetEvaluationCriteriaByDescription(request.CriDesc!);
                response.Message = "New description request is exist.";
                if (existEvaCri != null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                    return BadRequest(response);
                }

                // get new evaluation criteria mapping
                var newEvaCri = _mapper.Map<EvaluationCriteria>(request);
                var now = DateTime.Now;

                newEvaCri.CreatedOn = now;
                newEvaCri.CreatedBy = userId;
                newEvaCri.ModifiedOn = now;
                newEvaCri.ModifiedBy = userId;

                // add new evaluation criteria
                var newId = await _evaCriRepository.AddEvaluationCriteria(newEvaCri);
                response.Message = "New evaluation criteria is added failure.";
                if (newId == 0)
                {
                    return Conflict(response);
                }

                // response new evaluation criteria after adding
                existEvaCri = await _evaCriRepository.GetEvaluationCriteria(newId);
            }
            else
            {
                return Forbid();
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "The user successfully added new evalation criteria.");

            return Ok(new APIResponse { Success = true, Message = "New evaluation criteria is added successfully.", Data = existEvaCri });
        }

        [HttpPut("{criId}")]
        public async Task<IActionResult> EditEvaluationCriteria([FromRoute] int criId, [FromBody] EvaluationCriteriaEditRequest request)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var existEvaCri = new EvaluationCriteria();
            // get current user
            var userId = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            // current user role is qao or admin
            if (role != null && (role.Contains("Criteria7") || role.Contains("Criteria15")))
            {
                // check requested body
                // set error message
                APIResponse response = new APIResponse { Success = false, Message = "Evaluation criteria request is wrong format." };
                if (!ModelState.IsValid)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // check evaCri id matched
                response.Message = "Id request is not matching.";
                if (criId != request.CriId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // check service
                response.Message = "Service request is wrong format.";
                if (request.SerId <= 0)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // check evaluation criteria exist
                existEvaCri = await _evaCriRepository.GetEvaluationCriteria(criId);
                response.Message = "Evaluation criteria request is not exist.";
                if (existEvaCri == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return NotFound(response);
                }

                // check exist description
                existEvaCri = await _evaCriRepository.GetEvaluationCriteriaByDescription(request.CriDesc!);
                response.Message = "New description request is exist.";
                if (existEvaCri != null && existEvaCri.CriId != criId)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return BadRequest(response);
                }

                // get current evaluation criteria mapping
                var newEvaCri = _mapper.Map<EvaluationCriteria>(request);
                newEvaCri.ModifiedOn = DateTime.Now;
                newEvaCri.ModifiedBy = userId;

                // update current evaluation criteria
                var editId = await _evaCriRepository.EditEvaluationCriteria(newEvaCri);
                response.Message = "Current evaluation criteria is updated failure.";
                if (editId == 0)
                {
                    return Conflict(response);
                }

                // response current evaluation criteria after editing
                existEvaCri = await _evaCriRepository.GetEvaluationCriteria(editId);
            }
            else
            {
                return Forbid();
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "The user successfully updated the evaluation criteria.");

            return Ok(new APIResponse { Success = true, Message = "Current evaluation criteria is updated successfully.", Data = existEvaCri });
        }

        [HttpPut("change-status/{criId}")]
        public async Task<IActionResult> ChagneStatus([FromRoute] int criId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var existEvaCri = new EvaluationCriteria();
            // get current user
            var userId = Common.GetUserId(HttpContext);
            var role = Common.GetUserRole(HttpContext);

            // current user role is qao or admin
            if (role != null && role.Contains("Criteria15"))
            {
                // check evaluation criteria exist
                existEvaCri = await _evaCriRepository.GetEvaluationCriteria(criId);
                APIResponse response = new APIResponse { Success = false, Message = "Evaluation criteria request is not exist." };
                if (existEvaCri == null)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return NotFound(response);
                }

                // deactivate status current evaluation criteria
                existEvaCri.ModifiedOn = DateTime.Now;
                existEvaCri.ModifiedBy = userId;
                var deactId = await _evaCriRepository.ChagneStatus(existEvaCri);

                // set error message
                response.Message = (bool)existEvaCri.Status! ? "Current evaluation criteria is deactivated failure." :
                    "Current evaluation criteria is activated failure.";
                if (deactId == 0)
                {
                    // Add log
                    await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                    return Conflict(response);
                }

                // response current evaluation criteria after deactivating
                existEvaCri = await _evaCriRepository.GetEvaluationCriteria(deactId);
            }
            else
            {
                return Forbid();
            }

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User changed evaluation criteria status.");

            return Ok(new APIResponse
            {
                Success = true,
                Message = (bool)existEvaCri.Status! ? "Current evaluation criteria is activated successfully." :
                "Current evaluation criteria is deactivated successfully.",
                Data = existEvaCri
            });
        }
    }
}
