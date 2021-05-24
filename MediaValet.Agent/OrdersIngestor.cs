using System;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using MediatR;
using MediaValet.Application.Handlers.ConfirmationHandler.Commands;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Module;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediaValet.Agent
{
  public class OrdersIngestor : AbsrtactProcessor
  {

    private readonly IMediator _mediator;
    private readonly ILogger<OrdersIngestor> _logger;

    public OrdersIngestor(IMediator mediator, IMessagingService messagingService,
      ILogger<OrdersIngestor> logger) : base(messagingService, logger)
    {
      _mediator = mediator;
      _logger = logger;
    }


    protected override async Task<bool> Process(QueueMessage message, int magicNumber)
    {
      var order = JsonConvert.DeserializeObject<Order>(message.MessageText);

      _logger.LogInformation($"Received order {order.OrderId}");

      if (order.RandomNumber == magicNumber)
      {
        _logger.LogInformation($"Oh no, my magic number was found");
        return false;
      }

      _logger.LogInformation($"Order text : {order.OrderText}");

      var confirmation = new Confirmation
      {
        AgentId = AgentId,
        OrderId = order.OrderId,
        OrderStatus = "Processed"
      };

      return await _mediator.Send(new ConfirmationOrderCommand(confirmation));


    }
  }
}
