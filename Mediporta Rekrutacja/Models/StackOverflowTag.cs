using System;
using System.Text.Json.Serialization;

public class StackOverflowTag
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class PercentageOfTags : StackOverflowTag
{
    public double Percentage { get; set; }
}