using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerRelationshipManagementService
{
  public class CrmManager : QueueBase
  {
    static void Main(string[] args)
    {
      var factory = GetConnectionFactory();
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          DeclareExchange(channel);
          DeclareQ(channel, Constants.MASTER_CUSTOMER_QUEUE_NAME);
          SetUpQoS(channel);

          BindToQ(channel, Constants.MASTER_CUSTOMER_QUEUE_NAME, Constants.NEW_CUSTOMER_EVENT);

          Console.WriteLine(" [CRM] Waiting for messages.");

          var consumer = new EventingBasicConsumer(channel);
          consumer.Received += (Model, ea) =>
          {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            Console.WriteLine(" [>>>>>>>>>>] Received MESSAGE '{0}'", routingKey);

            ProcessMessage(channel, message, routingKey);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
          };
          channel.BasicConsume(queue: Constants.MASTER_CUSTOMER_QUEUE_NAME, noAck: false, consumer: consumer);

          Console.WriteLine(" Press [enter] to exit.");
          Console.ReadLine();
        }
      }
    }

    private static void ProcessMessage(IModel channel, string message, string routingKey)
    {
      switch (routingKey)
      {
        case Constants.NEW_CUSTOMER_EVENT:
          ProcessNewCustomer(channel, message);
          return;
      }

      Console.WriteLine("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, message);
    }

    private static void ProcessNewCustomer(IModel channel, string message)
    {
      var ready = JsonConvert.DeserializeObject<NewCustomer>(message);
      Console.Write(" [>>>>>>>>>>] Received New Customer message '{0}'...", ready.CustomerId);
      Thread.Sleep(1000);
      Console.WriteLine("Customer Created");
    }
  }

  internal class NewCustomer : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }
}
