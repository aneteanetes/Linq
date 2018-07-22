namespace Bars.NuGet.Querying.Visitors
{
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    internal class NuGetEqualsVisitor : NuGetVisitor
    {
        public bool Parsed = false;
        private string propertyName;
        public NuGetEqualsVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is MemberExpression memberExpr)
            {
                this.VisitMember(memberExpr);
            }

            var value = GetRight(node.Right);

            //BindProperty(property, node.Left, value);

            return Expression.Constant(true);
            return Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(true), Expression.Constant(true));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = node.Member;
            if (typeof(NuGetPackageInfo).IsAssignableFrom(member.ReflectedType))
            {
                propertyName = member.Name;
            }

            return base.VisitMember(node);
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