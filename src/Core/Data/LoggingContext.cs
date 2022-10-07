using System;
using eda.core;
using Microsoft.EntityFrameworkCore;

namespace eda.core.data
{
  public class LoggingContext : DbContext
  { 
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();

    // public LoggingContext() { }
    public LoggingContext(DbContextOptions<LoggingContext> options) : base(options)
    { }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
		// 	// optionsBuilder.UseSqlServer();
    //   //optionsBuilder.UseSqlServer(CONNECTION_STRING);
    // }
  }
}