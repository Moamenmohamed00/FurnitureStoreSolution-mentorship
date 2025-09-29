using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Get: api/address/user/{userId}
        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetUserAddresses(string userId)
        //{
        //    var addresses = await _addressService.GetUserAddressesAsync(userId);
        //    return Ok(addresses);
        //}

        // Get: api/address/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null) return NotFound();
            return Ok(address);
        }

        // Post: api/address
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto createAddressDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdAddress = await _addressService.CreateAddressAsync(createAddressDto);
            if (createdAddress == null) return BadRequest("Failed to create address");

            return Ok("Address created successfully");
        }

        // Put: api/address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] CreateAddressDto updateAddressDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _addressService.UpdateAddressAsync(id, updateAddressDto);
            if (!updated) return NotFound("Address not found");

            return Ok("Address updated successfully");
        }

        // Delete: api/address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _addressService.DeleteAddressAsync(id);
            if (!deleted) return NotFound("Address not found");

            return Ok("Address deleted successfully");
        }
    }
}

