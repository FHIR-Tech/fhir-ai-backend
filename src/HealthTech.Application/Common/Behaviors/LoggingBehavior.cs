using HealthTech.Application.Common.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HealthTech.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = request is BaseRequest<object> baseRequest ? baseRequest.RequestId : Guid.Empty;

        _logger.LogInformation("Handling {RequestName} with RequestId: {RequestId}", requestName, requestId);

        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation("Handled {RequestName} with RequestId: {RequestId} in {ElapsedMilliseconds}ms", 
            requestName, requestId, sw.ElapsedMilliseconds);

        return response;
    }
}
