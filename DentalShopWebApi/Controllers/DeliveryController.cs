using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public DeliveryController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/Delivery/GetDeliveryFees
        [HttpGet("GetDeliveryFees")]
        public async Task<ActionResult<Maininfo>> GetDeliveryFees()
        {
            var mainInfo = await _context.Maininfos.FirstOrDefaultAsync();
            if (mainInfo == null)
            {
                return NotFound();
            }

            return mainInfo;
        }

        [HttpPost("UpdateDeliveryFees")]
        public async Task<ActionResult<Maininfo>> UpdateDeliveryFees(Maininfo request)
        {
            var mainInfo = await _context.Maininfos.FirstOrDefaultAsync();
            if (mainInfo == null)
            {
                return NotFound();
            }
            _context.Maininfos.Add(request);
            mainInfo.OutCairoFees = request.OutCairoFees;
            mainInfo.Cairofees = request.Cairofees;
            _context.Entry(mainInfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(mainInfo);
        }
    }
}