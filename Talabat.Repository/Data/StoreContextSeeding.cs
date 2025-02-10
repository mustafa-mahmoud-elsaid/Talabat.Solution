using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeeding
    {
        public static async Task SeedAsync(StoreDbContext _dbContext)
        {
            if (_dbContext.ProductBrands.Count() == 0)
            {
                var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                if (brands?.Count > 0)
                {
                    foreach (var brand in brands)
                    {
                        await _dbContext.Set<ProductBrand>().AddAsync(brand);
                    }
                    await _dbContext.SaveChangesAsync();
                } 
            }
            if (_dbContext.ProductCategories.Count() == 0)
            {
                var categoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);
                if (categories?.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        await _dbContext.Set<ProductCategory>().AddAsync(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            if(_dbContext.Products.Count() == 0)
            {
                var productsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if(products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        await _dbContext.Products.AddAsync(product);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
