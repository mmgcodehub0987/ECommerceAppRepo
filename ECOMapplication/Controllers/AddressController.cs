using ECOMapplication.DTO;
using ECOMapplication.DTOs.AddressDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;
        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("CreateAddress")]
        public async Task<ActionResult<ApiResponse<AddressResponseDTO>>> CreateAddress([FromBody]AddressCreateDTO aadressCreateDTO)
        {
            var response = await _addressService.CreateAddress(aadressCreateDTO);
            if (response.StatusCode !=200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetAddressById/{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponseDTO>>> GetAddressByID(int AddressId)
        {
            var response = await _addressService.GetAddressByID(AddressId);
            if(response.StatusCode !=200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateAddress([FromBody]AddressUpdateDTO addressUpdateDTO)
        {
            var response = await _addressService.UpdateAddress(addressUpdateDTO);
            if(response.StatusCode !=200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);

        }

        [HttpDelete("DeleteAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteAddress([FromBody] AddressDeleteDTO addressDeleteDTO)
        {
            var response = await _addressService.DeleteAddress(addressDeleteDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetAddressByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> GetAddressByCustomerId(int CustomerId)
        {
            var response = await _addressService.GetAddressByCustomerID(CustomerId);
            if( response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
