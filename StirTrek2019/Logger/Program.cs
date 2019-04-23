using System;
using Newtonsoft.Json;
using eda.core;
using RabbitMQ.Client.Events;
using eda.Logger.Models;
using System.Text;
using RabbitMQ.Client;

namespace eda.Logger
{
  class EdaLogger : QBase
  {
    static void Main(string[] args)
    {
      var factory = GetConnectionFactory();
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          DeclareExchange(channel);
          DeclareQ(channel, Constants.LOGGING_QUEUE_NAME);
          SetUpQoS(channel);
          foreach (var eventName in Constants.EventCollection)
          {
            BindToQ(channel, Constants.LOGGING_QUEUE_NAME, eventName);
          }

          Console.WriteLine(" [LOGGER] Waiting for messages.");

          var consumer = new EventingBasicConsumer(channel);
          consumer.Received += (model, ea) =>
          {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            LogMessage(routingKey, message);
            Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
            //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
          };
          channel.BasicConsume(queue: Constants.LOGGING_QUEUE_NAME, autoAck: true, consumer: consumer);

          Console.WriteLine(" Press [enter] to exit.");
          Console.ReadLine();
        }
      }
    }

    public static void LogMessage(string routingKey, string message)
    {
      var obj = JsonConvert.DeserializeObject<OrderIdModel>(message);

      var model = new LogEntryEntity
      {
        OrderId = obj.OrderId,
        RoutingKey = routingKey,
        Content = message,
        WhenReceived = DateTime.UtcNow
      };

      using (var db = new LoggingContext())
      {
        db.LogEntries.Add(model);
        db.SaveChanges();
      }
    }
  }
}
