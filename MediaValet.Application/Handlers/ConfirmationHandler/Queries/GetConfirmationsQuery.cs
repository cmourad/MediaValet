using System.Collections.Generic;
using MediatR;
using MediaValet.Module;

namespace MediaValet.Application.Handlers.ConfirmationHandler.Queries
{
  public class GetConfirmationsQuery: IRequest<IEnumerable<Confirmation>>
  {

  }
}