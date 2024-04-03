using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static PostgresDatabaseService;

[ApiController]
[Route("api/DatabaseTags")]
public class DatabaseTagController : ControllerBase
{
    private readonly PostgresDatabaseService _dbContext;

    public DatabaseTagController( PostgresDatabaseService dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("GetPercentageOfTags {page} {pageSize}")]
    public async Task<IActionResult> GetTagsAsync(int page = 1, int pageSize = 100)
    {
        var tags = _dbContext.GetPercentageOfTags(page, pageSize);

        return Ok(tags);
    }

    [HttpGet("GetTags {page} {pageSize} {sortBy} {sortDescending}")]
    public async Task<IActionResult> GetTagsAsync(int page = 1, int pageSize = 100, string sortBy = "id", bool sortDescending = false)
    {
        var sortingType = sortDescending ? SortingType.desc : SortingType.asc;
        var sortByColumn = TagColumn.id;
        if (Enum.TryParse<TagColumn>(sortBy, out var result))
            sortByColumn = result;

        var tags = _dbContext.GetTags(page, pageSize, sortByColumn, sortingType);

        return Ok(tags);
    }

}
