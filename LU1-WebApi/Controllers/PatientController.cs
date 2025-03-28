using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAuthenticationService _authenticationService;

    public PatientController(IPatientRepository patientRepository, IAuthenticationService authenticationService)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpPost("create")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult> CreatePatient([FromBody] PatientDTO patient)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var result = await _patientRepository.CreatePatient(patient, userId);

            if (result == null || !result.IsSuccess)
                return BadRequest(result?.ErrorMessage ?? "An unknown error occurred during patient creation.");

            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during patient creation: {ex.Message}");
        }
    }

    //[HttpGet("{patientId}")]
    //[Authorize(Policy = "CanReadEntity")]
    //public async Task<ActionResult<PatientDTO>> GetPatient(Guid patientId)
    //{
    //    try
    //    {
    //        var patient = await _patientRepository.GetPatient(patientId);

    //        if (patient == null)
    //            return NotFound($"Patient with ID {patientId} not found.");

    //        return Ok(patient);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"An error occurred while fetching the patient: {ex.Message}");
    //    }
    //}

    [HttpGet("me")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<PatientDTO>> GetPatientFromLoggedInUser()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var patient = await _patientRepository.GetPatientFromUserLoggedIn(userId);

            if (patient == null)
                return NotFound("No patient found for the logged-in user.");

            return Ok(patient);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the patient: {ex.Message}");
        }
    }

    // POST: /patient/mark-module-done
    [HttpPost("mark-module-done")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult> MarkModuleDone(int moduleId, int stickerId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var patient = await _patientRepository.GetPatientFromUserLoggedIn(userId);
            if (patient == null)
                return NotFound("No patient record found for the current user.");

            var result = await _patientRepository.MarkModuleDone(patient.PatientID, moduleId, stickerId);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while marking the module as done: {ex.Message}");
        }
    }

    // GET: /patient/modules
    [HttpGet("modules")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<ModuleVoortgangDTO>>> GetModules()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var patient = await _patientRepository.GetPatientFromUserLoggedIn(userId);
            if (patient == null)
                return NotFound("No patient record found for the current user.");

            var modules = await _patientRepository.GetModules(patient.PatientID);

            if (modules == null || !modules.Any())
                return NotFound("No modules found for the current user.");

            return Ok(modules);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching modules: {ex.Message}");
        }
    }




}
