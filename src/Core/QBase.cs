using System;
using RabbitMQ.Client;

namespace eda.core
{
  public abstract class QBase
  {
    protected static ConnectionFactory GetConnectionFactory()
    {
      var factory = new ConnectionFactory() {
        HostName = "localhost",
        Port = 5672,
      };

      return factory;
    }

    protected static void DeclareExchange(IModel channel)
    {
      channel.ExchangeDeclare(Constants.EXCHANGE_NAME, "direct", true);
    }

    protected static void DeclareQ(IModel channel, string name)
    {
      channel.QueueDeclare(name, true, false, false, null);
    }

    protected static void SetUpQoS(IModel channel)
    {
      channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected static void BindToQ(IModel channel, string queuename, string evt)
    {
      Console.Write("Binding to event {0}...", evt);

      channel.QueueBind(
                  queue: queuename,
                  exchange: Constants.EXCHANGE_NAME,
                  routingKey: evt);
      Console.WriteLine("Done");
    }
  }
}
