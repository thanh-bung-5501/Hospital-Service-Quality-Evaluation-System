using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using HSEApi.Controllers;
using UserObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repositories;
using Repositories.Models;

namespace HSEApi.Tests.Controllers
{
    //public class UserControllerTest
    //{
    //    private readonly UserController _sut;
    //    private readonly IFixture _fixture;
    //    private readonly Mock<IUserRepository> _userRepository;
    //    private readonly Mock<IRoleDistributionRepository> _roleRepositoryMock;
    //    private readonly Mock<IAzureStorage> _azureStorage;
    //    private readonly Mock<IMapper> _mapper;

    //    public UserControllerTest()
    //    {
    //        _fixture = new Fixture();
    //        _fixture.Customize(new AutoMoqCustomization());
    //        _userRepository = _fixture.Freeze<Mock<IUserRepository>>();
    //        _roleRepositoryMock = _fixture.Freeze<Mock<IRoleDistributionRepository>>();
    //        _mapper = _fixture.Freeze<Mock<IMapper>>();
    //        _sut = new UserController(_userRepository.Object, _roleRepositoryMock.Object, _azureStorage.Object, _mapper.Object); //Create the implement in memory
    //    }

    //    [Fact]
    //    public async void AddUser_Success()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "a@gmail.com";
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = true, Message = "Add new user successful", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);
    //        //mock.Setup(x => x.GetUserRole(httpContextMock.Object)).Returns("a");

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((OkObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<OkObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //        Assert.Equal(((UserResponse)apiResponse.Data).Email, ((UserResponse)apiResponseActual.Data).Email);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenEmailIsNull()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = null;
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Please enter required field!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenPasswordIsNull()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "a@gmail.com";
    //        userResponse.Password = null;
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Please enter required field!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenEmailIsExisted()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "a@gmail.com";
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Email already registered!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(true);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenEmailIsInvalid()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "@gmail.com";
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Invalid email format!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(false);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenPasswordIsInvalid()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "@gmail.com";
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "123456789";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Invalid password!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(false);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(true);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }

    //    [Fact]
    //    public async void AddUser_BadRequest_WhenPhoneNumberIsInvalid()
    //    {
    //        // Arrange
    //        var userResponse = new UserResponse();
    //        userResponse.Email = "a@gmail.com";
    //        userResponse.Password = "1234567a";
    //        userResponse.FirstName = "Test";
    //        userResponse.LastName = "Test";
    //        userResponse.GenderId = 1;
    //        try
    //        {
    //            userResponse.Dob = DateTime.Parse("2001-10-03");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //        userResponse.PhoneNumber = "1234567abc";
    //        userResponse.Address = "Address";
    //        userResponse.RoleDistribution = new RoleDistribution { UserId = 1, MAdmin = true, MBOM = false, MQAO = false };

    //        APIResponse apiResponse = new APIResponse { Success = false, Message = "Invalid phone number format!", Data = userResponse };

    //        _userRepository.Setup(x => x.CheckExistedEmail(userResponse.Email)).ReturnsAsync(false);
    //        _userRepository.Setup(x => x.IsValidEmail(userResponse.Email)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPassword(userResponse.Password)).Returns(true);
    //        _userRepository.Setup(x => x.IsValidPhoneNumber(userResponse.PhoneNumber)).Returns(false);

    //        // Act
    //        var addUserResponse = await _sut.AddUser(userResponse);
    //        APIResponse apiResponseActual = (APIResponse)((BadRequestObjectResult)addUserResponse).Value;

    //        // Assert
    //        Assert.IsType<BadRequestObjectResult>(addUserResponse);
    //        Assert.Equal(apiResponse.Success, apiResponseActual.Success); //(expected, actual)
    //        Assert.Equal(apiResponse.Message, apiResponseActual.Message);
    //    }
    //}
}