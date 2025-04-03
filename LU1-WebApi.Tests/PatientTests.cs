using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[TestClass]
public class PatientTests
{
    private Mock<IPatientRepository> _mockPatientRepo;
    private Mock<IAuthenticationService> _mockAuthService;
    private PatientController _controller;

    public PatientTests()
    {
        _mockPatientRepo = new Mock<IPatientRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _controller = new PatientController(_mockPatientRepo.Object, _mockAuthService.Object);
    }

    [TestMethod]
    //Authenticated gebruiker zonder patiënt → Succesvol patiënt toegevoegd
    public async Task CreatePatient_ReturnsOk_WhenPatientIsCreated()
    {
        var patient = new PatientDTO { Naam = "John Doe" };
        var userId = Guid.NewGuid().ToString();

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockPatientRepo.Setup(repo => repo.CreatePatient(It.IsAny<PatientDTO>(), It.IsAny<Guid>()))
                        .ReturnsAsync(Result.Success("Patient created successfully!"));

        var result = await _controller.CreatePatient(patient);
        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Patient created successfully!", okResult.Value);
    }

    [TestMethod]
    //Authenticated gebruiker met bestaande patiënt → "This user already has a linked patient."
    public async Task CreatePatient_ReturnsBadRequest_WhenPatientAlreadyExists()
    {
        var patient = new PatientDTO { Naam = "John Doe" };
        var userId = Guid.NewGuid().ToString();

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockPatientRepo.Setup(repo => repo.CreatePatient(It.IsAny<PatientDTO>(), It.IsAny<Guid>()))
                        .ReturnsAsync(Result.Failure("This user already has a linked patient."));

        var result = await _controller.CreatePatient(patient);
        var badRequestResult = result as BadRequestObjectResult;

        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("This user already has a linked patient.", badRequestResult.Value);
    }

    [TestMethod]
    public async Task GetModules_ReturnsOk_WhenModulesExist()
    {
        var userId = Guid.NewGuid().ToString();

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockPatientRepo.Setup(repo => repo.GetPatientFromUserLoggedIn(It.IsAny<Guid>()))
                        .ReturnsAsync(new PatientDTO { PatientID = Guid.NewGuid() });
        _mockPatientRepo.Setup(repo => repo.GetModules(It.IsAny<Guid>()))
                        .ReturnsAsync(new List<ModuleVoortgangDTO> { new ModuleVoortgangDTO { ModuleID = 1, StickerID = 1 } });

        var result = await _controller.GetModules();
        var okResult = result.Result as OkObjectResult;
        var modules = okResult?.Value as IEnumerable<ModuleVoortgangDTO>;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(modules?.Any());
    }

    [TestMethod]
    //Nog niet gemarkeerde module → Succesvolle toevoeging
    public async Task MarkModuleDone_ReturnsOk_WhenModuleIsMarked()
    {
        var userId = Guid.NewGuid().ToString();
        var moduleId = 1;
        var stickerId = 1;

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockPatientRepo.Setup(repo => repo.GetPatientFromUserLoggedIn(It.IsAny<Guid>()))
                        .ReturnsAsync(new PatientDTO { PatientID = Guid.NewGuid() });
        _mockPatientRepo.Setup(repo => repo.MarkModuleDone(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(Result.Success("Module marked as done!"));

        var result = await _controller.MarkModuleDone(moduleId, stickerId);
        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Module marked as done!", okResult.Value);
    }


    [TestMethod]
    //Al voltooide module → "Module has already been marked as done."
    public async Task MarkModuleDone_ReturnsBadRequest_WhenModuleAlreadyMarked()
    {
        var userId = Guid.NewGuid().ToString();
        var moduleId = 1;
        var stickerId = 1;

        _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId())
                        .Returns(userId);
        _mockPatientRepo.Setup(repo => repo.GetPatientFromUserLoggedIn(It.IsAny<Guid>()))
                        .ReturnsAsync(new PatientDTO { PatientID = Guid.NewGuid() });
        _mockPatientRepo.Setup(repo => repo.MarkModuleDone(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(Result.Failure("Module has already been marked as done."));

        var result = await _controller.MarkModuleDone(moduleId, stickerId);
        var badRequestResult = result as BadRequestObjectResult;

        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Module has already been marked as done.", badRequestResult.Value);
    }
}
