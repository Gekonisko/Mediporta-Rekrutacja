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

    public StackOverflowAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StackOverflowTag>> GetTagsAsync(int page = 1, int pageSize = 5)
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
