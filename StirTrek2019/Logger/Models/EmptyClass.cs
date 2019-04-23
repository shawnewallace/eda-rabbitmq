using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eda.core;
using Microsoft.EntityFrameworkCore;

namespace eda.Logger.Models
{
  [Table(name: "log_entries", Schema = "app")]
  public class LogEntryEntity
  {
    [Key] public int Id { get; set; }
    public Guid OrderId { get; set; }
    public string RoutingKey { get; set; }
    public string Content { get; set; }
    public DateTime WhenReceived { get; set; }
  }

  public class LoggingContext : DbContext
  {
    public DbSet<LogEntryEntity> LogEntries { get; set; }

    public LoggingContext() //: base("EDA:Logger")
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
  }

  public class OrderIdModel : IOrderId
  {
    public Guid OrderId { get; set; }
  }
}
