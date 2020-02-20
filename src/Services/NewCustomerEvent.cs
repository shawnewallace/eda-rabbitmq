using System;
using eda.core.events;

namespace eda.services
{
  public class NewCustomerEvent : INewCustomer
  {
    public Guid CustomerId { get; set; }
  }
}
