using System;
using System.Text;
using eda.core;
using eda.core.events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace eda.services
{

  public class OrderCreator
  {
    public void Submit(IOrderAccepted order)
    {
      var factory = new ConnectionFactory { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          channel.ExchangeDeclare(AppConstants.EXCHANGE_NAME, "direct", true);

          var message = JsonConvert.SerializeObject(order);
          var body = Encoding.UTF8.GetBytes(message);

          channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.ORDER_ACCEPTED_EVENT, null, body);
          Console.WriteLine(" [x] Sent '{0}':'{1}'", AppConstants.ORDER_ACCEPTED_EVENT, message);
        }
      }
    }
  }
}
