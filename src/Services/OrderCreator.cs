using System;
using System.Text;
using System.Threading.Channels;
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

					var properties = channel.CreateBasicProperties();
					properties.Persistent = true;

          channel.BasicPublish(
						exchange: AppConstants.EXCHANGE_NAME,
						routingKey: AppConstants.ORDER_ACCEPTED_EVENT,
						basicProperties: properties,
						body: body);
          Console.WriteLine(" [x] Sent '{0}':'{1}'", AppConstants.ORDER_ACCEPTED_EVENT, message);
        }
      }
    }
  }
}
