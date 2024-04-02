using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private readonly StackOverflowAPIService _stackOverflowAPIService;
    private readonly ApplicationDbContext _dbContext;

    public TagController(StackOverflowAPIService stackOverflowAPIService, ApplicationDbContext dbContext)
    {
        _stackOverflowAPIService = stackOverflowAPIService;
        _dbContext = dbContext;
    }

    [HttpGet("{page} {pageSize}")]
    public async Task<IActionResult> GetTagsAsync(int page, int pageSize)
    {
        var tags = await _stackOverflowAPIService.GetTagsAsync(page, pageSize);
        await _dbContext.Tags.AddRangeAsync(tags);
        await _dbContext.SaveChangesAsync();

        return Ok(tags);
    }
}
