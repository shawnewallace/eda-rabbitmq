using System;
using Core.Events;

namespace SalesService
{
  public class NewCustomerEvent : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }
}