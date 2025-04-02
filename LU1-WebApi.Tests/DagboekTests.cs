using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class DagboekTests
{
    private Mock<IDagboekRepository> _mockDagboekRepo;
    private Mock<IAuthenticationService> _mockAuthService;
    private DagboekController _controller;

    public DagboekTests()
    {
        _mockDagboekRepo = new Mock<IDagboekRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _controller = new DagboekController(_mockDagboekRepo.Object, _mockAuthService.Object);
    }

    [TestMethod]
    //Authenticated gebruiker → Notitie succesvol aangemaakt
    public async Task CreateDagboek_ReturnsOk_WhenDagboekIsCreated()
    {
        var dagboek = new DagboekDTO { Note = "Test Entry" };
        var userId = Guid.NewGuid().ToString();

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockDagboekRepo.Setup(repo => repo.CreateDagboek(It.IsAny<DagboekDTO>(), It.IsAny<Guid>()))
                        .ReturnsAsync(Result.Success("Dagboek entry created successfully!"));

        var result = await _controller.CreateDagboek(dagboek);
        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Dagboek entry created successfully!", okResult.Value);
    }

    [TestMethod]
    //Niet-authenticated gebruiker → Foutmelding "No authenticated user found."
    public async Task CreateDagboek_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(string.Empty);

        var result = await _controller.CreateDagboek(new DagboekDTO { Note = "Test Entry" });
        var unauthorizedResult = result as UnauthorizedObjectResult;

        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        Assert.AreEqual("User is not authenticated or invalid user ID.", unauthorizedResult.Value);
    }

    [TestMethod]
    //Authenticated gebruiker → Lijst met dagboeknotities van gebruiker
    public async Task GetDagboekenFromLoggedInUser_ReturnsOk_WhenDagboekenExist()
    {
        var userId = Guid.NewGuid().ToString();

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockDagboekRepo.Setup(repo => repo.GetDagboekenByUserId(It.IsAny<Guid>()))
                        .ReturnsAsync(new List<DagboekDTO> { new DagboekDTO { Note = "Test Entry" } });

        var result = await _controller.GetDagboekenFromLoggedInUser();
        var okResult = result.Result as OkObjectResult;
        var dagboeken = okResult?.Value as IEnumerable<DagboekDTO>;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(dagboeken?.Any());
    }

    [TestMethod]
    //Niet-authenticated gebruiker → Niks
    public async Task GetDagboekenFromLoggedInUser_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(string.Empty);

        var result = await _controller.GetDagboekenFromLoggedInUser();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;

        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        Assert.AreEqual("User is not authenticated or invalid user ID.", unauthorizedResult.Value);
    }
}
