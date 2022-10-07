namespace eda.core.data
{
  public class OrderIdModel : IHaveOrderId, IEvent
  {
    public Guid OrderId { get; set; }

		public DateTime Start { get; set; }
		public Guid EventId { get; set; }
		public Guid CorrelationId { get; set; }
	}
}