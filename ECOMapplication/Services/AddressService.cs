using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.AddressDTOs;
using ECOMapplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.Services
{
    public class AddressService
    {
        private readonly ApplicationDbContext _Dbcontext;
        public AddressService(ApplicationDbContext context)
        {
            _Dbcontext = context;
        }

        //Create Address
        public async Task<ApiResponse<AddressResponseDTO>> CreateAddress(AddressCreateDTO createDTO)
        {
            try
            {
                var customer = await _Dbcontext.Customers.FindAsync(createDTO.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<AddressResponseDTO>(404, "User Not found");
                }
                var address = new Address()
                {
                    AddressLine1 = createDTO.AddressLine1,
                    AddressLine2 = createDTO.AddressLine2,
                    City = createDTO.City,
                    State = createDTO.State,
                    Country = createDTO.Country,
                    PostalCode = createDTO.PostalCode,
                    CustomerId = createDTO.CustomerId
                };

                _Dbcontext.Addresses.Add(address);
                await _Dbcontext.SaveChangesAsync();

                //create response
                var responseAddress = new AddressResponseDTO()
                {
                    Id = address.AddressId,
                    CustomerId = address.CustomerId,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    PostalCode = address.PostalCode
                };
                return new ApiResponse<AddressResponseDTO>(200, responseAddress);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }


        public async Task<ApiResponse<AddressResponseDTO>> GetAddressByID(int id)
        {
            try
            {
                var address = await _Dbcontext.Addresses.FirstOrDefaultAsync(ad => ad.AddressId == id);
                if (address == null)
                    return new ApiResponse<AddressResponseDTO>(404, "Adress not found");

                //create response
                var responseAddress = new AddressResponseDTO()
                {
                    Id = address.AddressId,
                    CustomerId = address.CustomerId,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    PostalCode = address.PostalCode
                };
                return new ApiResponse<AddressResponseDTO>(200, responseAddress);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }


        //update Address
        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateAddress(AddressUpdateDTO updateDTO)
        {
            try
            {
                var address = await _Dbcontext.Addresses.FirstOrDefaultAsync(ad => ad.AddressId == updateDTO.AddressId &&
                ad.CustomerId == updateDTO.CustomerId);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(401, "Address Not found");

                address.AddressLine1 = updateDTO.AddressLine1;
                address.AddressLine2 = updateDTO.AddressLine2;
                address.City = updateDTO.City;
                address.State = updateDTO.State;
                address.Country = updateDTO.Country;
                address.PostalCode = updateDTO.PostalCode;

                //prepare a response
                var responseMessage = new ConfirmationResponseDTO()
                {
                    Message = "Address Updated successfully"
                };

                await _Dbcontext.SaveChangesAsync();
                return new ApiResponse<ConfirmationResponseDTO>(200, responseMessage);
            }

            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");

            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteAddress(AddressDeleteDTO deleteDTO)
        {
            try
            {
                var address = await _Dbcontext.Addresses.FirstOrDefaultAsync(ad => ad.AddressId == deleteDTO.AddressId &&
                ad.CustomerId == deleteDTO.CustomerId);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Address Not found");

                _Dbcontext.Addresses.Remove(address);
                await _Dbcontext.SaveChangesAsync();

                var responseMessage = new ConfirmationResponseDTO()
                {
                    Message = $"Address with ID: {deleteDTO.AddressId} deleted successfully"
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, responseMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<AddressResponseDTO>>> GetAddressByCustomerID(int customerID)
        {
            try
            {
                var customer = await _Dbcontext.Customers.AsNoTracking().Include(c => c.Adresses)
                    .FirstOrDefaultAsync(c => c.Id == customerID);
                if (customer == null)
                    return new ApiResponse<List<AddressResponseDTO>>(404, "User not found");

                if (customer.Adresses == null)
                {
                    return new ApiResponse<List<AddressResponseDTO>>(404, "The user does not have an address");
                }
                var addresses = customer.Adresses.Select(a => new AddressResponseDTO()
                {
                    Id = a.AddressId,
                    CustomerId = a.CustomerId,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    City = a.City,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    State = a.State
                }).ToList();

                return new ApiResponse<List<AddressResponseDTO>>(200, addresses);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AddressResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}
