using System.Text.Json.Serialization;

namespace FLLJudge.Shared
{
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
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text of the comment.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

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
}