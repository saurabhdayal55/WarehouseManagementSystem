﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseManagement.Data.Models;

namespace WarehouseManagement.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
    }
}
