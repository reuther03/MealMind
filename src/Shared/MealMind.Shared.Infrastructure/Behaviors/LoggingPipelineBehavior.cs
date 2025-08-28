// using System.Diagnostics;
// using MediatR;
// using Microsoft.Extensions.Logging;
//
// namespace MangaDen.Shared.Infrastructure.Behaviors;
//
// public class LoggingPipelineBehavior<TRequest, TResponse>
//     : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IBaseRequest
// {
//     private const string QuerySuffix = "Query";
//     private const string CommandSuffix = "Command";
//
//     private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;
//
//     public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
//     {
//         _logger = logger;
//     }
//
//     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//     {
//         var baseTypeName = GetBaseTypeName(request);
//         var startTime = Stopwatch.GetTimestamp();
//
//         try
//         {
//             var response = await next();
//
//             var delta = Stopwatch.GetElapsedTime(startTime, Stopwatch.GetTimestamp());
//
//             _logger.LogInformation(
//                 "[{RequestType}] Handled successfully {RequestName} | {Time}",
//                 baseTypeName,
//                 typeof(TRequest).Name,
//                 delta
//             );
//
//             return response;
//         }
//         catch (Exception e)
//         {
//             var delta = Stopwatch.GetElapsedTime(startTime, Stopwatch.GetTimestamp());
//
//             _logger.LogError(
//                 e,
//                 "[{RequestType}] Error handling {RequestName} | {Time}ms",
//                 baseTypeName,
//                 typeof(TRequest).Name,
//                 delta
//             );
//
//             throw;
//         }
//     }
//
//
//     private static string GetBaseTypeName(TRequest request)
//     {
//         var typeName = request.GetType().Name;
//
//         if (typeName.Contains(QuerySuffix))
//             return QuerySuffix;
//
//         if (typeName.Contains(CommandSuffix))
//             return CommandSuffix;
//
//         return "Unknown";
//     }
// }