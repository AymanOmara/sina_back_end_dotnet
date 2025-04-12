using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public OrderController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/Orders/GetAllOrders
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .OrderByDescending(o => o.Orderid)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // POST: api/Orders/MakeOrder
        [HttpPost("MakeOrder")]
        public async Task<ActionResult<Order>> MakeOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u=>u.Userid == orderRequest.UserId);
                if (user == null)
                {
                    return BadRequest("user id is not right");
                }
                // Step 1: Create the order
                var order = new Order
                {
                    Userid = orderRequest.UserId,
                    Ordertime = DateTime.UtcNow,
                    Orderstatus = orderRequest.OrderStatus,
                    Orderaccepttime = orderRequest.OrderAcceptTime,
                    Orderprice = orderRequest.OrderPrice,
                    Orderlocation = orderRequest.OrderLocation,
                    Orderphone = orderRequest.OrderPhone,
                    Paymentmethod = orderRequest.PaymentMethod,
                    Deliveryfees = orderRequest.DeliveryFees,
                    Orderexcutetime = orderRequest.OrderExcuteTime,
                    Ordercompletetime = orderRequest.OrderCompleteTime,
                    Useravilabletime = orderRequest.UserAvilableTime,
                    Orderamount = orderRequest.OrderAmount,
                    Ordergovernorate = orderRequest.OrderGovernorate,
                    OrderUserName = orderRequest.OrderUserName,
                    UserName = user.Username,
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Step 2: Add products to the order
                foreach (var product in orderRequest.OrderProducts)
                {
                    var orderProduct = new Orderprouductlist
                    {
                        Orderid = order.Orderid,
                        Prouductid = product.ProuductId,
                        Price = product.Price,
                        Amount = product.Amount
                    };

                    _context.Orderprouductlists.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                return Ok(new { order = order, id = order.Orderid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Orders/EditOrder/5
        [HttpPut("EditOrder/{id}")]
        public async Task<IActionResult> EditOrder(int id, [FromBody] OrderRequest orderRequest)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Userid == orderRequest.UserId);
                if (user == null)
                {
                    return BadRequest("user id is not right");
                }

                // Step 1: Update the order
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                order.Userid = orderRequest.UserId;
                order.Ordertime = DateTime.UtcNow;
                order.Orderstatus = orderRequest.OrderStatus;
                order.Orderaccepttime = orderRequest.OrderAcceptTime;
                order.Orderprice = orderRequest.OrderPrice;
                order.Orderlocation = orderRequest.OrderLocation;
                order.Orderphone = orderRequest.OrderPhone;
                order.Paymentmethod = orderRequest.PaymentMethod;
                order.Deliveryfees = orderRequest.DeliveryFees;
                order.Orderexcutetime = orderRequest.OrderExcuteTime;
                order.Ordercompletetime = orderRequest.OrderCompleteTime;
                order.Useravilabletime = orderRequest.UserAvilableTime;
                order.Orderamount = orderRequest.OrderAmount;
                order.Ordergovernorate = orderRequest.OrderGovernorate;
                order.UserName = user.Username;

                _context.Entry(order).State = EntityState.Modified;

                // Step 2: Delete existing products for the order
                var existingProducts = await _context.Orderprouductlists
                    .Where(op => op.Orderid == id)
                    .ToListAsync();

                _context.Orderprouductlists.RemoveRange(existingProducts);

                // Step 3: Add new products to the order
                foreach (var product in orderRequest.OrderProducts)
                {
                    var orderProduct = new Orderprouductlist
                    {
                        Orderid = id,
                        Prouductid = product.ProuductId,
                        Price = product.Price,
                        Amount = product.Amount
                    };

                    _context.Orderprouductlists.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Orders/GetOrders/5
        [HttpGet("GetOrders/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.Userid == userId)
                    .OrderByDescending(o => o.Orderid)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Orders/GetOrderProducts/5
        [HttpGet("GetOrderProducts/{orderId}")]
        public async Task<ActionResult<IEnumerable<Orderprouductlist>>> GetOrderProducts(int orderId)
        {
            try
            {
                var orderProducts = await _context.Orderprouductlists
                    .Where(op => op.Orderid == orderId)
                    .ToListAsync();

                return Ok(orderProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class OrderRequest
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderAcceptTime { get; set; }
        public decimal OrderPrice { get; set; }
        public string OrderLocation { get; set; }
        public string OrderPhone { get; set; }
        public string PaymentMethod { get; set; }
        public decimal DeliveryFees { get; set; }
        public DateTime? OrderExcuteTime { get; set; }
        public DateTime? OrderCompleteTime { get; set; }
        public string UserAvilableTime { get; set; }
        public string OrderUserName { get; set; } = string.Empty;
        public int OrderAmount { get; set; }
        public string OrderGovernorate { get; set; }
        public List<OrderProductDto> OrderProducts { get; set; }
    }

    public class OrderProductDto
    {
        public int ProuductId { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}