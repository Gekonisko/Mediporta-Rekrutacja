using Newtonsoft.Json;

public class StackOverflowAPIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StackOverflowAPIService> _logger;


    private readonly int _maxPageSize = 100;

    public StackOverflowAPIService(HttpClient httpClient, ILogger<StackOverflowAPIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<StackOverflowTag>> GetTags(int page = 1, int size = 1000)
    {
        _logger.Log(LogLevel.Information, $"GetTags page={page} size={size}");

        var requests = (int)Math.Ceiling(size / (double)_maxPageSize);

        var tasks = new Task<List<StackOverflowTag>>[requests];

        for(int i = 0; i < requests - 1; i++) 
        {
            tasks[i] = GetTagsPageAsync(page + i, _maxPageSize);
        }

        var rest = size - ((requests - 1) * _maxPageSize);
        var lastPage = ((requests - 1) / rest) + page;
        tasks[requests - 1] = GetTagsPageAsync(lastPage, rest);

        await Task.WhenAll(tasks);

        var tags = new List<StackOverflowTag>();
        foreach (var task in tasks)
        {
            tags.AddRange(await task);
        }

        return tags;
    }

    private async Task<List<StackOverflowTag>> GetTagsPageAsync(int page, int size)
    {
        _logger.Log(LogLevel.Information, $"GetTagsPageAsync page={page} size={size}");

        var tags = new List<StackOverflowTag>();
        try
        {
            var response = await _httpClient.GetAsync($"https://api.stackexchange.com/2.3/tags?page={page}&pagesize={size}&order=desc&sort=popular&site=stackoverflow");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<StackOverflowApiResponse>(content);

                tags.AddRange(result.Items.Select(item => new StackOverflowTag
                {
                    Name = item.Name,
                    Count = item.Count
                }));
            }
            else
            {
                _logger.LogError("Failed to retrieve tags from StackOverflow API");
            }
        }
        catch
        {
            _logger.LogError("Failed to retrieve tags from StackOverflow API");
            throw new Exception("Failed to retrieve tags from StackOverflow API");

        }

        return tags;
    }
}
