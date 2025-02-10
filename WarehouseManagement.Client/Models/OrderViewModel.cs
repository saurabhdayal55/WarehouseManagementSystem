namespace WarehouseManagement.Client.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        //public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public List<Product> Products { get; set; } = new List<Product>(); // For dropdown selection
    }

}
