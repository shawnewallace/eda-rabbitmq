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
	}
}
