using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Repositories.Utils;

namespace HSEApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IEvaluationCriteriaRepository _evaCriRepository;
        private readonly IEvaluationDataRepository _evaDataRepository;
        private readonly IServiceFeedbackRepository _serFeedbackRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IMapper _mapper;

        public AdminController(IServiceRepository serviceRepository, IEvaluationCriteriaRepository evaCriRepository,
            IEvaluationDataRepository evaDataRepository, IServiceFeedbackRepository serFeedbackRepository,
            IPatientRepository patientRepository, ISystemInformationRepository systemInformationRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _evaCriRepository = evaCriRepository;
            _evaDataRepository = evaDataRepository;
            _serFeedbackRepository = serFeedbackRepository;
            _patientRepository = patientRepository;
            _systemInformationRepository = systemInformationRepository;
            _mapper = mapper;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard(DateTime date)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                return BadRequest(new APIResponse { Success = false, Message = "Filter request is wrong format." });
            }

            // date = 12/11/2023

            // 1/11/2023
            var firstDateInMonth = DateTime.Parse(date.ToString("MM/yyyy"));

            // 1/12/2023
            date = DateTime.Parse(date.AddMonths(1).ToString("MM/yyyy"));

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed dashboard");

            // NumberOfEvaluations
            int numberOfEvaluations = await _serFeedbackRepository.CountNumberOfEvaluations(date);
            int numberOfEvaluationsPreMonth = await _serFeedbackRepository.CountNumberOfEvaluations(date.AddMonths(-1));
            if (numberOfEvaluationsPreMonth == 0) numberOfEvaluationsPreMonth = 1;

            // PercentVsPreMonthOfNumEva
            decimal percentVsPreMonthOfNumEva =
                Common.FormatPecentageDecimal((decimal)(numberOfEvaluations - numberOfEvaluationsPreMonth) / numberOfEvaluationsPreMonth);

            int numberPatient = await _patientRepository.CountAllPatient();
            if (numberPatient == 0)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Number of the patients is zero"
                });
            }

            int numberPatientEvaluated = await _serFeedbackRepository.CountNumberOfPatientEvaluated(date);
            int numberPatientEvaluatedPreMonth = await _serFeedbackRepository.CountNumberOfPatientEvaluated(date.AddMonths(-1));
            if (numberPatientEvaluatedPreMonth == 0) numberPatientEvaluatedPreMonth = 1;

            // PercentOfPatientEvaluated
            decimal percentOfPatientEvaluated = Common.FormatPecentageDecimal((decimal)numberPatientEvaluated / numberPatient);
            decimal percentOfPatientEvaluatedPreMonth = Common.FormatPecentageDecimal((decimal)numberPatientEvaluatedPreMonth / numberPatient);

            // PercentVsPreMonthOfPatEva
            decimal percentVsPreMonthOfPatientEvaluated = Common.FormatPecentageDecimal((percentOfPatientEvaluated - percentOfPatientEvaluatedPreMonth) / percentOfPatientEvaluatedPreMonth);

            int numberCurVote = await _evaDataRepository.CountVote(date);
            if (numberCurVote == 0)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Number of the votes is zero"
                });
            }

            int numberPreVote = await _evaDataRepository.CountVote(date.AddMonths(-1));
            if (numberPreVote == 0) numberPreVote = 1;

            int numberConcurVote = await _evaDataRepository.CountConcurVote(date);
            int numberConcurVotePreMonth = await _evaDataRepository.CountConcurVote(date.AddMonths(-1));
            if (numberConcurVotePreMonth == 0) numberConcurVotePreMonth = 1;

            // PercentOfConcurLevel
            decimal percentOfConcurLevel = Common.FormatPecentageDecimal((decimal)numberConcurVote / numberCurVote);
            decimal percentOfConcurLevelPreMonth = Common.FormatPecentageDecimal((decimal)numberConcurVotePreMonth / numberPreVote);

            // PercentVsPreMonthOfConcurLevel
            decimal percentVsPreMonthOfConcurLevel = Common.FormatPecentageDecimal((percentOfConcurLevel - percentOfConcurLevelPreMonth) / percentOfConcurLevelPreMonth);

            #region PieChart
            // Stringly AgreeVote
            int numberStronglyAgreeVote = await _evaDataRepository.CountVoteByPoint(Constants.STRONGLY_AGREE_POINT, date);

            // AgreeVote
            int numberAgreeVote = await _evaDataRepository.CountVoteByPoint(Constants.AGREE_POINT, date);

            // Neutral
            int numberNeutralVote = await _evaDataRepository.CountVoteByPoint(Constants.NEUTRAL_POINT, date);

            // DisagreeVote
            int numberDisagreeVote = await _evaDataRepository.CountVoteByPoint(Constants.DISAGREE_POINT, date);

            // Strongly disagree
            int numberStronglyDisagreeVote = await _evaDataRepository.CountVoteByPoint(Constants.STRONGLY_DISAGREE_POINT, date);

            // OverallEvaluativeLevels
            var overallEvaluativeLevels = new List<OverallEvaluativeLevel>()
            {
                new OverallEvaluativeLevel()
                {
                    Level = Constants.STRONGLY_AGREE,
                    NumberEvaluated = numberStronglyAgreeVote
                },
                new OverallEvaluativeLevel()
                {
                    Level = Constants.AGREE,
                    NumberEvaluated = numberAgreeVote
                },
                new OverallEvaluativeLevel()
                {
                    Level = Constants.NEUTRAL,
                    NumberEvaluated = numberNeutralVote
                },
                new OverallEvaluativeLevel()
                {
                    Level = Constants.DISAGREE,
                    NumberEvaluated = numberDisagreeVote
                },
                new OverallEvaluativeLevel()
                {
                    Level = Constants.STRONGLY_DISAGREE,
                    NumberEvaluated = numberStronglyDisagreeVote
                },
            };
            #endregion

            #region LineChart
            var dataLineChart = new List<ServiceData>();
            var servicesLineChart = await _serviceRepository.GetServicesByStatus(true);

            foreach (var service in servicesLineChart)
            {
                var serviceData = new ServiceData
                {
                    ServiceName = service.SerName == null ? "No service name" : service.SerName,
                    NumberOfEvaluate = await _serFeedbackRepository.CountNumberOfPatientEvaluated(service.SerId,
                        firstDateInMonth, firstDateInMonth.AddDays(7), firstDateInMonth.AddDays(14), firstDateInMonth.AddDays(21), date)
                };
                dataLineChart.Add(serviceData);
            }

            var numberOfPatientsOverTimeForServices = new ChartMui
            {
                Labels = Constants.MONTH_TIMELINE_LINE_CHART,
                Data = dataLineChart
            };
            #endregion

            #region BarChart
            var dataBarChart = new List<ServiceData>();
            var servicesBarChart = await _serviceRepository.GetServicesByStatus(true);

            foreach (var service in servicesBarChart)
            {
                var serviceData = new ServiceData
                {
                    ServiceName = service.SerName == null ? "No service name" : service.SerName,
                    NumberOfEvaluate = await _serFeedbackRepository.CountNumberOfEvaluations(service.SerId,
                        firstDateInMonth, firstDateInMonth.AddDays(7), firstDateInMonth.AddDays(14), firstDateInMonth.AddDays(21), date)
                };
                dataBarChart.Add(serviceData);
            }

            var summaryOfEvaluatedServices = new ChartMui
            {
                Labels = Constants.MONTH_TIMELINE_BAR_CHART,
                Data = dataBarChart
            };
            #endregion

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get data for dashboard successfully",
                Data = new DashboardResponse
                {
                    NumberOfEvaluations = numberOfEvaluations,
                    PercentVsPreMonthOfNumEva = percentVsPreMonthOfNumEva,
                    PercentOfPatientEvaluated = percentOfPatientEvaluated,
                    PercentVsPreMonthOfPatEva = percentVsPreMonthOfPatientEvaluated,
                    PercentOfConcurLevel = percentOfConcurLevel,
                    PercentVsPreMonthOfConcurLevel = percentVsPreMonthOfConcurLevel,
                    OverallEvaluativeLevels = overallEvaluativeLevels,
                    NumberOfPatientsOverTimeForServices = numberOfPatientsOverTimeForServices,
                    SummaryOfEvaluatedServices = summaryOfEvaluatedServices,
                }
            });
        }

        [HttpGet("view-evaluation-result-for-bom")]
        //public async Task<IActionResult> GetAllEvaluationResultForBOM([FromQuery] FilteredResponse filterResponse, [FromQuery] EvaluationResultForBOMRequest bOMRequest)
        //public async Task<IActionResult> GetAllEvaluationResultForBOM([FromQuery] EvaluationResultForBOMRequest bOMRequest)
        public async Task<IActionResult> GetAllEvaluationResultForBOM(DateTime dateFrom, DateTime dateTo)
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

            // set valid page and page size
            //var validFilter = new FilteredResponse(filterResponse.search, filterResponse.sortedBy, filterResponse.page, filterResponse.pageSize);
            //var pagedResponse = new PagedResponse<EvaluationResultForBOMResponse>();
            var dataMap = new List<EvaluationResultForBOMResponse>();

            // get all evaluation result
            //var data = await _evaDataRepository.GetAllEvaluationResultForBOM(bOMRequest, validFilter);
            var data = await _evaDataRepository.GetAllEvaluationResultForBOM(dateFrom, dateTo);

            // get all evaluation result mapping
            dataMap = _mapper.Map<List<EvaluationResultForBOMResponse>>(data);
            // total data
            var totalRecords = dataMap.Count;
            //var totalRecords = await _evaDataRepository.CountAllEvaluationResultForBOM();
            // total page
            //var totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)validFilter.pageSize));

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the list or evaluation result.");

            // list empty
            if (totalRecords == 0)
            {
                return Ok(new APIResponse
                {
                    Success = true,
                    //Message = "Get all evaluation result successfully.",
                    //Data = new PagedResponse<EvaluationResultForBOMResponse>
                    //{
                    //    TotalItems = totalRecords,
                    //    Results = null,
                    //    TotalPages = totalPages,
                    //    CurrentPage = validFilter.page,
                    //    PageSize = validFilter.pageSize
                    //}
                    Message = "The list of evaluation result is empty."
                });
            }

            // error message
            //if (validFilter.page > totalPages)
            //{
            //    return BadRequest(new APIResponse { Success = false, Message = "Current page must be smaller than total pages." });
            //}

            foreach (var item in dataMap)
            {
                // mapping data
                MappingEvaluationResult(item);
            }

            // pagination by page and page size
            //var pagedData = dataMap.Skip((validFilter.page - 1) * validFilter.pageSize).Take(validFilter.pageSize).ToList();
            //pagedResponse = new PagedResponse<EvaluationResultForBOMResponse>
            //{
            //    TotalItems = totalRecords,
            //    //Results = pagedData,
            //    Results = dataMap,
            //    TotalPages = totalPages,
            //    CurrentPage = validFilter.page,
            //    PageSize = validFilter.pageSize
            //};

            // success message
            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get all evaluation result successfully.",
                Data = dataMap
                //Data = pagedResponse
            });
        }
        private void MappingEvaluationResult(EvaluationResultForBOMResponse item)
        {
            // each evaluation result mapping with patient, eva...cri..., service, ser...feed...
            var patient = _mapper.Map<PatientResponse>(_patientRepository.GetPatient(item.PatientId!).Result);
            var evaCri = _mapper.Map<EvaluationCriteriaResponse>(_evaCriRepository.GetEvaluationCriteria((int)item.CriId!).Result);
            var service = _mapper.Map<ServiceResponse>(_serviceRepository.GetService((int)evaCri.SerId!).Result);
            var serviceFb = _mapper.Map<ServiceFeedbackResponse>(_serFeedbackRepository.GetServiceFeedbackByDateForBOM((DateTime)item.CreatedOn!).Result);
            item.PatientName = patient != null ? patient.FullName : default;
            item.CriDesc = evaCri != null ? evaCri.CriDesc : default;
            item.SerId = service != null ? service.SerId : default;
            item.SerName = service != null ? service.SerName : default;
            item.SerFbId = serviceFb != null ? serviceFb.FbId : default;
            item.SerFb = serviceFb != null ? serviceFb.Feedback : default;
            item.Option = item.Point == 1 ? Constants.STRONGLY_DISAGREE : item.Point == 2 ? Constants.DISAGREE : item.Point == 3 ? Constants.NEUTRAL :
                item.Point == 4 ? Constants.AGREE : Constants.STRONGLY_AGREE;
        }

        [HttpGet("view-evaluation-result-for-bom/{evaDataId}")]
        public async Task<IActionResult> GetEvaluationResultForBOM([FromRoute] int evaDataId)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var evaDataMap = new EvaluationResultForBOMResponse();

            // get evaluation result by id
            var evaData = await _evaDataRepository.GetEvaluationResultForBOM(evaDataId);
            // error message
            if (evaData == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return NotFound(new APIResponse { Success = false, Message = $"Can't not found evaluation result with id: {evaDataId}." });
            }
            if (evaData.Status == false)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return BadRequest(new APIResponse { Success = false, Message = "Evaluation result request is disabled." });
            }

            // get evaluation result mapping
            evaDataMap = _mapper.Map<EvaluationResultForBOMResponse>(evaData);
            MappingEvaluationResult(evaDataMap);

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed an evaluation result.");

            // succes message
            return Ok(new APIResponse
            {
                Success = true,
                Message = $"Get an evaluation result with id: {evaDataId} successfully.",
                Data = evaDataMap
            });
        }

        [HttpGet("view-service-feedback-for-bom")]
        public async Task<IActionResult> GetAllServiceFeedbackForBOM([FromQuery] ServiceFeedbackForBOMRequest bOMRequest)
        {
            //Check date validation
            if (bOMRequest.from > bOMRequest.to)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            // get all service feedback
            var data = await _serFeedbackRepository.GetAllServiceFeedbackForBOM(bOMRequest);

            // get all evaluation result mapping
            var dataMap = _mapper.Map<List<ServiceFeedbackForBOMResponse>>(data);

            // total data
            var totalRecords = dataMap.Count;

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the list of service feedback.");

            // list empty
            if (totalRecords == 0)
            {

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "The list of service feedback is empty."
                });
            }

            foreach (var item in dataMap)
            {
                // mapping data
                MappingServiceFeedback(item);
            }

            // success message
            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get all service feedback successfully.",
                Data = dataMap
            });
        }
        private void MappingServiceFeedback(ServiceFeedbackForBOMResponse item)
        {
            // each service feedback mapping with patient, service
            var patient = _patientRepository.GetPatient(item.PatientId!).Result;
            var service = _serviceRepository.GetService((int)item.SerId!).Result;
            item.PatientName = patient != null ? patient.LastName + ' ' + patient.FirstName : default;
            item.SerName = service != null ? service.SerName : default;
        }

        [HttpGet("view-service-feedback-for-bom/{fbId}")]
        public async Task<IActionResult> GetServiceFeedbackForBOM([FromRoute] int fbId)
        {
            // get service feedback by id
            var serFb = await _serFeedbackRepository.GetServiceFeedbackForBOM(fbId);

            // error message
            if (serFb == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return NotFound(new APIResponse { Success = false, Message = "Can not find service feedback." });
            }
            if (serFb.Status == false)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return BadRequest(new APIResponse { Success = false, Message = "Service feedback request is disabled." });
            }

            // get service feedback mapping
            var serFbMap = _mapper.Map<ServiceFeedbackForBOMResponse>(serFb);
            MappingServiceFeedback(serFbMap);

            // Add log
            await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed service feedback details.");

            // succes message
            return Ok(new APIResponse { Success = true, Message = "Get service feedback successfully.", Data = serFbMap });
        }
    }
}
