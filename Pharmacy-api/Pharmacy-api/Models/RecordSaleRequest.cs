namespace Pharmacy_api.Models
{
    /// <summary>
    /// Data Transfer Object (DTO) for recording a sale request.
    /// This DTO is used to accept sale data from the API client.
    /// It separates the API contract from the internal SaleRecord model.
    /// </summary>
    public class RecordSaleRequest
    {
        /// <summary>
        /// The unique identifier of the medicine being sold.
        /// </summary>
        public int MedicineId { get; set; }

        /// <summary>
        /// The quantity of the medicine being sold.
        /// Must be a positive integer.
        /// </summary>
        public int QuantitySold { get; set; }
    }
}
