using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace FLLJudge.Client;

/// <summary>
/// Represents the model of the application.
/// </summary>
public class Model
{
    /// <summary>
    /// Gets or sets the list of areas.
    /// </summary>
    [JsonPropertyName("areas")]
    public List<Area> Areas { get; set; }

    /// <summary>
    /// Fetches the model from the server and derives the tags from frequent terms used in the comments.
    /// </summary>
    /// <param name="baseAddress">The base address of the server.</param>
    /// <returns>The fetched model.</returns>
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
                .SelectMany(c => c.Text.Split(" \t;.:,'".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
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

/// <summary>
/// Represents an area.
/// </summary>
public class Area
{
    /// <summary>
    /// Gets or sets the ID of the area.
    /// </summary>
    [JsonPropertyName("areaid")]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the area.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of comments in the area.
    /// </summary>
    [JsonPropertyName("comments")]
    public List<Comment> Comments { get; set; }

    /// <summary>
    /// Gets or sets the list of tags in the area.
    /// </summary>
    [JsonIgnore]
    public List<Tag> Tags { get; set; }
}

/// <summary>
/// Represents a comment.
/// </summary>
public class Comment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Sections { GreatJob, ThinkAbout }

    /// <summary>
    /// Gets or sets the ID of the comment.
    /// </summary>
    [JsonPropertyName("commentid")]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the comment.
    /// </summary>
    [JsonPropertyName("text")]
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets the section of the comment.
    /// </summary>
    [JsonPropertyName("section")]
    public Sections Section { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the comment is selected.
    /// </summary>
    [JsonIgnore]
    public bool Selected { get; set; }
}

/// <summary>
/// Represents a tag.
/// </summary>
public class Tag
{
    /// <summary>
    /// Gets or sets the text of the tag.
    /// </summary>
    [JsonIgnore]
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets the count of the tag.
    /// </summary>
    [JsonIgnore]
    public int Count { get; set; }
}
