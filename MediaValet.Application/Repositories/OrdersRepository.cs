using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaValet.Application.Repositories.TableEntities;
using MediaValet.Infrastructure.Cloud.Storage;
using MediaValet.Module;

namespace MediaValet.Application.Repositories
{
  public class OrdersRepository : IOrdersRepository
  {
    private readonly IStorageService<ConfirmationTableEntity> _storageService;
    static int _incrementalNumber;

    public OrdersRepository(IStorageService<ConfirmationTableEntity> storageService)
    {
      _storageService = storageService;
    }


    public int GetOrderId()
    {
      var id = _incrementalNumber;
      Interlocked.Increment(ref _incrementalNumber);

      return id;
    }

    public int GetRandomId()
    {
     return new Random().Next(1, 10);
    }

    public Task<bool> SaveOrder(Confirmation confirmation)
    {

      return _storageService.Add(new ConfirmationTableEntity(confirmation.AgentId.ToString())
      {
        OrderId = confirmation.OrderId,
        AgentId = confirmation.AgentId,
        OrderStatus = confirmation.OrderStatus
      });
    }


    
    public async Task<IEnumerable<Confirmation>> GetConfirmations()
    {
      var confirmations = new List<Confirmation>();
      var result = await _storageService.Read();
      foreach (var r in result)
      {
        confirmations.Add(new Confirmation
        {
          AgentId = r.AgentId,
          OrderId = r.OrderId,
          OrderStatus = r.OrderStatus
        });
      }

      return confirmations;
    }
  }
}