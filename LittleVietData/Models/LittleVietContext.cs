using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models
{
    public class LittleVietContext : DbContext
    {
        public LittleVietContext(DbContextOptions<LittleVietContext> options)
            : base(options)
        {
        }

    }
}
