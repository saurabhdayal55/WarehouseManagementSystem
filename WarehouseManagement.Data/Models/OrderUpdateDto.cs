using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Data.Models
{
    public class OrderUpdateDto
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int DeliveryOptionID { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductIDs { get; set; }  // Comma-separated ProductIDs
        public string Quantities { get; set; }  // Comma-separated Quantities
    }
}
