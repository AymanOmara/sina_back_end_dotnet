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
        public async Task<ActionResult<List<Prouduct>>> GetProductsByCategoryId(string category, string firstYear = "F", string secondYear = "F", string thirdYear = "F", string forthYear = "F", string fifthYear = "F", string Clothes = "F", string Teeth = "F")
        {
            var Query =  _context.Prouducts.AsQueryable();

            if (category.ToLower().Trim() != "student")
                Query = Query.Where(pr => pr.Type.ToLower().Trim() == category.ToLower().Trim());

            //apply filter
            if (firstYear!="F")
                Query = Query.Where(pr => pr.Firstyear.ToLower().Trim() == firstYear.ToLower().Trim());

            if (secondYear != "F")
                Query= Query.Where(pr => pr.Secondyear.ToLower().Trim() == secondYear.ToLower().Trim());

            if (thirdYear != "F")
                Query = Query.Where(pr => pr.Thirdyear.ToLower().Trim() == thirdYear.ToLower().Trim());

            if (forthYear != "F")
                Query = Query.Where(pr => pr.Fourthyear.ToLower().Trim() == forthYear.ToLower().Trim());

            if (fifthYear != "F")
                Query = Query.Where(pr => pr.Fifthyear.ToLower().Trim() == fifthYear.ToLower().Trim());

            if (Clothes != "F")
                Query = Query.Where(pr => pr.Clothes.ToLower().Trim() == Clothes.ToLower().Trim());

            if (Teeth != "F")
                Query = Query.Where(pr => pr.Teeth.ToLower().Trim() == Teeth.ToLower().Trim());

            return await Query.ToListAsync();
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