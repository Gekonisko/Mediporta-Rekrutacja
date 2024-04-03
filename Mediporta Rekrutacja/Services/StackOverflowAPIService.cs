using System.Collections.Generic;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class StackOverflowAPIService
{
    private readonly HttpClient _httpClient;

    private readonly int _maxPageSize = 100;

    public StackOverflowAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StackOverflowTag>> GetTagsFromApiAsync(int page = 1, int pageSize = 1000)
    {
        var requests = (int)Math.Ceiling(pageSize / 100.0);

        var tagsTasks = new Task<List<StackOverflowTag>>[requests];

        for(int i = 0; i < requests - 1; i++) 
        {
            tagsTasks[i] = GetTagsAsync(page + i, _maxPageSize);
        }

        var rest = pageSize - ((requests - 1) * _maxPageSize);
        tagsTasks[requests - 1] = GetTagsAsync(requests, rest);

        await Task.WhenAll(tagsTasks);

        var tags = new List<StackOverflowTag>();
        foreach (var task in tagsTasks)
        {
            tags.AddRange(await task);
        }

        return tags;
    }

    private async Task<List<StackOverflowTag>> GetTagsAsync(int page, int pageSize)
    {
        var tags = new List<StackOverflowTag>();

        var response = await _httpClient.GetAsync($"https://api.stackexchange.com/2.3/tags?page={page}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow");

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
            throw new Exception("Failed to retrieve tags from StackOverflow API.");
        }

        return tags;
    }
}
