using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FLLJudge.Shared;

public class Model
{
    /// <summary>
    /// Gets or sets the list of areas.
    /// </summary>
    [JsonPropertyName("areas")]
    public List<Area> Areas { get; set; }
}