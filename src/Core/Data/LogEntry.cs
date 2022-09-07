using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eda.core.data
{
  [Table(name: "log_entries", Schema = "dbo")]
	public class LogEntry
	{
		[Key] public int Id { get; set; }
		public Guid OrderId { get; set; }
		public string RoutingKey { get; set; } = "";
		public string Content { get; set; } = "";
		public DateTime WhenReceived { get; set; } = DateTime.UtcNow;
	}
}