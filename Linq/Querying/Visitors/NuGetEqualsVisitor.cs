namespace Bars.NuGet.Querying.Visitors
{
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    internal class NuGetEqualsVisitor : NuGetVisitor
    {
        public NuGetEqualsVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var property = GetLeft(node.Left);
            var value = GetRight(node.Right);

            BindProperty(property, node.Left, value);

            return base.VisitBinary(node);
        }

        private string GetLeft(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (expression as MemberExpression).Member.Name;
            }

            return string.Empty;
        }

        private object GetRight(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                return (expression as ConstantExpression).Value;
            }

            return default;
        }

        private void BindProperty(string leftProperty, Expression left, object value)
        {
            string[] internalBools = { "IncludePrerelease", "IncludeDelisted", "Latest", "SyncIncompatibility" };
            if (internalBools.Contains(leftProperty))
            {
                (left as MemberExpression).Member.SetValue(this.nuGetQueryFilter, value);
            }
        }
    }
}