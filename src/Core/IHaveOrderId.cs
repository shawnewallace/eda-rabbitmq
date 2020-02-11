using System;

namespace eda.core
{
  public interface IHaveOrderId
  {
    Guid OrderId { get; set; }
  }
}
