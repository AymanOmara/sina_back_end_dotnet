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

        public PannerController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/Panner/GetPanner
        [HttpGet("GetPanner")]
        public async Task<ActionResult<IEnumerable<Panner>>> GetPanner()
        {
            return await _context.Panners.ToListAsync();
        }

        // POST: api/Panner/AddPanner
        [HttpPost("AddPanner")]
        public async Task<ActionResult<Panner>> AddPanner([FromBody] Panner panner)
        {
            _context.Panners.Add(panner);
            await _context.SaveChangesAsync();
            return Ok( panner);
        }
    }
}
