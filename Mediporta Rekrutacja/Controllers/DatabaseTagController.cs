using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/db/tags")]
public class DatabaseTagController : ControllerBase
{
    private readonly PostgresDatabaseService _dbContext;
    private readonly ILogger<DatabaseTagController> _logger;

    public DatabaseTagController(PostgresDatabaseService dbContext, ILogger<DatabaseTagController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCountAsync([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "size")] int size = 20)
    {
        _logger.Log(LogLevel.Information, $"api/db/tags/count page={page} size={size}");
        try
        {
            var tags = _dbContext.GetPercentageOfTags(page, size);
            return Ok(tags);
        }
        catch 
        {
            _logger.LogError("Failed to retrieve tags from database");
        }
        return BadRequest();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetTagsAsync([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "size")] int size = 20, [FromQuery(Name = "sort")] string sort = "id", [FromQuery(Name = "direction")] string direction = "asc")
    {
        _logger.Log(LogLevel.Information ,$"api/db/tags page={page} size={size} sort={sort} direction={direction}");

        if(Enum.TryParse<SortingType>(direction, out var directionResult) == false) 
        {
            return BadRequest("Query param 'direction ' must be 'asc' or 'desc'");
        }

        if (Enum.TryParse<TagColumn>(sort, out var sortResult) == false)
        {
            return BadRequest("Query param 'sort ' must be 'id', 'name' or 'count'");
        }

        try
        {
            var tags = _dbContext.GetTags(page, size, Enum.Parse<TagColumn>(sort), Enum.Parse<SortingType>(direction));
            return Ok(tags);
        }
        catch
        {
            _logger.LogError("Failed to retrieve tags from database");
        }

        return BadRequest();

        
    }

}
