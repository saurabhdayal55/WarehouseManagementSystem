using WarehouseManagement.Admin.Models;

namespace WarehouseManagement.Admin.Services
{
    public interface IApiService
    {
        Task<List<Product>> GetAllProducts(); 
        Task<bool> CreateProduct(Product product); 

        // Order API Methods
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(int id);
        Task<bool> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> DeleteOrder(int id);
        Task<List<ProductType>> GetProductTypes(); // Fetch product types
        Task<List<Product>> GetFilterProduct(string searchTerm); // Fetch product types
        Task<Product> GetProductById(int id);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);

        Task<bool> FulfillOrder(int id, DateTime fulfillmentDate);
    }



}
