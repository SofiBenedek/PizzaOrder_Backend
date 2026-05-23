using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyExam.Backend.Models.DbMysqlModels;

namespace MyExam.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaOrderController : ControllerBase
    {
        private readonly OrderItemsContext _context = new OrderItemsContext();

        [HttpGet("/api/order-items/order/{orderId}")]
        public async Task<IActionResult> GetOrdersById(int orderId)
        {
            
            if (!await _context.PizzaRendelesTeteleks.AnyAsync(x => x.OrderId == orderId))
            {
                return NotFound (new { error = "Nincs tétel a megadott rendeléshez!" });
            }

            var result = await _context.PizzaRendelesTeteleks.Where(x => x.OrderId == orderId).OrderBy(x => x.Id).ToListAsync();

            return Ok(result);

        }
        [HttpGet("/api/order-items/search")]
        public async Task<IActionResult> GetPizzaByName([FromQuery] string pizzaName)
        {

            if (string.IsNullOrWhiteSpace(pizzaName))
            {
                return BadRequest(new { error = "A pizzaName megadása kötelező!" });
            }

            var result = await _context.PizzaRendelesTeteleks.Where(x => x.Name.ToLower().Contains(pizzaName.ToLower())).OrderByDescending(x => x.Price).ThenBy(x => x.Name).ToListAsync();

            return Ok(result);

        }

        [HttpGet("/api/order-items/price-over/{price}")]
        public async Task<IActionResult> GetOrderOverPrice(string price)
        {

            if (!int.TryParse(price, out int parsedPrice))
            {
                return BadRequest(new { error = "Érvénytelen árparaméter!" });
            }

            var result = await _context.PizzaRendelesTeteleks.Where(x => x.Price > parsedPrice).OrderByDescending(x => x.Price).ToListAsync();

            return Ok(result);

        }

        //2.

        [HttpGet("/api/orders/{orderId}/total")]
        public async Task<IActionResult> GetOneOrderSumPrice(int orderId)
        {

            if (!await _context.PizzaRendelesTeteleks.AnyAsync(x => x.OrderId == orderId))
            {
                return NotFound(new { error = "A megadott rendelés nem található!" });
                }

            var result = await _context.PizzaRendelesTeteleks.Where(x => x.OrderId == orderId).SumAsync(x => x.Price);

            return Ok(new {orderId = orderId, totalAmount = result});

        }

        [HttpGet("/api/stats/average-line-value")]
        public async Task<IActionResult> GetAverageLineValue()
        {

            if (!await _context.PizzaRendelesTeteleks.AnyAsync())
            {
                return NotFound(new { error = "Nincs adat az adatbázisban!" });
            }

            var result = await _context.PizzaRendelesTeteleks.AverageAsync(x => x.Price * x.Amount);

            return Ok(new { averageLineValue = result });

        }

        //crude
        [HttpPost("/api/order-items")]
        public async Task<ActionResult> PostNewOrder([FromBody] PizzaRendelesTetelek newOrder)
        {
            if (newOrder == null || newOrder.OrderId == null || newOrder.Amount == null || newOrder.Price == null || string.IsNullOrWhiteSpace(newOrder.Name))
            {
                return BadRequest(new { error = "Minden mező kitöltése kötelező!" });
            }
            var maxId = await _context.PizzaRendelesTeteleks.MaxAsync(x => x.Id);

            newOrder.Id = maxId + 1;

            await _context.PizzaRendelesTeteleks.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rendelési tétel sikeresen rögzítve!", id = newOrder.Id });

        }
        [HttpPatch("/api/order-items/{id}/quantity")]
        public async Task<ActionResult> PatchNewAmount(int id, [FromBody] string newAmount)
        {
            
            if (!await _context.PizzaRendelesTeteleks.AnyAsync(x => x.Id == id))
            {
                return NotFound(new { error = "Nincs ilyen azonosítójú tétel!" });
            }
            if (string.IsNullOrWhiteSpace(newAmount) || !int.TryParse(newAmount, out int parsedNewAmount))
            {
                return BadRequest(new { error = "A mennyiseg mező kötelező és pozitív egész szám kell legyen!" });
            }
            var findId = await _context.PizzaRendelesTeteleks.FindAsync(id);

            findId.Amount = parsedNewAmount;
            await _context.SaveChangesAsync();

            return Ok(new { message = "A mennyiség módosítása sikeres!" });
        }

        [HttpDelete("/api/order-items/{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var findId = await _context.PizzaRendelesTeteleks.FindAsync(id);

            if (!await _context.PizzaRendelesTeteleks.AnyAsync(x => x.Id == id))
            {
                return NotFound(new { error = "Nincs ilyen azonosítójú tétel!" });
            }

            _context.PizzaRendelesTeteleks.Remove(findId);
            await _context.SaveChangesAsync();

            return Ok(new { messaeg = "A rendelési tétel törlése sikeres!" });
        }

        [HttpGet("/api/reports/orders/totals")]
        public async Task<IActionResult> GetTotalAmountByOrder()
        {
            var result = await _context.PizzaRendelesTeteleks.GroupBy(x => x.OrderId).Select(x => new
            {
                OrderId = x.Key,
                TotalAmount = x.Sum(y => y.Amount * y.Price)
            }).OrderByDescending(x => x.TotalAmount).ToListAsync();



            return Ok(result);
        }
        [HttpGet("/api/reports/pizzas/quantities")]
        public async Task<IActionResult> GetTotalQuantitybyName()
        {
            var result = await _context.PizzaRendelesTeteleks.GroupBy(x => x.Name).Select(x => new
            {
                rendeles = x.Key,
                totalQuantity = x.Sum(x => x.Amount)
            }).OrderByDescending(x => x.totalQuantity).ToListAsync();



            return Ok(result);
        }
        [HttpGet("/api/reports/pizzas/revenue")]
        public async Task<IActionResult> GetRevenueByName()
        {
            var result = await _context.PizzaRendelesTeteleks.GroupBy(x => x.Name).Select(x => new
            {
                rendeles = x.Key,
                revenue = x.Sum(x => x.Amount * x.Price)
            }).OrderByDescending(x => x.revenue).ToListAsync();



            return Ok(result);
        }


    }
}
