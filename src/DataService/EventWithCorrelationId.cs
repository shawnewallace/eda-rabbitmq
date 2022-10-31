using eda.core;

namespace eda.dataService;

internal class EventWithCorrelationId : IEvent
{
	public DateTime Start { get; set; }
	public Guid EventId { get; set; }
	public Guid CorrelationId { get; set; }
}