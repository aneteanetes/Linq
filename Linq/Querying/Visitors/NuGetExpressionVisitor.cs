namespace Bars.NuGet.Querying.Visitors
{
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    internal class NuGetExpressionVisitor : NuGetVisitor
    {
        internal NuGetExpressionVisitor() : base(new NuGetQueryFilter())
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var methodName = node.Method.Name;

            string[] skipTake = { "Skip", "Take" };
            if (skipTake.Contains(methodName))
            {
                return this.Visit<NuGetSkipTakeVisitor, MethodCallExpression>(node, VisitMethodCall);
            }

            if (methodName == "Where")
            {
                return this.Visit<NuGetWhereVisitor, Expression>(node, Visit);
            }

            return base.VisitMethodCall(node);
        }
    }
}