using System;
using System.Text;
using eda.core;
using eda.core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace eda.SalesService
{
  public class CustomerCreator : QBase
  {
    public void Submit(INewCustomer customer)
    {
      var factory = GetConnectionFactory();
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          DeclareExchange(channel);

          var message = JsonConvert.SerializeObject(customer);
          var body = Encoding.UTF8.GetBytes(message);

          channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.NEW_CUSTOMER_EVENT, null, body);
          Console.WriteLine(" [x] Sent '{0}':'{1}'", Constants.NEW_CUSTOMER_EVENT, message);
        }
      }
    }
  }
}
