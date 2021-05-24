using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Application.Repositories;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Module;
using Microsoft.Extensions.Logging;

namespace MediaValet.Application.Handlers.OrdersHandler
{
  public class OrderHandler : IRequestHandler<OrderCommand, string>
  {
    private readonly IOrdersRepository _orderOrdersRepository;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<OrderHandler> _logger;

    public OrderHandler(IOrdersRepository orderOrdersRepository, IMessagingService messagingService,
      ILogger<OrderHandler> logger)
    {
      _orderOrdersRepository = orderOrdersRepository;
      _messagingService = messagingService;
      _logger = logger;
    }
    public async Task<string> Handle(OrderCommand request, CancellationToken cancellationToken)
    {
      
      var orderId =  _orderOrdersRepository.GetOrderId();
      var randomId = _orderOrdersRepository.GetRandomId();
      
      var newOrder = new Order
      {
        OrderId = orderId,
        OrderText = request.OrderText,
        RandomNumber = randomId
      };

      if (await _messagingService.Enqueue(newOrder.ToString()))
      {
        var result = $"Send order {newOrder.OrderId} with random number {newOrder.RandomNumber}";
        _logger.LogInformation(result);
        return result;
      }

      return "";
    }
  }
}