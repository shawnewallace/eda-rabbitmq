using System;
using eda.core.events;

namespace eda.shippingConsumer
{
  internal class OrderReady : IOrderReadyForShipment
  {
    public Guid OrderId { get; set; }

		public DateTime Start { get; set; } = DateTime.UtcNow;
		public Guid EventId { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; } = default!;

  }
}
