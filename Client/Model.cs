using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp.Client;
// see https://stackoverflow.com/questions/58523617/blazor-client-side-webassembly-reading-a-json-file-on-startup-cs
public class Model
{
    [JsonPropertyName("areas")]
    public List<Area>? Areas { get; set; }

    public async static Task<Model> GetModel(HttpClient httpClient)
    {
        var json = await httpClient.GetStringAsync("comments.json")
            .ConfigureAwait(false);
        var model = JsonSerializer.Deserialize<Model>(json);
        foreach (var area in model.Areas)
        {
            if (area == null) continue;
            area.Chips = area.Comments
                .SelectMany(c =>
                {
                    return c.Text.Split(" \t;.:,".ToCharArray())
                        .Where(w => w.Length > 3)
                        .GroupBy(w => w)
                        .Where(g => g.Count() < 7)
                        .SelectMany(g => g);
                })
                .ToList();
        }
        return model;
    }
}

public class Area
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("comments")]
    public List<Comment>? Comments { get; set; }
    [JsonIgnore]
    public List<string>? Chips { get; set; }
}

public class Comment
{
    [JsonPropertyName("key")]
    public int Key { get; set; }
    [JsonPropertyName("text")]
    public required string Text { get; set; }
    [JsonPropertyName("rating")]
    public int Rating { get; set; }
}