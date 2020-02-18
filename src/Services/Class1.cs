using System;
using System.Collections.Generic;
using System.Text;
using eda.core;
using eda.core.events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace eda.services
{
  public class OrderAcceptedEvent : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }

  public class NewCustomerEvent : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }

  public class OrderCreator : QBase
  {
    public void Submit(IOrderAccepted order)
    {
      var factory = GetConnectionFactory();
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          DeclareExchange(channel);

          var message = JsonConvert.SerializeObject(order);
          var body = Encoding.UTF8.GetBytes(message);

          channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.ORDER_ACCEPTED_EVENT, null, body);
          Console.WriteLine(" [x] Sent '{0}':'{1}'", Constants.ORDER_ACCEPTED_EVENT, message);
        }
      }
    }
  }
}
