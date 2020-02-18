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

namespace eda.invoicingConsumer
{
  public class InvoicingConsumer : BackgroundService
  {
    private readonly ILogger<InvoicingConsumer> _logger;
    private IConnection _connection;
    private IModel _channel;
    private static int _ordersReceived = 0;


    public InvoicingConsumer(ILogger<InvoicingConsumer> logger)
    {
      _logger = logger;

      Init();
    }

    private void Init()
    {
      _logger.LogInformation("[INVOICER] Init");


      var factory = new ConnectionFactory { HostName = "host.docker.internal" };
      _connection = factory.CreateConnection();

      _channel = _connection.CreateModel();

      _channel.ExchangeDeclare(Constants.EXCHANGE_NAME, ExchangeType.Direct, true);
      _channel.QueueDeclare(Constants.INVOICING_QUEUE_NAME, true, false, false, null);
      _channel.QueueBind(
                  queue: Constants.INVOICING_QUEUE_NAME,
                  exchange: Constants.EXCHANGE_NAME,
                  routingKey: Constants.ORDER_ACCEPTED_EVENT);
      _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

      _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      _logger.LogInformation("[INVOICER] Init COMPLETE");
      _logger.LogInformation("[INVOICER] Waiting for messages.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(_channel);
      consumer.Received += (ch, ea) =>
      {
        // received message  
        var content = System.Text.Encoding.UTF8.GetString(ea.Body);
        var orderEvent = DeserializeMessage(content);
        var routingKey = ea.RoutingKey;

        _logger.LogInformation($" [>>>>>>>>>>] Received Order Accepted '{routingKey}':'{_ordersReceived++}'");

        // handle the received message  
        ProcessEvent(orderEvent);
        _channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      _channel.BasicConsume(queue: Constants.INVOICING_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void ProcessEvent(IOrderAccepted orderEvent)
    {
      _logger.LogInformation("\tProcessing order {0}...", orderEvent.OrderId);
      ICustomerBilled billedEvent = new CustomerBilledEvent(orderEvent.OrderId);
      Thread.Sleep(5000);
      var message = JsonConvert.SerializeObject(billedEvent);
      var body = System.Text.Encoding.UTF8.GetBytes(message);
      _channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.CUSTOMER_BILLED_EVENT, null, body);

      _logger.LogInformation("Done");
    }

    private static IOrderAccepted DeserializeMessage(string message)
    {
      return JsonConvert.DeserializeObject<Order>(message);
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
      _channel.Close();
      _connection.Close();
      base.Dispose();
    }
  }
}
