using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Models;
using eda.core.data;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly LoggingContext _context;

  public HomeController(ILogger<HomeController> logger, LoggingContext context)
  {
    _logger = logger;
    _context = context;
  }

  public async Task<IActionResult> Index()
  {
    var result = await _context.LogEntries.OrderBy(m => m.OrderId).ThenBy(m => m.WhenReceived).ToListAsync();

    return View(result);
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}

