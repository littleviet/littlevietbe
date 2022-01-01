using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.ServiceHelper
{

    public enum RoleEnum{
        ADMIN = 1,
        MANAGER = 2,
        AUTHORIZED = 3,
        UNAUTHORIZED = 4
    }
}
