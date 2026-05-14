using System.Text.Json;
using Pharmacy_api.Models;
using Pharmacy_api.Repositories.Interfaces;

namespace Pharmacy_api.Repositories.Json
{
    /// <summary>
    /// Implementation of IMedicineRepository using JSON file storage.
    /// This class handles all file I/O operations for medicine data persistence.
    /// Applies the Dependency Inversion Principle by implementing the IMedicineRepository interface.
    /// </summary>
    public class MedicineRepository : IMedicineRepository
    {
        // Path to the JSON file where medicines are stored
        private readonly string _filePath;

        // In-memory cache of medicines to reduce file I/O operations
        private List<Medicine> _medicines = new();

        public MedicineRepository(IWebHostEnvironment environment)
        {
            // Construct the full path to medicines.json in the Data folder
            _filePath = Path.Combine(environment.ContentRootPath, "Data", "medicines.json");
        }

        /// <summary>
        /// Retrieves all medicines from the in-memory cache.
        /// If cache is empty, it loads data from the JSON file first.
        /// </summary>
        /// <returns>List of all medicines</returns>
        public async Task<List<Medicine>> GetAllAsync()
        {
            // If cache is empty, load from file
            if (_medicines.Count == 0)
            {
                await LoadFromFileAsync();
            }

            return _medicines;
        }

        /// <summary>
        /// Retrieves a specific medicine by ID from the cache.
        /// Returns null if the medicine is not found.
        /// </summary>
        /// <param name="id">The medicine ID to search for</param>
        /// <returns>The medicine if found; null otherwise</returns>
        public async Task<Medicine?> GetByIdAsync(int id)
        {
            // Ensure cache is loaded
            if (_medicines.Count == 0)
            {
                await LoadFromFileAsync();
            }

            // Search for medicine by ID using LINQ
            return _medicines.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Adds a new medicine to the cache and assigns an auto-generated ID.
        /// The ID is generated as the next sequential number based on existing medicines.
        /// Note: Changes are not persisted until SaveAsync() is called.
        /// </summary>
        /// <param name="medicine">The medicine object to add</param>
        /// <returns>The added medicine with the generated ID</returns>
        public async Task<Medicine> AddAsync(Medicine medicine)
        {
            // Ensure cache is loaded
            if (_medicines.Count == 0)
            {
                await LoadFromFileAsync();
            }

            // Generate ID: set it to max existing ID + 1, or 1 if no medicines exist
            medicine.Id = _medicines.Any() ? _medicines.Max(m => m.Id) + 1 : 1;

            // Add the medicine to the in-memory cache
            _medicines.Add(medicine);

            return medicine;
        }

        /// <summary>
        /// Updates an existing medicine in the cache.
        /// The medicine to update is identified by its ID.
        /// Note: Changes are not persisted until SaveAsync() is called.
        /// </summary>
        /// <param name="medicine">The medicine object with updated values</param>
        /// <returns>The updated medicine</returns>
        public async Task<Medicine> UpdateAsync(Medicine medicine)
        {
            // Ensure cache is loaded
            if (_medicines.Count == 0)
            {
                await LoadFromFileAsync();
            }

            // Find the index of the medicine with the matching ID
            int index = _medicines.FindIndex(m => m.Id == medicine.Id);

            // If medicine exists, replace it; otherwise, add it
            if (index >= 0)
            {
                _medicines[index] = medicine;
            }
            else
            {
                _medicines.Add(medicine);
            }

            return medicine;
        }

        /// <summary>
        /// Saves the current in-memory cache of medicines to the JSON file.
        /// Uses System.Text.Json for serialization with readable formatting.
        /// </summary>
        public async Task SaveAsync()
        {
            try
            {
                // Serialize medicines list to JSON with indented formatting for readability
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(_medicines, options);

                // Write the JSON content to file asynchronously
                await File.WriteAllTextAsync(_filePath, jsonContent);
            }
            catch (Exception ex)
            {
                // Log or handle file write errors
                throw new InvalidOperationException($"Error saving medicines to file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads medicines from the JSON file into the in-memory cache.
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
                    _medicines = new List<Medicine>();
                    return;
                }

                // Read the JSON file content asynchronously
                string jsonContent = await File.ReadAllTextAsync(_filePath);

                // Check if file is empty or contains invalid JSON
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _medicines = new List<Medicine>();
                    return;
                }

                // Deserialize JSON content to Medicine list
                var deserializedMedicines = JsonSerializer.Deserialize<List<Medicine>>(jsonContent);

                // Set the cache to the deserialized list, or empty list if deserialization returns null
                _medicines = deserializedMedicines ?? new List<Medicine>();
            }
            catch (Exception ex)
            {
                // Log or handle file read errors
                throw new InvalidOperationException($"Error loading medicines from file: {ex.Message}", ex);
            }
        }
    }
}
