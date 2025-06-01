using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FLLJudge.Shared;

/// <summary>
/// Represents an area.
/// </summary>
public class Area
{
    /// <summary>
    /// Gets or sets the ID of the area.
    /// </summary>
    [JsonPropertyName("areaid")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the area.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

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