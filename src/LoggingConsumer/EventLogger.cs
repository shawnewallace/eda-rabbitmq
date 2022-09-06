using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using eda.core;
using eda.core.events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace eda.loggingConsumer
{

  public class EventLogger : BackgroundQService<EventLogger>
  {
    public EventLogger(ILogger<EventLogger> logger, IConfiguration configuration ) : base(logger, configuration)
    {
      Init();
    }

    private void Init()
    {
      Logger.LogInformation("[LOGGER] Init");

			using(var db = new LoggingContext())
			{
				LogginContextInitializer.Initialize(db);
			}

      var factory = GetConnectionFactory();
      Connection = factory.CreateConnection();

      Channel = Connection.CreateModel();

      DeclareExchange();
      DeclareQ(AppConstants.LOGGING_QUEUE_NAME);

      foreach (var eventName in AppConstants.EventCollection)
      {
        BindToQ(queueName: AppConstants.LOGGING_QUEUE_NAME,
                  eventName: eventName);
      }
      SetUpQoS();

      Logger.LogInformation("[LOGGER] Init COMPLETE");
      Logger.LogInformation("[LOGGER] Waiting for messages.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (ch, ea) =>
      {
        // received message  
        var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
        var routingKey = ea.RoutingKey;

        Logger.LogInformation($" [>>>>>>>>>>] LOGGER Received  '{routingKey}':'{content}'");

        LogMessage(routingKey, content);
        Channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: AppConstants.LOGGING_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void LogMessage(string routingKey, string content)
    {
      var model = new LogEntry {
        RoutingKey = routingKey,
        Content = content
      };

      try{
        var order = JsonConvert.DeserializeObject<OrderIdModel>(content);
        model.OrderId = order.OrderId;
      }
      catch
      {
        model.OrderId = Guid.Empty;
      }

      using(var db = new LoggingContext())
      {
        db.LogEntries.Add(model);
        db.SaveChanges();
      }
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
      Channel.Close();
      Connection.Close();
      base.Dispose();
    }
  }
}
