using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Events;
using SalesService;

namespace CustomerImportClient
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("hit enter to start");
      Console.ReadLine();


      var service = new CustomerCreator();

      for (var i = 0; i < 100; i++)
      {
        var customer = CreateNewSampleCustomerEvent();
        service.Submit(customer);
      }
    }

    private static INewCustomer CreateNewSampleCustomerEvent()
    {
      INewCustomer customer = new NewCustomerEvent();
      customer.CustomerId = Guid.NewGuid();
      return customer;
    }
  }
}
