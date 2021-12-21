using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Repositories
{
    internal class Coupon
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public string CouponCode { get; set; }
        public bool IsDelete { get; set; }
    }
}
