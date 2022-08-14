using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _dataContext;
        public CategoryService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
        {
            category.Editing = category.IsNew = false;
            _dataContext.Categories.Add(category);
            await _dataContext.SaveChangesAsync();
            return await GetAdminCategories();
                 

        }

        public async Task<ServiceResponse<List<Category>>> DeleteCategory(int Id)
        {
           Category category =  await GetCategoryById(Id);
            if(category == null)
            {
                return new ServiceResponse<List<Category>>
                {

                    Success = false,
                    Message = "Category not Founf"
                };
            }

            category.Deleted = true;
            await _dataContext.SaveChangesAsync();

            return await GetAdminCategories();



        }

        private async Task<Category> GetCategoryById(int Id)
        {
            return await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
        {
            var categories = await _dataContext.Categories
                .Where(c => !c.Deleted)
                .ToListAsync();

            return new ServiceResponse<List<Category>>
            {
                Data = categories
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _dataContext.Categories
                .Where(c => !c.Deleted && c.Visible)
                .ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories
            };
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
        {
            var dbCategory =  await GetCategoryById(category.Id);

            if(dbCategory == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Categories not found."
                };
            }

            dbCategory.Name = category.Name;
            dbCategory.Url =  category.Url;
            dbCategory.Visible = category.Visible;

            await _dataContext.SaveChangesAsync();

            return await GetAdminCategories();

        }
    }
}
