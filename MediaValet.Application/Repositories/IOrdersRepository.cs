using System.Collections.Generic;
using System.Threading.Tasks;
using MediaValet.Module;

namespace MediaValet.Application.Repositories
{
  public interface IOrdersRepository
  {
    int GetOrderId();
    int GetRandomId();
    Task<bool> SaveOrder(Confirmation confirmation);
    Task<IEnumerable<Confirmation>> GetConfirmations();
  }
}