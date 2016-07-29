using System;
using System.Text;
using Core;
using Core.Events;
using Newtonsoft.Json;

namespace SalesService
{
  public class CustomerCreator : QueueBase
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