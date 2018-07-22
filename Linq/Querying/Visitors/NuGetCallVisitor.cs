namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
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

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value.GetType().ReflectedType == typeof(NuGetQueryExtensions))
            {
                this.Visit<NuGetFrameworkVisitor>(node);
            }

            return base.VisitConstant(node);
        }
    }
}
