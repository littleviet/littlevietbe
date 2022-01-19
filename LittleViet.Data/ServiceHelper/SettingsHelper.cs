using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace LittleViet.Data.ServiceHelper;

public class AppSettings
{
    public string JwtSecret { get; set; }
}

public class Role
{
    public const string ADMIN = "ADMIN";
    public const string AUTHORIZED = "AUTHORIZED";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string MANAGER = "MANAGER";
}

