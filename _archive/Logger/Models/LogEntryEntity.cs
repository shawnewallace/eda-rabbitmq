using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logger.Models
{
[Table(name:"log_entries", Schema = "app")]
	public class LogEntryEntity
	{
		[Key] public int Id { get; set; }
		public Guid OrderId { get; set; }
		public string RoutingKey { get; set; }
		public string Content { get; set; }
		public DateTime WhenReceived { get; set; }
	}
}