using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace MediaValet.Infrastructure.Cloud.Queue
{
  public class MessagingService : IMessagingService
  {
    private readonly QueueClient _queue;

    public MessagingService(string connectionString, string queueName)
    {
      _queue = new QueueClient(connectionString, queueName);
    }
    public async Task<bool> Enqueue(string payload)
    {
      if (null != await _queue.CreateIfNotExistsAsync())
      {
        Console.WriteLine("The queue was created.");
      }

      return (await _queue.SendMessageAsync(payload) != null);
    }

    public async Task<QueueMessage> PollMessages()
    {
      if (await _queue.ExistsAsync())
      {
        QueueProperties properties = await _queue.GetPropertiesAsync();

        if (properties.ApproximateMessagesCount > 0)
        {
          QueueMessage[] retrievedMessage = await _queue.ReceiveMessagesAsync(1,TimeSpan.FromSeconds(1));
          return retrievedMessage.FirstOrDefault();
        }

        return null;
      }

      return null;
    }

    public async Task<bool> Dequeue(QueueMessage message)
    {
      var result = await _queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);

      return result != null;
    }
  }
}