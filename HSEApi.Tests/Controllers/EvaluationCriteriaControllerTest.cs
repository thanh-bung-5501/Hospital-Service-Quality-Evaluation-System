using AutoFixture;
using AutoMapper;
using HSEApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repositories;
using Repositories.Models;
using SystemObjects;

namespace HSEApi.Tests.Controllers
{
    //public class EvaluationCriteriaControllerTest
    //{
    //    private readonly EvaluationCriteriaController _sut;
    //    private readonly IFixture _fixture;
    //    private readonly Mock<IUserRepository> _uRepoMock;
    //    private readonly Mock<IServiceRepository> _serRepoMock;
    //    private readonly Mock<IEvaluationCriteriaRepository> _evaCriRepoMock;
    //    private readonly Mock<IMapper> _mapper;

    //    public EvaluationCriteriaControllerTest()
    //    {
    //        _fixture = new Fixture();
    //        _uRepoMock = _fixture.Freeze<Mock<IUserRepository>>();
    //        _serRepoMock = _fixture.Freeze<Mock<IServiceRepository>>();
    //        //_evaCriRepoMock = _fixture.Freeze<Mock<IEvaluationCriteriaRepository>>();
    //        _evaCriRepoMock = new Mock<IEvaluationCriteriaRepository>();
    //        //_mapper = new Mock<IMapper>();
    //        _sut = new EvaluationCriteriaController(_serRepoMock.Object, _uRepoMock.Object, _evaCriRepoMock.Object, null); //Create the implement in memory
    //    }

    //    [Fact]
    //    public async void GetAllEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        var request = new EvaluationCriteriaRequest();
    //        var filtered = new FilteredResponse();
    //        var pagedResponse = new PagedResponse<EvaluationCriteriaResponse>
    //        {
    //            CurrentPage = filtered.page,
    //            PageSize = filtered.pageSize,
    //        };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "All evaluation criteria is retrieved successfully.", Data = pagedResponse };

    //        try
    //        {
    //            // Check validation
    //            string role = "QAO";
    //            _evaCriRepoMock.Setup(x => x.GetAllEvaluationCriteria(role, request, filtered)).ReturnsAsync(new List<EvaluationCriteria>());

    //            // Act
    //            var pagedResponseActual = await _sut.GetAllEvaluationCriteria(filtered, request);
    //            APIResponse apiResponseActual = (APIResponse)((OkObjectResult)pagedResponseActual).Value;

    //            // Assert
    //            Assert.IsType<OkObjectResult>(pagedResponseActual);
    //            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //            Assert.Equal(((PagedResponse<EvaluationCriteriaResponse>)apiResponse.Data).CurrentPage,
    //                ((PagedResponse<EvaluationCriteriaResponse>)apiResponseActual.Data).CurrentPage);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }

    //    [Fact]
    //    public async void GetEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        var dataResponse = new EvaluationCriteriaResponse
    //        {
    //            CriId = 1,
    //            CriDesc = "Test",
    //            CreatedBy = 10,
    //            ModifiedBy = 10,
    //            SerId = 1,
    //        };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "Evaluation criteria is retrieved successfully.", Data = dataResponse };

    //        try
    //        {
    //            // Check validation
    //            _evaCriRepoMock.Setup(x => x.GetEvaluationCriteria(dataResponse.CriId)).ReturnsAsync(new EvaluationCriteria());

    //            // Act
    //            var dataResponseActual = await _sut.GetEvaluationCriteria(dataResponse.CriId);
    //            APIResponse apiResponseActual = (APIResponse)((OkObjectResult)dataResponseActual).Value;

    //            // Assert
    //            Assert.IsType<OkObjectResult>(dataResponseActual);
    //            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).CriDesc,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).CriDesc);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }

    //    [Fact]
    //    public async void AddEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        EvaluationCriteria data = null;
    //        var dataResponse = new EvaluationCriteriaAddRequest
    //        {
    //            CriDesc = "Test14",
    //            CreatedBy = 10,
    //            SerId = 2,
    //        };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "New evaluation criteria is added successfully.", Data = dataResponse };

    //        try
    //        {
    //            // Check validation
    //            _evaCriRepoMock.Setup(x => x.GetEvaluationCriteriaByDescription(dataResponse.CriDesc)).ReturnsAsync(data);

    //            // Act
    //            var dataResponseActual = await _sut.AddEvaluationCriteria(dataResponse);
    //            APIResponse apiResponseActual = (APIResponse)((OkObjectResult)dataResponseActual).Value;

    //            // Assert
    //            Assert.IsType<OkObjectResult>(dataResponseActual);
    //            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).CriDesc,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).CriDesc);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).CreatedBy,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).CreatedBy);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }

    //    [Fact]
    //    public async void EditEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        var data = new EvaluationCriteria
    //        {
    //            CriId = 3,
    //            CriDesc = "Test2",
    //            ModifiedBy = 11,
    //            SerId = 2,
    //        };
    //        var dataResponse = new EvaluationCriteriaEditRequest
    //        {
    //            CriId = 3,
    //            CriDesc = "Test2",
    //            ModifiedBy = 10,
    //            SerId = 3,
    //        };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "Current evaluation criteria is updated successfully.", Data = dataResponse };

    //        try
    //        {
    //            // Check validation
    //            _evaCriRepoMock.Setup(x => x.GetEvaluationCriteria(dataResponse.CriId)).ReturnsAsync(data);
    //            _evaCriRepoMock.Setup(x => x.GetEvaluationCriteriaByDescription(dataResponse.CriDesc)).ReturnsAsync(data);

    //            // Act
    //            var dataResponseActual = await _sut.EditEvaluationCriteria(dataResponse.CriId, dataResponse);
    //            APIResponse apiResponseActual = (APIResponse)((OkObjectResult)dataResponseActual).Value;

    //            // Assert
    //            Assert.IsType<OkObjectResult>(dataResponseActual);
    //            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).CriDesc,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).CriDesc);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).ModifiedBy,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).ModifiedBy);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }

    //    [Fact]
    //    public async void ToggleEvaluationCriteria_Success()
    //    {
    //        // Arrange
    //        var data = new EvaluationCriteria
    //        {
    //            CriId = 8,
    //            ModifiedBy = null,
    //            Status = false,
    //        };
    //        var dataResponse = new EvaluationCriteria
    //        {
    //            CriId = 8,
    //            ModifiedBy = 10,
    //            Status = true,
    //        };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "Current evaluation criteria is activated successfully.", Data = dataResponse };

    //        try
    //        {
    //            // Check validation
    //            _evaCriRepoMock.Setup(x => x.GetEvaluationCriteria(dataResponse.CriId)).ReturnsAsync(data);

    //            // Act
    //            var dataResponseActual = await _sut.ToggleEvaluationCriteria(dataResponse.CriId);
    //            APIResponse apiResponseActual = (APIResponse)((OkObjectResult)dataResponseActual).Value;

    //            // Assert
    //            Assert.IsType<OkObjectResult>(dataResponseActual);
    //            Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //            Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).ModifiedBy,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).ModifiedBy);
    //            Assert.Equal(((EvaluationCriteriaResponse)apiResponse.Data).Status,
    //                ((EvaluationCriteriaResponse)apiResponseActual.Data).Status);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }
    //}
}
