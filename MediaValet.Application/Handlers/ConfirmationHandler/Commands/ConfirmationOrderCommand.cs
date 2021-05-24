using MediatR;
using MediaValet.Module;

namespace MediaValet.Application.Handlers.ConfirmationHandler.Commands
{
  public class ConfirmationOrderCommand : IRequest<bool>
  {
    
    public ConfirmationOrderCommand(Confirmation confirmation)
    {
      Confirmation = confirmation;
    }

    public Confirmation Confirmation { get; }
  }
}