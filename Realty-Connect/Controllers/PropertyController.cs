using Microsoft.AspNetCore.Mvc;
using Realty_Connect.DTO;
using Realty_Connect.Services;

namespace Realty_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyService _propertyService;
        public PropertyController(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllProperties();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyById(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty([FromBody] Property property)
        {
            await _propertyService.AddProperty(property);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] Property property)
        {
            if (id != property.Id)
            {
                return BadRequest();
            }
            await _propertyService.UpdateProperty(property);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            await _propertyService.DeleteProperty(id);
            return Ok();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProperty(int id, [FromBody] Dictionary<string, object> updates)
        {
            var stringUpdates = updates.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? "");
            await _propertyService.PatchProperty(id, stringUpdates);
            return Ok();
        }
    }
}
