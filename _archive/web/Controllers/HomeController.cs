using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SalesService;
using web.Models;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var r = new Random();
            var numItemsInOrder = r.Next(0, 20);

            var model = new NewOrderModel(numItemsInOrder);

            var orderCreator = new OrderCreator();
            orderCreator.Submit(model);

            return View();
        }
    }
}