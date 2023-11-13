using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiIsolated
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;

        public HttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
        }

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
            response.WriteAsJsonAsync(result);

            _logger.LogMetric("TransactionTimeNS", sw.Elapsed.TotalNanoseconds);
            return response;
        }

        private string GetSummary(int temp) =>
             temp switch
             {
                 >= 32 => "Hot",
                 <= 16 and > 0 => "Cold",
                 <= 0 => "Freezing",
                 _ => "Mild",
             };
    }
}
