using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace eda.loggingConsumer
{
  public class LoggingContext : DbContext
  {
    public DbSet<LogEntry> LogEntries { get; set; }

  }

  [Table(name: "log_entries", Schema = "app")]
  public class LogEntry
  {
    [Key] public int Id { get; set; }
    public Guid OrderId { get; set; }
    public string RoutingKey { get; set; }
    public string Content { get; set; }
    public DateTime WhenReceived { get; set; }
  }
}