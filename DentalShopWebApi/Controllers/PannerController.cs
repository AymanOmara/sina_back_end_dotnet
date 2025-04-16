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
    public class PannerController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;
        private readonly Services _services;

        public PannerController(db_aa382a_ibnsinadentalContext context , Services services)
        {
            _context = context;
            this._services = services;
        }

        // GET: api/Panner/GetPanner
        [HttpGet("GetPanner")]
        public async Task<ActionResult<IEnumerable<Panner>>> GetPanner()
        {
            try
            {
                return await _context.Panners.ToListAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Panner/AddPanner
        [HttpPost("AddPanner")]
        public async Task<ActionResult<Panner>> AddPanner([FromBody] Panner panner)
        {
            try
            {

                if (panner.Link != null)
                {
                    string imageUrl = await _services.SaveImageAsync(panner.Link, Guid.NewGuid(), "1", "panner");
                    if (imageUrl == null)
                        return BadRequest("Invalid image data");

                    panner.Link = imageUrl;
                }
                _context.Panners.Add(panner);
                await _context.SaveChangesAsync();
                return Ok(panner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // PUT: api/Panner/UpdatePanner/{id}
        [HttpPut("UpdatePanner/{id}")]
        public async Task<IActionResult> UpdatePanner(int id, [FromBody] Panner UpdatedPanner)
        {
            try
            {

                var existingPanner = await _context.Panners.FindAsync(id);
                if (existingPanner == null)
                    return NotFound("Panner not found");

                if (!string.IsNullOrEmpty(UpdatedPanner.Link))
                {
                    var res = await _services.DeleteImageAsync(existingPanner.Link);
                    //if (res)
                    //{
                        string imageUrl = await _services.SaveImageAsync(UpdatedPanner.Link, Guid.NewGuid(), "1", "panner");
                        if (imageUrl == null)
                            return BadRequest("Invalid image data");

                        existingPanner.Link = imageUrl;
                    //}
                }

                _context.Entry(existingPanner).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingPanner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        // DELETE: api/Panner/DeletePanner/{id}
        [HttpDelete("DeletePanner/{id}")]
        public async Task<IActionResult> DeletePanner(int id)
        {
            try
            {

                var panner = await _context.Panners.FindAsync(id);
                if (panner == null)
                    return NotFound("Panner not found");

                if (!string.IsNullOrEmpty(panner.Link))
                {

                    var res = await _services.DeleteImageAsync(panner.Link);
                }
           
                    _context.Panners.Remove(panner);
                    await _context.SaveChangesAsync();

                    return Ok("Panner deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }


    }
}
