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
    public class ReportController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IEvaluationCriteriaRepository _evaCriRepository;
        private readonly IEvaluationDataRepository _evaDataRepository;
        private readonly IServiceFeedbackRepository _serviceFeedbackRepository;
        private readonly ISystemInformationRepository _systemInformationRepository;
        private readonly IMapper _mapper;

        public ReportController(IServiceRepository serviceRepository, IEvaluationCriteriaRepository evaCriRepository,
            IEvaluationDataRepository evaDataRepository, IServiceFeedbackRepository serviceFeedbackRepository,
            ISystemInformationRepository systemInformationRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _evaCriRepository = evaCriRepository;
            _evaDataRepository = evaDataRepository;
            _serviceFeedbackRepository = serviceFeedbackRepository;
            _systemInformationRepository = systemInformationRepository;
            _mapper = mapper;
        }

        [HttpGet("result-of-evaluation-service")]
        public async Task<IActionResult> ReportOfEvaluationService()
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            try
            {
                var services = await _serviceRepository.GetServicesByStatus(true);
                var reportServiceResponses = new List<ReportServiceResponse>();

                foreach (var service in services)
                {
                    var reportServiceResponse = _mapper.Map<ReportServiceResponse>(service);
                    reportServiceResponse.NumberOfEvaluated = await _serviceFeedbackRepository.CountNumberOfEvaluations(service.SerId);
                    reportServiceResponses.Add(reportServiceResponse);
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the result of evaluation service.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get result of evaluation service successfully",
                    Data = reportServiceResponses
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("result-of-evaluation-service/{serId}")]
        public async Task<IActionResult> ReportOfEvaluationService(int serId, DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var service = await _serviceRepository.GetService(serId, true);

            // Check serviceId
            if (service == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return NotFound();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            try
            {
                var criterias = await _evaCriRepository.GetAllEvaluationCriteria(serId);
                var criteriaResult = new List<ResultEvaluationCriteria>();

                foreach (var criteria in criterias)
                {
                    var optionResult = new List<ResultEvaluationOption>();
                    int numberVotes = await _evaDataRepository.CountVoteByCriId(criteria.CriId, dateFrom, dateTo);

                    if (numberVotes > 0)
                    {
                        int numberVoteStronglyAgree = await _evaDataRepository.CountVoteByCriIdAndPoint(criteria.CriId, Constants.STRONGLY_AGREE_POINT, dateFrom, dateTo);
                        int numberVoteAgree = await _evaDataRepository.CountVoteByCriIdAndPoint(criteria.CriId, Constants.AGREE_POINT, dateFrom, dateTo);
                        int numberVoteNeutral = await _evaDataRepository.CountVoteByCriIdAndPoint(criteria.CriId, Constants.NEUTRAL_POINT, dateFrom, dateTo);
                        int numberVoteDisagree = await _evaDataRepository.CountVoteByCriIdAndPoint(criteria.CriId, Constants.DISAGREE_POINT, dateFrom, dateTo);
                        int numberVoteStronglyDisagree = await _evaDataRepository.CountVoteByCriIdAndPoint(criteria.CriId, Constants.STRONGLY_DISAGREE_POINT, dateFrom, dateTo);

                        optionResult.Add(new ResultEvaluationOption
                        {
                            OptionName = Constants.STRONGLY_AGREE,
                            OptionVote = numberVoteStronglyAgree,
                            VotePercent = Common.FormatPecentageDecimal((decimal)numberVoteStronglyAgree / numberVotes)
                        });

                        optionResult.Add(new ResultEvaluationOption
                        {
                            OptionName = Constants.AGREE,
                            OptionVote = numberVoteAgree,
                            VotePercent = Common.FormatPecentageDecimal((decimal)numberVoteAgree / numberVotes)
                        });

                        optionResult.Add(new ResultEvaluationOption
                        {
                            OptionName = Constants.NEUTRAL,
                            OptionVote = numberVoteNeutral,
                            VotePercent = Common.FormatPecentageDecimal((decimal)numberVoteNeutral / numberVotes)
                        });

                        optionResult.Add(new ResultEvaluationOption
                        {
                            OptionName = Constants.DISAGREE,
                            OptionVote = numberVoteDisagree,
                            VotePercent = Common.FormatPecentageDecimal((decimal)numberVoteDisagree / numberVotes)
                        });

                        optionResult.Add(new ResultEvaluationOption
                        {
                            OptionName = Constants.STRONGLY_DISAGREE,
                            OptionVote = numberVoteStronglyDisagree,
                            VotePercent = Common.FormatPecentageDecimal((decimal)numberVoteStronglyDisagree / numberVotes)
                        });

                        criteriaResult.Add(new ResultEvaluationCriteria
                        {
                            CriId = criteria.CriId,
                            CriDesc = criteria.CriDesc,
                            Options = optionResult,
                            Vote = numberVotes
                        });
                    }
                    else
                    {
                        criteriaResult.Add(new ResultEvaluationCriteria
                        {
                            CriId = criteria.CriId,
                            CriDesc = criteria.CriDesc,
                            Vote = 0
                        });
                    }
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the detailed results of the evaluation service.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get results of the evaluation service successfully",
                    Data = new ResultEvaluationService
                    {
                        SerId = serId,
                        SerName = service.SerName,
                        SerDesc = service.SerDesc,
                        Icon = service.Icon,
                        Criterias = criteriaResult
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("service-comparison")]
        public async Task<IActionResult> ServiceComparsion(int ser1Id, int ser2Id, DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var ser1 = await _serviceRepository.GetService(ser1Id, true);
            var ser2 = await _serviceRepository.GetService(ser2Id, true);

            if (ser1 == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Service1 not found"
                });
            }

            if (ser2 == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Service2 not found"
                });
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            try
            {
                var typeComparisons = new List<TypeComparison>();
                int countVoteService1Result = await _evaDataRepository.CountVoteBySerId(ser1Id, dateFrom, dateTo);
                int countVoteService2Result = await _evaDataRepository.CountVoteBySerId(ser2Id, dateFrom, dateTo);

                // handle get satisfaction degree
                int countConcurVoteService1 = await _evaDataRepository.CountConcurVoteBySerId(ser1Id, dateFrom, dateTo);
                int countConcurVoteService2 = await _evaDataRepository.CountConcurVoteBySerId(ser2Id, dateFrom, dateTo);

                decimal service1Result, service2Result;

                if (countVoteService1Result != 0)
                {
                    service1Result = Common.FormatPecentageDecimal((decimal)countConcurVoteService1 / countVoteService1Result);
                }
                else
                {
                    service1Result = 0;
                }

                if (countVoteService2Result != 0)
                {
                    service2Result = Common.FormatPecentageDecimal((decimal)countConcurVoteService2 / countVoteService2Result);
                }
                else
                {
                    service2Result = 0;
                }

                // Compare satisfaction degree
                typeComparisons.Add(new TypeComparison
                {
                    TypeName = Constants.SATISFACTION_DEGREE,
                    TypeDescription = Constants.SATISFACTION_DEGREE_DESCRIPTION,
                    Service1Result = service1Result,
                    Service2Result = service2Result
                });

                // handle get neutral degree
                int countNeutralVoteService1 = await _evaDataRepository.CountNeutralVoteBySerId(ser1Id, dateFrom, dateTo);
                int countNeutralVoteService2 = await _evaDataRepository.CountNeutralVoteBySerId(ser2Id, dateFrom, dateTo);

                if (countVoteService1Result != 0)
                {
                    service1Result = Common.FormatPecentageDecimal((decimal)countNeutralVoteService1 / countVoteService1Result);
                }
                else
                {
                    service1Result = 0;
                }

                if (countVoteService2Result != 0)
                {
                    service2Result = Common.FormatPecentageDecimal((decimal)countNeutralVoteService2 / countVoteService2Result);
                }
                else
                {
                    service2Result = 0;
                }

                // Compare neutral degree
                typeComparisons.Add(new TypeComparison
                {
                    TypeName = Constants.NEUTRAL_DEGREE,
                    TypeDescription = Constants.NEUTRAL_DEGREE_DESCRIPTION,
                    Service1Result = service1Result,
                    Service2Result = service2Result
                });

                // handle get dissatisfaction degree
                int countDisconcurVoteService1 = await _evaDataRepository.CountDisconcurVoteBySerId(ser1Id, dateFrom, dateTo);
                int countDisconcurVoteService2 = await _evaDataRepository.CountDisconcurVoteBySerId(ser2Id, dateFrom, dateTo);

                if (countVoteService1Result != 0)
                {
                    service1Result = Common.FormatPecentageDecimal((decimal)countDisconcurVoteService1 / countVoteService1Result);
                }
                else
                {
                    service1Result = 0;
                }

                if (countVoteService2Result != 0)
                {
                    service2Result = Common.FormatPecentageDecimal((decimal)countDisconcurVoteService2 / countVoteService2Result);
                }
                else
                {
                    service2Result = 0;
                }

                // Compare dissatisfaction degree
                typeComparisons.Add(new TypeComparison
                {
                    TypeName = Constants.DISSATISFACTION_DEGREE,
                    TypeDescription = Constants.DISSATISFACTION_DEGREE_DESCRIPTION,
                    Service1Result = service1Result,
                    Service2Result = service2Result
                });

                // handle get total point
                int totalPointSer1 = await _evaDataRepository.TotalPoint(ser1Id, dateFrom, dateTo);
                int totalPointSer2 = await _evaDataRepository.TotalPoint(ser2Id, dateFrom, dateTo);

                // Neutral degree
                typeComparisons.Add(new TypeComparison
                {
                    TypeName = Constants.POINT,
                    TypeDescription = Constants.POINT_DESCRIPTION,
                    Service1Result = totalPointSer1,
                    Service2Result = totalPointSer2
                });

                // handle get number of survey
                int numberOfSurveyInSer1 = await _serviceFeedbackRepository.CountNumberOfEvaluations(ser1Id, dateFrom, dateTo);
                int numberOfSurveyInSer2 = await _serviceFeedbackRepository.CountNumberOfEvaluations(ser2Id, dateFrom, dateTo);

                // Number of survey
                typeComparisons.Add(new TypeComparison
                {
                    TypeName = Constants.NUMBER_OF_SURVEY,
                    TypeDescription = Constants.NUMBER_OF_SURVEY_DESCRIPTION,
                    Service1Result = numberOfSurveyInSer1,
                    Service2Result = numberOfSurveyInSer2
                });

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed service comparison.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Services compare successfully",
                    Data = new ServiceComparisonResponse
                    {
                        Service1Name = ser1.SerName!,
                        Service2Name = ser2.SerName!,
                        TypeComparisons = typeComparisons
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("overview-score-by-service-criteria/{serId}")]
        public async Task<IActionResult> OverviewScoreByServiceCriteria(int serId, DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var service = await _serviceRepository.GetService(serId, true);

            // Check serviceId
            if (service == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return NotFound();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            try
            {
                var criterias = await _evaCriRepository.GetAllEvaluationCriteria(serId);
                var criteriasName = new List<string>();
                var criteriasPoint = new List<int>();

                foreach (var criteria in criterias)
                {
                    criteriasName.Add(criteria.CriDesc == null ? "" : criteria.CriDesc);
                    int criteriaPoint = await _evaDataRepository.TotalPointByCriId(criteria.CriId, dateFrom, dateTo);
                    criteriasPoint.Add(criteriaPoint);
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewd the score by service criteria.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get score by service criteria successfully.",
                    Data = new OverviewScoreByServiceCriteriaResponse
                    {
                        Criterias = criteriasName,
                        Points = criteriasPoint
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("overview-votes-by-degree/{serId}")]
        public async Task<IActionResult> OverviewVotesByDegree(int serId, DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var service = await _serviceRepository.GetService(serId, true);

            // Check serviceId
            if (service == null)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to view data not permitted.");

                return NotFound();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            try
            {
                var optionsName = new List<string>
                {
                    Constants.STRONGLY_AGREE,
                    Constants.AGREE,
                    Constants.NEUTRAL,
                    Constants.DISAGREE,
                    Constants.STRONGLY_DISAGREE
                };

                var criterias = await _evaCriRepository.GetAllEvaluationCriteria(serId);
                var criIds = new List<int>();

                foreach (var criteria in criterias)
                {
                    criIds.Add(criteria.CriId);
                }

                var optionsVote = new List<int>
                {
                    await _evaDataRepository.CountVote(criIds, Constants.STRONGLY_AGREE_POINT, dateFrom, dateTo),
                    await _evaDataRepository.CountVote(criIds, Constants.AGREE_POINT, dateFrom, dateTo),
                    await _evaDataRepository.CountVote(criIds, Constants.NEUTRAL_POINT, dateFrom, dateTo),
                    await _evaDataRepository.CountVote(criIds, Constants.DISAGREE_POINT, dateFrom, dateTo),
                    await _evaDataRepository.CountVote(criIds, Constants.STRONGLY_DISAGREE_POINT, dateFrom, dateTo),
                };

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewd the votes by degree.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get votes by degree successfully.",
                    Data = new OverviewVotesByDegreeResponse
                    {
                        Options = optionsName,
                        Votes = optionsVote
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("statistics-satisfaction")]
        public async Task<IActionResult> StatisticsSatisfaction(DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid."
                });
            }

            try
            {
                var services = await _serviceRepository.GetServicesByStatus(true);
                var labels = new List<string>()
                {
                    Constants.STRONGLY_AGREE,
                    Constants.AGREE,
                    Constants.NEUTRAL,
                    Constants.DISAGREE,
                    Constants.STRONGLY_DISAGREE,
                };

                var serviceData = new List<SatisfactionDegreeByService>();

                foreach (var service in services)
                {
                    var criterias = await _evaCriRepository.GetAllEvaluationCriteria(service.SerId);
                    var criIds = new List<int>();

                    foreach (var criteria in criterias)
                    {
                        criIds.Add(criteria.CriId);
                    }

                    var totalVote = await _evaDataRepository.CountVote(criIds, dateFrom, dateTo);

                    if (totalVote > 0)
                    {
                        var countStronglyAgreeVote = await _evaDataRepository.CountVote(criIds, Constants.STRONGLY_AGREE_POINT, dateFrom, dateTo);
                        var countArgeeVote = await _evaDataRepository.CountVote(criIds, Constants.AGREE_POINT, dateFrom, dateTo);
                        var countNeutralVote = await _evaDataRepository.CountVote(criIds, Constants.NEUTRAL_POINT, dateFrom, dateTo);
                        var countDisargeeVote = await _evaDataRepository.CountVote(criIds, Constants.DISAGREE_POINT, dateFrom, dateTo);
                        var countStronglyDisagreeVote = await _evaDataRepository.CountVote(criIds, Constants.STRONGLY_DISAGREE_POINT, dateFrom, dateTo);

                        var percentOfStronglyAgreeVote = Common.FormatPecentageDecimal((decimal)countStronglyAgreeVote / totalVote);
                        var percentOfAgreeVote = Common.FormatPecentageDecimal((decimal)countArgeeVote / totalVote);
                        var percentOfNeutralVote = Common.FormatPecentageDecimal((decimal)countNeutralVote / totalVote);
                        var percentOfDisagreeVote = Common.FormatPecentageDecimal((decimal)countDisargeeVote / totalVote);
                        var percentOfStronglyDisagreeVote = Common.FormatPecentageDecimal((decimal)countStronglyDisagreeVote / totalVote);

                        serviceData.Add(new SatisfactionDegreeByService
                        {
                            ServiceName = service.SerName == null ? "No name" : service.SerName,
                            PercentOfDegree = new List<decimal>() {
                            percentOfStronglyAgreeVote,
                            percentOfAgreeVote,
                            percentOfNeutralVote,
                            percentOfDisagreeVote,
                            percentOfStronglyDisagreeVote,
                        }
                        });
                    }
                    else
                    {
                        // This service has not evaluated
                        serviceData.Add(new SatisfactionDegreeByService
                        {
                            ServiceName = service.SerName == null ? "No name" : service.SerName,
                            PercentOfDegree = new List<decimal>() { 0, 0, 0, 0, 0 }
                        });
                    }
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed  statistics satisfaction.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get statistics satisfaction successfully.",
                    Data = new StatisticSatisfactionResponse
                    {
                        Labels = labels,
                        Data = serviceData
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("statistics-satisfaction/{point}")]
        public async Task<IActionResult> StatisticsSatisfactionDetails(int point, DateTime dateFrom, DateTime dateTo)
        {
            if (Common.CheckUserIsBlock(HttpContext))
            {
                return Forbid();
            }

            var services = await _serviceRepository.GetServicesByStatus(true);

            var labels = new Dictionary<int, string>()
            {
                { Constants.STRONGLY_AGREE_POINT, Constants.STRONGLY_AGREE },
                { Constants.AGREE_POINT, Constants.AGREE },
                { Constants.NEUTRAL_POINT, Constants.NEUTRAL },
                { Constants.DISAGREE_POINT, Constants.DISAGREE },
                { Constants.STRONGLY_DISAGREE_POINT, Constants.STRONGLY_DISAGREE },
            };

            if (!labels.ContainsKey(point))
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send virtual data.");

                return NotFound();
            }

            //Check date validation
            if (dateFrom > dateTo)
            {
                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User is attempting to send invalid data.");

                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Date input is invalid"
                });
            }

            try
            {
                var serviceData = new List<SatisfactionDegreeByServiceDetails>();

                foreach (var service in services)
                {
                    var criterias = await _evaCriRepository.GetAllEvaluationCriteria(service.SerId);
                    var criIds = new List<int>();

                    foreach (var criteria in criterias)
                    {
                        criIds.Add(criteria.CriId);
                    }

                    var totalVote = await _evaDataRepository.CountVote(criIds, dateFrom, dateTo);

                    if (totalVote > 0)
                    {
                        var countVote = await _evaDataRepository.CountVote(criIds, point, dateFrom, dateTo);

                        serviceData.Add(new SatisfactionDegreeByServiceDetails
                        {
                            ServiceName = service.SerName == null ? "No name" : service.SerName,
                            Votes = countVote
                        });
                    }
                    else
                    {
                        // This service has not evaluated
                        serviceData.Add(new SatisfactionDegreeByServiceDetails
                        {
                            ServiceName = service.SerName == null ? "No name" : service.SerName,
                            Votes = 0
                        });
                    }
                }

                // Add log
                await _systemInformationRepository.AddSystemLog(HttpContext, "User viewed the statistics satisfaction details.");

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get statistics satisfaction details successfully.",
                    Data = new StatisticSatisfactionDetailsResponse
                    {
                        Label = labels[point],
                        Data = serviceData
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
