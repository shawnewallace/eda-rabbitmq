using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eda.core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eda.loggingConsumer
{
	public class LogginContextInitializer
	{
		public static void Initialize(LoggingContext context, ILogger<EventLogger> logger)
		{
			logger.LogInformation("[LOGGER] Initializing database.");
			
			var iler = new LogginContextInitializer();
			iler.SeedEverything(context);
			
			logger.LogInformation("[LOGGER] Initializing database...COMPLETE");

		}

		private void SeedEverything(LoggingContext context)
		{
			try
			{
				// context.Database.EnsureDeleted();
				context.Database.EnsureCreated();
				context.Database.Migrate();
			}
			catch { return; }
		}
	}


	public class LoggingContext(string connectionString) : DbContext
	{
		// private const string CONNECTION_STRING = @"Server=db;Database=event_logger;User=sa;Password=21239Admin;";
		// private string CONNECTION_STRING = @"Data Source=localhost;Initial Catalog=event_logger;User=sa;Password=21239Admin;Encrypt=False";

		public DbSet<LogEntry> LogEntries { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(connectionString);
		}
	}

	[Table(name: "log_entries", Schema = "dbo")]
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