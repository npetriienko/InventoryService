using Serilog.Context;

namespace InventoryService.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            return next(context);
        }
    }
    
    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}