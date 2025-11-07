namespace ECOMapplication.DTOs.CartDTOs
{
    public class CartResponseDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal AmountSaved { get; set; }
        public List<CartItemResponseDTO>? CartItems { get; set; }
    }
}
