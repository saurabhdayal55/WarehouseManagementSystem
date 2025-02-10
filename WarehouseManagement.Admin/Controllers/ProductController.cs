using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Admin.Models;
using WarehouseManagement.Admin.Services;

namespace WarehouseManagement.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IApiService _apiService;

        public ProductController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetAllProducts();

            var model = new ProductViewModel
            {
                Products = products
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var products = await _apiService.GetFilterProduct(searchTerm);
            var viewModel = new ProductViewModel { Products = products };
            return View(viewModel);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var productTypes = await _apiService.GetProductTypes();

            var model = new ProductViewModel
            {
                ProductTypes = productTypes
            };

            return View(model);
        }

        // POST: Product/Create
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ProductTypes = await _apiService.GetProductTypes(); // Ensure dropdown doesn't break on validation failure
                return View(model);
            }

            bool isCreated = await _apiService.CreateProduct(model.NewProduct);

            if (isCreated)
            {
                TempData["SuccessMessage"] = "Product Created successfully!";
                return RedirectToAction("Index"); // Redirect to product list after success 
            } 

            ModelState.AddModelError("", "Failed to create product.");
            model.ProductTypes = await _apiService.GetProductTypes(); // Repopulate dropdown
            return View(model);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _apiService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var productTypes = await _apiService.GetProductTypes(); // Fetch product types

            var viewModel = new ProductViewModel
            {
                NewProduct = product,
                ProductTypes = productTypes
            };

            return View(viewModel);
        }


        // POST: Product/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            bool isUpdated = await _apiService.UpdateProduct(product.NewProduct);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "Product Updated successfully!";
                return RedirectToAction("Index");
            }
                

            ModelState.AddModelError("", "Failed to update product.");
            return View(product);
        }

        // DELETE: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            bool isDeleted = await _apiService.DeleteProduct(id);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Product Deleted successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to delete product.";
            return RedirectToAction("Index");
        }
    }
} 


