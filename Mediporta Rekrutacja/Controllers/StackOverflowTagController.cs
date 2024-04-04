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
    }

    [HttpGet()]
    public async Task<IActionResult> FillDatabaseWithTags([FromQuery(Name = "size")] int size = 1000)
    {
        _logger.Log(LogLevel.Debug, $"api/so/tags size={size}");

        List<StackOverflowTag> tags = new ();

        try
        {
            int tagCount = await _dbContext.GetSizeOfTagTable();

            double missingTags = size - tagCount;
            if (missingTags > 0)
            {
                var page = (int)Math.Ceiling(tagCount / missingTags + 1);
                tags = await _stackOverflowAPIService.GetTags(page, (int)missingTags);

                await _dbContext.FillTagTable(tags, tagCount);

                return Ok(tags);

            }
            return Ok("Databe is fulfilled");
        }
        catch
        {
            _logger.LogError("Failed to save tags into database");
        }
        return BadRequest(tags);
    }
}
