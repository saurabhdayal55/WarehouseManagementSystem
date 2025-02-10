using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WarehouseManagement.Admin.Models;
using WarehouseManagement.Admin.Services;

namespace WarehouseManagement.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IApiService _apiService;

        public OrderController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _apiService.GetAllOrders();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var products = await _apiService.GetAllProducts();
            var model = new Order
            {
                OrderDate = DateTime.Now,
                ProductIDs = new List<int>(),
                Quantities = new List<int>()
            };
            ViewBag.Products = products;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order model, string OrderDetailsJson)
        {
            if (ModelState.IsValid)
            {
                model.ProductIDs = new List<int>();
                model.Quantities = new List<int>();

                var orderDetails = JsonConvert.DeserializeObject<List<Order>>(OrderDetailsJson);
                foreach (var detail in orderDetails)
                {
                    model.ProductIDs.Add(detail.ProductID);
                    model.Quantities.Add(detail.Quantity);
                }

                var success = await _apiService.CreateOrder(model);
                if (success) return RedirectToAction("Index");
            }

            ViewBag.Products = await _apiService.GetAllProducts();
            return View(model);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var order = await _apiService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var products = await _apiService.GetAllProducts(); // Fetch all products for dropdown
            var model = new Order
            {
                OrderID = order.OrderID,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                DeliveryOptionID = order.DeliveryOptionID,
                OrderDate = order.OrderDate,
                AvailableProducts = products,
                OrderItems = order.OrderItems.Select(x => new OrderItem
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Order order)
        {
            bool result = await _apiService.UpdateOrder(order);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update order."); 
                return View(order);
            }

            return RedirectToAction("Index");
             
        }


        [HttpPost]
        public async Task<IActionResult> FulfillOrder(int orderId, DateTime fulfillmentDate)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            var success = await _apiService.FulfillOrder(orderId, fulfillmentDate);
            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to fulfill the order.");
            return View();
        }

        public async Task<IActionResult> GetOrderProduct(int id)
        {
            var order = await _apiService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var model = new Order
            {
                OrderItems = order.OrderItems.Select(x => new OrderItem
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity
                }).ToList()
            };

            return PartialView("_OrderProductModal", model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var order = await _apiService.GetOrderById(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _apiService.DeleteOrder(id);
            if (success) return RedirectToAction("Index");
            return View();
        }
    }

}
