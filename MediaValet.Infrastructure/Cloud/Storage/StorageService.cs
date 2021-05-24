using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace MediaValet.Infrastructure.Cloud.Storage
{
  public class StorageService<T> : IStorageService<T> where T : TableEntity, new()
  {
    private readonly CloudTable _table;
    public StorageService(string connectionString, string tableName)
    {
      var account = CloudStorageAccount.Parse(connectionString);
      var client = account.CreateCloudTableClient();

      var table = client.GetTableReference(tableName);
      table.CreateIfNotExists();

      _table = table;
    }
    public async Task<bool> Add(T tableEntity)
    {
      TableOperation insertOperation = TableOperation.Insert(tableEntity);
      var result = await _table.ExecuteAsync(insertOperation);

      return result != null;
    }

    public async Task<IEnumerable<T>> Read()
    {
      IEnumerable<T> result;
      var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, "");
      var query = new TableQuery<T>().Where(condition);

      // Print the fields for each customer.
      TableContinuationToken token = null;
      do
      {
        TableQuerySegment<T> resultSegment = await _table.ExecuteQuerySegmentedAsync(query, token);
        token = resultSegment.ContinuationToken;
        result = resultSegment.Results;
      } while (token != null);

      return result;

    }
  }
}