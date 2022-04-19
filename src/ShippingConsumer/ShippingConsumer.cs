using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eda.core;
using eda.core.events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eda.shippingConsumer
{
  public class ShippingConsumer : BackgroundQService<ShippingConsumer>
  {

    public ShippingConsumer(ILogger<ShippingConsumer> logger) : base(logger)
    {
      Init();
    }

    private void Init()
    {
      Logger.LogInformation("[SHIPPER] Init");

      var factory = new ConnectionFactory { HostName = "host.docker.internal" };
      Connection = factory.CreateConnection();

      Channel = Connection.CreateModel();

      DeclareExchange();
      DeclareQ(AppConstants.SHIPPING_QUEUE_NAME);
      BindToQ(queueName: AppConstants.SHIPPING_QUEUE_NAME,
                eventName: AppConstants.CUSTOMER_BILLED_EVENT);
      BindToQ(queueName: AppConstants.SHIPPING_QUEUE_NAME,
                eventName: AppConstants.ORDER_ACCEPTED_EVENT);
      SetUpQoS();

      Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      Logger.LogInformation("[SHIPPER] Init COMPLETE");
      Logger.LogInformation("[SHIPPER] Waiting for messages.");
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

        Logger.LogInformation($" [>>>>>>>>>>] Received  '{routingKey}':'{content}'");

        // handle the received message  
        ProcessMessage(content, routingKey);
        Channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: AppConstants.SHIPPING_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void ProcessMessage(string message, string routingKey)
    {
      switch (routingKey)
      {
        case AppConstants.ORDER_ACCEPTED_EVENT:
          ProcessOrderAccepted(message);
          return;
        case AppConstants.CUSTOMER_BILLED_EVENT:
          ProcessCustomerBilled(message);
          return;
      }

      Logger.LogInformation("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, message);

      //throw new ArgumentException("Could not process message for routing key {0}", routingKey);
    }

    private void ProcessCustomerBilled(string message)
    {
      var billed = JsonConvert.DeserializeObject<CustomerBilledEvent>(message);
      Logger.LogInformation(" [>>>>>>>>>>] Received Customer Billed '{0}'...", billed.OrderId);
      Thread.Sleep(10000);
      IOrderReadyForShipment ready = new OrderReady { OrderId = billed.OrderId };
      var orderMessage = JsonConvert.SerializeObject(billed);
      var body = Encoding.UTF8.GetBytes(orderMessage);
      Channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.READY_FOR_SHIPMENT_EVENT, null, body);
      Logger.LogInformation("Processed");
    }

    private void ProcessOrderAccepted(string message)
    {
      var order = JsonConvert.DeserializeObject<Order>(message);
      Logger.LogInformation(" [>>>>>>>>>>] Received Order Accepted '{0}'...", order.OrderId);
      Thread.Sleep(500);
      Logger.LogInformation("Processed");
    }
  }
}
