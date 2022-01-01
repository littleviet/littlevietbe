using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.ServiceHelper
{

    public enum RoleEnum{
        [Display(Name = "ADMIN")]
        ADMIN = 1,
        [Display(Name = "MANAGER")]
        MANAGER = 2,
        [Display(Name = "AUTHORIZED")]
        AUTHORIZED = 3,
        [Display(Name = "UNAUTHORIZED")]
        UNAUTHORIZED = 4
    }
}
