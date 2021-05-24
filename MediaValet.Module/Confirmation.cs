using System;

namespace MediaValet.Module
{
  public class Confirmation
  {
    public int OrderId { get; set; }
    public Guid AgentId { get; set; }
    public string OrderStatus { get; set; }
  }
}