using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Application.Repositories;
using MediaValet.Module;

namespace MediaValet.Application.Handlers.ConfirmationHandler.Queries
{
  public class GetConfirmationsHandler : IRequestHandler<GetConfirmationsQuery, IEnumerable<Confirmation>>
  {
    private readonly IOrdersRepository _ordersRepository;

    public GetConfirmationsHandler(IOrdersRepository ordersRepository)
    {
      _ordersRepository = ordersRepository;
    }
    public async Task<IEnumerable<Confirmation>> Handle(GetConfirmationsQuery request, CancellationToken cancellationToken)
    {
      return await _ordersRepository.GetConfirmations();
    }
  }
}