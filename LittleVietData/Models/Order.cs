using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Repositories
{
    internal class Order
    { 
        public string Id { get; set; }
        public int OrderType { get; set; }
        public double TotalPrice { get; set; }
        public int PaymentType { get; set; }
        public int  Status { get; set; }
        public DateTime PickupTime { get; set; }
        public bool IsDelete { get; set; }
    }
}
