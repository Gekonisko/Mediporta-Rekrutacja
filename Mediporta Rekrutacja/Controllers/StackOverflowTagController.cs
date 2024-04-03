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

    [HttpGet("{size}")]
    public async Task<IActionResult> SaveTagsIntoDatabase(int size = 1000)
    {
        var tags = await _stackOverflowAPIService.GetTags(1, size);
        await _dbContext.CreateTagTable(tags);

        return Ok(tags);
    }
}
