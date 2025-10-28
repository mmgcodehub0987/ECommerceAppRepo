using ECOMapplication.DTO;
using ECOMapplication.DTOs.CustomerDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _CustomerService;
        public CustomerController(CustomerService customerService)
        {
            _CustomerService = customerService;
        }

        //Register a new customer
        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<ApiResponse<RegistrationResponseDTO>>> RegisterCustomerAsync([FromBody] RegistrationDTO registrationDTO)
        {
            var response = await _CustomerService.RegisterAsync(registrationDTO);
            if (response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        //Login a customer
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> LoginCustomer([FromBody] LoginDTO loginDto)
        {
            var response = await _CustomerService.LoginAsync(loginDto);
            if(response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationResponseDTO>>> GetCustomerById(int id)
        {
            var response = await _CustomerService.GetCustomerByIDAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }


        // Update.
        [HttpPut("UpdateCustomer")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateCustomer([FromBody] CustomerUpdateDTO customerDto)
        {
            var response = await _CustomerService.UpdateCustomerAsync(customerDto);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteCustomer(int id)
        {
            var response = await _CustomerService.DeleteCustomer(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
        // Changes the password for an existing customer.
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var response = await _CustomerService.ChangePassword(changePasswordDto);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}