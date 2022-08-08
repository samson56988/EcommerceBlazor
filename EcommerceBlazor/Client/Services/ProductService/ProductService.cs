﻿namespace EcommerceBlazor.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public event Action ProductsChanged;

        public ProductService(HttpClient http)
        {
          _httpClient = http;
        }


        public string Message { get; set; } = "Loading Product........";
        //public async Task GetProducts()
        //{

        //    var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
        //    if(result != null && result.Data != null)
        //    Products = result.Data;
            
        //}

        public async Task<ServiceResponse<Product>> GetProductById(int productId)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;

        }

        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null ?
            await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
               await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
            if (result != null && result.Data != null)
                Products = result.Data;

            ProductsChanged.Invoke();
        }

        public async Task SearchProducts(string searchTxt)
        {
            var result =  await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchTxt}");
            if (result != null && result.Data != null)
                Products = result.Data;
            if (Products.Count == 0) Message = "No products found.";
            ProductsChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchTxt)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestion/{searchTxt}");
            return result.Data;
        }

        public List<Product> Products { get; set; } = new List<Product>();
       
    }
}
