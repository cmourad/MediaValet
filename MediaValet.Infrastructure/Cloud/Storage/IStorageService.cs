using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace MediaValet.Infrastructure.Cloud.Storage
{
  public interface IStorageService<T> where T : TableEntity
  {
    Task<bool> Add(T tableEntity);
    Task<IEnumerable<T>> Read();
  }
}