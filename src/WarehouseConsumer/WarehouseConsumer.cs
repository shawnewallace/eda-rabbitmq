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
using System.Text;

namespace eda.warehouseConsumer
{
  public class WarehouseConsumer : BackgroundQService<WarehouseConsumer>
  {
    public WarehouseConsumer(ILogger<WarehouseConsumer> logger) : base(logger)
    {
      Init();
    }

    private void Init()
    {
      Logger.LogInformation("[WAREHOUSE] Init");

      var factory = new ConnectionFactory { HostName = "host.docker.internal" };
      //var factory = new ConnectionFactory { HostName = "localhost" };
      Connection = factory.CreateConnection();

      Channel = Connection.CreateModel();

      DeclareExchange();
      DeclareQ(Constants.WAREHOUSE_QUEUE_NAME);
      BindToQ(queueName: Constants.WAREHOUSE_QUEUE_NAME,
                eventName: Constants.READY_FOR_SHIPMENT_EVENT);
      SetUpQoS();

      Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      Logger.LogInformation("[WAREHOUSE] Init COMPLETE");
      Logger.LogInformation("[WAREHOUSE] Waiting for messages.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (ch, ea) =>
      {
        // received message  
        var content = System.Text.Encoding.UTF8.GetString(ea.Body);
        var routingKey = ea.RoutingKey;

        Logger.LogInformation($" [>>>>>>>>>>] Received '{routingKey}':'{content}'");

        // handle the received message
        try
        {
          ProcessMessage(content, routingKey);
        }
        finally
        {
          Channel.BasicAck(ea.DeliveryTag, false);
        }
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: Constants.WAREHOUSE_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void ProcessMessage(string content, string routingKey)
    {
      switch (routingKey)
      {
        case Constants.READY_FOR_SHIPMENT_EVENT:
          ProcessCustomerBilled(content);
          return;
      }

      Logger.LogError("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, content);
      throw new ArgumentException("Could not process message for routing key {0}", routingKey);
    }

    private void ProcessCustomerBilled(string message)
    {
      var ready = JsonConvert.DeserializeObject<OrderReady>(message);
      Console.Write(" [>>>>>>>>>>] Received Order Ready For Shipment message '{0}'...", ready.OrderId);
      Thread.Sleep(15000);
      IOrderShipped shipped = new OrderShipped { OrderId = ready.OrderId };
      var orderMessage = JsonConvert.SerializeObject(ready);
      var body = Encoding.UTF8.GetBytes(orderMessage);
      Channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.SHIPPED_EVENT, null, body);
      Console.WriteLine("Shipped");
    }

    public override void Dispose()
    {
      Channel.Close();
      Connection.Close();
      base.Dispose();
    }
  }

  internal class OrderShipped : IOrderShipped
  {
    public Guid OrderId { get; set; }
  }

  internal class OrderReady : IOrderReadyForShipment
  {
    public Guid OrderId { get; set; }
  }
}
