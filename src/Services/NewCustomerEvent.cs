using System;
using eda.core.events;

namespace eda.services
{
	public class NewCustomerEvent : INewCustomer
	{
		public Guid CustomerId { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string EmailAddress { get; set; } = null!;

		public DateTime Start { get; set; } = DateTime.UtcNow;
		public Guid EventId { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; } = default!;
	}
}
