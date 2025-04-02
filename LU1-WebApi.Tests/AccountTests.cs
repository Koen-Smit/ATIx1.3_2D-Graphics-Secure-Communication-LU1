using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

[TestClass]
public class AccountControllerTests
{
    private Mock<IAccountRepository> _mockAccountRepo;
    private Mock<IAuthenticationService> _mockAuthService;
    private Mock<UserManager<AppUser>> _mockUserManager;
    private AccountController _controller;

    public AccountControllerTests()
    {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        var userStore = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _controller = new AccountController(_mockAccountRepo.Object, _mockAuthService.Object, _mockUserManager.Object);
    }

    [TestMethod]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        var request = new AccountRequest { UserName = "testuser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Success("Registration successful!"));

        var result = await _controller.Register(request);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Registration successful!", okResult.Value);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenUsernameAlreadyExists()
    {
        var request = new AccountRequest { UserName = "existinguser", Password = "Test@12345" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Failure("Username is already in use."));

        var result = await _controller.Register(request);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Username is already in use.", badRequestResult.Value);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenPasswordIsTooShort()
    {
        var request = new AccountRequest { UserName = "newuser", Password = "short" };
        _mockAccountRepo.Setup(repo => repo.RegisterUser(request))
                        .ReturnsAsync(Result.Failure("Password does not meet the requirements."));

        var result = await _controller.Register(request);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Password does not meet the requirements.", badRequestResult.Value);
    }

    [TestMethod]
    public async Task Logout_ReturnsOk_WhenLogoutSucceeds()
    {
        _mockAccountRepo.Setup(repo => repo.LogoutUser())
                        .ReturnsAsync(Result.Success("Logout successful!"));

        var result = await _controller.Logout();
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Logout successful!", okResult.Value);
    }
}
