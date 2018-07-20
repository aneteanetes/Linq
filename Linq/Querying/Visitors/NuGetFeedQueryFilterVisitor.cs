namespace Bars.NuGet.Querying.Visitors
{
    using Bars.NuGet.Querying.Types;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryFilterVisitor : ExpressionVisitor
    {
        public NuGetQueryFilter NuGetQueryFilter { get; }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.ReturnType==typeof(bool))
            {
                return RewriteWhere(node);
            }

            return base.VisitLambda(node);
        }

        private Expression RewriteWhere(LambdaExpression expression)
        {
            return Expression.Lambda<Func<NuGetPackage,bool>>(Expression.Constant(false),expression.Parameters);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        //protected override Expression VisitMember(MemberExpression node)
        //{
        //    return base.VisitMember(node);
        //}
    }
}