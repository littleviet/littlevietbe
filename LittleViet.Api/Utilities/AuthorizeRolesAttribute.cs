using Microsoft.AspNetCore.Authorization;

namespace LittleViet.Api.Utilities;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params string[] roles) : base()
    {
        Roles = string.Join(",", roles);
    }
}

