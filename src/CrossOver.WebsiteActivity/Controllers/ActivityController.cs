using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrossOver.WebsiteActivity.Controllers;

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

    [HttpPost("{key}")]
    public IActionResult RegisterNewActivity(string key, [FromBody] ActivityValue activity)
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
    public IActionResult GetActivityTotal(string key)
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