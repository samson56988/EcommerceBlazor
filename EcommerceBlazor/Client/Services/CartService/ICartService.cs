using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;

        Task AddToCart(CartItem cartItem);
        //Task<List<CartItem>> GetCartItem();
        Task<List<CartProductResponse>> GetCartProduct();
        Task RemoveProductFromCart(int ProductId, int productTypeId);
        Task UpdateQuantity(CartProductResponse product);
        Task StoreCartItems(bool EmptyLocalCart);

        Task GetItemsCount();
    }
}
