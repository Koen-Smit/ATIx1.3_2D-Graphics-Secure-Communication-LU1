using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("dagboek")]
public class DagboekController : ControllerBase
{
    private readonly IDagboekRepository _dagboekRepository;
    private readonly IAuthenticationService _authenticationService;

    public DagboekController(IDagboekRepository dagboekRepository, IAuthenticationService authenticationService)
    {
        _dagboekRepository = dagboekRepository ?? throw new ArgumentNullException(nameof(dagboekRepository));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    // POST: /dagboek/create
    [HttpPost("create")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult> CreateDagboek([FromBody] DagboekDTO dagboek)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var result = await _dagboekRepository.CreateDagboek(dagboek, userId);

            if (result == null || !result.IsSuccess)
                return BadRequest(result?.ErrorMessage ?? "An unknown error occurred during diary entry creation.");

            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during diary entry creation: {ex.Message}");
        }
    }

    // GET: /dagboek/me
    [HttpGet("me")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<DagboekDTO>>> GetDagboekenFromLoggedInUser()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var dagboeken = await _dagboekRepository.GetDagboekenByUserId(userId);

            if (dagboeken == null || !dagboeken.Any())
                return NotFound("No diary entries found for the logged-in user.");

            return Ok(dagboeken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching diary entries: {ex.Message}");
        }
    }
}
