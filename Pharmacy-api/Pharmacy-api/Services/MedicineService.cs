using Pharmacy_api.Models;
using Pharmacy_api.Repositories.Interfaces;
using Pharmacy_api.Services.Interfaces;

namespace Pharmacy_api.Services
{
    /// <summary>
    /// Implementation of IMedicineService that contains all business logic for medicine and sales operations.
    /// This service coordinates between controllers and repositories, enforcing all business rules.
    /// Applies Single Responsibility Principle (SRP) - only responsible for business logic.
    /// Applies Dependency Inversion Principle (DIP) - depends on repository interfaces, not concrete classes.
    /// </summary>
    public class MedicineService : IMedicineService
    {
        // Repository dependencies injected via constructor
        private readonly IMedicineRepository _medicineRepository;
        private readonly ISaleRepository _saleRepository;

        public MedicineService(IMedicineRepository medicineRepository, ISaleRepository saleRepository)
        {
            _medicineRepository = medicineRepository;
            _saleRepository = saleRepository;
        }

        /// <summary>
        /// Retrieves all medicines from the repository.
        /// </summary>
        /// <returns>List of all medicines in the system</returns>
        public async Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return await _medicineRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a specific medicine by ID from the repository.
        /// </summary>
        /// <param name="id">The medicine ID to retrieve</param>
        /// <returns>The medicine if found; null otherwise</returns>
        public async Task<Medicine?> GetMedicineByIdAsync(int id)
        {
            return await _medicineRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Adds a new medicine with validation checks.
        /// Validates that required fields are provided.
        /// </summary>
        /// <param name="medicine">The medicine object to add</param>
        /// <returns>The added medicine with auto-generated ID</returns>
        /// <exception cref="ArgumentException">Thrown if validation fails</exception>
        public async Task<Medicine> AddMedicineAsync(Medicine medicine)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(medicine.FullName))
                throw new ArgumentException("Medicine full name is required.");

            if (string.IsNullOrWhiteSpace(medicine.Brand))
                throw new ArgumentException("Medicine brand is required.");

            if (medicine.Price <= 0)
                throw new ArgumentException("Medicine price must be greater than zero.");

            if (medicine.Quantity < 0)
                throw new ArgumentException("Medicine quantity cannot be negative.");

            if (medicine.ExpiryDate < DateTime.Now.Date)
                throw new ArgumentException("Medicine expiry date cannot be in the past.");

            // Add medicine to repository (ID is auto-generated)
            var addedMedicine = await _medicineRepository.AddAsync(medicine);

            // Persist changes to JSON file
            await _medicineRepository.SaveAsync();

            return addedMedicine;
        }

        /// <summary>
        /// Records a sale of medicine with critical business logic validation.
        /// This method:
        /// 1. Verifies the medicine exists in the system
        /// 2. Checks that sufficient stock is available
        /// 3. Reduces the medicine quantity by the amount sold
        /// 4. Creates and records the sale transaction
        /// 5. Persists both the updated medicine and the sale record
        /// </summary>
        /// <param name="medicineId">The ID of the medicine being sold</param>
        /// <param name="quantitySold">The quantity being sold</param>
        /// <returns>The created SaleRecord with calculated total amount</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if:
        /// - The medicine does not exist
        /// - There is insufficient stock available
        /// </exception>
        public async Task<SaleRecord> RecordSaleAsync(int medicineId, int quantitySold)
        {
            // Step 1: Validate input quantity
            if (quantitySold <= 0)
                throw new ArgumentException("Quantity sold must be greater than zero.");

            // Step 2: Retrieve the medicine from repository
            var medicine = await _medicineRepository.GetByIdAsync(medicineId);

            // Step 3: Verify medicine exists
            if (medicine == null)
                throw new InvalidOperationException($"Medicine with ID {medicineId} not found.");

            // Step 4: Check if sufficient stock is available
            if (medicine.Quantity < quantitySold)
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {medicine.Quantity}, Requested: {quantitySold}");

            // Step 5: Reduce the medicine quantity
            medicine.Quantity -= quantitySold;

            // Step 6: Update medicine in repository
            await _medicineRepository.UpdateAsync(medicine);

            // Step 7: Create sale record with calculated total amount
            var saleRecord = new SaleRecord
            {
                MedicineId = medicineId,
                QuantitySold = quantitySold,
                SoldAt = DateTime.Now,
                // Calculate total amount: price per unit * quantity sold
                TotalAmount = medicine.Price * quantitySold
            };

            // Step 8: Add sale record to repository
            var recordedSale = await _saleRepository.AddAsync(saleRecord);

            // Step 9: Persist both changes to JSON files
            await _medicineRepository.SaveAsync();
            await _saleRepository.SaveAsync();

            return recordedSale;
        }
    }
}
