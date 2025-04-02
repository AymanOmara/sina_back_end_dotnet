using DentalShopWebApi.AllServices;
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
        private readonly Services _services;

        public ProductController(db_aa382a_ibnsinadentalContext context, Services services)
        {
            _context = context;
            this._services = services;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prouduct>>> GetProducts()
        {
            try
            {
                return await _context.Prouducts.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetProduct/{category}")]
        public async Task<ActionResult<List<Prouduct>>> GetProductsByCategoryId(string category, string firstYear = "F",
            string secondYear = "F", string thirdYear = "F", string forthYear = "F", string fifthYear = "F",
            string Clothes = "F", string Teeth = "F")
        {
            try
            {
                var Query = _context.Prouducts.AsQueryable();

                if (category.ToLower().Trim() != "student")
                    Query = Query.Where(pr => pr.Type.ToLower().Trim() == category.ToLower().Trim());

                //apply filter
                if (firstYear != "F")
                    Query = Query.Where(pr => pr.Firstyear.ToLower().Trim() == firstYear.ToLower().Trim());

                if (secondYear != "F")
                    Query = Query.Where(pr => pr.Secondyear.ToLower().Trim() == secondYear.ToLower().Trim());

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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Product/AddProduct
        [HttpPost("AddProduct")]
        public async Task<ActionResult<Prouduct>> AddProduct([FromBody] Prouduct product)
        {
            try
            {
                if (product.Firstphoto != null)
                {
                    string imageUrl =
                        await _services.SaveImageAsync(product.Firstphoto, Guid.NewGuid(), "1", "product");
                    if (imageUrl == null)
                        return BadRequest("Invalid image data");


                    product.Firstphoto = imageUrl;
                    product.Secondphoto = imageUrl;
                    product.Thirdphoto = imageUrl;
                    product.Fourthphoto = imageUrl;
                    product.Fifthphoto = imageUrl;
                }

                _context.Prouducts.Add(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Product/AddFavorite
        [HttpPost("AddFavorite")]
        public async Task<ActionResult<Usersprouduct>> AddFavorite([FromBody] Usersprouduct favorite)
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            var id = BitConverter.ToInt32(bytes, 0);
            favorite.Id = id;
            try
            {
                _context.Usersprouducts.Add(favorite);
                await _context.SaveChangesAsync();
                var product = await _context.Prouducts.FindAsync(favorite.Prouductid);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Product/DeleteFavorite
        [HttpPost("DeleteFavorite")]
        public async Task<IActionResult> DeleteFavorite([FromBody] Usersprouduct request)
        {
            try
            {
                var favorite = await _context.Usersprouducts.FirstOrDefaultAsync(pr =>
                    pr.Userid == request.Userid && pr.Prouductid == request.Prouductid);
                if (favorite == null)
                {
                    return NotFound();
                }
                _context.Usersprouducts.Remove(favorite);
                await _context.SaveChangesAsync();
                var product = await _context.Prouducts.FindAsync(favorite.Prouductid);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllFavorites")]
        public async Task<ActionResult<IEnumerable<Prouduct>>> GetAllFavorites(int userId)
        {
            var ayman = await _context.Usersprouducts.Where(pr => pr.Userid == userId).ToListAsync();
            var favoritesIds = await _context.Usersprouducts
                .Where((fav) => fav.Userid == userId)
                .Select((pr) => pr.Prouductid)
                .ToListAsync();
            var products = await _context.Prouducts
                .Where((product) => favoritesIds
                    .Contains(product.Productid))
                .ToListAsync();
            return Ok(products);
        }

        // PUT: api/Product/UpdateProduct/5
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Prouduct updatedProduct)
        {
            try
            {
                var existingProduct = await _context.Prouducts.FindAsync(id);
                if (existingProduct == null)
                    return NotFound();

                // Update fields
                existingProduct.Prouductname = updatedProduct.Prouductname;
                existingProduct.Type = updatedProduct.Type;
                existingProduct.Firstyear = updatedProduct.Firstyear;
                existingProduct.Secondyear = updatedProduct.Secondyear;
                existingProduct.Thirdyear = updatedProduct.Thirdyear;
                existingProduct.Fourthyear = updatedProduct.Fourthyear;
                existingProduct.Fifthyear = updatedProduct.Fifthyear;
                existingProduct.Clothes = updatedProduct.Clothes;
                existingProduct.Teeth = updatedProduct.Teeth;
                existingProduct.Price = updatedProduct.Price;

                if (!string.IsNullOrEmpty(updatedProduct.Firstphoto))
                {
                    var res = await _services.DeleteImageAsync(existingProduct.Firstphoto);
                    if (res)
                    {
                        string imageUrl = await _services.SaveImageAsync(updatedProduct.Firstphoto, Guid.NewGuid(), "1",
                            "product");
                        if (imageUrl == null)
                            return BadRequest("Invalid image data");

                        existingProduct.Firstphoto = imageUrl;
                        existingProduct.Secondphoto = imageUrl;
                        existingProduct.Thirdphoto = imageUrl;
                        existingProduct.Fourthphoto = imageUrl;
                        existingProduct.Fifthphoto = imageUrl;
                    }
                }

                _context.Entry(existingProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Product/DeleteProduct/5
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Prouducts.FindAsync(id);
                if (product == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(product.Firstphoto))
                {
                    await _services.DeleteImageAsync(product.Firstphoto);
                }

                _context.Prouducts.Remove(product);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}