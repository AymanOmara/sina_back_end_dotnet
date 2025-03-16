using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public ProductController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prouduct>>> GetProducts()
        {
            return await _context.Prouducts.ToListAsync();
        }

        [HttpGet("GetProduct/{category}")]
        public async Task<ActionResult<IEnumerable<Prouduct>>> GetProductsByCategoryId(string category)
        {
            return await _context.Prouducts.Where((pr) => pr.Type == category).ToListAsync();
        }

        // POST: api/Product/AddProduct
        [HttpPost("AddProduct")]
        public async Task<ActionResult<Prouduct>> AddProduct([FromBody] Prouduct product)
        {
            _context.Prouducts.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Productid }, product);
        }

        // POST: api/Product/AddFavorite
        [HttpPost("AddFavorite")]
        public async Task<ActionResult<Usersprouduct>> AddFavorite([FromBody] Usersprouduct favorite)
        {
            _context.Usersprouducts.Add(favorite);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFavorite", new { id = favorite.Id }, favorite);
        }

        // DELETE: api/Product/DeleteFavorite/5
        [HttpDelete("DeleteFavorite/{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var favorite = await _context.Usersprouducts.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Usersprouducts.Remove(favorite);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}