using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.CartDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _DbContext;

        public CartService(ApplicationDbContext context)
        {
            _DbContext = context;
        }


        public async Task<ApiResponse<CartResponseDTO>> AddToCart(AddToCartDTO addToCartDTO)
        {
            try
            {
                //retireve an active cart for the customer
                var activeCart = await _DbContext.Carts.Include(ci => ci.CartItems).ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.CustomerId == addToCartDTO.ProductId && !c.IsCheckedOut);

                if(activeCart == null)
                {
                    var newCart = new Cart()
                    {
                        CustomerId = addToCartDTO.CustomerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };
                    _DbContext.Carts.Add(newCart);
                    await _DbContext.SaveChangesAsync();
                }
                //product to be added to cart. Fetch it
                var product = await _DbContext.Products.FindAsync(addToCartDTO.ProductId);
                if(product == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Product Not Found");
                }
                if(addToCartDTO.Quantity> product.StockQuantity)
                {
                    return new ApiResponse<CartResponseDTO>(400, $"Only {product.StockQuantity} units of {product.ProductName} are left");
                }

                //Is product im trying to add already in the cart? if yes add to its quantity.
                var existingCartItem = activeCart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDTO.ProductId);
                if(existingCartItem != null )
                {
                    if (existingCartItem.Quantity + addToCartDTO.Quantity > product.StockQuantity)
                        return new ApiResponse<CartResponseDTO>(400, $"Quantity exceeds availables stock");
                    existingCartItem.Quantity += addToCartDTO.Quantity;
                    existingCartItem.TotalPrice = (existingCartItem.UnitPrice - existingCartItem.Discount) * existingCartItem.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;

                    _DbContext.CartItems.Update(existingCartItem);
                }
                else
                {
                    var discount = product.DiscountPercentage > 0 ? product.Price * product.DiscountPercentage / 100 : 0;
                    var cartItem = new CartItem
                    {
                        CartId = activeCart.Id,
                        ProductId = product.Id,
                        Quantity = addToCartDTO.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = (product.Price - discount) * addToCartDTO.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _DbContext.CartItems.Add(cartItem);
                }
                activeCart.UpdatedAt = DateTime.UtcNow;
                _DbContext.Carts.Update(activeCart);
                await _DbContext.SaveChangesAsync();

                activeCart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == activeCart.Id) ?? new Cart();
                // Map the cart entity to the DTO, which includes price calculations.
                var cartDTO = MapCartToDTO(activeCart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> GetCartByCustomerId(int customerId)
        {
            try
            {
                var cart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsCheckedOut);

                // If there is no active cart found, create an empty DTO with default values.
                if (cart == null)
                {
                    var emptyCartDTO = new CartResponseDTO
                    {
                        CustomerId = customerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItemResponseDTO>(),
                        TotalBasePrice = 0,
                        TotalDiscount = 0,
                        TotalAmount = 0
                    };
                    return new ApiResponse<CartResponseDTO>(200, emptyCartDTO);
                }

                // Map the cart entity to its corresponding DTO (includes price calculations).
                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> UpdateCartItem(UpdateCartItemDTO updateCartItemDTO)
        {
            try
            {
                // Retrieve the active cart for the customer along with cart items and product details.
                var cart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == updateCartItemDTO.CustomerId && !c.IsCheckedOut);

                if (cart == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Active cart not found.");
                }
  
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == updateCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Cart item not found.");
                }

                // Ensure the updated quantity does not exceed the available product stock.
                if (updateCartItemDTO.Quantity > cartItem.Product.StockQuantity)
                {
                    return new ApiResponse<CartResponseDTO>(400, $"Only {cartItem.Product.StockQuantity} units of {cartItem.Product.ProductName} are available.");
                }
                // Update the cart item's quantity and recalculate its total price.
                cartItem.Quantity = updateCartItemDTO.Quantity;
                cartItem.TotalPrice = (cartItem.UnitPrice - cartItem.Discount) * cartItem.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                // Mark the cart item as updated.
                _DbContext.CartItems.Update(cartItem);

                // Update the cart's updated timestamp.
                cart.UpdatedAt = DateTime.UtcNow;
                _DbContext.Carts.Update(cart);
                await _DbContext.SaveChangesAsync();

                // Reload the updated cart with its items.
                cart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id) ?? new Cart();

                // Map the updated cart to the DTO.
                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> RemoveCartItem(RemoveCartItemDTO removeCartItemDTO)
        {
            try
            {
                // Retrieve the active cart along with its items and product details.
                var cart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == removeCartItemDTO.CustomerId && !c.IsCheckedOut);
                if (cart == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Active cart not found.");
                }

                // Find the cart item to be removed.
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == removeCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Cart item not found.");
                }

                _DbContext.CartItems.Remove(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;
                await _DbContext.SaveChangesAsync();

                // Reload the updated cart after removal.
                cart = await _DbContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id) ?? new Cart();
                // Map the updated cart to the DTO.
                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                // Return error response if an exception occurs.
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> ClearCart(int customerId)
        {
            try
            {
                // Retrieve the active cart along with its items.
                var cart = await _DbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsCheckedOut);
                // Return 404 if no active cart is found.
                if (cart == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Active cart not found.");
                }
                // If there are any items in the cart, remove them.
                if (cart.CartItems.Any())
                {
                    _DbContext.CartItems.RemoveRange(cart.CartItems);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _DbContext.SaveChangesAsync();
                }
                // Create a confirmation response DTO.
                var confirmation = new ConfirmationResponseDTO
                {
                    Message = "Cart has been cleared successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                // Return error response if an exception occurs.
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private CartResponseDTO MapCartToDTO(Cart cart)
        {
            // Map each CartItem entity to its corresponding CartItemResponseDTO.
            var cartItemsDto = cart.CartItems?.Select(ci => new CartItemResponseDTO
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.ProductName,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                Discount = ci.Discount,
                TotalPrice = ci.TotalPrice
            }).ToList() ?? new List<CartItemResponseDTO>();
            // Initialize totals for base price, discount, and amount after discount.
            decimal totalBasePrice = 0;
            decimal totalDiscount = 0;
            decimal totalAmount = 0;
            // Iterate through each cart item DTO to accumulate the totals.
            foreach (var item in cartItemsDto)
            {
                totalBasePrice += item.UnitPrice * item.Quantity;
                totalAmount += item.TotalPrice;
            }
            // Create and return the final CartResponseDTO with all details and calculated totals.
            return new CartResponseDTO
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                IsCheckedOut = cart.IsCheckedOut,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                CartItems = cartItemsDto,
                TotalBasePrice = totalBasePrice,
                TotalDiscount = totalAmount - totalBasePrice,
                TotalAmount = totalAmount
            };
        }
    }
}
