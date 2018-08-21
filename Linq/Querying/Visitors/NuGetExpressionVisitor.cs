namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    /// <summary>
    /// internal
    /// </summary>
    internal class NuGetExpressionVisitor : NuGetVisitor
    {
        public NuGetExpressionVisitor() : base(new NuGetQueryFilter())
        {
        }

        internal IQueryable<NuGetPackage> GetNotEvaluated(IQueryable<NuGetPackage> from)
        {
            var root = (Expression)Expression.Constant(from);
            appliers.Reverse();
            foreach (var applier in appliers)
            {
                root = applier(root);
            }
            return (IQueryable<NuGetPackage>)Expression.Lambda(root).Compile().DynamicInvoke();
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
            
            //orderby
            if (NuGetOrderByVisitor.ApplicableOrderByMethods.Contains(methodName))
            {
                return this.Visit<NuGetOrderByVisitor, Expression>(node, Visit);
            }

            return base.VisitMethodCall(node);
        }
    }
}