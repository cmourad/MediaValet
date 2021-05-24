using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaValet.Application.Interceptors
{
  public class LoggingInterceptor<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly ILogger<LoggingInterceptor<TRequest, TResponse>> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor<TRequest, TResponse>> logger)
    {
      _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      //Request
      _logger.LogInformation($"Handling {typeof(TRequest).Name}");
      Type myType = request.GetType();
      IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
      foreach (PropertyInfo prop in props)
      {
        object propValue = prop.GetValue(request, null);
        _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        // Do something with propValue
      }

      var response = await next();

      //Response
      _logger.LogInformation($"Handled {typeof(TResponse).Name}");
      return response;
    }
  }
}