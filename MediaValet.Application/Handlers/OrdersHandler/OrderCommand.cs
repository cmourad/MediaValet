using MediatR;

namespace MediaValet.Application.Handlers.OrdersHandler
{
  public class OrderCommand : IRequest<string>
  {
    
    public OrderCommand(string orderText)
    {
      OrderText = orderText;
    }

    public string OrderText { get; }
  }
}