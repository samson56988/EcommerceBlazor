using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        public ProductService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data =  await _dataContext.Products
                .Where(p => p.Featured)
                .Include(p => p.variants)
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            var product = await _dataContext.Products
                .Include(p => p.variants)
                .ThenInclude(v => v.productType)
                .FirstOrDefaultAsync(p => p.Id == productId);
            if(product == null)
            {
                response.Success = false;
                response.Message = "Sorry but this product does not exist";
            }
            else
            {
                response.Success = true;
                response.Data = product;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dataContext.Products
                .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                .Include(p => p.variants)
                .ToListAsync()  
            };

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dataContext.Products.Include(p => p.variants).ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductBySearchText(searchText);

            List<string> result = new List<string>();

            foreach(var product in products)
            {
                if(product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();

                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuation));

                    foreach(var word in words)
                    {
                        if(word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            && !result.Contains(word))
                        {
                            result.Add(word);

                        }

                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {
            var pageresult = 2f;
            var pagecount = Math.Ceiling((await FindProductBySearchText(searchText)).Count / pageresult);
            var products = await _dataContext.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                            ||
                            p.Description.ToLower().Contains(searchText.ToLower()))
                            .Include(p => p.variants)
                            .Skip((page - 1) * (int)pageresult)
                            .Take((int)pageresult)
                            .ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pagecount
                }

            };

            return response;
        }

        private async Task<List<Product>> FindProductBySearchText(string searchText)
        {
            return await _dataContext.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                            ||
                            p.Description.ToLower().Contains(searchText.ToLower()))
                            .Include(p => p.variants)
                            .ToListAsync();
        }
    }
}
