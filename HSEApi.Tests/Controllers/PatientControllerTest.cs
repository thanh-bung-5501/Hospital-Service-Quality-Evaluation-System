using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using DataAccess;
using FluentAssertions.Common;
using HSEApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Repositories;
using Repositories.Models;
using ServiceEvaObjects;
using SystemObjects;

namespace HSEApi.Tests.Controllers
{
    public class PatientControllerTest
    {
        private readonly PatientController _sut;
        private readonly IFixture _fixture;
        private readonly Mock<IPatientRepository> _patientRepository;
        private readonly Mock<IServiceRepository> _serviceRepository;
        private readonly Mock<IEvaluationCriteriaRepository> _evaluationCriteriaRepository;
        private readonly Mock<IServiceFeedbackRepository> _serviceFeedbackRepository;
        private readonly Mock<IEvaluationDataRepository> _evaluationDataRepository;
        private readonly Mock<ISystemInformationRepository> _sysInforRepository;
        private readonly Mock<IMapper> _mapper;

        public PatientControllerTest()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _patientRepository = _fixture.Freeze<Mock<IPatientRepository>>();
            _serviceRepository = _fixture.Freeze<Mock<IServiceRepository>>();
            _sysInforRepository = _fixture.Freeze<Mock<ISystemInformationRepository>>();
            _mapper = _fixture.Freeze<Mock<IMapper>>();
            _evaluationCriteriaRepository = _fixture.Freeze<Mock<IEvaluationCriteriaRepository>>();
            _serviceFeedbackRepository = _fixture.Freeze<Mock<IServiceFeedbackRepository>>();
            _evaluationDataRepository = _fixture.Freeze<Mock<IEvaluationDataRepository>>();
            _sut = new PatientController(_serviceRepository.Object, _mapper.Object, _evaluationCriteriaRepository.Object,
                _patientRepository.Object, _evaluationDataRepository.Object, _serviceFeedbackRepository.Object, _sysInforRepository.Object); //Create the implement in memory
        }

        //UT for api VerifyPatientCode
        #region
        [Fact]
        public async void VerifyPatientCode_Success_WhenPatientCodeIsValid()
        {
            // Arrange
            string patientId = "pat1";
            Patient patient = new Patient();
            patient.PatientId = patientId;
            patient.PhoneNumber = "0123456789";
            patient.FirstName = "firstName";
            patient.LastName = "lastName";

            APIResponse apiResponse = new APIResponse { Success = true, Message = "Verify patient code successfully.", Data = patient};

            _patientRepository.Setup(x => x.VerifyPatientCode(patientId)).ReturnsAsync(patient);

            // Act
            OkObjectResult verifyPatientCode = (OkObjectResult)await _sut.VerifyPatientCode(patientId);
            APIResponse apiResponseActual = (APIResponse)verifyPatientCode.Value;

            // Assert
            var result = Assert.IsAssignableFrom<APIResponse>(apiResponseActual);
            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
            Assert.Equal(((Patient)apiResponse.Data).PatientId, ((Patient)apiResponseActual.Data).PatientId);
            Assert.Equal(((Patient)apiResponse.Data).FirstName, ((Patient)apiResponseActual.Data).FirstName);
            Assert.Equal(((Patient)apiResponse.Data).LastName, ((Patient)apiResponseActual.Data).LastName);
        }

        [Fact]
        public async void VerifyPatientCode_Success_WhenPatientCodeIsNotExist()
        {
            // Arrange
            APIResponse apiResponse = new APIResponse { Success = false, Message = "Patient code is not exist", Data = null };

            string patientId = "pat1";

            Patient patient = null;

            _patientRepository.Setup(x => x.VerifyPatientCode(patientId)).ReturnsAsync(patient);

            // Act
            NotFoundObjectResult verifyPatientCode = (NotFoundObjectResult)await _sut.VerifyPatientCode(patientId);
            APIResponse apiResponseActual = (APIResponse)verifyPatientCode.Value;

            // Assert
            var result = Assert.IsAssignableFrom<APIResponse>(apiResponseActual);
            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
            Assert.Equal(apiResponse.Data, apiResponseActual.Data);
        }

        [Fact]
        public async void VerifyPatientCode_Success_BadRequest()
        {
            // Arrange
            string patientId = null;

            // Act
            var verifyPatientCode = await _sut.VerifyPatientCode(patientId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(verifyPatientCode);
        }
        #endregion

        //UT for api GetServices
        #region
        [Fact]
        public async void ViewEvaluationService_Success()
        {
            // Arrange
            List<Service> services = new List<Service>();

            Service service1 = new Service();
            service1.SerId = 1;
            service1.SerName = "ServiceName1";

            Service service2 = new Service();
            service2.SerId = 2;
            service2.SerName = "ServiceName2";

            Service service3 = new Service();
            service3.SerId = 3;
            service3.SerName = "ServiceName3";

            services.Add(service1);
            services.Add(service2);
            services.Add(service3);

            List<ServiceResponseForPatient> serviceResponseForPatients = new List<ServiceResponseForPatient>();

            ServiceResponseForPatient serviceResponseForPatient1 = new ServiceResponseForPatient();
            serviceResponseForPatient1.SerId = 1;
            serviceResponseForPatient1.SerName = "ServiceName1";

            ServiceResponseForPatient serviceResponseForPatient2 = new ServiceResponseForPatient();
            serviceResponseForPatient2.SerId = 2;
            serviceResponseForPatient2.SerName = "ServiceName2";

            ServiceResponseForPatient serviceResponseForPatient3 = new ServiceResponseForPatient();
            serviceResponseForPatient3.SerId = 3;
            serviceResponseForPatient3.SerName = "ServiceName3";

            serviceResponseForPatients.Add(serviceResponseForPatient1);
            serviceResponseForPatients.Add(serviceResponseForPatient2);
            serviceResponseForPatients.Add(serviceResponseForPatient3);

            _serviceRepository.Setup(x => x.GetServices(null)).ReturnsAsync(services);
            _mapper.Setup(x => x.Map<List<ServiceResponseForPatient>>(services)).Returns(serviceResponseForPatients);

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = true,
                Message = "Get services successful",
                Data = serviceResponseForPatients
            };
            // Act
            var getServices = await _sut.GetServices();
            var actualApiRespone = (APIResponse)((OkObjectResult)getServices).Value;

            // Assert
            Assert.IsType<OkObjectResult>(getServices);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
            Assert.Equal(3, ((List<ServiceResponseForPatient>)actualApiRespone.Data).Count);
        }
        #endregion

        //UT for api GetEvaluationCriteriaForPatient
        #region
        [Fact]
        public async void GetEvaluationCriteriaForPatient_Success()
        {
            // Arrange
            Service service1 = new Service();
            service1.SerId = 1;
            service1.SerName = "ServiceName1";

            List<EvaluationCriteria> evaluationCriterias = new List<EvaluationCriteria>();
            EvaluationCriteria evaluationCriteria1 = new EvaluationCriteria();
            evaluationCriteria1.CriId = 1;
            evaluationCriteria1.SerId = 1;

            EvaluationCriteria evaluationCriteria2 = new EvaluationCriteria();
            evaluationCriteria2.CriId = 2;
            evaluationCriteria2.SerId = 1;

            EvaluationCriteria evaluationCriteria3 = new EvaluationCriteria();
            evaluationCriteria3.CriId = 3;
            evaluationCriteria3.SerId = 1;

            evaluationCriterias.Add(evaluationCriteria1);
            evaluationCriterias.Add(evaluationCriteria2);
            evaluationCriterias.Add(evaluationCriteria3);

            List<EvaluationCriteriaForPatientResponse> response = new List<EvaluationCriteriaForPatientResponse>();
            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse1 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse1.CriId = 1;

            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse2 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse2.CriId = 2;

            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse3 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse3.CriId = 3;

            response.Add(evaluationCriteriaForPatientResponse1);
            response.Add(evaluationCriteriaForPatientResponse2);
            response.Add(evaluationCriteriaForPatientResponse3);

            _serviceRepository.Setup(x => x.GetService(1)).ReturnsAsync(service1);
            _evaluationCriteriaRepository.Setup(x => x.GetEvaluationCriteriaForPatient(1)).ReturnsAsync(evaluationCriterias);
            _mapper.Setup(x => x.Map<List<EvaluationCriteriaForPatientResponse>>(evaluationCriterias)).Returns(response);

            var customDTO = new
            {
                SerId = 1,
                SerName = "ServiceName1",
                Results = response
            };

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = true,
                Message = "All evaluation criteria for patient is retrieved successfully.",
                Data = customDTO
            };
            // Act
            var getEvaluationCriteriaForPatient = await _sut.GetEvaluationCriteriaForPatient(1);
            var actualApiRespone = (APIResponse)((OkObjectResult)getEvaluationCriteriaForPatient).Value;

            // Assert
            Assert.IsType<OkObjectResult>(getEvaluationCriteriaForPatient);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void GetEvaluationCriteriaForPatient_NotFound()
        {
            // Arrange
            Service service1 = null;
            _serviceRepository.Setup(x => x.GetService(1)).ReturnsAsync(service1);

            APIResponse expectedApiResponse = new APIResponse 
            { 
                Success = false, 
                Message = "Service request is not exist." 
            };

            // Act
            var getEvaluationCriteriaForPatient = await _sut.GetEvaluationCriteriaForPatient(1);
            var actualApiRespone = (APIResponse)((NotFoundObjectResult)getEvaluationCriteriaForPatient).Value;

            // Assert
            Assert.IsType<NotFoundObjectResult>(getEvaluationCriteriaForPatient);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }
        #endregion

        //UT for api GetEvaluationResultForPatient
        #region
        [Fact]
        public async void GetEvaluationResultForPatient_Success()
        {
            // Arrange
            EvaluationResultForPatientRequest request = new EvaluationResultForPatientRequest();
            request.serId = 1;
            request.patId = "patId001";

            Service service1 = new Service();
            service1.SerId = 1;
            service1.SerName = "ServiceName1";

            Patient patient = new Patient();
            patient.PatientId = "patId001";
            patient.PhoneNumber = "1234567890";

            List<EvaluationCriteria> evaluationCriterias = new List<EvaluationCriteria>();
            EvaluationCriteria evaluationCriteria1 = new EvaluationCriteria();
            evaluationCriteria1.CriId = 1;
            evaluationCriteria1.SerId = 1;

            EvaluationCriteria evaluationCriteria2 = new EvaluationCriteria();
            evaluationCriteria2.CriId = 2;
            evaluationCriteria2.SerId = 1;

            EvaluationCriteria evaluationCriteria3 = new EvaluationCriteria();
            evaluationCriteria3.CriId = 3;
            evaluationCriteria3.SerId = 1;

            evaluationCriterias.Add(evaluationCriteria1);
            evaluationCriterias.Add(evaluationCriteria2);
            evaluationCriterias.Add(evaluationCriteria3);

            ServiceFeedback serviceFeedback = new ServiceFeedback();
            serviceFeedback.FbId = 1;
            serviceFeedback.SerId = 1;
            serviceFeedback.PatientId = "patId001";
            serviceFeedback.Feedback = "Feed back";

            List<EvaluationCriteriaForPatientResponse> response = new List<EvaluationCriteriaForPatientResponse>();
            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse1 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse1.CriId = 1;

            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse2 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse2.CriId = 2;

            EvaluationCriteriaForPatientResponse evaluationCriteriaForPatientResponse3 = new EvaluationCriteriaForPatientResponse();
            evaluationCriteriaForPatientResponse3.CriId = 3;

            response.Add(evaluationCriteriaForPatientResponse1);
            response.Add(evaluationCriteriaForPatientResponse2);
            response.Add(evaluationCriteriaForPatientResponse3);

            EvaluationData evaluationData1 = new EvaluationData();
            evaluationData1.CriId = 1;
            evaluationData1.PatientId = "patId001";
            evaluationData1.Point = 5;

            EvaluationData evaluationData2 = new EvaluationData();
            evaluationData2.CriId = 2;
            evaluationData2.PatientId = "patId001";
            evaluationData2.Point = 4;

            EvaluationData evaluationData3 = new EvaluationData();
            evaluationData3.CriId = 3;
            evaluationData3.PatientId = "patId001";
            evaluationData3.Point = 3;

            EvaluationResultForPatientResponse evaluationResultForPatientResponse1 = new EvaluationResultForPatientResponse();
            evaluationResultForPatientResponse1.EvaDataId = 1;
            evaluationResultForPatientResponse1.Point = 5;

            EvaluationResultForPatientResponse evaluationResultForPatientResponse2 = new EvaluationResultForPatientResponse();
            evaluationResultForPatientResponse2.EvaDataId = 1;
            evaluationResultForPatientResponse2.Point = 5;

            EvaluationResultForPatientResponse evaluationResultForPatientResponse3 = new EvaluationResultForPatientResponse();
            evaluationResultForPatientResponse3.EvaDataId = 1;
            evaluationResultForPatientResponse3.Point = 5;

            ServiceFeedbackForPatientResponse serviceFeedbackForPatientResponse = new ServiceFeedbackForPatientResponse();
            serviceFeedbackForPatientResponse.FbId = 1;
            serviceFeedbackForPatientResponse.Feedback = "Feed back";

            _serviceRepository.Setup(x => x.GetService(request.serId)).ReturnsAsync(service1);
            _patientRepository.Setup(x => x.GetPatient(request.patId)).ReturnsAsync(patient);
            _evaluationCriteriaRepository.Setup(x => x.GetEvaluationCriteriaForPatient(request.serId)).ReturnsAsync(evaluationCriterias);
            _serviceFeedbackRepository.Setup(x => x.GetServiceFeedbackForPatient(request)).ReturnsAsync(serviceFeedback);
            _mapper.Setup(x => x.Map<List<EvaluationCriteriaForPatientResponse>>(evaluationCriterias)).Returns(response);
            _evaluationDataRepository.Setup(x => x.GetEvaluationResultForPatient(1, request)).ReturnsAsync(evaluationData1);
            _mapper.Setup(x => x.Map<EvaluationResultForPatientResponse>(evaluationData1)).Returns(evaluationResultForPatientResponse1);
            _evaluationDataRepository.Setup(x => x.GetEvaluationResultForPatient(2, request)).ReturnsAsync(evaluationData2);
            _mapper.Setup(x => x.Map<EvaluationResultForPatientResponse>(evaluationData2)).Returns(evaluationResultForPatientResponse2);
            _evaluationDataRepository.Setup(x => x.GetEvaluationResultForPatient(3, request)).ReturnsAsync(evaluationData3);
            _mapper.Setup(x => x.Map<EvaluationResultForPatientResponse>(evaluationData3)).Returns(evaluationResultForPatientResponse3);
            _mapper.Setup(x => x.Map<ServiceFeedbackForPatientResponse>(serviceFeedback)).Returns(serviceFeedbackForPatientResponse);

            var customDTO = new
            {
                Patient = patient,
                Service = service1,
                ServiceFeedback = serviceFeedback,
                Results = response
            };

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = true,
                Message = "All evaluation result for patient is retrieved successfully.",
                Data = customDTO
            };
            // Act
            var getEvaluationResultForPatient = await _sut.GetEvaluationResultForPatient(request);
            var actualApiRespone = (APIResponse)((OkObjectResult)getEvaluationResultForPatient).Value;

            // Assert
            Assert.IsType<OkObjectResult>(getEvaluationResultForPatient);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void GetEvaluationResultForPatient_BadRequest_ServiceNotFound()
        {
            // Arrange
            EvaluationResultForPatientRequest request = new EvaluationResultForPatientRequest();
            request.serId = 1;
            request.patId = "patId001";

            Service service = null;
            _serviceRepository.Setup(x => x.GetService(1)).ReturnsAsync(service);

            APIResponse expectedApiResponse = new APIResponse
            { 
                Success = false, 
                Message = "Service request is not exist." 
            };

            // Act
            var getEvaluationResultForPatient = await _sut.GetEvaluationResultForPatient(request);
            var actualApiRespone = (APIResponse)((NotFoundObjectResult)getEvaluationResultForPatient).Value;

            // Assert
            Assert.IsType<NotFoundObjectResult>(getEvaluationResultForPatient);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void GetEvaluationResultForPatient_BadRequest_PatientNotFound()
        {
            // Arrange
            EvaluationResultForPatientRequest request = new EvaluationResultForPatientRequest();
            request.serId = 1;
            request.patId = "patId001";

            Service service1 = new Service();
            service1.SerId = 1;
            service1.SerName = "ServiceName1";

            Patient patient = null;
            
            _serviceRepository.Setup(x => x.GetService(1)).ReturnsAsync(service1);
            _patientRepository.Setup(x => x.GetPatient(request.patId)).ReturnsAsync(patient);

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = false,
                Message = "Patient request is not exist."
            };

            // Act
            var getEvaluationResultForPatient = await _sut.GetEvaluationResultForPatient(request);
            var actualApiRespone = (APIResponse)((NotFoundObjectResult)getEvaluationResultForPatient).Value;

            // Assert
            Assert.IsType<NotFoundObjectResult>(getEvaluationResultForPatient);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }
        #endregion

        //UT for api SubmitEvaluationResult
        #region
        [Fact]
        public async void SubmitEvaluationResult_Success()
        {
            // Arrange
            string patientId = "patId001";

            EvaluationDataAnswer evaluationDataRequest1 = new EvaluationDataAnswer();
            evaluationDataRequest1.CriId = 1;
            evaluationDataRequest1.Point = 5;

            EvaluationDataAnswer evaluationDataRequest2 = new EvaluationDataAnswer();
            evaluationDataRequest2.CriId = 2;
            evaluationDataRequest2.Point = 4;

            EvaluationDataAnswer evaluationDataRequest3 = new EvaluationDataAnswer();
            evaluationDataRequest3.CriId = 3;
            evaluationDataRequest3.Point = 3;

            List<EvaluationDataAnswer> evaluationDataRequestList = new List<EvaluationDataAnswer> { evaluationDataRequest1, evaluationDataRequest2, evaluationDataRequest3 };

            EvaluationData evaluationData1 = new EvaluationData();
            evaluationData1.CriId = 1;
            evaluationData1.Point = 5;

            EvaluationData evaluationData2 = new EvaluationData();
            evaluationData2.CriId = 2;
            evaluationData1.Point = 4;

            EvaluationData evaluationData3 = new EvaluationData();
            evaluationData3.CriId = 3;
            evaluationData1.Point = 3;

            List<EvaluationData> evaluationDataList = new List<EvaluationData> { evaluationData1, evaluationData2, evaluationData3 };

            EvaluationSubmitRequest request = new EvaluationSubmitRequest();
            request.SerId = 1;
            request.Feedback = "Feed back";
            request.EvaluationData = evaluationDataRequestList;

            ServiceFeedback serviceFeedback = new ServiceFeedback();
            serviceFeedback.FbId = 1;
            serviceFeedback.SerId = 1;
            serviceFeedback.PatientId = "patId001";
            serviceFeedback.Feedback = "Feed back";

            _mapper.Setup(x => x.Map<List<EvaluationData>>(request.EvaluationData)).Returns(evaluationDataList);
            _mapper.Setup(x => x.Map<ServiceFeedback>(request)).Returns(serviceFeedback);
            _patientRepository.Setup(x => x.AddAllEvaluaionData(evaluationDataList)).ReturnsAsync(3);
            _patientRepository.Setup(x => x.AddServiceFeedback(serviceFeedback)).ReturnsAsync(true);

            APIResponse expectedApiResponse = new APIResponse 
            { 
                Success = true, 
                Message = "Submit evaluation result successfully."
            };

            // Act
            var submitEvaluationResult = await _sut.SubmitEvaluation(patientId, request);
            var actualApiRespone = (APIResponse)((OkObjectResult)submitEvaluationResult).Value;

            // Assert
            Assert.IsType<OkObjectResult>(submitEvaluationResult);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void SubmitEvaluationResult_Conflict_SaveDataConflict()
        {
            // Arrange
            string patientId = "patId001";

            EvaluationDataAnswer evaluationDataRequest1 = new EvaluationDataAnswer();
            evaluationDataRequest1.CriId = 1;
            evaluationDataRequest1.Point = 5;

            EvaluationDataAnswer evaluationDataRequest2 = new EvaluationDataAnswer();
            evaluationDataRequest2.CriId = 2;
            evaluationDataRequest2.Point = 4;

            EvaluationDataAnswer evaluationDataRequest3 = new EvaluationDataAnswer();
            evaluationDataRequest3.CriId = 3;
            evaluationDataRequest3.Point = 3;

            List<EvaluationDataAnswer> evaluationDataRequestList = new List<EvaluationDataAnswer> { evaluationDataRequest1, evaluationDataRequest2, evaluationDataRequest3 };

            EvaluationData evaluationData1 = new EvaluationData();
            evaluationData1.CriId = 1;
            evaluationData1.Point = 5;

            EvaluationData evaluationData2 = new EvaluationData();
            evaluationData2.CriId = 2;
            evaluationData1.Point = 4;

            EvaluationData evaluationData3 = new EvaluationData();
            evaluationData3.CriId = 3;
            evaluationData1.Point = 3;

            List<EvaluationData> evaluationDataList = new List<EvaluationData> { evaluationData1, evaluationData2, evaluationData3 };

            EvaluationSubmitRequest request = new EvaluationSubmitRequest();
            request.SerId = 1;
            request.Feedback = "Feed back";
            request.EvaluationData = evaluationDataRequestList;

            ServiceFeedback serviceFeedback = new ServiceFeedback();
            serviceFeedback.FbId = 1;
            serviceFeedback.SerId = 1;
            serviceFeedback.PatientId = "patId001";
            serviceFeedback.Feedback = "Feed back";

            _mapper.Setup(x => x.Map<List<EvaluationData>>(request.EvaluationData)).Returns(evaluationDataList);
            _mapper.Setup(x => x.Map<ServiceFeedback>(request)).Returns(serviceFeedback);
            _patientRepository.Setup(x => x.AddAllEvaluaionData(evaluationDataList)).ReturnsAsync(2); //2 != 3
            _patientRepository.Setup(x => x.AddServiceFeedback(serviceFeedback)).ReturnsAsync(true);

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = false,
                Message = "Submit evaluation result failure."
            };

            // Act
            var submitEvaluationResult = await _sut.SubmitEvaluation(patientId, request);
            var actualApiRespone = (APIResponse)((ConflictObjectResult)submitEvaluationResult).Value;

            // Assert
            Assert.IsType<ConflictObjectResult>(submitEvaluationResult);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void SubmitEvaluationResult_Conflict_SaveDataOccurError()
        {
            // Arrange
            string patientId = "patId001";

            EvaluationDataAnswer evaluationDataRequest1 = new EvaluationDataAnswer();
            evaluationDataRequest1.CriId = 1;
            evaluationDataRequest1.Point = 5;

            EvaluationDataAnswer evaluationDataRequest2 = new EvaluationDataAnswer();
            evaluationDataRequest2.CriId = 2;
            evaluationDataRequest2.Point = 4;

            EvaluationDataAnswer evaluationDataRequest3 = new EvaluationDataAnswer();
            evaluationDataRequest3.CriId = 3;
            evaluationDataRequest3.Point = 3;

            List<EvaluationDataAnswer> evaluationDataRequestList = new List<EvaluationDataAnswer> { evaluationDataRequest1, evaluationDataRequest2, evaluationDataRequest3 };

            EvaluationData evaluationData1 = new EvaluationData();
            evaluationData1.CriId = 1;
            evaluationData1.Point = 5;

            EvaluationData evaluationData2 = new EvaluationData();
            evaluationData2.CriId = 2;
            evaluationData1.Point = 4;

            EvaluationData evaluationData3 = new EvaluationData();
            evaluationData3.CriId = 3;
            evaluationData1.Point = 3;

            List<EvaluationData> evaluationDataList = new List<EvaluationData> { evaluationData1, evaluationData2, evaluationData3 };

            EvaluationSubmitRequest request = new EvaluationSubmitRequest();
            request.SerId = 1;
            request.Feedback = "Feed back";
            request.EvaluationData = evaluationDataRequestList;

            ServiceFeedback serviceFeedback = new ServiceFeedback();
            serviceFeedback.FbId = 1;
            serviceFeedback.SerId = 1;
            serviceFeedback.PatientId = "patId001";
            serviceFeedback.Feedback = "Feed back";

            _mapper.Setup(x => x.Map<List<EvaluationData>>(request.EvaluationData)).Returns(evaluationDataList);
            _mapper.Setup(x => x.Map<ServiceFeedback>(request)).Returns(serviceFeedback);
            _patientRepository.Setup(x => x.AddAllEvaluaionData(evaluationDataList)).ReturnsAsync(3); //2 != 3
            _patientRepository.Setup(x => x.AddServiceFeedback(serviceFeedback)).ReturnsAsync(false);

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = false,
                Message = "Submit evaluation result failure."
            };

            // Act
            var submitEvaluationResult = await _sut.SubmitEvaluation(patientId, request);
            var actualApiRespone = (APIResponse)((ConflictObjectResult)submitEvaluationResult).Value;

            // Assert
            Assert.IsType<ConflictObjectResult>(submitEvaluationResult);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }

        [Fact]
        public async void SubmitEvaluationResult_Conflict_DatabaseOccurError()
        {
            // Arrange
            string patientId = "patId001";

            EvaluationDataAnswer evaluationDataRequest1 = new EvaluationDataAnswer();
            evaluationDataRequest1.CriId = 1;
            evaluationDataRequest1.Point = 5;

            EvaluationDataAnswer evaluationDataRequest2 = new EvaluationDataAnswer();
            evaluationDataRequest2.CriId = 2;
            evaluationDataRequest2.Point = 4;

            EvaluationDataAnswer evaluationDataRequest3 = new EvaluationDataAnswer();
            evaluationDataRequest3.CriId = 3;
            evaluationDataRequest3.Point = 3;

            List<EvaluationDataAnswer> evaluationDataRequestList = new List<EvaluationDataAnswer> { evaluationDataRequest1, evaluationDataRequest2, evaluationDataRequest3 };

            EvaluationData evaluationData1 = new EvaluationData();
            evaluationData1.CriId = 1;
            evaluationData1.Point = 5;

            EvaluationData evaluationData2 = new EvaluationData();
            evaluationData2.CriId = 2;
            evaluationData1.Point = 4;

            EvaluationData evaluationData3 = new EvaluationData();
            evaluationData3.CriId = 3;
            evaluationData1.Point = 3;

            List<EvaluationData> evaluationDataList = new List<EvaluationData> { evaluationData1, evaluationData2, evaluationData3 };

            EvaluationSubmitRequest request = new EvaluationSubmitRequest();
            request.SerId = 1;
            request.Feedback = "Feed back";
            request.EvaluationData = evaluationDataRequestList;

            ServiceFeedback serviceFeedback = new ServiceFeedback();
            serviceFeedback.FbId = 1;
            serviceFeedback.SerId = 1;
            serviceFeedback.PatientId = "patId001";
            serviceFeedback.Feedback = "Feed back";

            _mapper.Setup(x => x.Map<List<EvaluationData>>(request.EvaluationData)).Returns(evaluationDataList);
            _mapper.Setup(x => x.Map<ServiceFeedback>(request)).Returns(serviceFeedback);
            _patientRepository.Setup(x => x.AddAllEvaluaionData(evaluationDataList)).ReturnsAsync(3); //2 != 3
            _patientRepository.Setup(x => x.AddServiceFeedback(serviceFeedback)).Throws(new Exception("Database occur error"));

            APIResponse expectedApiResponse = new APIResponse
            {
                Success = false,
                Message = "Database occur error"
            };

            // Act
            var submitEvaluationResult = await _sut.SubmitEvaluation(patientId, request);
            var actualApiRespone = (APIResponse)((ConflictObjectResult)submitEvaluationResult).Value;

            // Assert
            Assert.IsType<ConflictObjectResult>(submitEvaluationResult);
            Assert.Equal(expectedApiResponse.Message, actualApiRespone.Message); // (expected, actual)
        }
        #endregion
    }
}