using Pharmacy_api.Models;

namespace Pharmacy_api.Services.Interfaces
{
    /// <summary>
    /// Service interface for medicine business logic operations.
    /// This interface defines the contract for all business logic operations related to medicines and sales.
    /// Implementing the Interface Segregation Principle (ISP) and Single Responsibility Principle (SRP) from SOLID.
    /// </summary>
    public interface IMedicineService
    {
        /// <summary>
        /// Retrieves all medicines from the repository.
        /// </summary>
        /// <returns>A list of all medicines</returns>
        Task<List<Medicine>> GetAllMedicinesAsync();

        /// <summary>
        /// Retrieves a specific medicine by its unique identifier.
        /// </summary>
        /// <param name="id">The medicine ID</param>
        /// <returns>The medicine object if found; null otherwise</returns>
        Task<Medicine?> GetMedicineByIdAsync(int id);

        /// <summary>
        /// Adds a new medicine with validation.
        /// </summary>
        /// <param name="medicine">The medicine object to add</param>
        /// <returns>The added medicine with auto-generated ID</returns>
        Task<Medicine> AddMedicineAsync(Medicine medicine);

        /// <summary>
        /// Records a sale of medicine with business logic validation.
        /// This method ensures:
        /// 1. The medicine exists
        /// 2. Sufficient stock is available
        /// 3. The medicine quantity is reduced
        /// 4. The sale record is saved
        /// </summary>
        /// <param name="medicineId">The ID of the medicine being sold</param>
        /// <param name="quantitySold">The quantity being sold</param>
        /// <returns>The recorded sale object</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if medicine not found or insufficient stock available
        /// </exception>
        Task<SaleRecord> RecordSaleAsync(int medicineId, int quantitySold);
    }
}
