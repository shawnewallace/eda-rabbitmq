using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eda.core.data
{
	// public class LoggingContextFactory : IDesignTimeDbContextFactory<LoggingContext>
	// {
	// 	public LoggingContext CreateDbContext(string[] args)
	// 	{
	// 		var optionsBuilder = new DbContextOptionsBuilder<LoggingContext>();
	// 		optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

	// 		return new LoggingContext(optionsBuilder.Options);
	// 	}
	// }

  public class LogginContextInitializer
  {
    public static void Initialize(LoggingContext context)
    {
      var iler = new LogginContextInitializer();
      iler.SeedEverything(context);
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
}