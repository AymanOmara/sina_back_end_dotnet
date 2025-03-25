using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductListController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public OrderProductListController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/OrderProductList/GetOrderProducts/5
        [HttpGet("GetOrderProducts/{orderId}")]
        public async Task<ActionResult<IEnumerable<Orderprouductlist>>> GetOrderProducts(int orderId)
        {
            try
            {

                return await _context.Orderprouductlists
                    .Where(op => op.Orderid == orderId)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
