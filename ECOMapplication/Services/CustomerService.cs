using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.CustomerDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace ECOMapplication.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _dbContext;

        //DI
        public CustomerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<RegistrationResponseDTO>> RegisterAsync(RegistrationDTO CustomerDto)
        {
            try
            {
                if (await _dbContext.Customers.AnyAsync(c => c.Email.ToLower() == CustomerDto.Email.ToLower()))
                    return new ApiResponse<RegistrationResponseDTO>(400, "Email already in use.");

                //if not exists -> manual mapping dto to model
                var Customer = new Customer()
                {
                    FirstName = CustomerDto.FirstName,
                    LastName = CustomerDto.LastName,
                    Email = CustomerDto.Email,
                    PhoneNumber = CustomerDto.PhoneNumber,
                    DateOfBirth = CustomerDto.DateOfBirth,
                    IsActive = true,
                    Password = BCrypt.Net.BCrypt.HashPassword(CustomerDto.Password)
                };

                //adding this new customer to Database
                _dbContext.Customers.Add(Customer);
                await _dbContext.SaveChangesAsync();

                //Response data need to be created: use DTO 
                var CustomerResponse = new RegistrationResponseDTO()
                {
                    Id = Customer.Id,
                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    Email = Customer.Email,
                    PhoneNumber = Customer.PhoneNumber,
                    DateOfBirth = Customer.DateOfBirth
                };
                return new ApiResponse<RegistrationResponseDTO>(200, CustomerResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistrationResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var customer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Email == loginDto.Email);
                if (customer == null)
                    return new ApiResponse<LoginResponseDTO>(401, "Invalid Email or Password");
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.Password);
                if (!isPasswordValid)
                    return new ApiResponse<LoginResponseDTO>(401, "Invalid Email or Password");

                //prepare login Response data
                var loginResponse = new LoginResponseDTO()
                {
                    CustomerId = customer.Id,
                    Message = "Login successfull",
                    CustomerName = string.Concat(customer.FirstName, " ", customer.LastName),
                };
                return new ApiResponse<LoginResponseDTO>(200, loginResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistrationResponseDTO>> GetCustomerByIDAsync(int id)
        {
            try
            {
                var customer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync( c=> c.Id == id);
                if (customer == null)
                    return new ApiResponse<RegistrationResponseDTO>(404, "Customer Not found");

                //create Response
                var CustomerResponse = new RegistrationResponseDTO()
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    DateOfBirth = customer.DateOfBirth
                };
                return new ApiResponse<RegistrationResponseDTO>(200, CustomerResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistrationResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }
        }


        //update
        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCustomerAsync(CustomerUpdateDTO updateDTO)
        {
            try
            {
                var customer = await _dbContext.Customers.FindAsync(updateDTO.CustomerId);
                if (customer == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Customer not found");

                if (customer.Email.ToLower() != updateDTO.Email.ToLower() && await _dbContext.Customers.AnyAsync(c => c.Email.ToLower() == updateDTO.Email.ToLower()))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Email already in use");

                customer.FirstName = updateDTO.FirstName;
                customer.LastName = updateDTO.LastName;
                customer.Email = updateDTO.Email;
                customer.PhoneNumber = updateDTO.PhoneNumber;
                customer.DateOfBirth = updateDTO.DateOfBirth;

                await _dbContext.SaveChangesAsync();

                //prepare response
                var confirmationResponse = new ConfirmationResponseDTO()
                {
                    Message = $"Customer with Id {updateDTO.CustomerId} updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }
        }

        //delete
        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
                if (customer == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "User not found.");

                //soft delete
                customer.IsActive = false;
                await _dbContext.SaveChangesAsync();

                //create response
                var deleteResponse = new ConfirmationResponseDTO()
                {
                    Message = $"User with user id: {id} deleted successfully"
                };
                return new ApiResponse<ConfirmationResponseDTO>(400, deleteResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }

        }
        //Change password
        public async Task<ApiResponse<ConfirmationResponseDTO>> ChangePassword(ChangePasswordDTO passwordDTO)
        {
            try
            {
                var customer = await _dbContext.Customers.FindAsync(passwordDTO.CustomerId);
                if (customer == null || !customer.IsActive)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "User Not found");

                if (!BCrypt.Net.BCrypt.Verify(passwordDTO.CurrentPassword, customer.Password))
                    return new ApiResponse<ConfirmationResponseDTO>(401, "Current Password is incorrect");

                //hash the new password
                customer.Password = BCrypt.Net.BCrypt.HashPassword(passwordDTO.NewPassword);
                await _dbContext.SaveChangesAsync();

                var confirmationMessage = new ConfirmationResponseDTO()
                {
                    Message = "Password Changes Successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);

            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occured while processing your request. Error Details: {ex.Message}");
            }
        }
    }
}
