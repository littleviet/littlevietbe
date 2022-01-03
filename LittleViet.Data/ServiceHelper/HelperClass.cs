using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace LittleViet.Data.ServiceHelper;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params string[] roles) : base()
    {
        Roles = string.Join(",", roles);
    }
}
public class RemoveCastsVisitor : ExpressionVisitor
{
    private static readonly ExpressionVisitor Default = new RemoveCastsVisitor();

    private RemoveCastsVisitor()
    {
    }

    public new static Expression Visit(Expression node)
    {
        return RemoveCastsVisitor.Default.Visit(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Convert && node.Type.IsAssignableFrom(node.Operand.Type))
        {
            return base.Visit(node.Operand);
        }
        return base.VisitUnary(node);
    }
}

