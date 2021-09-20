using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStore.Models;

namespace TestStore.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        ApplicationContext db;
        public OrderController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet("/api/getorders")]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            IEnumerable<Order> orders = await db.Orders.ToListAsync();

            if (orders != null)
            {
                return Ok(orders);
            }

            return BadRequest(new { errorText = "Нет ни одного заказа" });
        }

        [HttpGet("/api/getuserorders/{userid}")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            IEnumerable<Order> orders = await db.Orders.Where(o => o.UserId == userId).ToListAsync();

            if (orders != null)
            {
                return Ok(orders);
            }

            return BadRequest(new { errorText = "У Вас нет ни одного заказа" });
        }

        [HttpGet("/api/order/{orderid}")]
        [Authorize]
        public async Task<IActionResult> Order(int orderId)
        {
            IEnumerable<Position> positions = await db.Positions
               .Include(p => p.Good).Where(p => p.OrderId == orderId).ToListAsync();

            if (positions != null)
            {
                return Ok(positions);
            }

            return BadRequest(new { errorText = "В заказе нет позиций" });
        }

        [HttpPost("/api/saveorder/{userid}")]
        [Authorize]
        public async Task<IActionResult> saveorder(int userId, [FromBody] Basket basket)
        {
            if (basket != null)
            {
                var lastOrder = await db.Orders.LastOrDefaultAsync();

                var orderNumber = (Convert.ToInt32(lastOrder.OrderNumber) + 1).ToString();

                Order order = new Order
                {
                    UserId = userId,
                    OrderNumber = orderNumber
                };

                db.Orders.Add(order);

                await db.SaveChangesAsync();

                int newOrderId = order.OrderId;

                var goods = basket.goods;

                foreach (var good in goods)
                {
                    Position position = new Position
                    {
                        GoodId = good.GoodId,
                        Good = good,
                        OrderId = newOrderId,
                        Order = await db.Orders.Where(o => o.OrderId == newOrderId).FirstOrDefaultAsync()
                    };

                    db.Positions.Add(position);
                }

                await db.SaveChangesAsync();

                return Ok(new { okText = "Заказ сохранен" });
            }

            return BadRequest(new { errorText = "Некорректные исходные данные" });
        }


    }
}
