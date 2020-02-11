using System;

namespace eda.core
{
  public interface IEvent
  {
    DateTime Start { get; set; }
    Guid EventId { get; set; }
  }
}
