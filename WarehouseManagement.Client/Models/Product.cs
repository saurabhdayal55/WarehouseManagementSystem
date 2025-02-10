namespace WarehouseManagement.Client.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
        public string? ProductTypeName { get; set; } // New Property
        public DateTime? WarrantyDate { get; set; }
        public int CurrentQuantity { get; set; }
    }


}
