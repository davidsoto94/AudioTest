using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;
using test.Models.DTOs;
using test.Services;
using test.Services.Interfaces;

namespace test.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class AudioController (ILogger<AudioController> logger ,IStorageService StorageService, IAudioRepository audioRepository, AudioService audioService) : ControllerBase
{
    private readonly ILogger<AudioController> _logger = logger;
    private IStorageService _storageService = StorageService;
    private IAudioRepository _audioRepository = audioRepository;
    private readonly AudioService _audioService = audioService;

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var audio = await _audioService.GetAudio(id);
        if (audio == null)
            return NotFound();
        return Ok(audio);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Post(AudioPostDTO audioDTO)
    {
        try
        {
            await _storageService.SaveFile(audioDTO);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message, ex);
            return BadRequest(ex.Message);
        }
        
    }
}
