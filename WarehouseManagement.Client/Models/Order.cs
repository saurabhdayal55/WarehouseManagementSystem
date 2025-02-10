namespace WarehouseManagement.Client.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string? DeliveryOptionName { get; set; }
        public string CustomerAddress { get; set; }
        public int DeliveryOptionID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Include Order Items
        public List<Product> AvailableProducts { get; set; } = new List<Product>(); // For dropdown
        public List<DeliveryOption> deliveryOptions { get; set; } = new List<DeliveryOption>(); // For dropdown

        public List<int> ProductIDs { get; set; } = new List<int>();
        public List<int> Quantities { get; set; } = new List<int>();
        public DateTime? FulfillmentDate { get; set; }

        public int ProductID { get; set; }
        public int Quantity { get; set; }

    }

}
