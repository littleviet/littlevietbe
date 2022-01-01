using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace LittleViet.Data.ServiceHelper
{
    internal class AuditableClass
    {
    }
    public class AuditableEntity: IEntity, IActive
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("CreatedBy")]
        public Guid CreatedBy { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("UpdatedBy")]
        public Guid UpdatedBy { get; set; }
    }

    public interface IEntity
    {

    }
    public interface IActive
    {
        public bool IsDeleted { get; set; }
    }

    public interface IRepository
    {

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
    public class AppSettings
    {
        public string Secret { get; set; }
    }

    public class Role
    {
        public const string ADMIN = "ADMIN";
        public const string AUTHORIZED = "AUTHORIZED";
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string MANAGER = "MANAGER";
    }
}
