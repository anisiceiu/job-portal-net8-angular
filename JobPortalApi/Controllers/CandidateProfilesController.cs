using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateProfilesController : ControllerBase
{
    private readonly ICandidateProfileService _service;

    public CandidateProfilesController(ICandidateProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidateProfile>>> GetAll()
    {
        var items = await _service.GetAllCandidateProfilesAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidateProfile>> Get(int id)
    {
        var item = await _service.GetCandidateProfileByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CandidateProfile>> Create(CreateCandidateProfileDto dto)
    {
        // Validate user exists
        var userExists = await _service.UserExistsAsync(dto.UserId);
        if (!userExists) return BadRequest(new { message = $"User with ID {dto.UserId} does not exist." });

        var profile = await _service.CreateCandidateProfileAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = profile.CandidateProfileId }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCandidateProfileDto dto)
    {
        var profile = await _service.GetCandidateProfileByIdAsync(id);
        if (profile == null) return NotFound();

        await _service.UpdateCandidateProfileAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var profile = await _service.GetCandidateProfileByIdAsync(id);
        if (profile == null) return NotFound();

        await _service.DeleteCandidateProfileAsync(id);
        return NoContent();
    }
}
