using System;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Application.Handlers.ConfirmationHandler.Commands;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Module;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediaValet.Agent
{
  public class OrdersIngestor
  {
    private readonly IMessagingService _messagingService;
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersIngestor> _logger;
    private readonly Guid _agentId;
    private readonly int _magicNumber;

    public OrdersIngestor(IMessagingService messagingService, IMediator mediator,
      ILogger<OrdersIngestor> logger)
    {
      _messagingService = messagingService;
      _mediator = mediator;
      _logger = logger;
      _agentId = Guid.NewGuid();
      _magicNumber = new Random().Next(1, 10);
    }

    public async Task Run()
    {
      _logger.LogInformation($"I'm agent {_agentId}, my magic number is  {_magicNumber}");
      while (true)
      {
        var message = await _messagingService.PollMessages();
        if (message == null)
        {
          continue;
        }


        var order = JsonConvert.DeserializeObject<Order>(message.MessageText);
        _logger.LogInformation($"Received order {order.OrderId}");

        if (order.RandomNumber == _magicNumber)
        {
          _logger.LogInformation("Oh no, my magic number was found");
        }
        else
        {
          _logger.LogInformation($"Order text : {order.OrderText}");
          var confirmation = new Confirmation
          {
            AgentId = _agentId,
            OrderId = order.OrderId,
            OrderStatus = "Processed"
          };

          var result = await _mediator.Send(new ConfirmationOrderCommand(confirmation));

          if (result)
          {
            await _messagingService.Dequeue(message);
          }
        }

      }
    }
  }
}
