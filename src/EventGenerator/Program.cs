using System;
using System.Collections.Generic;
using Bogus;
using eda.core;
using eda.core.events;
using eda.services;

namespace EventGenerator
{
  public class Program
  {
    public static int Main(string[] args)
    {
      string usageText = $"Usage: EventGenerator [1 = '{AppConstants.ORDER_ACCEPTED_EVENT}', 2 = '{AppConstants.NEW_CUSTOMER_EVENT}'] [num_to_generate]";

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
			// Randomizer.Seed = new Random(8675309);
			var orderItemDescription = new []{"appple", "banana", "widget", "car", "pizza"};
			var testOrderItems = new Faker<OrderItem>()
				.RuleFor(o => o.Description, f => f.PickRandom(orderItemDescription))
				.RuleFor(o => o.ItemId, f => Guid.NewGuid())
				.RuleFor(o => o.Price, f => f.Random.Double(5.00, 100.00))
				.RuleFor(o => o.Quantity, f => f.Random.Int(1, 20));

			var testOrder = new Faker<OrderAcceptedEvent>()
				.RuleFor(o => o.OrderId, f => Guid.NewGuid())
				.RuleFor(o => o.CustomerId, f => Guid.NewGuid())
				.RuleFor(o => o.OrderItems, f => testOrderItems.Generate(f.Random.Int(1, 50)));

			return testOrder.Generate();
    }

    private static INewCustomer CreateNewSampleCustomerEvent()
    {
			// Randomizer.Seed = new Random(8675309);
			var testCustomer = new Faker<NewCustomerEvent>()
				.RuleFor(o => o.CustomerId, f => Guid.NewGuid())
				.RuleFor(o => o.FirstName, f => f.Name.FirstName())
				.RuleFor(o => o.LastName, f => f.Name.LastName())
				.RuleFor(o => o.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName));


			return testCustomer.Generate();


      // INewCustomer customer = new NewCustomerEvent();
      // customer.CustomerId = Guid.NewGuid();
      // return customer;
    }
  }
}
