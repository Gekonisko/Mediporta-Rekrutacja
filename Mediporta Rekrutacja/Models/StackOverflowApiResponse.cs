using System.Collections.Generic;
using System.Text.Json.Serialization;

public class StackOverflowApiResponse
{
    [JsonPropertyName("items")]
    public List<StackOverflowTag> Items { get; set; }
}