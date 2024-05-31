using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
public class HttpTrigger(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<HttpTrigger>();

    /// <summary>
    /// Runs the HTTP trigger.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>The HTTP response data.</returns>
    [Function("CommentDataFunction")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var sw = Stopwatch.StartNew();
        var result = Enumerable.Range(1, 5)
            .Select(index => new CommentData
            {
                Date = DateTime.Now.AddDays(index),
                Summary = GetSummary(0)
            })
            .ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteAsJsonAsync(result).AsTask().Wait();

        _logger.LogMetric("TransactionTimeNS", sw.Elapsed.TotalNanoseconds);
        return response;
    }

    /// <summary>
    /// Gets the summary based on the temperature.
    /// </summary>
    /// <param name="temp">The temperature.</param>
    /// <returns>The summary.</returns>
    private string GetSummary(int temp) =>
         temp switch
         {
             >= 32 => "Hot",
             <= 16 and > 0 => "Cold",
             <= 0 => "Freezing",
             _ => "Mild",
         };
}
