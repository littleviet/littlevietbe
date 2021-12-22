using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Models
{ 
    internal class ProductImage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public bool IsDelete { get; set; }
    }
}
