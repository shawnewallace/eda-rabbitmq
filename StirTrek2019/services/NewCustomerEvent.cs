using System;
using eda.core.Events;

namespace eda.SalesService
{
  public class NewCustomerEvent : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }
}
