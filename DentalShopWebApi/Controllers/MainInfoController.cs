using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainInfoController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public MainInfoController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/MainInfo/GetMainInfo
        [HttpGet("GetMainInfo")]
        public async Task<ActionResult<Maininfo>> GetMainInfo()
        {
            try
            {
                var mainInfo = await _context.Maininfos.FirstOrDefaultAsync();
                if (mainInfo == null)
                    return NotFound("MainInfo not found");

                return Ok(mainInfo);
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("AddMainInfo")]
        public async Task<ActionResult<Maininfo>> AddMainInfo([FromBody] Maininfo mainInfo)
        {
            _context.Maininfos.Add(mainInfo);
            await _context.SaveChangesAsync();
            return Ok(mainInfo);
        }
        // POST: api/MainInfo/AddMainInfo


        // PUT: api/MainInfo/UpdateMainInfo
        [HttpPut("UpdateMainInfo")]
        public async Task<IActionResult> UpdateMainInfo([FromBody] Maininfo mainInfo)
        {
            try
            {

                var existingMainInfo = await _context.Maininfos.FirstOrDefaultAsync();
                if (existingMainInfo == null)
                    return NotFound("MainInfo not found");

                existingMainInfo.Cairofees = mainInfo.Cairofees;
                existingMainInfo.OutCairoFees = mainInfo.OutCairoFees;

                await _context.SaveChangesAsync();
                return Ok(existingMainInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        // DELETE: api/MainInfo/DeleteMainInfo
        [HttpDelete("DeleteMainInfo/{id}")]
        public async Task<IActionResult> DeleteMainInfo(int id)
        {
            var mainInfo = await _context.Maininfos.FirstOrDefaultAsync(x=>x.Id == id);
            if (mainInfo == null)
                return NotFound("MainInfo not found");

            _context.Maininfos.Remove(mainInfo);
            await _context.SaveChangesAsync();

            return Ok("MainInfo deleted successfully");
        }
    }
}
