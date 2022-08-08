using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant>()
                .HasKey(p => new { p.ProductId, p.ProductTypeId });


            modelBuilder.Entity<ProductType>().HasData(
                  new ProductType { Id = 1, Name = "Default" },
                  new ProductType { Id = 2, Name = "Paperback" },
                  new ProductType { Id = 3, Name = "E-Book" },
                  new ProductType { Id = 4, Name = "Audiobook" },
                  new ProductType { Id = 5, Name = "Stream" },
                  new ProductType { Id = 6, Name = "Blu-ray" },
                  new ProductType { Id = 7, Name = "VHS" },
                  new ProductType { Id = 8, Name = "PC" },
                  new ProductType { Id = 9, Name = "PlayStation" },
                  new ProductType { Id = 10, Name = "Xbox" }
              );

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Books",
                    Url = "books"
                },
                new Category
                {
                    Id = 2,
                    Name = "Movies",
                    Url = "movies"
                },
                new Category
                {
                    Id = 3,
                    Name = "Video Games",
                    Url = "video-games"
                }
                );
            modelBuilder.Entity<Product>().HasData(
          new Product
          {
              Id = 1,
              Title = "The Hitchhiker's Guide to the Galaxy",
              Description = "The Hitchhiker's Guide to the Galaxy [Note 1] (sometimes you have to travel the stars)",
              ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/bd/H2G2_UK_front_cover.jpg",
              CategoryId = 1,
              Featured = true
          },
         new Product
         {
             Id = 2,
             Title = "Ready Player One",
             Description = "Ready Player One is a 2011 science fiction novel, and the debut novel for the best player in America",
             ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a4/Ready_Player_One_cover.jpg",
             CategoryId = 1,
             Featured = true
         },
         new Product
         {
             Id = 3,
             Title = "Ninteen Eighty-Four",
             Description = "Ninteen Eighty-Four (also stylised as 1984) is a native store of USA background",
             ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/c/c3/1984first.jpg",
             CategoryId = 1
         }
         ); ;

            modelBuilder.Entity<ProductVariant>().HasData(
               new ProductVariant
               {
                   ProductId = 1,
                   ProductTypeId = 3,
                   Price = 9.99m,
                   OriginalPrice = 19.99m
               },
               new ProductVariant
               {
                   ProductId = 2,
                   ProductTypeId = 3,
                   Price = 7.99m
               },
               new ProductVariant
               {
                   ProductId = 3,
                   ProductTypeId = 3,
                   Price = 19.99m,
                   OriginalPrice = 29.99m
               }
           );
        }


        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductVariant> ProductVariant { get; set; }
    }




    }

