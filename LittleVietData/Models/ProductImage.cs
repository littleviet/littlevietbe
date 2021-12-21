using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Repositories
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
