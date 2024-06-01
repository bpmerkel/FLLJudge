using System.Text.Json.Serialization;

namespace FLLJudge.Shared
{
    /// <summary>
    /// Represents a tag.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets or sets the text of the tag.
        /// </summary>
        [JsonIgnore]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the count of the tag.
        /// </summary>
        [JsonIgnore]
        public int Count { get; set; }
    }
}