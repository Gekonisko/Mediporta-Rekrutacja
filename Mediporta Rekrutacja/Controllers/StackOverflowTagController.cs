using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/StackOverflowTags")]
public class StackOverflowTagController : ControllerBase
{
    private readonly StackOverflowAPIService _stackOverflowAPIService;
    private readonly PostgresDatabaseService _dbContext;

    public StackOverflowTagController(StackOverflowAPIService stackOverflowAPIService, PostgresDatabaseService dbContext)
    {
        _stackOverflowAPIService = stackOverflowAPIService;
        _dbContext = dbContext;
        /*SaveTagsIntoDatabase(1000);*/
    }

    [HttpGet("SaveTagsIntoDatabase {tagsCount}")]
    public async Task<IActionResult> SaveTagsIntoDatabase(int tagsCount = 1000)
    {
        var tags = await _stackOverflowAPIService.GetTagsFromApiAsync(1, tagsCount);
        _dbContext.AddTags(tags);
        await _dbContext.SaveChangesAsync();

        return Ok(tags);
    }
}
