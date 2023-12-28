using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Repositories.Utils;
using ServiceEvaObjects;
using SystemObjects;

namespace HSEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly IEvaluationCriteriaRepository _evaCriRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IEvaluationDataRepository _evaDataRepository;
        private readonly IServiceFeedbackRepository _serFeedRepository;
        private readonly ISystemInformationRepository _sysInforRepository;

        public PatientController(IServiceRepository serviceRepository, IMapper mapper,
            IEvaluationCriteriaRepository evaCriRepository, IPatientRepository patientRepository,
            IEvaluationDataRepository evaDataRepository, IServiceFeedbackRepository serFeedRepository, ISystemInformationRepository sysInforRepository)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _evaCriRepository = evaCriRepository;
            _patientRepository = patientRepository;
            _evaDataRepository = evaDataRepository;
            _serFeedRepository = serFeedRepository;
            _sysInforRepository = sysInforRepository;
        }

        [HttpGet("view-evaluation-services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _serviceRepository.GetServices(null);

            var serviceDTOs = _mapper.Map<List<ServiceResponseForPatient>>(services);

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get services successful",
                Data = serviceDTOs
            });
        }        
        
        [HttpGet("get-system-information")]
        public async Task<IActionResult> GetSystemInformation()
        {
            var sysInfo = await _sysInforRepository.GetSystemInformationBySysId(1);

            if (sysInfo == null)
            {
                return NotFound(new APIResponse { Success = false, Message = "This System Information is not available." });
            }

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Get System Information successfully.",
                Data = sysInfo
            });
        }


        [HttpGet("view-evaluation-criteria-for-patient/{serId}")]
        public async Task<IActionResult> GetEvaluationCriteriaForPatient([FromRoute] int serId)
        {
            //var services = await _serviceRepository.GetServices(null);
            //var service = services.SingleOrDefault(x => x.SerId == serId);
            var service = await _serviceRepository.GetService(serId);
            if (service == null)
            {
                return NotFound(new APIResponse { Success = false, Message = "Service request is not exist." });
            }
            if (service.Status == false)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Service request is disabled." });
            }
            var evaCri = await _evaCriRepository.GetEvaluationCriteriaForPatient(serId);

            // mapping
            var evaCriMap = _mapper.Map<List<EvaluationCriteriaForPatientResponse>>(evaCri);

            // data custom
            var customDTO = new
            {
                SerId = service.SerId,
                SerName = service.SerName,
                Results = evaCriMap
            };

            return Ok(new APIResponse
            {
                Success = true,
                Message = "All evaluation criteria for patient is retrieved successfully.",
                Data = customDTO
            });
        }

        [HttpGet("view-evaluation-result-for-patient")]
        public async Task<IActionResult> GetEvaluationResultForPatient([FromQuery] EvaluationResultForPatientRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Filter request is wrong format." });
            }

            var service = await _serviceRepository.GetService(request.serId);
            if (service == null)
            {
                return NotFound(new APIResponse { Success = false, Message = "Service request is not exist." });
            }
            if (service.Status == false)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Service request is disabled." });
            }

            var patient = await _patientRepository.GetPatient(request.patId);
            if (patient == null)
            {
                return NotFound(new APIResponse { Success = false, Message = "Patient request is not exist." });
            }

            var evaCri = await _evaCriRepository.GetEvaluationCriteriaForPatient(request.serId);
            var serFeedback = await _serFeedRepository.GetServiceFeedbackForPatient(request);

            // mapping
            var evaCriMap = _mapper.Map<List<EvaluationCriteriaForPatientResponse>>(evaCri);
            foreach (var item in evaCriMap)
            {
                var evaData = await _evaDataRepository.GetEvaluationResultForPatient(item.CriId, request);

                item.ResultForPatient = evaData != null ? _mapper.Map<EvaluationResultForPatientResponse>(evaData) : null;

                if (item.ResultForPatient != null)
                {
                    item.ResultForPatient.Option = item.ResultForPatient.Point == 1 ? Constants.STRONGLY_DISAGREE : item.ResultForPatient.Point == 2
                        ? Constants.DISAGREE : item.ResultForPatient.Point == 3 ? Constants.NEUTRAL : item.ResultForPatient.Point == 4 ? Constants.AGREE : Constants.STRONGLY_AGREE;
                }
            }
            var serFeedbackMap = _mapper.Map<ServiceFeedbackForPatientResponse>(serFeedback);

            // data custom
            var customDTO = new
            {
                Patient = patient,
                Service = service,
                ServiceFeedback = serFeedbackMap,
                Results = evaCriMap
            };

            return Ok(new APIResponse
            {
                Success = true,
                Message = "All evaluation result for patient is retrieved successfully.",
                Data = customDTO
            });
        }

        [HttpGet("verify-patient-code/{patientId}")]
        public async Task<IActionResult> VerifyPatientCode(string patientId)
        {
            if (!ModelState.IsValid || patientId == null)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Patient code can not be null", Data = null }); ; ;
            }

            Patient patient = await _patientRepository.VerifyPatientCode(patientId);

            if (patient == null)
            {
                return NotFound(new APIResponse
                {
                    Success = false,
                    Message = "Patient code is not exist"
                });
            }
            else
            {
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Verify patient code successfully.",
                    Data = patient
                });
            }
        }

        [HttpPost("submit-evaluation/{patientId}")]
        public async Task<IActionResult> SubmitEvaluation(string patientId, [FromBody] EvaluationSubmitRequest request)
        {
            var patient = await _patientRepository.GetPatient(patientId);

            if (patient == null)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Patient not found"
                });
            }

            var service = await _serviceRepository.GetService(request.SerId);

            if (service == null || service.Status == false)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Service not found or disabled"
                });
            }

            var points = new List<int>
            {
                Constants.STRONGLY_AGREE_POINT,
                Constants.AGREE_POINT,
                Constants.NEUTRAL_POINT,
                Constants.DISAGREE_POINT,
                Constants.STRONGLY_DISAGREE_POINT
            };

            foreach (var criteria in request.EvaluationData)
            {
                var checkCriteria = await _evaCriRepository.GetEvaluationCriteria(criteria.CriId);

                if (checkCriteria == null)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = $"Cannot found this criteriaId: {criteria.CriId}"
                    });
                }

                if (!points.Contains(criteria.Point))
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = $"Invalid point: {criteria.Point}"
                    });
                }
            }

            var newSerivceFeedback = new ServiceFeedback
            {
                SerId = request.SerId,
                Feedback = String.IsNullOrEmpty(request.Feedback) ? null : request.Feedback,
                PatientId = patientId,
            };

            var newEvaluationData = new List<EvaluationData>();

            foreach (var criteria in request.EvaluationData)
            {
                newEvaluationData.Add(new EvaluationData
                {
                    CriId = criteria.CriId,
                    Point = criteria.Point,
                    PatientId = patientId,
                });
            }

            bool result = await _patientRepository.SubmitEvaluation(newSerivceFeedback, newEvaluationData);

            if (result)
            {
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Submit evaluation data successfully"
                });
            }
            else
            {
                return Conflict(new APIResponse
                {
                    Success = false,
                    Message = "Submit evaluation data failure"
                });
            }
        }
    }
}
