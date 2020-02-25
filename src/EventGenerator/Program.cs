using System;
using System.Collections.Generic;
using eda.core;
using eda.core.events;
using eda.services;

namespace EventGenerator
{
  public class Program
  {
    public static int Main(string[] args)
    {
      string usageText = $"Usage: EventGenerator [1 = '{Constants.ORDER_ACCEPTED_EVENT}', 2 = '{Constants.NEW_CUSTOMER_EVENT}'] [num_to_generate]";

      if (args.Length < 2)
      {
        Console.WriteLine(usageText);
        return 1;
      }

      int whichEvent = short.Parse(args[0]);
      int n = short.Parse(args[1]);

      if (whichEvent == 1)
      {
        Console.WriteLine($"Generating {n} ORDERS. Hit enter to start");
        Console.ReadLine();
        GenerateOrders(n);
      }
      if (whichEvent == 2)
      {
        Console.WriteLine($"Generating {n} NEW CUSTOMERS. Hit enter to start");
        Console.ReadLine();
        GenerateNewCustomers(n);
      }

      return 0;
    }

    private static void GenerateNewCustomers(int n)
    {
      var service = new CustomerCreator();

      for (var i = 0; i < n; i++)
      {
        var customer = CreateNewSampleCustomerEvent();
        service.Submit(customer);
      }
    }

    private static void GenerateOrders(int n)
    {
      var service = new OrderCreator();

      for (var i = 0; i < n; i++)
      {
        var order = CreateNewSampleOrderAcceptedEvent();
        service.Submit(order);
      }
    }

    private static IOrderAccepted CreateNewSampleOrderAcceptedEvent()
    {
      IOrderAccepted order = new OrderAcceptedEvent();

      order.CustomerId = Guid.NewGuid();
      order.OrderId = Guid.NewGuid();
      order.OrderItems = new List<OrderItem>
      {
        new OrderItem {Description = "Item One", ItemId = Guid.NewGuid(), Price = 100.21, Quantity = 4},
        new OrderItem {Description = "Item Two", ItemId = Guid.NewGuid(), Price = 19.99, Quantity = 6},
        new OrderItem {Description = "Item Three", ItemId = Guid.NewGuid(), Price = 5, Quantity = 1}
      };
      return order;
    }

    private static INewCustomer CreateNewSampleCustomerEvent()
    {
      INewCustomer customer = new NewCustomerEvent();
      customer.CustomerId = Guid.NewGuid();
      return customer;
    }
  }
}
