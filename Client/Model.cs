﻿using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace BlazorApp.Client;
// see https://stackoverflow.com/questions/58523617/blazor-client-side-webassembly-reading-a-json-file-on-startup-cs
public class Model
{
    [JsonPropertyName("areas")]
    public List<Area> Areas { get; set; }

    public async static Task<Model> GetModel(string baseAddress)
    {
        Console.WriteLine($"getting model from {baseAddress}");
        using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        var model = await httpClient.GetFromJsonAsync<Model>("comments.json")
            .ConfigureAwait(false);
        Console.WriteLine(model);
        foreach (var area in model.Areas)
        {
            // set the tags from the text
            area.Tags = area.Comments
                .SelectMany(c => c.Text.Split(" \t;.:,".ToCharArray()))
                .Where(w => w.Length > 4)
                .Select(w => w.ToLower())
                .GroupBy(w => w)
                .Select(g => new { g.Key, count = g.Count() })
                .OrderByDescending(g => g.count)
                .Take(15)
                .Select(g => g.Key)
                .ToList();
        }
        Console.WriteLine(model);
        return model;
    }
}

public class Area
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("comments")]
    public List<Comment> Comments { get; set; }

    [JsonIgnore]
    public List<string> Tags { get; set; }
}

public class Comment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Sections { GreatJob, ThinkAbout }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("section")]
    public Sections Section { get; set; }

    [JsonIgnore]
    public bool Selected { get; set; }
}