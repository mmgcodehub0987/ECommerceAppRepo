using Pharmacy_api.Models;

namespace Pharmacy_api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Sale data persistence operations.
    /// This interface defines the contract for all sale-related data access operations.
    /// Implementing the Repository Pattern and Interface Segregation Principle (ISP) from SOLID.
    /// </summary>
    public interface ISaleRepository
    {
        /// <summary>
        /// Retrieves all recorded sales from storage.
        /// </summary>
        /// <returns>A list of all sale records</returns>
        Task<List<SaleRecord>> GetAllAsync();

        /// <summary>
        /// Adds a new sale record to storage.
        /// </summary>
        /// <param name="saleRecord">The sale record to add</param>
        /// <returns>The added sale record with auto-generated ID</returns>
        Task<SaleRecord> AddAsync(SaleRecord saleRecord);

        /// <summary>
        /// Saves all sale records to the persistent storage (JSON file).
        /// </summary>
        Task SaveAsync();
    }
}
