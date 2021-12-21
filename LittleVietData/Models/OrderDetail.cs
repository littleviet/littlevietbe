using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Repositories
{
    internal class OrderDetail
    {
        public string ServingId { get; set; }
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
