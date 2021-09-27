using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrossOver.WebsiteActivity.Controllers;

/// <summary>
/// Controller for route /activity
/// </summary>
[ApiController]
[Route("[controller]")]
public class ActivityController : ControllerBase
{

    private readonly IRecordingService _activityService;
    private readonly IReportingService _reportingService;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(ILogger<ActivityController> logger, IRecordingService recordingService, IReportingService reportingService)
    {
        _logger = logger;
        _activityService = recordingService;
        _reportingService = reportingService;
    }

    /// <summary>
    /// Registers a new activity with
    /// </summary>
    /// <param name="key"></param>
    /// <param name="activity"></param>
    /// <returns></returns>
    [HttpPost("{key}")]
    public IActionResult RegisterNewActivity([FromRoute] string key, [FromBody] ActivityValue activity)
    {
        try
        {
            _activityService.Register(key, activity.Value);
            return Ok();
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{key}/total")]
    public IActionResult GetActivityTotal([FromRoute]string key)
    {
        try
        {
            var total = _reportingService.GetTotal(key);
            return Ok(total);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}