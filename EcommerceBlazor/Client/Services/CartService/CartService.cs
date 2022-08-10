using Blazored.LocalStorage;

namespace EcommerceBlazor.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;

        public CartService(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorageService = localStorage;
            _httpClient = http;
        }
        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                cart = new List<CartItem>();
            }

            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
            x.ProductTypeID == cartItem.ProductTypeID);
            if(sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorageService.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItem()
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;

        }

        public async Task<List<CartProductResponse>> GetCartProduct()
        {
            var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

            var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
            var cartproducts =
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartproducts.Data;

        }

        public async Task RemoveProductFromCart(int ProductId, int productTypeId)
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == ProductId
            && x.ProductTypeID == productTypeId);
            if(cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorageService.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
            
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == product.ProductId
            && x.ProductTypeID == product.ProductTypeId);
            if( cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorageService.SetItemAsync("cart", cart);
            }
        }
    }
}
