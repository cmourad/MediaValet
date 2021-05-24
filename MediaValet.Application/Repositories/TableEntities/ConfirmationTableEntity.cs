using System;
using Microsoft.Azure.Cosmos.Table;

namespace MediaValet.Application.Repositories.TableEntities
{
  public class ConfirmationTableEntity : TableEntity
  {
    public ConfirmationTableEntity()
    {
      PartitionKey = Guid.NewGuid().ToString();
      RowKey = Guid.NewGuid().ToString();
    }
    public ConfirmationTableEntity(string agentId)
    {
      PartitionKey = agentId;
      RowKey = Guid.NewGuid().ToString();
    }
    public int OrderId { get; set; }
    public Guid AgentId { get; set; }
    public string OrderStatus { get; set; }
  }
}