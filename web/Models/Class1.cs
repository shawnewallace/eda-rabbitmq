using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core;
using Core.Events;

namespace web.Models
{
    public class NewOrderModel : IOrderAccepted
    {
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public NewOrderModel(int numNewItems)
        {
            for (var i = 0; i < numNewItems; i++)
            {
                OrderItems.Add(new OrderItem
                {
                    ItemId = Guid.NewGuid(),
                    Description = $"new item {i}",
                    Price = 10,
                    Quantity = i
                });
            }
        }
    }
}