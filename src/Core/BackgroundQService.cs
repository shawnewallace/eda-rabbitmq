using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eda.core
{
  public abstract class BackgroundQService<T> : BackgroundService
  {
    protected readonly ILogger<T> Logger;
    protected IConnection Connection;
    protected IModel Channel;

    protected BackgroundQService(ILogger<T> logger)
    {
      Logger = logger;
    }

    protected static ConnectionFactory GetConnectionFactory()
    {
      var factory = new ConnectionFactory()
      {
        HostName = "localhost",
        Port = 5672
      };

      return factory;
    }

    protected void DeclareExchange()
    {
      Channel.ExchangeDeclare(Constants.EXCHANGE_NAME, "direct", true);
    }

    protected void DeclareQ(string name)
    {
      Channel.QueueDeclare(name, true, false, false, null);
    }

    protected void SetUpQoS()
    {
      Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected  void BindToQ(string queueName, string eventName)
    {
      Logger.LogInformation("Binding to event {0}...", eventName);

      Channel.QueueBind(
                  queue: queueName,
                  exchange: Constants.EXCHANGE_NAME,
                  routingKey: eventName);
      Logger.LogInformation("Done");
    }

    protected void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    protected void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    protected void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    protected void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    protected void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }
  }
}
