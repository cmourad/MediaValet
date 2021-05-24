using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Application.Handlers.OrdersHandler;
using MediaValet.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace MediaValet.Application.Handlers.ConfirmationHandler.Commands
{
  public class ConfirmationOrderHandler : IRequestHandler<ConfirmationOrderCommand, bool>
  {
    private readonly IOrdersRepository _ordersRepository;
    private readonly ILogger<OrderHandler> _logger;

    public ConfirmationOrderHandler(IOrdersRepository ordersRepository, 
      ILogger<OrderHandler> logger)
    {
      _ordersRepository = ordersRepository;
      _logger = logger;
    }
    public async Task<bool> Handle(ConfirmationOrderCommand request, CancellationToken cancellationToken)
    {
      
      var result = await _ordersRepository.SaveOrder(request.Confirmation);
      if (result)
      {
        _logger.LogInformation("Order created");
      }

      return result;
    }
  }
}