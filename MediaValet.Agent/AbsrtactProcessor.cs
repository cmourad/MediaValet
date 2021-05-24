using System;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using MediaValet.Infrastructure.Cloud.Queue;
using Microsoft.Extensions.Logging;
using Polly;

namespace MediaValet.Agent
{
  public abstract class AbsrtactProcessor
  {

    private readonly IMessagingService _messagingService;
    private readonly ILogger<AbsrtactProcessor> _logger;

    protected AbsrtactProcessor(IMessagingService messagingService, ILogger<OrdersIngestor> logger)
    {
      _messagingService = messagingService;
      _logger = logger;
      _magicNumber = new Random().Next(1, 10);
    }

    protected Guid AgentId => Guid.NewGuid();

    private readonly int _magicNumber;

    public async Task Run()
    {
      _logger.LogInformation($"I'm agent {AgentId}, my magic number is  {_magicNumber}");

      var maxRetryAttempts = 3;
      var pauseBetweenFailures = TimeSpan.FromSeconds(2);

      var retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);
      while (true)
      {
        var message = await _messagingService.PollMessages();
        if (message == null)
        {
          continue;
        }

        var continueExecution = await retryPolicy.ExecuteAsync(async () =>
         {
           var result = await Process(message, _magicNumber);
           if (result)
           {
             await _messagingService.Dequeue(message);
           }
           return result;
         });


        if (!continueExecution)
        {
          break;
        }


      }

      Console.Read();
    }

    protected abstract Task<bool> Process(QueueMessage message, int magicNumber);
  }
}