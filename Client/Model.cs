using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace BlazorApp.Client;

public class Model
{
    [JsonPropertyName("areas")]
    public List<Area> Areas { get; set; }

    public async static Task<Model> GetModel(string baseAddress)
    {
        // Fetch the JSOPN file from the server
        // see https://stackoverflow.com/questions/58523617/blazor-client-side-webassembly-reading-a-json-file-on-startup-cs
        using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        var model = await httpClient.GetFromJsonAsync<Model>("comments.json")
            .ConfigureAwait(false);

        // derive the tags from frequent terms used in the comments
        foreach (var area in model.Areas)
        {
            area.Tags = area.Comments
                .SelectMany(c => c.Text.Split(" \t;.:,'".ToCharArray()))
                .Where(w => w.Length > 4)
                .Select(w => w.ToLower())
                .GroupBy(w => w)
                .Select(g => new Tag { Text = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count)
                .Take(15)
                .ToList();
        }
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
    public List<Tag> Tags { get; set; }
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

public class Tag
{
    [JsonIgnore]
    public required string Text { get; set; }

    [JsonIgnore]
    public int Count { get; set; }
}