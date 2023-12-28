using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using HSEApi.Controllers;
using UserObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Moq;
using Repositories;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObjects;

namespace HSEApi.Tests.Controllers
{
    //public class E1Test
    //{
    //    private readonly EvaluationCriteriaController _sut;
    //    private readonly IFixture _fixture;
    //    private readonly Mock<IServiceRepository> _serviceRepository;
    //    private readonly Mock<IUserRepository> _userRepository;
    //    private readonly Mock<IEvaluationCriteriaRepository> _evaCriRepository;
    //    private readonly Mock<IMapper> _mapper;

    //    public E1Test()
    //    {
    //        _fixture = new Fixture();
    //        _fixture.Customize(new AutoMoqCustomization());
    //        _userRepository = _fixture.Freeze<Mock<IUserRepository>>();
    //        _serviceRepository = _fixture.Freeze<Mock<IServiceRepository>>();
    //        _evaCriRepository = _fixture.Freeze<Mock<IEvaluationCriteriaRepository>>();
    //        _mapper = _fixture.Freeze<Mock<IMapper>>();
    //        _sut = new EvaluationCriteriaController(_serviceRepository.Object, _userRepository.Object, _evaCriRepository.Object, _mapper.Object); //Create the implement in memory
    //    }

    //    [Fact]
    //    public async void GetAllEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        FilteredResponse filteredResponse = new FilteredResponse();
    //        filteredResponse.search = "abc";
    //        filteredResponse.sortedBy = "id";
    //        filteredResponse.page = 1;
    //        filteredResponse.pageSize = 2;

    //        EvaluationCriteriaRequest evaluationCriteriaRequest = new EvaluationCriteriaRequest();
    //        evaluationCriteriaRequest.serId = 1;
    //        evaluationCriteriaRequest.from = DateTime.Parse("2023-10-01");
    //        evaluationCriteriaRequest.to = DateTime.Parse("2023-11-01");

    //        List<EvaluationCriteria> evaluationCriteriaList = new List<EvaluationCriteria>();

    //        EvaluationCriteria e1 = new EvaluationCriteria();
    //        e1.CriId = 1;
    //        e1.SerId = 1;
    //        e1.CreatedBy = 1;

    //        EvaluationCriteria e2 = new EvaluationCriteria();
    //        e1.CriId = 2;
    //        e1.SerId = 1;
    //        e1.CreatedBy = 1;

    //        evaluationCriteriaList.Add(e1);
    //        evaluationCriteriaList.Add(e2);

    //        List<EvaluationCriteriaResponse> evaluationCriteriaResponseList = new List<EvaluationCriteriaResponse>();
    //        EvaluationCriteriaResponse eResponse1 = new EvaluationCriteriaResponse();
    //        e1.CriId = 1;
    //        e1.SerId = 1;
    //        e1.CreatedBy = 1;

    //        EvaluationCriteriaResponse eResponse2 = new EvaluationCriteriaResponse();
    //        e1.CriId = 2;
    //        e1.SerId = 1;
    //        e1.CreatedBy = 1;

    //        evaluationCriteriaResponseList.Add(eResponse1);
    //        evaluationCriteriaResponseList.Add(eResponse2);

    //        User user = new User();
    //        user.UserId = 1;

    //        Service service = new Service();
    //        service.SerId = 1;

    //        UserResponse userResponse = new UserResponse();
    //        ServiceResponse serviceResponse = new ServiceResponse();

    //        _evaCriRepository.Setup(x => x.GetAllEvaluationCriteria("QAO", evaluationCriteriaRequest, filteredResponse)).ReturnsAsync(evaluationCriteriaList);
    //        _mapper.Setup(x => x.Map<List<EvaluationCriteriaResponse>>(evaluationCriteriaList)).Returns(evaluationCriteriaResponseList);
    //        _userRepository.Setup(x => x.GetUserByUserId(evaluationCriteriaResponseList[0].CreatedBy)).ReturnsAsync(user);
    //        _serviceRepository.Setup(x => x.GetService(evaluationCriteriaResponseList[0].SerId)).ReturnsAsync(service);
    //        _userRepository.Setup(x => x.GetUserByUserId(evaluationCriteriaResponseList[1].CreatedBy)).ReturnsAsync(user);
    //        _serviceRepository.Setup(x => x.GetService(evaluationCriteriaResponseList[1].SerId)).ReturnsAsync(service);
    //        _mapper.Setup(x => x.Map<UserResponse>(user)).Returns(userResponse);
    //        _mapper.Setup(x => x.Map<ServiceResponse>(service)).Returns(serviceResponse);

    //        // Act
    //        var getAllEvaluationCriteriaResponse = await _sut.GetAllEvaluationCriteria(filteredResponse, evaluationCriteriaRequest);

    //        // Assert
    //        Assert.IsType<OkObjectResult>(getAllEvaluationCriteriaResponse);
    //    }
    //}
}
