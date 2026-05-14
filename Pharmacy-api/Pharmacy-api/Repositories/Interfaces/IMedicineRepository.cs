using Pharmacy_api.Models;

namespace Pharmacy_api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Medicine data persistence operations.
    /// This interface defines the contract for all medicine-related data access operations.
    /// Implementing the Repository Pattern and Interface Segregation Principle (ISP) from SOLID.
    /// </summary>
    public interface IMedicineRepository
    {
        /// <summary>
        /// Retrieves all medicines from storage.
        /// </summary>
        /// <returns>A list of all medicines</returns>
        Task<List<Medicine>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific medicine by its unique identifier.
        /// </summary>
        /// <param name="id">The medicine ID</param>
        /// <returns>The medicine object if found; null otherwise</returns>
        Task<Medicine?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new medicine to storage.
        /// </summary>
        /// <param name="medicine">The medicine object to add</param>
        /// <returns>The added medicine with auto-generated ID</returns>
        Task<Medicine> AddAsync(Medicine medicine);

        /// <summary>
        /// Updates an existing medicine in storage.
        /// </summary>
        /// <param name="medicine">The medicine object with updated values</param>
        /// <returns>The updated medicine</returns>
        Task<Medicine> UpdateAsync(Medicine medicine);

        /// <summary>
        /// Saves all medicines to the persistent storage (JSON file).
        /// </summary>
        Task SaveAsync();
    }
}
