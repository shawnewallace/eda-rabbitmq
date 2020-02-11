using System.Collections.Generic;

namespace eda.core.events
{
  public interface IOrderAccepted : IHaveCustomerId, IHaveOrderId
  {
    List<OrderItem> OrderItems { get; set; }
  }
}