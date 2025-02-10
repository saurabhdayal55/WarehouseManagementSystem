namespace WarehouseManagement.Admin.Models
{
    public class ProductViewModel
    {

        public List<Product> Products { get; set; } = new List<Product>();
        public Product NewProduct { get; set; } = new Product();
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>(); // Product Type List
    }

}
