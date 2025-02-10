using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data.Models;
using WarehouseManagement.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseManagement.Data;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly WarehouseDbContext _context;

        public ProductController(WarehouseDbContext context)
        {
            _context = context;
        }

        // CREATE Product
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductCode", product.ProductCode),
                new SqlParameter("@ProductName", product.ProductName),
                new SqlParameter("@WarrantyDate", product.WarrantyDate ?? (object)DBNull.Value),
                new SqlParameter("@ProductTypeID", product.ProductTypeID),
                new SqlParameter("@CurrentQuantity", product.CurrentQuantity)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC CreateProduct @ProductCode, @ProductName, @WarrantyDate, @ProductTypeID, @CurrentQuantity", parameters);
            return Ok();
        }

        // GET All Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _context.Products
                .FromSqlRaw("EXEC GetAllProducts")
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("Search")]
        public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] string searchTerm = "")
        {
            var products = await _context.Products
                .FromSqlRaw("EXEC sp_GetFilterProducts @SearchTerm",
                    new SqlParameter("@SearchTerm", searchTerm ?? (object)DBNull.Value))
                .ToListAsync();

            return Ok(products);
        }

        // GET Product by ID
        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            var product =  _context.Products
                .FromSqlRaw("EXEC GetProductById @ProductID={0}", id)
                .AsEnumerable()  // Forces the execution to the client-side
                .FirstOrDefault();  // Now we can safely perform the query composition client-side

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        // UPDATE Product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            if (id != product.ProductID)
            {
                return BadRequest("Product ID mismatch.");
            }

            try
            {
                var parameters = new[]
                {
            new SqlParameter("@ProductID", product.ProductID),
            new SqlParameter("@ProductCode", product.ProductCode ?? (object)DBNull.Value),
            new SqlParameter("@ProductName", product.ProductName ?? (object)DBNull.Value),
            new SqlParameter("@WarrantyDate", product.WarrantyDate ?? (object)DBNull.Value),
            new SqlParameter("@ProductTypeID", product.ProductTypeID),
            new SqlParameter("@CurrentQuantity", product.CurrentQuantity)
        };

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC UpdateProduct @ProductID, @ProductCode, @ProductName, @WarrantyDate, @ProductTypeID, @CurrentQuantity",
                    parameters);

                if (rowsAffected > 0)
                {
                    return Ok("Product updated successfully.");
                }
                else
                {
                    return NotFound("Product not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        // UPDATE Product

        //    public async Task<IActionResult> FulfillOrder(int id, Order order)
        //    {
        //        // Ensure that the provided id matches the order's ID
        //        if (id != order.OrderID)
        //        {
        //            return BadRequest();
        //        }

        //        // Prepare parameters for the stored procedure
        //        var parameters = new[]
        //        {
        //    new SqlParameter("@OrderID", order.OrderID),
        //    new SqlParameter("@FulfillmentDate", order.FulfillmentDate ?? (object)DBNull.Value)
        //};

        //        // Execute the stored procedure to update the order
        //        await _context.Database.ExecuteSqlRawAsync("EXEC sp_FulfillOrder @OrderID, @FulfillmentDate", parameters);

        //        // Return the updated order with a 200 OK response
        //        return Ok(order);
        //    }


        // DELETE Product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var parameter = new SqlParameter("@ProductID", id);

            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteProduct @ProductID", parameter);

            return NoContent();
        }

        // ===== PRODUCT TYPE METHODS =====

        // GET: api/Product/producttypes
        [HttpGet("producttypes")]
        public IActionResult GetProductTypes()
        {
            var productTypes = _context.productTypes.ToList();
            return Ok(productTypes);
        }

        // POST: api/Product/producttypes
        [HttpPost("producttypes")]
        public IActionResult CreateProductType([FromBody] ProductType productType)
        {
            if (productType == null)
                return BadRequest("Invalid Product Type data.");

            _context.productTypes.Add(productType);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProductTypes), new { id = productType.ProductTypeID }, productType);
        }
    }
} 
