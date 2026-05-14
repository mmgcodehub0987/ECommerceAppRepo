using System.Text.Json;
using Pharmacy_api.Models;
using Pharmacy_api.Repositories.Interfaces;

namespace Pharmacy_api.Repositories.Json
{
    /// <summary>
    /// Implementation of ISaleRepository using JSON file storage.
    /// This class handles all file I/O operations for sale data persistence.
    /// Applies the Dependency Inversion Principle by implementing the ISaleRepository interface.
    /// </summary>
    public class SaleRepository : ISaleRepository
    {
        // Path to the JSON file where sale records are stored
        private readonly string _filePath;

        // In-memory cache of sale records to reduce file I/O operations
        private List<SaleRecord> _sales = new();

        public SaleRepository(IWebHostEnvironment environment)
        {
            // Construct the full path to sales.json in the Data folder
            _filePath = Path.Combine(environment.ContentRootPath, "Data", "sales.json");
        }

        /// <summary>
        /// Retrieves all recorded sales from the in-memory cache.
        /// If cache is empty, it loads data from the JSON file first.
        /// </summary>
        /// <returns>List of all sale records</returns>
        public async Task<List<SaleRecord>> GetAllAsync()
        {
            // If cache is empty, load from file
            if (_sales.Count == 0)
            {
                await LoadFromFileAsync();
            }

            return _sales;
        }

        /// <summary>
        /// Adds a new sale record to the cache and assigns an auto-generated ID.
        /// The ID is generated as the next sequential number based on existing sales.
        /// Note: Changes are not persisted until SaveAsync() is called.
        /// </summary>
        /// <param name="saleRecord">The sale record to add</param>
        /// <returns>The added sale record with the generated ID</returns>
        public async Task<SaleRecord> AddAsync(SaleRecord saleRecord)
        {
            // Ensure cache is loaded
            if (_sales.Count == 0)
            {
                await LoadFromFileAsync();
            }

            // Generate ID: set it to max existing ID + 1, or 1 if no sales exist
            saleRecord.Id = _sales.Any() ? _sales.Max(s => s.Id) + 1 : 1;

            // Add the sale record to the in-memory cache
            _sales.Add(saleRecord);

            return saleRecord;
        }

        /// <summary>
        /// Saves the current in-memory cache of sale records to the JSON file.
        /// Uses System.Text.Json for serialization with readable formatting.
        /// </summary>
        public async Task SaveAsync()
        {
            try
            {
                // Serialize sales list to JSON with indented formatting for readability
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(_sales, options);

                // Write the JSON content to file asynchronously
                await File.WriteAllTextAsync(_filePath, jsonContent);
            }
            catch (Exception ex)
            {
                // Log or handle file write errors
                throw new InvalidOperationException($"Error saving sales to file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads sale records from the JSON file into the in-memory cache.
        /// Creates an empty list if the file doesn't exist or is invalid.
        /// </summary>
        private async Task LoadFromFileAsync()
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(_filePath))
                {
                    // Initialize empty cache if file doesn't exist
                    _sales = new List<SaleRecord>();
                    return;
                }

                // Read the JSON file content asynchronously
                string jsonContent = await File.ReadAllTextAsync(_filePath);

                // Check if file is empty or contains invalid JSON
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _sales = new List<SaleRecord>();
                    return;
                }

                // Deserialize JSON content to SaleRecord list
                var deserializedSales = JsonSerializer.Deserialize<List<SaleRecord>>(jsonContent);

                // Set the cache to the deserialized list, or empty list if deserialization returns null
                _sales = deserializedSales ?? new List<SaleRecord>();
            }
            catch (Exception ex)
            {
                // Log or handle file read errors
                throw new InvalidOperationException($"Error loading sales from file: {ex.Message}", ex);
            }
        }
    }
}
