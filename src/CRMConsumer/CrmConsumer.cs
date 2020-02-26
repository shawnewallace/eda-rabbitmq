using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using eda.core;
using eda.core.events;

namespace eda.crmConsumer
{
  public class CrmConsumer : BackgroundQService<CrmConsumer>
  {
    private static int _newCustomerReceived = 0;

    public CrmConsumer(ILogger<CrmConsumer> logger) : base(logger)
    {
      Init();
    }

    private void Init()
    {
      Logger.LogInformation("[CRM] Init");

      var factory = new ConnectionFactory { HostName = "host.docker.internal" };
      Connection = factory.CreateConnection();

      Channel = Connection.CreateModel();

      DeclareExchange();
      DeclareQ(Constants.MASTER_CUSTOMER_QUEUE_NAME);
      BindToQ(queueName: Constants.MASTER_CUSTOMER_QUEUE_NAME,
                eventName: Constants.NEW_CUSTOMER_EVENT);
      SetUpQoS();

      Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      Logger.LogInformation("[CRM] Init COMPLETE");
      Logger.LogInformation("[CRM] Waiting for messages.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (ch, ea) =>
      {
        // received message  
        var content = System.Text.Encoding.UTF8.GetString(ea.Body);
        var orderEvent = DeserializeMessage(content);
        var routingKey = ea.RoutingKey;

        Logger.LogInformation($" [>>>>>>>>>>] Received '{routingKey}':'{_newCustomerReceived++}'");

        // handle the received message  
        ProcessEvent(orderEvent);
        Channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: Constants.MASTER_CUSTOMER_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void ProcessEvent(INewCustomer newCustomer)
    {
      Logger.LogInformation("\tProcessing customer {0}...", newCustomer.CustomerId);
      Thread.Sleep(1000);
      Logger.LogInformation("Customer Created");
    }

    private static INewCustomer DeserializeMessage(string message)
    {
      return JsonConvert.DeserializeObject<NewCustomer>(message);
    }

    public override void Dispose()
    {
      Channel.Close();
      Connection.Close();
      base.Dispose();
    }
  }

  internal class NewCustomer : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }
}
