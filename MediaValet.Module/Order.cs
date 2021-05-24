using Newtonsoft.Json;

namespace MediaValet.Module
{
  public class Order
  {
    public int OrderId { get; set; }
    public int RandomNumber { get; set; }
    public string OrderText { get; set; }


    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}
