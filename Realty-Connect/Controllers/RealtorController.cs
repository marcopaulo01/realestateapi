using Microsoft.AspNetCore.Mvc;
using Realty_Connect.DTO;
using Realty_Connect.Services;

namespace Realty_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealtorController : ControllerBase
    {
        private readonly RealtorService _realtorService;

        public RealtorController(RealtorService realtorService)
        {
            _realtorService = realtorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRealtors()
        {
            var realtors = await _realtorService.GetAllRealtors();
            return Ok(realtors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRealtor(int id)
        {
            var realtor = await _realtorService.GetRealtorById(id);
            if (realtor == null)
            {
                return NotFound();
            }
            return Ok(realtor);
        }

        [HttpPost]
        public async Task<IActionResult> AddRealtor([FromBody] Realtor realtor)
        {
            await _realtorService.AddRealtor(realtor);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRealtor(int id, [FromBody] Realtor realtor)
        {
            if (id != realtor.Id)
            {
                return BadRequest();
            }
            await _realtorService.UpdateRealtor(realtor);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRealtor(int id)
        {
            await _realtorService.DeleteRealtor(id);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchRealtor(int id, [FromBody] Dictionary<string, object> updates)
        {
            var stringUpdates = updates.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? "");
            await _realtorService.PatchRealtor(id, stringUpdates);
            return Ok();
        }


    }
}
