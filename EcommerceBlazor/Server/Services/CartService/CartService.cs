using EcommerceBlazor.Server.Services.AuthService;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceBlazor.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;
        
        public CartService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;         
        }

        
        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>()
            };

            foreach(var item in cartItems)
            {
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();
                if(product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariant
                    .Where(v => v.ProductId == item.ProductId
                    && v.ProductTypeId == item.ProductTypeID)
                    .Include(v => v.productType)
                    .FirstOrDefaultAsync();

                if(productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    Price = productVariant.Price,
                    ProductType = productVariant.productType.Name,
                    ImageUrl = product.ImageUrl,
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity
                     
                };

                result.Data.Add(cartProduct);
            }

            return result; 
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
        {
            cartItems.ForEach(cartItems => cartItems.UserId =_authService.GetUserID());

            _context.CartItems.AddRange(cartItems);

            await _context.SaveChangesAsync();

            return await GetDbCartProducts();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = (await _context.CartItems.Where(ci => ci.UserId == _authService.GetUserID()).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
        {
            return await GetCartProducts(await _context.CartItems
                .Where(ci => ci.UserId == _authService.GetUserID()).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
        {
            cartItem.UserId = _authService.GetUserID();

            var sameItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId
                && ci.ProductTypeID == cartItem.ProductTypeID && ci.UserId == cartItem.UserId);
            if(sameItem == null)
            {
                _context.CartItems.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };

        }

        public async Task<ServiceResponse<bool>> UpdateQuntity(CartItem cartItem)
        {
            var dbcartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId
                && ci.ProductTypeID == cartItem.ProductTypeID && ci.UserId == _authService.GetUserID());
            if(dbcartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist"
                };
            }

            dbcartItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var dbcartItem = await _context.CartItems
               .FirstOrDefaultAsync(ci => ci.ProductId == productId
               && ci.ProductTypeID == productTypeId && ci.UserId == _authService.GetUserID());
            if (dbcartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist"
                };
            }

            _context.CartItems.Remove(dbcartItem);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data= true };
        }
    }
}
