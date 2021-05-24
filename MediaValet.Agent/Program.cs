using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Agent.Configs;
using MediaValet.Application.Handlers.ConfirmationHandler.Commands;
using MediaValet.Application.Repositories;
using MediaValet.Application.Repositories.TableEntities;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Infrastructure.Cloud.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaValet.Agent
{
  class Program
  {
    static async Task Main()
    {
      var services = new ServiceCollection();
      ConfigureServices(services);

      var serviceProvider = services.BuildServiceProvider();

      await serviceProvider.GetService<OrdersIngestor>().Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables()
        .Build();


      services.AddOptions()
        .AddSingleton<OrdersIngestor>()
        .AddLogging(configure => configure.AddConsole())
        .AddSingleton<IConfiguration>(configuration)
        .Configure<MessagingConfig>(configuration.GetSection(nameof(MessagingConfig)))
        .Configure<StorageConfig>(configuration.GetSection(nameof(StorageConfig)))
        //infra
        .AddSingleton<IMessagingService>(provider =>
        {
          var messagingConfig = provider.GetService<IOptions<MessagingConfig>>()?.Value;
          if (messagingConfig != null)
          {
            return new MessagingService(messagingConfig.ConnectionString, messagingConfig.QueueName);
          }
          throw new Exception("Missing messaging Config");
        })
        .AddSingleton<IStorageService<ConfirmationTableEntity>>(provider =>
        {
          var messagingConfig = provider.GetService<IOptions<StorageConfig>>()?.Value;
          if (messagingConfig != null)
          {
            return new StorageService<ConfirmationTableEntity>(messagingConfig.ConnectionString, messagingConfig.TableName);
          }
          throw new Exception("Missing storage Config");
        })
        //application
        .AddMediatR(typeof(ConfirmationOrderHandler).GetTypeInfo().Assembly)
        .AddSingleton(typeof(IOrdersRepository), typeof(OrdersRepository));
    }

  }
}
