using System.Threading.Tasks;
using Azure.Storage.Queues.Models;

namespace MediaValet.Infrastructure.Cloud.Queue
{
  public interface IMessagingService
  {
    Task<bool> Enqueue(string payload);

    Task<QueueMessage> PollMessages();
    Task<bool> Dequeue(QueueMessage message);
  }
}