using System;
using System.Text;
using System.Threading;
using eda.core;
using eda.core.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace eda.Invoicing
{
  public class Invoicer : QBase
  {
    private static int _ordersReceived = 0;
    static void Main(string[] args)
    {
      var factory = GetConnectionFactory();
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          DeclareExchange(channel);
          DeclareQ(channel, Constants.INVOICING_QUEUE_NAME);
          SetUpQoS(channel);
          BindToQ(channel, Constants.INVOICING_QUEUE_NAME, Constants.ORDER_ACCEPTED_EVENT);

          Console.WriteLine(" [INVOICER] Waiting for messages.");

          var consumer = new EventingBasicConsumer(channel);
          consumer.Received += (model, ea) =>
          {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine(" [>>>>>>>>>>] Received Order Accepted '{0}':'{1}'", routingKey, _ordersReceived++);

            var orderEvent = DeserializeMessage(message);
            ProcessEvent(orderEvent, channel);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
          };
          channel.BasicConsume(queue: Constants.INVOICING_QUEUE_NAME, autoAck: false, consumer: consumer);

          Console.WriteLine(" Press [enter] to exit.");
          Console.ReadLine();
        }
      }
    }

    private static void ProcessEvent(IOrderAccepted orderEvent, IModel channel)
    {
      Console.Write("\tProcessing order {0}...", orderEvent.OrderId);

      ICustomerBilled billedEvent = new CustomerBilledEvent(orderEvent.OrderId);
      Thread.Sleep(5000);
      var message = JsonConvert.SerializeObject(billedEvent);
      var body = Encoding.UTF8.GetBytes(message);
      channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.CUSTOMER_BILLED_EVENT, null, body);
      //Console.WriteLine(" [<<<<<<<<<<] Customer Billed '{0}':'{1}'", Constants.CUSTOMER_BILLED_EVENT, orderEvent.OrderId);
      Console.WriteLine("Done");
    }

    private static IOrderAccepted DeserializeMessage(string message)
    {
      return JsonConvert.DeserializeObject<Order>(message);
    }
  }

  internal class CustomerBilledEvent : ICustomerBilled
  {
    public CustomerBilledEvent(Guid orderEvent)
    {
      OrderId = orderEvent;
    }

    public Guid OrderId { get; set; }
  }

  internal class Order : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }
}
