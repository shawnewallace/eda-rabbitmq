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
using eda.core.data;

namespace eda.loggingConsumer
{

  public class EventLogger : BackgroundQService<EventLogger>
  {
    private readonly LoggingContext _context;

    public EventLogger(LoggingContext context, ILogger<EventLogger> logger, IConfiguration configuration) : base(logger, configuration)
    {
      Init();
      _context = context;
    }

    private void Init()
    {
      Logger.LogInformation("[LOGGER] Init");

      LogginContextInitializer.Initialize(_context);

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
      var model = new LogEntry
      {
        RoutingKey = routingKey,
        Content = content
      };

      try
      {
        var order = JsonConvert.DeserializeObject<OrderIdModel>(content);
        model.OrderId = order.OrderId;
      }
      catch
      {
        model.OrderId = Guid.Empty;
      }

      _context.LogEntries.Add(model);
      _context.SaveChanges();
    }

    public override void Dispose()
    {
      Channel.Close();
      Connection.Close();
      base.Dispose();
    }
  }
}
