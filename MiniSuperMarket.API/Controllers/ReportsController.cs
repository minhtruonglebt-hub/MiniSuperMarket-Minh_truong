using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;
        public ReportsController(SupermarketDbContext context) => _context = context;

        [HttpGet("quick")]
        public async Task<IActionResult> Quick([FromQuery] DateOnly? date)
        {
            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var start = selectedDate.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            var orders = _context.Orders.AsNoTracking().Where(x => x.CreatedDate >= start && x.CreatedDate < end);
            var summary = await orders.GroupBy(_ => 1).Select(g => new
            {
                TotalOrders = g.Count(),
                TotalRevenue = g.Sum(x => x.FinalAmount)
            }).SingleOrDefaultAsync();
            var bestSeller = await _context.OrderDetails.AsNoTracking()
                .Where(x => x.Order != null && x.Order.CreatedDate >= start && x.Order.CreatedDate < end)
                .GroupBy(x => x.Product!.ProductName)
                .Select(g => new { ProductName = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Quantity).ThenBy(x => x.ProductName).FirstOrDefaultAsync();
            return Ok(new
            {
                Date = selectedDate,
                TotalOrders = summary?.TotalOrders ?? 0,
                TotalRevenue = summary?.TotalRevenue ?? 0m,
                BestSeller = bestSeller?.ProductName ?? "Chưa có dữ liệu",
                BestSellerQuantity = bestSeller?.Quantity ?? 0
            });
        }
    }
}
