using System.Linq;
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
      DeclareQ(Constants.SHIPPING_QUEUE_NAME);
      BindToQ(queueName: Constants.SHIPPING_QUEUE_NAME,
                eventName: Constants.CUSTOMER_BILLED_EVENT);
      BindToQ(queueName: Constants.SHIPPING_QUEUE_NAME,
                eventName: Constants.ORDER_ACCEPTED_EVENT);
      SetUpQoS();

      Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      Logger.LogInformation("[SHIPPER] Init COMPLETE");
      Logger.LogInformation("[SHIPPER] Waiting for messages.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (ch, ea) =>
      {
        // received message  
        var content = System.Text.Encoding.UTF8.GetString(ea.Body);
        var orderEvent = DeserializeMessage(content);
        var routingKey = ea.RoutingKey;

        Logger.LogInformation($" [>>>>>>>>>>] Received  '{routingKey}':'{content}'");

        // handle the received message  
        ProcessEvent(orderEvent);
        Channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: Constants.SHIPPING_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private static void ProcessMessage(IModel channel, string message, string routingKey)
    {
      switch (routingKey)
      {
        case Constants.ORDER_ACCEPTED_EVENT:
          ProcessOrderAccepted(channel, message);
          return;
        case Constants.CUSTOMER_BILLED_EVENT:
          ProcessCustomerBilled(channel, message);
          return;
      }

      Console.WriteLine("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, message);

      //throw new ArgumentException("Could not process message for routing key {0}", routingKey);
    }

    private static void ProcessCustomerBilled(IModel channel, string message)
    {
      var billed = JsonConvert.DeserializeObject<CustomerBilledEvent>(message);
      Console.Write(" [>>>>>>>>>>] Received Customer Billed '{0}'...", billed.OrderId);
      Thread.Sleep(10000);
      IOrderReadyForShipment ready = new OrderReady { OrderId = billed.OrderId };
      var orderMessage = JsonConvert.SerializeObject(billed);
      var body = Encoding.UTF8.GetBytes(orderMessage);
      channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.READY_FOR_SHIPMENT_EVENT, null, body);
      Console.WriteLine("Processed");
    }

    private static void ProcessOrderAccepted(IModel channel, string message)
    {
      var order = JsonConvert.DeserializeObject<Order>(message);
      Console.Write(" [>>>>>>>>>>] Received Order Accepted '{0}'...", order.OrderId);
      Thread.Sleep(500);
      Console.WriteLine("Processed");
    }
  }

  internal class CustomerBilledEvent : ICustomerBilled
  {
    public CustomerBilledEvent(Guid orderEvent)
    {
      OrderId = orderEvent;
    }

    public Guid OrderId { get; set; }
  }

  internal class Order : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }

  internal class OrderReady : IOrderReadyForShipment
  {
    public Guid OrderId { get; set; }

  }
}
