using System.Collections.Generic;
using System.Text.Json.Serialization;

public class StackOverflowApiResponse
{
    [JsonPropertyName("items")]
    public List<StackOverflowTagResponse> Items { get; set; }
}

public class StackOverflowTagResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("count")]
    public int Count { get; set; }
}