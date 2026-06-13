using Prometheus;
using System.Diagnostics;

namespace UserAPI.Services;

public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter(
            "http_requests_total",
            "Total count of HTTP requests",
            new CounterConfiguration
            {
                LabelNames = new[]
                {
                    "method",
                    "endpoint",
                    "status_code"
                }
            });

    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram(
            "http_request_duration_seconds",
            "HTTP request duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[]
                {
                    "method",
                    "endpoint"
                },
                Buckets = Histogram.ExponentialBuckets(
                    start: 0.001,
                    factor: 2,
                    count: 16)
            });

    private static readonly Gauge ActiveRequests = Metrics
        .CreateGauge(
            "http_active_requests",
            "Current active HTTP requests");

    public TelemetryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/metrics"))
        {
            await _next(context);
            return;
        }

        string method = context.Request.Method;
        string endpoint = context.GetEndpoint()?.DisplayName ?? "unknown";

        ActiveRequests.Inc();

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            int statusCode = context.Response.StatusCode;

            RequestsTotal
                .Labels(
                    method,
                    endpoint,
                    statusCode.ToString())
                .Inc();

            RequestDuration
                .Labels(
                    method,
                    endpoint)
                .Observe(stopwatch.Elapsed.TotalSeconds);

            ActiveRequests.Dec();
        }
    }
}