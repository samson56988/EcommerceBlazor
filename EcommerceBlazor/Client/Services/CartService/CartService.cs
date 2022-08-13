using Blazored.LocalStorage;
using EcommerceBlazor.Client.Services.AuthService;

namespace EcommerceBlazor.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationState;
        private readonly IAuthService _authService;

        public CartService(ILocalStorageService localStorage, HttpClient http, IAuthService authService)
        {
            _localStorageService = localStorage;
            _httpClient = http;
            _authService = authService;
        }
        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _httpClient.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
                x.ProductTypeID == cartItem.ProductTypeID);
                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

                await _localStorageService.SetItemAsync("cart", cart);
            }
           
            await GetItemsCount();
        }

        public Task<List<CartItem>> GetCartItem()
        {
            throw new NotImplementedException();
        }



        //public async Task<List<CartItem>> GetCartItem()
        //{
        //    await GetItemsCount();
        //    var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
        //    if(cart == null)
        //    {
        //        cart = new List<CartItem>();
        //    }

        //    return cart;

        //}

        public async Task<List<CartProductResponse>> GetCartProduct()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if(cartItems == null)
                    return new List<CartProductResponse>(); 
                var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
                var cartproducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartproducts.Data;
            }
           

        }

        public async Task GetItemsCount()
        {
             if(await _authService.IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorageService.SetItemAsync<int>("cartItemsCount", count);
            }
             else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                await _localStorageService.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }

        public async Task RemoveProductFromCart(int ProductId, int productTypeId)
        {

            if(await _authService.IsUserAuthenticated())
            {
                await _httpClient.DeleteAsync($"api/cart/{ProductId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == ProductId
                && x.ProductTypeID == productTypeId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorageService.SetItemAsync("cart", cart);
                    
                }
            }
           
            
        }

        public async Task StoreCartItems(bool EmptyLocalCart = false)
        {
            var localCart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(localCart == null)
            {
                return;
            }

            await _httpClient.PostAsJsonAsync("api/cart",localCart);

            if(EmptyLocalCart)
            {
                await _localStorageService.RemoveItemAsync("cart");
            }
        }


        public async Task UpdateQuantity(CartProductResponse product)
        {

            if(await _authService.IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeID = product.ProductTypeId
                };

                await _httpClient.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId
                && x.ProductTypeID == product.ProductTypeId);
                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorageService.SetItemAsync("cart", cart);
                }
            }
            
        }

       
    }
}
