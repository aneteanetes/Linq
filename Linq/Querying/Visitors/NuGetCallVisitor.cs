namespace Bars.NuGet.Querying.Visitors
{
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    internal class NuGetCallVisitor : NuGetVisitor
    {
        internal NuGetCallVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {            
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }
    }
}
