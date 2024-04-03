using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/so/tags")]
public class StackOverflowTagController : ControllerBase
{
    private readonly StackOverflowAPIService _stackOverflowAPIService;
    private readonly PostgresDatabaseService _dbContext;
    private readonly ILogger<DatabaseTagController> _logger;

    public StackOverflowTagController(StackOverflowAPIService stackOverflowAPIService, PostgresDatabaseService dbContext, ILogger<DatabaseTagController> logger)
    {
        _stackOverflowAPIService = stackOverflowAPIService;
        _dbContext = dbContext;
        _logger = logger;
        /*SaveTagsIntoDatabase(1000);*/
    }

    [HttpGet()]
    public async Task<IActionResult> SaveTagsIntoDatabase([FromQuery(Name = "size")] int size = 1000)
    {
        _logger.Log(LogLevel.Debug, $"api/so/tags size={size}");

        try
        {
            var tags = await _stackOverflowAPIService.GetTags(1, size);
            await _dbContext.CreateTagTable(tags);
            return Ok(tags);

        }
        catch
        {
            _logger.LogError("Failed to save tags into database");
        }
        return BadRequest();
    }
}
