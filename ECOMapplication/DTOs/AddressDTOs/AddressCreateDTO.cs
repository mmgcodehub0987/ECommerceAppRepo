﻿using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.DTOs.AddressDTOs
{
    public class AddressCreateDTO
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }


        [Required(ErrorMessage = "Address Line 1 is required.")]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot exceed 100 characters.")]
        public string AddressLine1 { get; set; } = string.Empty;


        [Required(ErrorMessage = "Address Line 2 is required.")]
        [StringLength(100, ErrorMessage = "Address Line 2 cannot exceed 100 characters.")]
        public string AddressLine2 { get; set; } = string.Empty;


        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty;


        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        public string State { get; set; } = string.Empty;


        [Required(ErrorMessage = "Postal Code is required.")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Postal Code.")]
        public string PostalCode { get; set; } = string.Empty;


        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; } = string.Empty;
    }
}
