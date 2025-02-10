using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WarehouseManagement.Client.Models;
using WarehouseManagement.Client.Services;

namespace WarehouseManagement.Client.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _apiService;

        public OrderController(IOrderService apiService)
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
            var deliveryOptions = await _apiService.GetAllDeliveryOptions(); // Fetch Delivery Options

            var model = new Order
            {
                OrderDate = DateTime.Now,
                ProductIDs = new List<int>(),
                Quantities = new List<int>(),
                deliveryOptions = deliveryOptions
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
                if (success)
                {
                    TempData["SuccessMessage"] = "Order created successfully!";
                    return RedirectToAction("Index");
                }
                    
            }

            ViewBag.Products = await _apiService.GetAllProducts();
            return View(model);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var order = await _apiService.GetOrderById(id);
            var deliveryOptions = await _apiService.GetAllDeliveryOptions(); // Fetch Delivery Options
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
                DeliveryOptionName = order.DeliveryOptionName,
                deliveryOptions = deliveryOptions,
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



        [HttpPost]
        public async Task<IActionResult> Edit(Order order)
        {
            bool result = await _apiService.UpdateOrder(order);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update order.");
                return View(order);
            }
            TempData["SuccessMessage"] = "Order Updated successfully!";
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
