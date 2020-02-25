using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eda.core;
using Microsoft.EntityFrameworkCore;

namespace eda.loggingConsumer
{
  public class LoggingContext : DbContext
  {
    private const string CONNECTION_STRING = @"Server=db;Database=event_logger;User=sa;Password=21239Admin;";

    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(CONNECTION_STRING);
    }
  }

  [Table(name: "log_entries", Schema = "app")]
  public class LogEntry
  {
    [Key] public int Id { get; set; }
    public Guid OrderId { get; set; }
    public string RoutingKey { get; set; }
    public string Content { get; set; }
    public DateTime WhenReceived { get; set; } = DateTime.UtcNow;
  }

  internal class OrderIdModel : IHaveOrderId
  {
    public Guid OrderId { get; set; }
  }
}