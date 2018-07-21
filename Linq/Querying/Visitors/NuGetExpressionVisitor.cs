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
                var expression = new NuGetSkipTakeVisitor(this.nuGetQueryFilter).Visit(node);
                return this.VisitMethodCall(expression as MethodCallExpression);
            }

            if (methodName == "Where")
            {
                var expression = new NuGetWhereVisitor(this.nuGetQueryFilter).Visit(node);
                return this.Visit(expression);
            }

            return base.VisitMethodCall(node);
        }
    }
}