using System;

namespace FLLJudge.Shared
{
    /// <summary>
    /// Represents a comment with a date and summary.
    /// </summary>
    public class CommentData
    {
        /// <summary>
        /// Gets or sets the date of the comment.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the summary of the comment.
        /// </summary>
        public string Summary { get; set; }
    }
}