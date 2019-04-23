using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Logger.Models
{
	public class LoggingContext : DbContext
	{
		public LoggingContext() : base("EDA:Logger")
		{
		}
		public DbSet<LogEntryEntity> LogEntries { get; set; }
	}
}