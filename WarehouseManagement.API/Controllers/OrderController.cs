using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WarehouseManagement.Data;
using WarehouseManagement.Data.Models;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly WarehouseDbContext _context;

        public OrderController(WarehouseDbContext context)
        {
            _context = context;
        }

        // CREATE Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
        {
            var parameters = new[]
            {
                new SqlParameter("@CustomerName", orderRequest.CustomerName),
                new SqlParameter("@CustomerAddress", orderRequest.CustomerAddress),
                new SqlParameter("@DeliveryOptionID", orderRequest.DeliveryOptionID),
                new SqlParameter("@OrderDate", orderRequest.OrderDate),
                new SqlParameter("@ProductIDs", string.Join(",", orderRequest.ProductIDs)),
                new SqlParameter("@Quantities", string.Join(",", orderRequest.Quantities)),
                new SqlParameter("@OrderID", SqlDbType.Int) { Direction = ParameterDirection.Output } // Output parameter to capture OrderID
   
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_AddOrder @CustomerName, @CustomerAddress, @DeliveryOptionID, @OrderDate, @ProductIDs, @Quantities", parameters);
            return Ok();
        }
        [HttpPost("fulfillOrder")]
        public async Task<bool> FulfillOrder([FromBody] FulfillOrderRequest request)
        {
            var parameters = new[]
            {
        new SqlParameter("@OrderID", request.OrderId),
        new SqlParameter("@FulfillmentDate", request.FulfillmentDate)
    };

            int result = await _context.Database.ExecuteSqlRawAsync("EXEC sp_FulfillOrder @OrderID, @FulfillmentDate", parameters);
            return result > 0;
        }


        // GET All Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .FromSqlRaw("EXEC sp_GetAllOrders")
                .ToListAsync();

            return Ok(orders);
        }
        [HttpGet("GetDeliveryOptions")]
        public async Task<IActionResult> GetDeliveryOptions()
        {
            var deliveryOptions = await _context.deliveryOptions
                .FromSqlRaw("EXEC sp_GetDeliveryOptions")
                .ToListAsync();

            return Ok(deliveryOptions);
        }

        //// GET Order by ID
        //[HttpGet("{id}")]
        //public ActionResult<OrderResponse> GetOrderById(int id)
        //{
        //    var order = _context.Orders
        //        .FromSqlRaw("EXEC sp_GetOrderById @OrderID={0}", id)
        //        .AsEnumerable()
        //        .FirstOrDefault();

        //    if (order == null)
        //    {
        //        return NotFound();
        //    } 
        //    return Ok(order);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
        {
            var order = _context.Orders
                .FromSqlRaw("EXEC sp_GetOrderById @OrderID={0}", id)
                .AsEnumerable()
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            // Deserialize JSON_QUERY result into a List<OrderItemDto>
            var orderResponse = new OrderResponse
            {
                OrderID = order.OrderID,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                OrderDate = order.OrderDate,
                DeliveryOptionID = order.DeliveryOptionID,
                DeliveryOptionName = order.DeliveryOptionName,
                OrderItems = !string.IsNullOrEmpty(order.OrderItemsJson)
                    ? JsonConvert.DeserializeObject<List<OrderItemDto>>(order.OrderItemsJson)
                    : new List<OrderItemDto>()
            };

            return Ok(orderResponse);
        } 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            try
            {
                if (id != orderUpdateDto.OrderID)
                {
                    return BadRequest("Order ID mismatch");
                }

                if (orderUpdateDto.ProductIDs == null || !orderUpdateDto.ProductIDs.Any())
                {
                    return BadRequest("Products cannot be empty.");
                }

                var parameters = new[]
                {
            new SqlParameter("@OrderID", id),
            new SqlParameter("@CustomerName", orderUpdateDto.CustomerName ?? (object)DBNull.Value),
            new SqlParameter("@CustomerAddress", orderUpdateDto.CustomerAddress ?? (object)DBNull.Value),
            new SqlParameter("@DeliveryOptionID", orderUpdateDto.DeliveryOptionID),
            new SqlParameter("@OrderDate", orderUpdateDto.OrderDate),
            new SqlParameter("@ProductIDs", string.Join(",", orderUpdateDto.ProductIDs ?? new List<int>())),
            new SqlParameter("@Quantities", string.Join(",", orderUpdateDto.Quantities ?? new List<int>()))
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_UpdateOrder @OrderID, @CustomerName, @CustomerAddress, @DeliveryOptionID, @OrderDate, @ProductIDs, @Quantities", parameters);

                return Ok("Order updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // DELETE Order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var parameter = new SqlParameter("@OrderID", id);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteOrder @OrderID", parameter); 


            if (result == 0)
            {
                return BadRequest(new { Message = "Cannot delete order. Either order does not exist or is already fulfilled." });
            }

            return NoContent();
        }
    }

    // DTO Models
    public class OrderRequest
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int DeliveryOptionID { get; set; }

        public string? DeliveryOptionName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<int> ProductIDs { get; set; }
        public List<int> Quantities { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>(); // Include Order Items
    }

    public class OrderResponse : OrderRequest
    {
        public int OrderID { get; set; }
        public DateTime? FulfillmentDate { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderUpdateDto
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int DeliveryOptionID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<int> ProductIDs { get; set; }
        public List<int> Quantities { get; set; }
    }

    public class FulfillOrderRequest
    {
        public int OrderId { get; set; }
        public DateTime FulfillmentDate { get; set; }
    }


}
