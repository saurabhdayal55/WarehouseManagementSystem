using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Data.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerAddress { get; set; }

        [Required]
        public int DeliveryOptionID { get; set; }

        [Required]
        public string DeliveryOptionName { get; set; }

        public DateTime? FulfillmentDate { get; set; }
        public string? OrderItemsJson { get; set; }
    }
}
