using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using FLLJudge.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiIsolated;

/// <summary>
/// Represents a class that handles HTTP triggers.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HttpTrigger"/> class.
/// </remarks>
/// <param name="loggerFactory">The logger factory.</param>
public class CommentDataFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CommentDataFunction>();
    private static Model _model = null;     // keep local to cache the deserialization

    /// <summary>
    /// Runs the HTTP trigger.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>The HTTP response data.</returns>
    [Function(nameof(CommentDataFunction))]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var sw = Stopwatch.StartNew();
        if (_model == null)
        {
            var fileName = "comments.json";
            using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _model = JsonSerializer.Deserialize<Model>(fileStream);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteAsJsonAsync(_model).AsTask().Wait();
        _logger.LogMetric("TransactionTimeMS", sw.Elapsed.TotalMilliseconds);
        return response;
    }
}