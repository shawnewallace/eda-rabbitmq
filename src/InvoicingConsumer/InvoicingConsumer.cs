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
  public class InvoicingConsumer : BackgroundQService<InvoicingConsumer>
  {
    private static int _ordersReceived = 0;

    public InvoicingConsumer(ILogger<InvoicingConsumer> logger) : base(logger)
    {
      Init();
    }

    private void Init()
    {
      Logger.LogInformation("[INVOICER] Init");

      var factory = new ConnectionFactory { HostName = "host.docker.internal" };
      //var factory = new ConnectionFactory { HostName = "localhost" };
      Connection = factory.CreateConnection();

      Channel = Connection.CreateModel();

      DeclareExchange();
      DeclareQ(AppConstants.INVOICING_QUEUE_NAME);
      BindToQ(queueName: AppConstants.INVOICING_QUEUE_NAME,
                eventName: AppConstants.ORDER_ACCEPTED_EVENT);
      SetUpQoS();

      Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      Logger.LogInformation("[INVOICER] Init COMPLETE");
      Logger.LogInformation("[INVOICER] Waiting for messages.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (ch, ea) =>
      {
        // received message
        var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
        var orderEvent = DeserializeMessage(content);
        var routingKey = ea.RoutingKey;

        Logger.LogInformation($" [>>>>>>>>>>] Received Order Accepted '{routingKey}':'{_ordersReceived++}'");

        // handle the received message  
        ProcessEvent(orderEvent);
        Channel.BasicAck(ea.DeliveryTag, false);
      };

      consumer.Shutdown += OnConsumerShutdown;
      consumer.Registered += OnConsumerRegistered;
      consumer.Unregistered += OnConsumerUnregistered;
      consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

      Channel.BasicConsume(queue: AppConstants.INVOICING_QUEUE_NAME, autoAck: false, consumer: consumer);
      return Task.CompletedTask;
    }

    private void ProcessEvent(IOrderAccepted orderEvent)
    {
      Logger.LogInformation("\tProcessing order {0}...", orderEvent.OrderId);
      ICustomerBilled billedEvent = new CustomerBilledEvent(orderEvent.OrderId);
      Thread.Sleep(5000);
      var message = JsonConvert.SerializeObject(billedEvent);
      var body = System.Text.Encoding.UTF8.GetBytes(message);
      Channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.CUSTOMER_BILLED_EVENT, null, body);

      Logger.LogInformation("Done");
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
      Channel.Close();
      Connection.Close();
      base.Dispose();
    }
  }
}
