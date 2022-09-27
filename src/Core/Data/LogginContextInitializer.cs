using Microsoft.EntityFrameworkCore;

namespace eda.core.data
{
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