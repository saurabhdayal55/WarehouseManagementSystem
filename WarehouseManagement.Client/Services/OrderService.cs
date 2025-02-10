using Newtonsoft.Json;
using System.Text;
using WarehouseManagement.Client.Models;

namespace WarehouseManagement.Client.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public OrderService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiBaseUrl"];
        }
        public async Task<List<Product>> GetFilterProduct(string searchTerm = "")
        {
            //var response = await _httpClient.GetFromJsonAsync<List<Product>>($"{_apiBaseUrl}/Product/Search?searchTerm={searchTerm}");
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Product/Search?searchTerm={searchTerm}");

            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        // Get all products
        public async Task<List<Product>> GetAllProducts()
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Product");
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        // Get all orders
        public async Task<List<Order>> GetAllOrders()
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Order");
            return JsonConvert.DeserializeObject<List<Order>>(response);
        }
        // Get order by ID
        public async Task<Order> GetOrderById(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Order/{id}");
            return JsonConvert.DeserializeObject<Order>(response);
        }
        public async Task<bool> CreateOrder(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}Order", order);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateOrder(Order order)
        {
            if (order == null || order.OrderItems == null || !order.OrderItems.Any())
            {
                Console.WriteLine("Invalid order data: Order or OrderItems is null.");
                return false;
            }

            var orderUpdateDto = new Order
            {
                OrderID = order.OrderID,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                DeliveryOptionID = order.DeliveryOptionID,
                OrderDate = order.OrderDate,
                ProductIDs = order.OrderItems.Select(p => p.ProductID).ToList(),
                Quantities = order.OrderItems.Select(p => p.Quantity).ToList()
            };

            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}Order/{order.OrderID}", orderUpdateDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {errorMessage}");
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<List<DeliveryOption>> GetAllDeliveryOptions()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}Order/GetDeliveryOptions");
            if (!response.IsSuccessStatusCode)
            {
                return new List<DeliveryOption>();
            }
            return await response.Content.ReadFromJsonAsync<List<DeliveryOption>>();
        }


        public async Task<bool> DeleteOrder(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}Order/{id}");
            return response.IsSuccessStatusCode;
        }
        // Create a new product
        public async Task<bool> CreateProduct(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}Product", product);
            return response.IsSuccessStatusCode;
        }




        // Fulfill an order
        public async Task<bool> FulfillOrder(int orderId, DateTime fulfillmentDate)
        {
            var requestData = new
            {
                OrderId = orderId,
                FulfillmentDate = fulfillmentDate
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}Order/fulfillOrder", jsonContent);

            return response.IsSuccessStatusCode;
        }


        public async Task<List<ProductType>> GetProductTypes()
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Product/producttypes");
            return JsonConvert.DeserializeObject<List<ProductType>>(response);
        }
        public async Task<bool> UpdateProduct(Product product)
        { 
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}Product/{product.ProductID}", product); 
            return response.IsSuccessStatusCode;
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}Product/{id}");
            return JsonConvert.DeserializeObject<Product>(response);
        }
        public async Task<bool> DeleteProduct(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}Product/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
