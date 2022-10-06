using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eda.core
{
  public abstract class BackgroundQService<T> : BackgroundService
  {
    protected readonly ILogger<T> Logger;
		protected IConfiguration Configuration { get; set; }
		protected IConnection Connection = default!;
    protected IModel Channel = default!;

    protected BackgroundQService(ILogger<T> logger, IConfiguration configuration)
    {
      Logger = logger;
			Configuration = configuration;
    }

    protected ConnectionFactory GetConnectionFactory()
    {
			var host = Configuration["EventStreamHostName"];
			Logger.LogInformation($"Connecting to RabbitMQ host `{host}`.");

      var factory = new ConnectionFactory()
      {
				HostName = Configuration["EventStreamHostName"],
      };

      return factory;
    }

		protected void RandomWait()
		{
			Random rnd = new Random();
			var period = rnd.Next(1, 10);

			Logger.LogInformation($"***** Waiting for period {period} seconds *****");

			Thread.Sleep(period * 1000);
		}

    protected void DeclareExchange()
    {
      Channel.ExchangeDeclare(AppConstants.EXCHANGE_NAME, "direct", true);
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
                  exchange: AppConstants.EXCHANGE_NAME,
                  routingKey: eventName);
      Logger.LogInformation("Done");
    }

    protected void OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e) { }
    protected void OnConsumerUnregistered(object? sender, ConsumerEventArgs e) { }
    protected void OnConsumerRegistered(object? sender, ConsumerEventArgs e) { }
    protected void OnConsumerShutdown(object? sender, ShutdownEventArgs e) { }
    protected void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e) { }
  }
}
