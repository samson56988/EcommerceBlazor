﻿using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
          new Product
            {
                Id = 1,
                Title = "The Hitchhiker's Guide to the Galaxy",
                Description = "The Hitchhiker's Guide to the Galaxy [Note 1] (sometimes you have to travel the stars)",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/bd/H2G2_UK_front_cover.jpg",
                Price = 9.99m
            },
         new Product
         {
             Id = 2,
             Title = "Ready Player One",
             Description = "Ready Player One is a 2011 science fiction novel, and the debut novel for the best player in America",
             ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a4/Ready_Player_One_cover.jpg",
             Price = 7.99m
         },
         new Product
         {
             Id = 3,
             Title = "Ninteen Eighty-Four",
             Description = "Ninteen Eighty-Four (also stylised as 1984) is a native store of USA background",
             ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/c/c3/1984first.jpg",
             Price = 9.99m
         }
         );
        }

        public DbSet<Product> Products { get; set; }
    }
}
