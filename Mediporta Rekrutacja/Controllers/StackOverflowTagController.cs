using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/so/tags")]
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

    [HttpGet("{count}")]
    public async Task<IActionResult> SaveTagsIntoDatabase(int count = 1000)
    {
        var tags = await _stackOverflowAPIService.GetTagsFromApiAsync(1, count);
        _dbContext.AddTags(tags);
        await _dbContext.SaveChangesAsync();

        return Ok(tags);
    }
}
