using System.Collections.Generic;

namespace eda.core.events
{
  public interface IOrderAccepted : IHaveCustomerId, IHaveOrderId, IEvent
  {
    List<OrderItem> OrderItems { get; set; }
  }
}