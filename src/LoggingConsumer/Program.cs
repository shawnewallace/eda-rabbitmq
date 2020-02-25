using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace eda.loggingConsumer
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args)
        .Build()
        .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
      .ConfigureLogging(logging =>
      {
        logging.ClearProviders();
        logging.AddConsole();
      })
      .ConfigureServices((hostContext, services) =>
      {
        services.AddHostedService<EventLogger>();
      });
  }
}
