using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static PostgresDatabaseService;

[ApiController]
[Route("api/db/tags")]
public class DatabaseTagController : ControllerBase
{
    private readonly PostgresDatabaseService _dbContext;

    public DatabaseTagController(PostgresDatabaseService dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("count {page} {size}")]
    public async Task<IActionResult> GetTagsAsync(int page = 1, int size = 20)
    {
        var tags = _dbContext.GetPercentageOfTags(page, size);

        return Ok(tags);
    }

    [HttpGet("{page} {size} {sort} {direction}")]
    public async Task<IActionResult> GetTagsAsync(int page = 1, int size = 20, string sort = "id", bool direction = false)
    {
        var sortingType = direction ? SortingType.desc : SortingType.asc;
        if (Enum.TryParse<TagColumn>(sort, out var sortByColumn))
        {
            var tags = _dbContext.GetTags(page, size, sortByColumn, sortingType);

            return Ok(tags);
        }

        return BadRequest();

        
    }

}
